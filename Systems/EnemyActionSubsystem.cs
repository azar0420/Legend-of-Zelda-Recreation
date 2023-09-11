using amongus3902.Components;
using amongus3902.Components.EnemyActions;
using amongus3902.ContentMetadata;
using amongus3902.Data;
using amongus3902.Factories;
using amongus3902.MetaClasses;
using amongus3902.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using static System.Formats.Asn1.AsnWriter;

namespace amongus3902.Systems
{
    //contains functions that apply the specified action to the specified enemy (factored out of enemysystem due to length)
    internal class EnemyActionSubsystem
    {
        private World world;
        private SoundSystem sound;
        private RoomGenerationSystem roomGen;
        private ProjectileFactory projFact;
        private VectorUtils randVecFact;
        private LoadSystem loader;
        private EnemyFactory enemyFactory;
        private TextMaker textMaker;

        private Dictionary<Type, Action<Entity, EnemyBehavior, IEnemyAction>> enemyActions;

        private float MODIFY_MOVEMENT_SCALAR;

        private readonly int TILE_SIZE = RoomConstants.TILE_SIZE;
        private readonly int FIRE_DISTANCE = 3;
        private readonly float OLD_MAN_SHOOT_CHANCE = 0.25f;
        private readonly float PICKUP_SPEED_MULTIPLIER = 3;

        private static Random rand = new Random();

        public EnemyActionSubsystem(World world)
        {
            this.world = world;
            sound = world.GetSystem<SoundSystem>();
            roomGen = world.GetSystem<RoomGenerationSystem>();
            projFact = new ProjectileFactory(world);
            randVecFact = new VectorUtils();
            enemyActions = generateActMap();
            MODIFY_MOVEMENT_SCALAR = PhysicsSystem.MILLISECOND_MULTIPLIER;
            loader = world.GetSystem<LoadSystem>();
            enemyFactory = new EnemyFactory(world);
            textMaker = new TextMaker(world);
        }

        public Action<Entity, EnemyBehavior, IEnemyAction> GetAction(Type t)
        {
            return enemyActions[t];
        }

        private Dictionary<Type, Action<Entity, EnemyBehavior, IEnemyAction>> generateActMap()
        {
            return new Dictionary<Type, Action<Entity, EnemyBehavior, IEnemyAction>>
            {
                { typeof(ChangeVelocityAction), new(ChangeVelocity) },
                { typeof(ChangeAccelerationAction), new(ChangeAcceleration) },
                { typeof(ChangeAnimRowAction), new(ChangeAnimRow) },
                { typeof(RandomRookVelocityAction), new(RandomRookVelocity) },
                { typeof(RandomQueenVelocityAction), new(RandomQueenVelocity) },
                { typeof(KeeseAccelerationAction), new(KeeseAcceleration) },
                { typeof(ShootProjectileAction), new(ShootProjectile) },
                {
                    typeof(AquamentusShootProjectileSpreadAction),
                    new(AquamentusShootProjectileSpread)
                },
                { typeof(TrapAction), new(TrapAction) },
                { typeof(TryOldManShootAction), new(TryOldManShoot) },
                { typeof(HexaburstAction), new(Hexaburst) },
                { typeof(OhioBeamAction), new(OhioBeam) },
                { typeof(PrimalAspidTypeAction), new(PrimalAspidTypeBeam) },
                { typeof(SpiralAction), new(Spiral) },
                { typeof(ChangePosBySizeAction), new(ChangePosBySize) },
                { typeof(ChangeHitboxStatusAction), new(ChangeHitboxStatus) },
                { typeof(RandomTimedAction), new(DoRandomTImedAction) },
                { typeof(DragToWallAction), new(DragToWall) },
                { typeof(DoNothingAction), (_, _, _) => { } },
            };
        }

        private void ChangeVelocity(
            Entity enemy,
            EnemyBehavior enemyBehavior,
            IEnemyAction currentAction
        )
        {
            if (enemyBehavior.TimeSinceLastUpdate == 0)
            {
                PhysicsBody movement = enemy.Get<PhysicsBody>();

                movement.Velocity = ((ChangeVelocityAction)currentAction).Velocity;
            }
        }

        private void ChangeAcceleration(
            Entity enemy,
            EnemyBehavior enemyBehavior,
            IEnemyAction currentAction
        )
        {
            if (enemyBehavior.TimeSinceLastUpdate == 0)
            {
                PhysicsBody movement = enemy.Get<PhysicsBody>();

                movement.Acceleration = ((ChangeAccelerationAction)currentAction).Acceleration;
            }
        }

