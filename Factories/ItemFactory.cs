using amongus3902.Components;
using amongus3902.Components.EnemyActions;
using amongus3902.ContentMetadata;
using amongus3902.Data;
using amongus3902.MetaClasses;
using amongus3902.Systems;
using amongus3902.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace amongus3902.Factories
{
    public enum ItemType
    {
        Compass,
        Map,
        Key,
        HeartContainer,
        TriforcePiece,
        WoodenBoomerang,
        MagicBoomerang,
        Bow,
        Heart,
        Rupee,
        Bomb,
        Fairy,
        Clock,
        Arrow,
        MagicArrow,
        Fire,
        BigRupee,
        PortalGun,
        Candle,
        None
    }

    internal class ItemFactory
    {
        private readonly World _world;
        private readonly LoadSystem _loader;
        private readonly SoundSystem _sound;
        private readonly InventorySystem _inv;

        private readonly int FAIRY_VELOCITY = 1;
        private readonly int FAIRY_MOVE_DURATION = 250;

        public ItemFactory(World world)
        {
            _world = world;
            _loader = world.GetSystem<LoadSystem>();
            _sound = world.GetSystem<SoundSystem>();
            _inv = world.GetSystem<InventorySystem>();
        }

        public Entity CreateItem(ItemType type)
        {
            return type switch
            {
                ItemType.Compass => CreateCompass(),
                ItemType.Map => CreateMap(),
                ItemType.Key => CreateKey(),
                ItemType.HeartContainer => CreateHeartContainer(),
                ItemType.TriforcePiece => CreateTriforcePiece(),
                ItemType.WoodenBoomerang => CreateWoodenBoomerang(),
                ItemType.MagicBoomerang => CreateMagicBoomerang(),
                ItemType.Bow => CreateBow(),
                ItemType.Heart => CreateHeart(),
                ItemType.Rupee => CreateRupee(),
                ItemType.Bomb => CreateBomb(),
                ItemType.Fairy => CreateFairy(),
                ItemType.Clock => CreateClock(),
                ItemType.Arrow => CreateArrow(),
                ItemType.MagicArrow => CreateMagicArrow(),
                ItemType.Fire => CreateFire(),
                ItemType.BigRupee => CreateBigRupee(),
                ItemType.PortalGun => CreatePortalGun(),
                ItemType.Candle => CreateCandle(),
                _ => throw new NotImplementedException(),
            };
        }

        private Entity MakeItem(
            ZeldaSpriteSheet sheet,
            int startFrame,
            Animation anim = null,
            PickupType type = PickupType.Consumable,
            ZeldaSound collectionSound = ZeldaSound.GetItem,
            ItemType invType = ItemType.None,
            int consumableCount = 0
        )
        {
            Entity item = new();
            item.Attach(new Sprite(_loader.GetSheet(sheet), startFrame))
                .Attach(new Transform(Vector2.Zero, TransformData.ITEM_DEPTH));

            if (anim != null)
            {
                item.Attach(anim);
            }

            Pickup pickup = new Pickup(type);
            pickup.Collected += () => _sound.PlaySound(collectionSound);
            if (invType != ItemType.None)
            {
                if (
                    type == PickupType.PogItem
                    || type == PickupType.NoPogItem
                    || type == PickupType.GameWinning
                )
                {
                    pickup.Collected += () => _inv.AddKeyItem(invType);
                }
                else
                {
                    pickup.Collected += () => InvokeConsumableFunction(invType, consumableCount);
                }
            }

            if (invType == ItemType.TriforcePiece)
            {
                pickup.Collected += () => _sound.ForceLoopingPause();
                pickup.Collected += () => ScriptedSequences.PlayLinkTriforceWinAnimation(_world);
            }

            if (invType == ItemType.Fairy)
            {
                item.Attach(new PhysicsBody());
                item.Attach(
                    new EnemyBehavior(
                        new IEnemyAction[]
                        {
                            new RandomQueenVelocityAction(FAIRY_VELOCITY, FAIRY_MOVE_DURATION)
                        }
                    )
                );
            }

            item.Attach(pickup);

            return item;
        }

        // TODO just bind these to their respective items by passing a param instead of a switch case
        private void InvokeConsumableFunction(ItemType itemType, int consumableCount)
        {
            HurtBox hurtbox = _world
                .GetEntitiesWithComponentOfTypes(typeof(CharacterController))
                .First()
                .Get<HurtBox>();

            switch (itemType)
            {
                case ItemType.HeartContainer:

                    hurtbox.MaxHealth += 2;
                    hurtbox.Heal(hurtbox.MaxHealth);

                    break;
                case ItemType.Fairy:
                    hurtbox.Heal(hurtbox.MaxHealth);
                    break;

                default:
                    _inv.AddConsumables(itemType, consumableCount);
                    break;
            }
        }

        public Entity CreateCompass() =>
            MakeItem(
                ZeldaSpriteSheet.Compass,
                0,
                null,
                PickupType.NoPogItem,
                ZeldaSound.GetItem,
                ItemType.Compass
            );

        public Entity CreateMap() =>
            MakeItem(
                ZeldaSpriteSheet.Map,
                0,
                null,
                PickupType.NoPogItem,
                ZeldaSound.GetItem,
                ItemType.Map
            );

        public Entity CreateKey() =>
            MakeItem(
                ZeldaSpriteSheet.Key,
                0,
                null,
                PickupType.Consumable,
                ZeldaSound.GetItem,
                ItemType.Key,
                1
            );

        public Entity CreateHeartContainer() =>
            MakeItem(
                ZeldaSpriteSheet.HeartContainer,
                0,
                null,
                PickupType.Consumable,
                ZeldaSound.GetItem,
                ItemType.HeartContainer
            );

        public Entity CreateTriforcePiece() =>
            MakeItem(
                ZeldaSpriteSheet.TriforcePiece,
                0,
                new Animation(0, 1),
                PickupType.GameWinning,
                ZeldaSound.NewItemFanfare,
                ItemType.TriforcePiece
            );

        public Entity CreateWoodenBoomerang() =>
            MakeItem(
                ZeldaSpriteSheet.WoodenBoomerang,
                0,
                null,
                PickupType.PogItem,
                ZeldaSound.NewItemFanfare,
                ItemType.WoodenBoomerang
            );

        public Entity CreateMagicBoomerang()
        {
            Entity result = MakeItem(
                ZeldaSpriteSheet.MagicBoomerang,
                0,
                null,
                PickupType.PogItem,
                ZeldaSound.NewItemFanfare
            );

            result.Get<Pickup>().Collected += () =>
            {
                _inv.AddKeyItem(ItemType.MagicBoomerang);
                if (_inv.GetActiveItem() == ItemType.WoodenBoomerang)
                {
                    _inv.TrySetActiveItem(ItemType.MagicBoomerang);
                }
            };

            return result;

        }
            

        public Entity CreateBow() =>
            MakeItem(
                ZeldaSpriteSheet.Bow,
                0,
                null,
                PickupType.PogItem,
                ZeldaSound.NewItemFanfare,
                ItemType.Bow
            );

        public Entity CreateHeart()
        {
            Entity heart = MakeItem(
                ZeldaSpriteSheet.Heart,
                1,
                new Animation(0, 1),
                PickupType.Heart,
                ZeldaSound.GetHeart
            );

            heart.Get<Pickup>().Collected += () =>
            {
                List<Entity> characters = _world.GetEntitiesWithComponentOfTypes(
                    typeof(CharacterController)
                );

                    HurtBox hurtbox = characters[0].Get<HurtBox>();
                    int currentHealth = hurtbox.Health;
                    int maxHealth = hurtbox.MaxHealth;

                    hurtbox.Heal(2);
            };

            return heart;
        }

        public Entity CreateRupee() =>
            MakeItem(
                ZeldaSpriteSheet.Rupee,
                1,
                new Animation(0, 1),
                PickupType.Rupee,
                ZeldaSound.GetRupee,
                ItemType.Rupee,
                1
            );

        public Entity CreateBomb() =>
            MakeItem(
                ZeldaSpriteSheet.Bomb,
                0,
                null,
                PickupType.Consumable,
                ZeldaSound.GetItem,
                ItemType.Bomb,
                1
            );

        public Entity CreateFairy() =>
            MakeItem(
                ZeldaSpriteSheet.Fairy,
                1,
                new Animation(0, 1),
                PickupType.Consumable,
                ZeldaSound.GetItem,
                ItemType.Fairy
            );

        public Entity CreateClock() =>
            MakeItem(
                ZeldaSpriteSheet.Clock,
                0,
                null,
                PickupType.Consumable,
                ZeldaSound.GetItem,
                ItemType.Clock,
                1
            );

        public Entity CreateArrow() =>
            MakeItem(
                ZeldaSpriteSheet.Arrow,
                0,
                null,
                PickupType.NoPogItem,
                ZeldaSound.GetItem,
                ItemType.Arrow
            );

        public Entity CreateMagicArrow() =>
            MakeItem(
                ZeldaSpriteSheet.MagicArrowRL,
                0,
                null,
                PickupType.PogItem,
                ZeldaSound.NewItemFanfare,
                ItemType.MagicArrow
            );

        public Entity CreateCandle() =>
            MakeItem(
                ZeldaSpriteSheet.Candle,
                0,
                null,
                PickupType.PogItem,
                ZeldaSound.NewItemFanfare,
                ItemType.Candle
            );

        public Entity CreateFire()
        {
            // ideally fire would be a block, buts its already implemented as an item
            Entity fire = MakeItem(ZeldaSpriteSheet.Fire, 1, new Animation(0, 1));

            fire.Detatch<Pickup>();

            return fire;
        }

        public Entity CreateBigRupee() =>
            MakeItem(
                ZeldaSpriteSheet.Rupee,
                1,
                null,
                PickupType.Rupee,
                ZeldaSound.GetRupee,
                ItemType.Rupee,
                5
            );

        public Entity CreatePortalGun() =>
            MakeItem(
                ZeldaSpriteSheet.PortalGun,
                3,
                null,
                PickupType.PogItem,
                ZeldaSound.GetItem,
                ItemType.PortalGun
            );
    }
}
