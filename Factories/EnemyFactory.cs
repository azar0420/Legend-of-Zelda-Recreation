using amongus3902.Components;
using amongus3902.Components.EnemyActions;
using amongus3902.ContentMetadata;
using amongus3902.MetaClasses;
using amongus3902.Systems;
using amongus3902.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace amongus3902.Factories
{
    internal class EnemyFactory
    {
        private readonly World _world;
        private readonly LoadSystem _loader;
        private readonly SoundSystem _sound;
        private readonly LootSystem _loot;
        private readonly EnemyBehaviorFactory _bFact;
        private readonly ProjectileFactory _pFact;
        private readonly ItemFactory _iFact;

        private readonly int ENEMY_IMMUNITY_DURATION = 300;
        private readonly int ENEMY_STUN_DURATION = 750;

        public EnemyFactory(World world)
        {
            _world = world;
            _loader = world.GetSystem<LoadSystem>();
            _sound = world.GetSystem<SoundSystem>();
            _loot = world.GetSystem<LootSystem>();
            _bFact = new EnemyBehaviorFactory();
            _pFact = new ProjectileFactory(world);
            _iFact = new ItemFactory(world);
        }

        public Entity CreateEnemyInSmoke(Enemys enemyType)
        {
            ProjectileType smokeType = ProjectileType.SafeSmoke;
            if (enemyType == Enemys.AmogusSus)
            {
                smokeType = ProjectileType.SusSmoke;
            }
            Entity result = _pFact.CreateProjectile(
                smokeType,
                new Transform(Vector2.Zero, TransformData.ENEMY_DEPTH),
                Vector2.Zero,
                new()
            );
            result.Attach(new ProjBackup(result.Get<Projectile>()));
            Entity enemy = CreateEnemy(enemyType);
            EnemyBehavior startingState = enemy.Get<EnemyBehavior>().Duplicate();
            result.Attach<EnemySpawnerInfo>(new(enemy));
            result.Get<Projectile>().Despawned += () =>
            {
                enemy.Get<Transform>().Position = result.Get<Transform>().Position;
                enemy.Replace(startingState.Duplicate());
                _world.AddEntity(enemy);
            };
            if(smokeType == ProjectileType.SusSmoke)
            {
                result.OnAdd += () =>
                {
                    Sound s = _loader.GetSound(ZeldaSound.Candle);
                    s.Looping = true;
                    _sound.PlaySound(s);
                };
                result.OnRemove += () => _sound.RemoveAudioSourceOfType(ZeldaSound.Candle);
            }
            return result;
        }

        public Entity CreateEnemy(Enemys enemyType)
        {
            Entity enemy = new();
            var details = EnemyData.EnemyDetails[enemyType];

            SpriteSheet sheet = _loader.GetSheet(details.SpriteSheet);

            PhysicsBody body =
                new(details.Collides, sheet.FrameSize, Vector2.Zero, details.CollisionGroup);
            body.Touched += (Entity hit) => DefaultEnemyCollideAction(enemy, hit);

            EnemyBehavior behavior = _bFact.CreateBehavior(enemyType);
            behavior.EnemyType = enemyType;

            enemy
                .Attach(new Transform(Vector2.Zero, TransformData.ENEMY_DEPTH))
                .Attach(new Sprite(sheet, details.Animation.StartFrame, details.InitialDirection))
                .Attach(body)
                .Attach(behavior)
                .Attach(new Team(TeamType.Foe))
                // make sure to clone here so the enemies dont all share the same animation
                .Attach(details.Animation.Clone());

            if (details.Damage != null)
            {
                enemy.Attach(new HitBox(sheet.FrameSize, Vector2.Zero, (int)details.Damage));
            }

            if (details.Health != null)
            {
                HurtBox enemyHurtBox = new HurtBox(
                    sheet.FrameSize,
                    Vector2.Zero,
                    (int)details.Health
                );

                enemyHurtBox.Damaged += (Entity damageSource) =>
                    EnemyIFrameStart(enemy, damageSource, enemyType);

                enemyHurtBox.Killed += (Entity damageSource) =>
                    DefaultEnemyKill(enemy, damageSource);
                enemyHurtBox.Killed += (Entity _) => _sound.PlaySound(ZeldaSound.EnemyDie);

                enemy.Attach(enemyHurtBox);
            }

            // special case for aquamentus to open the final door
            if (enemyType == Enemys.Aquamentus)
            {
                enemy.Get<HurtBox>().Killed += (Entity e) =>
                    _world.GetSystem<RoomGenerationSystem>().InvokeCurrentRoomEvent();
            }

            if (enemyType == Enemys.WallMaster)
            {
                enemy.Get<Transform>().LayerDepth = TransformData.NON_MENU_DEPTH_OVERRIDE;
                enemy.Get<HitBox>().DidDamage += (Entity wm, Entity player) =>
                {
                    player.Replace(wm.Get<Transform>());
                    PhysicsBody body = player.Get<PhysicsBody>();
                    body.CanCollide = false;
                    body.Velocity = Vector2.Zero;
                    player.Get<HurtBox>().Enabled = false;
                    player.Get<CharacterController>().InputsLocked = true;
                    EnemyBehavior behavior = wm.Get<EnemyBehavior>();
                    behavior.Behaviors = new IEnemyAction[] { new DragToWallAction(player) };
                    behavior.CurrentAction = behavior.Behaviors[0];
                    behavior.BehaviorIndex = 0;

                    enemy.OnRemove += () =>
                    {
                        player.Get<Transform>().LayerDepth = TransformData.PLAYER_DEPTH;
                        PhysicsBody body = player.Get<PhysicsBody>();
                        body.CanCollide = true;
                        player.Get<HurtBox>().Enabled = true;
                        player.Get<CharacterController>().InputsLocked = false;
                        player.Get<CharacterController>().CurrentAction = CharacterAction.Look;
                    };
                };
            }
            if (enemyType == Enemys.AmogusSus)
            {
                enemy.Get<HurtBox>().Killed -= (Entity damageSource) =>
                    DefaultEnemyKill(enemy, damageSource);
                enemy.Get<HurtBox>().Killed += (Entity damageSource) =>
                    AmogusKill(enemy, damageSource);
                enemy.OnRemove += () =>
                {
                    _sound.RemoveAudioSourceOfType(ZeldaSound.DungeonDrip);
                };
                enemy.OnAdd += () =>
                {
                    _sound.PlaySound(ZeldaSound.DungeonDrip);
                    enemy.Replace(details.Animation.Clone());
                };
            }

            return enemy;
        }

        private static void DefaultEnemyCollideAction(Entity enemy, Entity hitEntity)
        {
            List<Type> CollisionSkippableActions = new List<Type>
            {
                typeof(ChangeAccelerationAction),
                typeof(ChangeVelocityAction),
                typeof(RandomQueenVelocityAction),
                typeof(RandomRookVelocityAction)
            };

            EnemyBehavior behavior = enemy.Get<EnemyBehavior>();
            bool canAdvance = CollisionSkippableActions.Contains(
                behavior.Behaviors[behavior.BehaviorIndex].GetType()
            );

            if (canAdvance)
            {
                behavior.BehaviorIndex = (behavior.BehaviorIndex + 1) % behavior.Behaviors.Length;
                behavior.TimeSinceLastUpdate = 0;
            }
        }

        private void EnemyIFrameStart(Entity enemy, Entity damageSource, Enemys enemyType)
        {
            EnemyBehavior b = enemy.Get<EnemyBehavior>();
            if (b.TakingDamage)
            {
                enemy.Get<HurtBox>().Health += damageSource.Get<HitBox>().DamageAmount;
                return;
            }
            if (damageSource.Has<Projectile>())
            {
                Projectile p = damageSource.Get<Projectile>();
                if (
                    p.Type == ProjectileType.WoodenBoomerang
                    && !EnemyData.BoomerangDamageable.Contains(enemyType)
                )
                {
                    enemy.Get<HurtBox>().Health += damageSource.Get<HitBox>().DamageAmount;
                    b.CurrentAction = new ChangeVelocityAction(Vector2.Zero, ENEMY_STUN_DURATION);
                    b.TimeSinceLastUpdate = 0;
                    return;
                }
            }

            b.TakingDamage = true;
            b.InvincibleMS = ENEMY_IMMUNITY_DURATION;
            _sound.PlaySound(ZeldaSound.EnemyHurt);
        }

        private void DefaultEnemyKill(Entity enemy, Entity damageSource)
        {
            Transform enemyTransform = enemy.Get<Transform>();
            _pFact.SpawnProjectile(
                ProjectileType.DeathSparkle,
                enemyTransform,
                Vector2.Zero,
                enemy
            );
            _loot.RollLoot(enemy.Get<EnemyBehavior>().LootGroup, enemyTransform);
            if (_world.HasEntity(enemy.UniqueID))
            {
                _world.RemoveEntity(enemy.UniqueID);
            }
        }

        private void AmogusKill(Entity enemy, Entity damageSource)
        {
            Transform susTransform = enemy.Get<Transform>();
            Sprite susSprite = enemy.Get<Sprite>();
            Vector2 pos = susTransform.Position;
            float width = susTransform.Scale * susSprite.Sheet.FrameSize.X;
            float height = susTransform.Scale * susSprite.Sheet.FrameSize.Y;

            Entity mArrow = _iFact.CreateMagicArrow();
            mArrow.Get<Transform>().Position = pos;
            _world.AddEntity(mArrow);

            //ha. p gun.
            Entity pGun = _iFact.CreatePortalGun();
            pGun.Get<Transform>().Position = pos;
            _world.AddEntity(pGun);

            Entity candle = _iFact.CreateCandle();
            candle.Get<Transform>().Position = pos + new Vector2(width / 2, 0);
            _world.AddEntity(candle);

            Entity hc = _iFact.CreateHeartContainer();
            hc.Get<Transform>().Position = pos + new Vector2(0, height / 2);
            _world.AddEntity(hc);

            Entity mBoom = _iFact.CreateMagicBoomerang();
            mBoom.Get<Transform>().Position = pos + new Vector2(width / 2, height / 2);
            _world.AddEntity(mBoom);

            if (_world.HasEntity(enemy.UniqueID))
            {
                _world.RemoveEntity(enemy.UniqueID);
            }
        }
    }
}