        private void ChangePosBySize(
            Entity enemy,
            EnemyBehavior enemyBehavior,
            IEnemyAction currentAction
        )
        {
            ChangePosBySizeAction castAction = (ChangePosBySizeAction)currentAction;
            Transform enemyTrans = enemy.Get<Transform>();
            Vector2 enemySize = enemyTrans.Scale * enemy.Get<Sprite>().Sheet.FrameSize;
            enemyTrans.Position += enemySize * castAction.Change;
        }

        private void ChangeAnimRow(
            Entity enemy,
            EnemyBehavior enemyBehavior,
            IEnemyAction currentAction
        )
        {
            ChangeAnimRowAction castAction = (ChangeAnimRowAction)currentAction;
            castAction.SwitchType.Invoke(enemy, castAction.Row);
            enemy.Get<Animation>().Looping = castAction.Looping;
        }

        private void ChangeHitboxStatus(
            Entity enemy,
            EnemyBehavior enemyBehavior,
            IEnemyAction currentAction
        )
        {
            ChangeHitboxStatusAction castAction = (ChangeHitboxStatusAction)currentAction;
            enemy.Get<HitBox>().Enabled = castAction.Value;
        }

        private void RandomRookVelocity(
            Entity enemy,
            EnemyBehavior enemyBehavior,
            IEnemyAction currentAction
        )
        {
            if (enemyBehavior.TimeSinceLastUpdate == 0)
            {
                PhysicsBody movement = enemy.Get<PhysicsBody>();

                float vectorMagnitude = ((RandomRookVelocityAction)currentAction).Magnitude;

                movement.Velocity = randVecFact.GenerateRandomRookVector(vectorMagnitude);

                AnimationChanger.MatchAnimToVelocity(enemy);
            }
        }

        private void RandomQueenVelocity(
            Entity enemy,
            EnemyBehavior enemyBehavior,
            IEnemyAction currentAction
        )
        {
            if (enemyBehavior.TimeSinceLastUpdate == 0)
            {
                PhysicsBody movement = enemy.Get<PhysicsBody>();

                float vectorMagnitude = ((RandomQueenVelocityAction)currentAction).Magnitude;

                movement.Velocity = randVecFact.GenerateRandomQueenVector(vectorMagnitude);
            }
        }

        private void DoRandomTImedAction(
            Entity enemy,
            EnemyBehavior enemyBehavior,
            IEnemyAction currentAction
        )
        {
            RandomTimedAction castAction = (RandomTimedAction)currentAction;
            if (enemyBehavior.TimeSinceLastUpdate == 0)
            {
                int selection = rand.Next(castAction.PossibleActions.Length);
                castAction.CurrentAction = castAction.PossibleActions[selection];
                castAction.Duration = castAction.CurrentAction.Duration;
            }
            ITimedEnemyAction cur = castAction.CurrentAction;
            Action<Entity, EnemyBehavior, IEnemyAction> action = GetAction(cur.GetType());
            action.Invoke(enemy, enemyBehavior, castAction.CurrentAction);
        }

        private void KeeseAcceleration(
            Entity enemy,
            EnemyBehavior enemyBehavior,
            IEnemyAction currentAction
        )
        {
            PhysicsBody movement = enemy.Get<PhysicsBody>();
            KeeseAccelerationAction castAction = (KeeseAccelerationAction)currentAction;
            if (enemyBehavior.TimeSinceLastUpdate == 0)
            {
                float accelerationMagnitude =
                    castAction.Magnitude / MODIFY_MOVEMENT_SCALAR / castAction.Duration;
                movement.Acceleration = randVecFact.GenerateRandomQueenVector(
                    accelerationMagnitude
                );

                if (castAction.IsDecelerating)
                {
                    movement.Velocity =
                        Vector2.Normalize(movement.Acceleration) * castAction.Magnitude;
                    movement.Velocity = Vector2.Negate(movement.Velocity);
                }
            }
            else
            {
                float percentDone = (float)enemyBehavior.TimeSinceLastUpdate / castAction.Duration;
                AnimationChanger.AccelerateAnimation(
                    enemy,
                    percentDone,
                    castAction.MinDelay,
                    castAction.IsDecelerating
                );
            }
        }

