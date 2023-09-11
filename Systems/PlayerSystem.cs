using amongus3902.Components;
using amongus3902.ContentMetadata;
using amongus3902.Factories;
using amongus3902.Gamepads;
using amongus3902.MetaClasses;
using amongus3902.Systems.Interfaces;
using amongus3902.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace amongus3902.Systems
{
    internal class PlayerSystem : ISystem
    {
        private World _world;
        private InputSystem _inputSystem;
        private InventorySystem _inventorySystem;
        private ProjectileFactory _projectileCreator;
        private SoundSystem _sound;

        private const int ITEM_PICKUP_TIME_MS = 2000;
        private const int DAMAGE_TIME_MS = 250;
        private const int KNOCKBACK_TIME_MS = DAMAGE_TIME_MS - 20;
        private const int BEEP_THRESHHOLD = 2;

        private static readonly Dictionary<
            (CharacterAction action, Directions direction),
            Animation
        > _linkAnimations = AnimationData.LINK_ANIMATIONS;

        public void Start(World world)
        {
            _world = world;
            _inputSystem = world.GetSystem<InputSystem>();
            _inventorySystem = world.GetSystem<InventorySystem>();
            _projectileCreator = new ProjectileFactory(world);
            _sound = world.GetSystem<SoundSystem>();

            // check if a player was added, and bind controls
            world.OnEntityAdded += (Entity e) =>
            {
                if (!e.Has<CharacterController>())
                {
                    return;
                }

                BindCharacterActions(e, world);
            };
        }

        public void BindCharacterActions(Entity character, World gameWorld)
        {
            IGamepad controls = character.Get<CharacterController>().Controls;

            BindWalkDirection(character, Directions.Up, controls.MoveUp);
            BindWalkDirection(character, Directions.Down, controls.MoveDown);
            BindWalkDirection(character, Directions.Left, controls.MoveLeft);
            BindWalkDirection(character, Directions.Right, controls.MoveRight);

            BindProjectileLaunch(character, ProjectileType.WoodenSword, controls.Attack);
            BindItemUse(character, controls.UseItem);
            BindItemSwitching(character, gameWorld, controls.UseItem);

            BindDamage(character);
            BindDeath(character, gameWorld);

            if (gameWorld.MODE == WorldMode.Debug)
            {
                BindDebugItems(character, controls);
            }
        }

        private void BindDamage(Entity character)
        {
            void damage(Entity byEntity)
            {
                TakeDamage(character, byEntity);
            }

            void heal()
            {
                _sound.RemoveAudioSourceOfType(ZeldaSound.LowHealth);
            }

            character.Get<HurtBox>().Damaged += damage;

            character.OnRemove += () => character.Get<HurtBox>().Damaged -= damage;

            character.Get<HurtBox>().Healed += heal;

            character.OnRemove += () => character.Get<HurtBox>().Healed -= heal;
        }

        public void TakeDamage(Entity character, Entity fromEntity)
        {
            CharacterController controller = character.Get<CharacterController>();

            // don't want to get damaged while damaged or if a clock is had
            if (controller.CurrentAction == CharacterAction.Damaged)
            {
                return;
            }

            if (_inventorySystem.ConsumablesCount(ItemType.Clock) > 0)
            {
                character.Get<HurtBox>().Health += fromEntity.Get<HitBox>().DamageAmount;
                return;
            }

            if (TryBlock(character, controller, fromEntity))
            {
                if (_world.HasEntity(fromEntity.UniqueID))
                {
                    _world.RemoveEntity(fromEntity.UniqueID);
                }
                _sound.PlaySound(ZeldaSound.Block);
                character.Get<HurtBox>().Health += fromEntity.Get<HitBox>().DamageAmount;
                return;
            }

            if(character.Get<HurtBox>().Health <= BEEP_THRESHHOLD)
            {
                _sound.PlaySound(ZeldaSound.LowHealth);
            }

            _sound.PlaySound(ZeldaSound.LinkHurt);
            SetAction(character, CharacterAction.Damaged);
            Physics.KnockbackFromEntity(character, fromEntity, KNOCKBACK_TIME_MS);
            if(!fromEntity.Has<EnemyBehavior>() || !(fromEntity.Get<EnemyBehavior>().EnemyType == Enemys.WallMaster))
            {
                StopInputThenWaitThenGiveInputBack(character, DAMAGE_TIME_MS);
            }
            
        }

        private static bool TryBlock(Entity character, CharacterController controller, Entity fromEntity)
        {
            if (controller.CurrentAction != CharacterAction.Look)
            {
                return false;
            }

            if (!fromEntity.Has<Projectile>() || !fromEntity.Has<PhysicsBody>())
            {
                return false;
            }

            Vector2 projDirection = VectorUtils.SnapVectorToNormalizedOrthogonal(
               Geometry.GetCenterPositionOfSpriteInWorld(character) - Geometry.GetCenterPositionOfSpriteInWorld(fromEntity)
            );

            return projDirection == -Direction.DirectionToVector(controller.CurrentDirection);
        }

        private void BindDeath(Entity character, World gameWorld)
        {
            void killed(Entity byEntity)
            {
                _sound.ForceLoopingPause();
                _sound.PlaySound(ZeldaSound.LinkDie);
                ScriptedSequences.PlayLinkDeathAnimation(character, gameWorld);
            }

            character.Get<HurtBox>().Killed += killed;

            character.OnRemove += () => character.Get<HurtBox>().Killed -= killed;
        }

        private void BindWalkDirection(Entity character, Directions dir, params Keys[] keys)
        {
            character.OnRemove += _inputSystem.Bind(
                () =>
                    QueueDirection(
                        character,
                        dir,
                        !_world.IsPaused && !character.Get<CharacterController>().InputsLocked
                    ),
                PressType.onDown,
                keys
            );

            character.OnRemove += _inputSystem.Bind(
                () =>
                    DequeueDirection(
                        character,
                        dir,
                        !character.Get<CharacterController>().InputsLocked
                    ),
                PressType.onUp,
                keys
            );
        }

        private void BindProjectileLaunch(Entity character, ProjectileType proj, params Keys[] keys)
        {
            character.OnRemove += _inputSystem.Bind(
                () => TryProjectileLaunch(character, proj),
                PressType.onDown,
                keys
            );
        }

        private void BindItemUse(Entity character, params Keys[] keys)
        {
            character.OnRemove += _inputSystem.Bind(
                () => TryProjectileLaunch(character, _inventorySystem.ActiveItemProjectileType()),
                PressType.onDown,
                keys
            );
        }

        private void BindItemSwitching(Entity character, World world, params Keys[] keys)
        {
            character.OnRemove += _inputSystem.Bind(
                () => world.GetSystem<PauseSystem>().ToggleActiveItems(),
                PressType.onDown,
                keys
            );
        }

        private void BindDebugItems(Entity character, IGamepad controls)
        {
            BindProjectileLaunch(character, ProjectileType.WoodenBoomerang, controls.TEMPItemUse1);
            BindProjectileLaunch(character, ProjectileType.MagicBoomerang, controls.TEMPItemUse2);
            BindProjectileLaunch(character, ProjectileType.WoodenArrow, controls.TEMPItemUse3);
            BindProjectileLaunch(character, ProjectileType.MagicArrow, controls.TEMPItemUse4);
            BindProjectileLaunch(character, ProjectileType.Bomb, controls.TEMPItemUse5);
            BindProjectileLaunch(character, ProjectileType.Fire, controls.TEMPItemUse6);
            BindProjectileLaunch(character, ProjectileType.SwordBeam, controls.TEMPItemUse7);
        }

        private void TryProjectileLaunch(Entity character, ProjectileType proj)
        {
            {
                if (_world.IsPaused)
                    return;
                if (proj == ProjectileType.None)
                    return;

                CharacterController controller = character.Get<CharacterController>();

                if (controller.InputsLocked)
                {
                    return;
                }
                if (
                    controller.LimitedProjCounts.ContainsKey(proj)
                    && controller.LimitedProjCounts[proj] <= 0
                )
                {
                    return;
                }
                if (
                    proj == ProjectileType.Bomb
                    && _inventorySystem.ConsumablesCount(ItemType.Bomb) <= 0
                )
                {
                    return;
                }

                SetAction(character, CharacterAction.Attack);
                Launch(proj, character, Direction.DirectionToVector(controller.CurrentDirection));
            }
        }

        private void Launch(ProjectileType proj, Entity fromCharacter, Vector2 direction)
        {

            Entity projectile = CreateCharProjectile(proj, fromCharacter, direction);


            CharacterController character = fromCharacter.Get<CharacterController>();
            if (character.LimitedProjCounts.ContainsKey(proj))
            {
                character.LimitedProjCounts[proj]--;
                projectile.OnRemove += () =>
                {
                    character.LimitedProjCounts[proj]++;
                };
            }
            if (proj == ProjectileType.WoodenSword)
            {
                HurtBox charHurt = fromCharacter.Get<HurtBox>();
                if (
                    charHurt.Health == charHurt.MaxHealth
                    && character.LimitedProjCounts[ProjectileType.SwordBeam] > 0
                )
                {
                    Entity beam = CreateCharProjectile(
                        ProjectileType.SwordBeam,
                        fromCharacter,
                        direction
                    );
                    _world.AddEntity(beam);
                    character.LimitedProjCounts[ProjectileType.SwordBeam]--;
                    beam.OnRemove += () =>
                    {
                        character.LimitedProjCounts[ProjectileType.SwordBeam]++;
                    };
                }
            }

            if (proj == ProjectileType.MagicArrow || proj == ProjectileType.WoodenArrow)
            {
                if (_inventorySystem.ConsumablesCount(ItemType.Rupee) <= 0)
                {
                    return;
                }
                else
                {
                    _inventorySystem.RemoveConsumables(ItemType.Rupee, 1);
                }
            }
            if (proj == ProjectileType.OrangeProjectile || proj == ProjectileType.BlueProjectile)
            {
                if (!_inventorySystem.HasKeyItem(ItemType.PortalGun))
                {
                    //don't launch portal gun projectile without having the portalgun item in inventory
                    return;
                }
            }

            if (proj == ProjectileType.Bomb)
            {
                _inventorySystem.RemoveConsumables(ItemType.Bomb, 1);
            }

            _world.AddEntity(projectile);
        }

        private Entity CreateCharProjectile(
            ProjectileType proj,
            Entity fromCharacter,
            Vector2 direction
        )
        {
            Transform characterTrans = fromCharacter.Get<Transform>();
            Entity projectile = _projectileCreator.CreateProjectile(
                proj,
                characterTrans,
                direction,
                fromCharacter
            );

            // center projectile
            Vector2 charCenterPos = Geometry.GetCenterPosForProjectile(
                fromCharacter,
                direction,
                projectile
            );
            projectile.Replace(
                new Transform(
                    charCenterPos,
                    TransformData.PLAYER_PROJECTILE_DEPTH,
                    characterTrans.Scale
                )
            );
            return projectile;
        }

        private static void QueueDirection(
            Entity character,
            Directions dir,
            bool shouldUpdateAction
        )
        {
            CharacterController controller = character.Get<CharacterController>();
            controller.DirectionalPriority.Add(dir);

            if (!shouldUpdateAction)
            {
                return;
            }

            SetAction(character, CharacterAction.Walk);
        }

        private static void DequeueDirection(
            Entity character,
            Directions dir,
            bool shouldUpdateAction
        )
        {
            CharacterController controller = character.Get<CharacterController>();
            controller.DirectionalPriority.Remove(dir);

            if (!shouldUpdateAction)
            {
                return;
            }

            if (controller.DirectionalPriority.Count == 0)
            {
                SetAction(character, CharacterAction.Look);
            }
            else
            {
                SetAction(character, CharacterAction.Walk);
            }
        }

        public void FindItem(Entity character, Entity item)
        {
            CharacterController controller = character.Get<CharacterController>();

            // don't want to pick up multiple items at once
            if (controller.CurrentAction == CharacterAction.Item)
            {
                return;
            }

            Pickup pickup = item.Get<Pickup>();
            pickup.Collect();

            if (_inventorySystem.ConsumablesCount(ItemType.Clock) > 0)
            {
                character.Get<Sprite>().SpriteTint = Color.Aqua;
            }

            // dont pog at unimportant items
            if (pickup.Type != PickupType.PogItem && pickup.Type != PickupType.GameWinning)
            {
                if (_world.HasEntity(item.UniqueID))
                {
                    _world.RemoveEntity(item.UniqueID);
                }
                return;
            }

            // center the item above link for pog
            Geometry.CenterSpriteOnPosition(
                item,
                Geometry.GetCenterPositionAboveSpriteInWorld(character)
            );

            // move item to front
            item.Get<Transform>().LayerDepth = TransformData.NON_MENU_DEPTH_OVERRIDE;
            SetAction(character, CharacterAction.Item);

            // don't give control back if you get the triforce, you won the game!
            if (pickup.Type == PickupType.GameWinning)
            {
                return;
            }

            StopInputThenWaitThenGiveInputBack(
                character,
                ITEM_PICKUP_TIME_MS,
                () =>
                {
                    // remove item after
                    if (_world.HasEntity(item.UniqueID))
                    {
                        _world.RemoveEntity(item.UniqueID);
                    }
                }
            );
        }

        private void StopInputThenWaitThenGiveInputBack(
            Entity character,
            int waitTimeMilliseconds,
            Action doAfterTimer = null
        )
        {
            doAfterTimer ??= () => { };

            CharacterController controller = character.Get<CharacterController>();

            controller.InputsLocked = true;

            new Task(() =>
            {
                // TODO factor out task creation that waits on the world paused
                int waitTimeLeft = waitTimeMilliseconds;
                int waitTimeDecrement = 5;
                while (waitTimeLeft > 0)
                {
                    if (!_world.IsPaused)
                    {
                        waitTimeLeft -= waitTimeDecrement;
                    }

                    Thread.Sleep(waitTimeDecrement);
                }

                doAfterTimer();

                // set link back to normal states
                SetAction(
                    character,
                    controller.DirectionalPriority.Count == 0
                        ? CharacterAction.Look
                        : CharacterAction.Walk
                );
                ChangeComponentsBasedOnController(character);

                controller.InputsLocked = false;
            }).Start();
        }

        private static void SetAction(Entity character, CharacterAction act)
        {
            CharacterController controller = character.Get<CharacterController>();

            controller.CurrentAction = act;
            ChangeCurrentDirection(controller);

            ChangeComponentsBasedOnController(character);
        }

        private static void ChangeCurrentDirection(CharacterController controller)
        {
            if (controller.DirectionalPriority.Count == 0)
            {
                return;
            }

            controller.CurrentDirection = controller.DirectionalPriority[0];
        }

        private static void ChangeComponentsBasedOnController(Entity character)
        {
            CharacterController controller = character.Get<CharacterController>();

            Directions dir = controller.CurrentDirection;
            CharacterAction act = controller.CurrentAction;

            // change sprite
            Sprite sprite = character.Get<Sprite>();
            sprite.Direction = dir;

            // change physics
            PhysicsBody body = character.Get<PhysicsBody>();

            body.Velocity =
                act == CharacterAction.Walk
                    ? Direction.DirectionToVector(dir) * controller.PlayerSpeed
                    : Vector2.Zero;

            // change animation
            Directions animDir = _linkAnimations.ContainsKey((act, dir)) ? dir : Directions.None;
            Animation newAnimation = _linkAnimations[(act, animDir)].Clone();
            sprite.CurrentFrame = newAnimation.StartFrame;
            character.Replace(newAnimation);
        }

        public static void MakeLinkDance(Entity character)
        {
            SetAction(character, CharacterAction.Dance);
        }
    }
}