        private void DragToWall(
            Entity enemy,
            EnemyBehavior enemyBehavior,
            IEnemyAction currentAction
        ) { 
            DragToWallAction castAction = (DragToWallAction)currentAction;
            if(castAction.Direction == Vector2.Zero)
            {
                castAction.Direction = Direction.DirectionToVector((Directions)rand.Next(0, 3));
                PhysicsBody b = enemy.Get<PhysicsBody>();
                b.Velocity = b.Velocity.Length() * castAction.Direction * PICKUP_SPEED_MULTIPLIER;
            }
            Transform eTrans = enemy.Get<Transform>();
            Vector2 enemyPos = eTrans.Position;
            //TODO: refactor to use some global screen size data
            if (enemyPos.X < 0 || enemyPos.X > Game1.NES_SCREEN_SIZE.X * eTrans.Scale ||enemyPos.Y<0 || enemyPos.Y > Game1.NES_SCREEN_SIZE.Y * eTrans.Scale)
            {
                roomGen.SwitchRoom(RoomName.e2_5, Vector2.UnitY * 0.9f);
            }
            
        }

        private void ShootProjectile(
            Entity enemy,
            EnemyBehavior enemyBehavior,
            IEnemyAction currentAction
        )
        {
            ProjectileType type = ((ShootProjectileAction)currentAction).Projectile;
            Vector2 direction = Direction.DirectionToVector(enemy.Get<Sprite>().Direction);
            Entity p = projFact.CreateProjectile(type, new Transform(), direction, enemy);
            Vector2 pos = Geometry.GetCenterPositionOfSpriteInWorld(enemy) - (p.Get<Sprite>().Sheet.FrameSize / 2);
            p.Replace(new Transform(pos, TransformData.ENEMY_PROJECTILE_DEPTH, enemy.Get<Transform>().Scale));
            world.AddEntity(p);
        }

        private void AquamentusShootProjectileSpread(
            Entity enemy,
            EnemyBehavior enemyBehavior,
            IEnemyAction currentAction
        )
        {
            Vector2 up = new Vector2(-1, 0.5f);
            Vector2 mid = new Vector2(-1, 0);
            Vector2 down = new Vector2(-1, -0.5f);
            Transform transform = enemy.Get<Transform>().Duplicate();
            transform.LayerDepth = TransformData.ENEMY_PROJECTILE_DEPTH;

            projFact.SpawnProjectile(ProjectileType.Fireball, transform.Duplicate(), up, enemy);
            projFact.SpawnProjectile(ProjectileType.Fireball, transform.Duplicate(), mid, enemy);
            projFact.SpawnProjectile(ProjectileType.Fireball, transform.Duplicate(), down, enemy);
            sound.PlaySound(ContentMetadata.ZeldaSound.BossScream);
            sound.PlaySound(ContentMetadata.ZeldaSound.Candle);
        }

        private void TryOldManShoot(
            Entity enemy,
            EnemyBehavior enemyBehavior,
            IEnemyAction currentAction
        )
        {
            if (enemyBehavior.TimeSinceLastUpdate == 0)
            {
                if (enemy.Get<HurtBox>().Health >= EnemyData.EnemyDetails[Enemys.OldMan].Health)
                {
                    return;
                }

                TryOldManShootAction castAction = (TryOldManShootAction)currentAction;
                if (castAction.TryAddOldManText)
                {
                    castAction.TryAddOldManText = false;
                    Entity text = textMaker.CreateText(TextType.RetirementText);
                    roomGen.AddToCurrentRoom(text);
                    world.AddEntity(text);
                }

                OldManMultiplies();

                int random = rand.Next(0, (int)(1 / OLD_MAN_SHOOT_CHANCE));

                if (random == 0)
                {
                    
                    Transform enemyTransform = enemy.Get<Transform>();
                    Vector2 enemyPos = enemyTransform.Position;
                    float enemyScale = enemyTransform.Scale;
                    Vector2 distToSpawn =
                        Direction.DirectionToVector(castAction.Side)
                        * enemyScale
                        * TILE_SIZE
                        * FIRE_DISTANCE;
                    Vector2 spawnPos = enemyPos + distToSpawn;

                    Transform projTransform = enemyTransform.Duplicate();
                    projTransform.Position = spawnPos;
                    Vector2 projDirection = EntityDetection.GetVectorToNearestPlayer(
                        world,
                        spawnPos
                    );
                    projFact.SpawnProjectile(
                        ProjectileType.Fireball,
                        projTransform,
                        projDirection,
                        enemy
                    );
                }
            }
        }

        private void Hexaburst(
            Entity enemy,
            EnemyBehavior enemyBehavior,
            IEnemyAction currentAction
        )
        {
            if (enemyBehavior.TimeSinceLastUpdate == 0)
            {
                HexaburstAction castAction = (HexaburstAction)currentAction;
                Vector2 spawnPos = Geometry.GetCenterPositionOfSpriteInWorld(enemy);
                Vector2 initDirection = EntityDetection.GetVectorToNearestPlayer(world, spawnPos);

                float velDif = (castAction.MaxVel - castAction.MinVel) / castAction.Bursts;
                float currentVel = castAction.MaxVel;
                float scale = enemy.Get<Transform>().Scale;

                for (int i = 0; i < castAction.Bursts; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        double angle = Math.Atan2(initDirection.X, -initDirection.Y);
                        angle += (j * Math.PI / 3) + Math.PI / 6;
                        Vector2 direction = new((float)Math.Cos(angle), (float)Math.Sin(angle));
                        Entity fireball = projFact.CreateProjectile(
                            ProjectileType.Fireball,
                            new Transform(spawnPos, TransformData.ENEMY_PROJECTILE_DEPTH, scale),
                            direction,
                            enemy
                        );
                        fireball.Get<PhysicsBody>().Velocity *= currentVel;
                        world.AddEntity(fireball);
                    }
                    currentVel -= velDif;
                }
            }
        }

        private void OhioBeam(Entity enemy, EnemyBehavior enemyBehavior, IEnemyAction currentAction)
        {
            //ohio beam goes pew pew
            int projTimeSpread = 20;

            OhioBeamAction castAction = (OhioBeamAction)currentAction;
            if (enemyBehavior.TimeSinceLastUpdate == 0)
            {
                castAction.Target = EntityDetection.GetRandomPlayer(world);
                castAction.NextUpdate = 0;
            }
            if (castAction.Duration - castAction.Cooldown > castAction.NextUpdate)
            {
                if (enemyBehavior.TimeSinceLastUpdate >= castAction.NextUpdate)
                {
                    Vector2 spawnPos = Geometry.GetCenterPositionOfSpriteInWorld(enemy);
                    Vector2 targetPos = castAction.Target.Get<Transform>().Position;
                    Vector2 ballDir = targetPos - spawnPos;
                    ballDir.Normalize();

                    Entity fireball = projFact.CreateProjectile(
                        ProjectileType.Fireball,
                        new Transform(
                            spawnPos,
                            TransformData.ENEMY_PROJECTILE_DEPTH,
                            enemy.Get<Transform>().Scale
                        ),
                        ballDir,
                        enemy
                    );
                    fireball.Get<PhysicsBody>().Velocity *= castAction.Velocity;
                    world.AddEntity(fireball);
                    castAction.NextUpdate += projTimeSpread;
                }
            }
        }

        private void PrimalAspidTypeBeam(
            Entity enemy,
            EnemyBehavior enemyBehavior,
            IEnemyAction currentAction
        )
        {
            PrimalAspidTypeAction castAction = (PrimalAspidTypeAction)currentAction;
            if (enemyBehavior.TimeSinceLastUpdate == 0)
            {
                castAction.NextShotTime = 0;
            }
            if (castAction.NextShotTime <= enemyBehavior.TimeSinceLastUpdate)
            {
                float scale = enemy.Get<Transform>().Scale;
                Vector2 spawnPos = Geometry.GetCenterPositionOfSpriteInWorld(enemy);
                Vector2 initDirection = EntityDetection.GetVectorToNearestPlayer(world, spawnPos);

                double radAngle = castAction.Angle * Math.PI / 180;
                double[] angles = new double[3];
                angles[0] = Math.Atan2(initDirection.X, -initDirection.Y) - Math.PI / 2;
                angles[1] = angles[0] - radAngle;
                angles[2] = angles[0] + radAngle;

                foreach (double angle in angles)
                {
                    Vector2 direction = new((float)Math.Cos(angle), (float)Math.Sin(angle));
                    Entity fireball = projFact.CreateProjectile(
                        ProjectileType.Fireball,
                        new Transform(spawnPos, TransformData.ENEMY_PROJECTILE_DEPTH, scale),
                        direction,
                        enemy
                    );
                    world.AddEntity(fireball);
                }
                castAction.NextShotTime += castAction.Duration / castAction.Shots;
            }
        }

        private void Spiral(Entity enemy, EnemyBehavior enemyBehavior, IEnemyAction currentAction)
        {
            int projTimeSpread = 40;
            SpiralAction castAction = (SpiralAction)currentAction;
            if (enemyBehavior.TimeSinceLastUpdate == 0)
            {
                castAction.Direction *= -1;
                castAction.NextUpdate = 0;
                castAction.CurrentAngle = rand.NextDouble() * Math.PI * 2;
            }
            if (castAction.Duration - castAction.Cooldown > castAction.NextUpdate)
            {
                if (enemyBehavior.TimeSinceLastUpdate >= castAction.NextUpdate)
                {
                    float scale = enemy.Get<Transform>().Scale;
                    Vector2 spawnPos = Geometry.GetCenterPositionOfSpriteInWorld(enemy);

                    Vector2 direction =
                        new(
                            (float)Math.Cos(castAction.CurrentAngle),
                            (float)Math.Sin(castAction.CurrentAngle)
                        );
                    Entity fireball = projFact.CreateProjectile(
                        ProjectileType.Fireball,
                        new Transform(spawnPos, TransformData.ENEMY_PROJECTILE_DEPTH, scale),
                        direction,
                        enemy
                    );
                    fireball.Get<PhysicsBody>().Velocity *= castAction.Velocity;
                    world.AddEntity(fireball);
                    castAction.NextUpdate += projTimeSpread;
                    castAction.CurrentAngle +=
                        castAction.Direction * castAction.AngleDiff * Math.PI / 180;
                }
            }
        }

        private void OldManMultiplies()
        {
            List<Entity> men = world.GetEntitiesWithComponentOfTypes(typeof(Sprite));
            //not too many men
            if (men.Count < 60)
            {
                Entity enemy = enemyFactory.CreateEnemy(Enemys.HarmlessOldMan);
                enemy.Replace(new Transform(new Vector2(400, 440), TransformData.ENEMY_DEPTH, 3));
                world.AddEntity(enemy);
            }
        }

        //TODO: refactor(?) (hard to check positional data in seperate actions)
        private void TrapAction(
            Entity enemy,
            EnemyBehavior enemyBehavior,
            IEnemyAction currentAction
        )
        {
            TrapAction castAction = (TrapAction)currentAction;
            PhysicsBody trapBody = enemy.Get<PhysicsBody>();
            Transform enemyTrans = enemy.Get<Transform>();
            switch (castAction.State)
            {
                case TrapState.Searching:

                    castAction.HomePos = enemyTrans.Position;
                    Entity player;
                    Vector2 dirToPlayer = EntityDetection.GetOrthogonalVectorToPlayerCollider(
                        world,
                        enemy,
                        out player
                    );
                    if (dirToPlayer != Vector2.Zero)
                    {
                        Transform playerTrans = player.Get<Transform>();
                        PhysicsBody playerBody = player.Get<PhysicsBody>();
                        castAction.State = TrapState.Attacking;
                        castAction.TargetPos =
                            playerTrans.Position + playerBody.ColliderOffset * playerTrans.Scale;
                        castAction.TargetSize = playerBody.ColliderSize * playerTrans.Scale;
                        trapBody.Velocity = castAction.AttackSpeed * dirToPlayer;
                    }
                    break;
                case TrapState.Attacking:
                    if (
                        Geometry.Overlaps(
                            castAction.TargetPos,
                            castAction.TargetSize,
                            enemyTrans.Position + trapBody.ColliderOffset * enemyTrans.Scale,
                            trapBody.ColliderSize * enemyTrans.Scale
                        )
                    )
                    {
                        castAction.State = TrapState.Returning;
                        if (castAction.HomePos - enemyTrans.Position != Vector2.Zero)
                        {
                            trapBody.Velocity =
                                castAction.ReturnSpeed
                                * Vector2.Normalize(castAction.HomePos - enemyTrans.Position);
                        }
                    }
                    break;
                case TrapState.Returning:
                    if (Geometry.EntityAtPos(enemy, castAction.HomePos))
                    {
                        castAction.State = TrapState.Searching;
                        trapBody.Velocity = Vector2.Zero;
                    }
                    break;
            }
        }
    }
}
