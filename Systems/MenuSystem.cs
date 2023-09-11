using amongus3902.Components;
using amongus3902.ContentMetadata;
using amongus3902.Data;
using amongus3902.Factories;
using amongus3902.MetaClasses;
using amongus3902.Systems.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace amongus3902.Systems
{
    internal class MenuSystem : IAlwaysActiveSystem, IUpdateSystem
    {
        private World _world;
        private LoadSystem _loader;
        private Entity _menu;
        private RoomName _roomName;
        private InventorySystem _inventorySystem;

        private Entity RoomHighlight;
        private bool JustGotMap,
            JustGotCompass;

        private readonly float SCALE = 3;

        //added in for pause system testing
        private readonly Vector2 MapCoords = new Vector2(40, 54);
        private readonly float MAP_SCALE = 4.5f;

        private readonly Dictionary<char, RoomName> CompassDest =
            new()
            {
                { 'e', RoomName.e5_1 },
                { 'd', RoomName.debug },
                { 'b', RoomName.basement },
                { 'p', RoomName.portalroom }
            };

        public void Start(World world)
        {
            _world = world;
            _loader = _world.GetSystem<LoadSystem>();
            _roomName = RoomGenerationSystem.GetRoomData().Name;
            _inventorySystem = _world.GetSystem<InventorySystem>();

            _world.AddEntity(MakeMenu(SCALE));

            _menu = _world.GetEntitiesWithComponentOfTypes(typeof(Menu)).First();
            JustGotCompass = false;
            JustGotMap = false;

            SetCounters();

            SetActiveItems();

            SetHearts();

            RoomHighlight = new Entity()
                .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.HighlightNoMap)))
                .Attach(new Map(true))
                .Attach(new Transform(Map.MapCoords, TransformData.HUD_MAP_PLAYER_DEPTH, SCALE));
        }

        public void Update(GameTime gameTime)
        {
            Menu menu = _menu.Get<Menu>();
            bool ChangeMenu = false;

            _roomName = RoomGenerationSystem.GetRoomData().Name;

            char CurrentLevel = Enum.GetName(_roomName)[0];

            ModifyHearts();
            SetCounts(menu);
            SetActiveItems(menu);

            if (JustCollected(ItemType.Map, JustGotMap) || CurrentLevel != menu.MapLevel)
            {
                if (menu.MapLevel != '!')
                    ChangeMenu = true;

                menu.MapLevel = CurrentLevel;
                ResetMaps();

                if (_inventorySystem.HasKeyItem(ItemType.Map))
                {
                    JustGotMap = true;
                    _world.AddEntity(CreateMap(CurrentLevel));
                }
                else
                    _world.AddEntity(
                        new Entity()
                            .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.NoMap), 0))
                            .Attach(new Map())
                            .Attach(
                                new Transform(
                                    Map.MapCoords,
                                    TransformData.HUD_ADDONS_DEPTH,
                                    MAP_SCALE
                                )
                            )
                            .Attach(new MenuObject())
                    );

                _world.AddEntity(RoomHighlight);
            }

            if (
                JustCollected(ItemType.Compass, JustGotCompass)
                || (ChangeMenu && _inventorySystem.HasKeyItem(ItemType.Compass))
            )
            {
                JustGotCompass = true;

                Entity compass = new Entity()
                    .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.HighlightWithMap), 0))
                    .Attach(
                        new Transform(
                            SetIconPos(CompassDest[CurrentLevel]),
                            TransformData.HUD_MAP_COMPASS_DEPTH,
                            SCALE
                        )
                    )
                    .Attach(new Animation(0, 1, true))
                    .Attach(new Map());

                _world.AddEntity(compass);
            }

            HighlightCurrentRoom();
        }

        public bool JustCollected(ItemType it, bool owned)
        {
            return !owned && _inventorySystem.HasKeyItem(it);
        }

        private Entity CreateMap(char levelToMap)
        {
            Entity map = new();
            ZeldaSpriteSheet spriteSheet = new();

            switch (levelToMap)
            {
                case 'e':
                    spriteSheet = ZeldaSpriteSheet.LevelOneMap;
                    break;

                default:
                    spriteSheet = ZeldaSpriteSheet.NoMap;
                    break;
            }
            map.Attach(new Sprite(_loader.GetSheet(spriteSheet), 0))
                .Attach(new Map())
                .Attach(new Transform(Map.MapCoords, TransformData.HUD_ADDONS_DEPTH, MAP_SCALE))
                .Attach(new Menu());

            return map;
        }

        private void SetActiveItems()
        {
            _menu.Get<Menu>().ActiveItems[0] = new Entity()
                .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.ActiveWoodenSword), 0))
                .Attach(
                    new Transform(
                        new Vector2(Menu.ActiveCoords.X, Menu.ActiveCoords.Y),
                        TransformData.HUD_ADDONS_DEPTH,
                        SCALE
                    )
                )
                .Attach(new Menu());

            _menu.Get<Menu>().ActiveItems[1] = new Entity()
                .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.ActiveWoodenSword), 0))
                .Attach(
                    new Transform(
                        new Vector2(Menu.ActiveCoords.X + Menu.ActiveScale, Menu.ActiveCoords.Y),
                        TransformData.HUD_ADDONS_DEPTH,
                        SCALE
                    )
                )
                .Attach(new Menu());

            for (int i = 0; i < _menu.Get<Menu>().ActiveItems.Length; i++)
                _world.AddEntity(_menu.Get<Menu>().ActiveItems[i]);
        }

        private void SetCounters()
        {
            //rupee count
            _world.AddEntity(
                new Entity()
                    .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.XCount), 0))
                    .Attach(new Transform(Menu.RupeeCoords, TransformData.HUD_ADDONS_DEPTH, SCALE))
                    .Attach(new Menu())
            );

            //key count
            _world.AddEntity(
                new Entity()
                    .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.XCount), 0))
                    .Attach(new Transform(Menu.KeyCoords, TransformData.HUD_ADDONS_DEPTH, SCALE))
                    .Attach(new Menu())
            );

            //bomb count
            _world.AddEntity(
                new Entity()
                    .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.XCount), 0))
                    .Attach(new Transform(Menu.BombCoords, TransformData.HUD_ADDONS_DEPTH, SCALE))
                    .Attach(new Menu())
            );

            for (int i = 0; i < _menu.Get<Menu>().RupeeCount.Length; i++)
            {
                _menu.Get<Menu>().RupeeCount[i] = new Entity()
                    .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.NoHeart), 0))
                    .Attach(
                        new Transform(
                            new Vector2(
                                Menu.RupeeCoords.X + Menu.HeartDiff * (i + 1),
                                Menu.RupeeCoords.Y
                            ),
                            TransformData.HUD_ADDONS_DEPTH,
                            SCALE
                        )
                    )
                    .Attach(new Menu());

                _world.AddEntity(_menu.Get<Menu>().RupeeCount[i]);

                _menu.Get<Menu>().KeyCount[i] = new Entity()
                    .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.NoHeart), 0))
                    .Attach(
                        new Transform(
                            new Vector2(
                                Menu.KeyCoords.X + Menu.HeartDiff * (i + 1),
                                Menu.KeyCoords.Y
                            ),
                            TransformData.HUD_ADDONS_DEPTH,
                            SCALE
                        )
                    )
                    .Attach(new Menu());

                _world.AddEntity(_menu.Get<Menu>().KeyCount[i]);

                _menu.Get<Menu>().BombCount[i] = new Entity()
                    .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.NoHeart), 0))
                    .Attach(
                        new Transform(
                            new Vector2(
                                Menu.BombCoords.X + Menu.HeartDiff * (i + 1),
                                Menu.BombCoords.Y
                            ),
                            TransformData.HUD_ADDONS_DEPTH,
                            SCALE
                        )
                    )
                    .Attach(new Menu());

                _world.AddEntity(_menu.Get<Menu>().BombCount[i]);
            }
        }

        private void ResetMaps()
        {
            List<Entity> EntityList = _world.GetEntitiesWithComponentOfTypes(typeof(Map));
            foreach (Entity entity in EntityList)
            {
                _world.RemoveEntity(entity.UniqueID);
            }
        }

        private Entity MakeMenu(float scale)
        {
            Entity menu = new();
            menu.Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.TopDisplay), 0))
                .Attach(new Transform(Vector2.Zero, TransformData.HUD_BG_DEPTH, scale))
                .Attach(new Menu());
            return menu;
        }

        private void SetHearts()
        {
            Menu menu = _menu.Get<Menu>();

            List<Entity> characters = _world.GetEntitiesWithComponentOfTypes(
                typeof(CharacterController)
            );

            if (characters.Count == 0)
            {
                return;
            }

            HurtBox hurtbox = characters.First().Get<HurtBox>();

            SetHeartSheet(hurtbox.Health, hurtbox.MaxHealth - hurtbox.Health, menu);

            for (int i = 0; i < menu.Hearts.Length; i++)
            {
                Entity heart = new();
                heart
                    .Attach(new Sprite(_loader.GetSheet(menu.HeartSpriteSheets[i]), 0))
                    .Attach(
                        new Transform(
                            new Vector2(
                                Menu.HeartInitX + Menu.HeartDiff * (i % 8),
                                Menu.HeartInitY - Menu.HeartDiff * (i / 8)
                            ),
                            TransformData.HUD_ADDONS_DEPTH,
                            SCALE
                        )
                    )
                    .Attach(new Menu());

                _world.AddEntity(heart);

                menu.Hearts[i] = heart;
            }
        }

        private void SetHeartSheet(int health, int damage, Menu menu)
        {
            for (int i = 0; i < menu.HeartSpriteSheets.Length; i++)
            {
                if (health > 1)
                {
                    menu.HeartSpriteSheets[i] = ZeldaSpriteSheet.FullHeart;
                    health -= 2;
                }
                else if (health == 1)
                {
                    menu.HeartSpriteSheets[i] = ZeldaSpriteSheet.HalfHeart;
                    health--;
                }
                else if (health == 0 && damage > 1)
                {
                    menu.HeartSpriteSheets[i] = ZeldaSpriteSheet.EmptyHeart;
                    damage -= 2;
                }
                else
                    menu.HeartSpriteSheets[i] = ZeldaSpriteSheet.NoHeart;
            }
        }

        public void ModifyHearts()
        {
            Menu menu = _menu.Get<Menu>();
            List<Entity> characters = _world.GetEntitiesWithComponentOfTypes(
                typeof(CharacterController)
            );

            if (characters.Count == 0)
            {
                return;
            }

            HurtBox hurtbox = characters.First().Get<HurtBox>();

            SetHeartSheet(hurtbox.Health, hurtbox.MaxHealth - hurtbox.Health, menu);

            for (int i = 0; i < menu.Hearts.Length; i++)
                menu.Hearts[i].Get<Sprite>().Sheet = _loader.GetSheet(menu.HeartSpriteSheets[i]);
        }

        public void SetCounts(Menu menu)
        {
            int BombCount = _inventorySystem.ConsumablesCount(Factories.ItemType.Bomb),
                RupeeCount = _inventorySystem.ConsumablesCount(Factories.ItemType.Rupee),
                KeyCount = _inventorySystem.ConsumablesCount(Factories.ItemType.Key);

            EditSheets(BombCount, menu.BombCount);
            EditSheets(RupeeCount, menu.RupeeCount);
            EditSheets(KeyCount, menu.KeyCount);
        }

        public void EditSheets(int itemCount, Entity[] numbers)
        {
            if (itemCount / 10 == 0)
            {
                numbers[1].Get<Sprite>().Sheet = _loader.GetSheet(ZeldaSpriteSheet.NoHeart);
                SetNumberSheet(numbers[0].Get<Sprite>(), itemCount % 10);
            }
            else
            {
                SetNumberSheet(numbers[1].Get<Sprite>(), itemCount % 10);
                SetNumberSheet(numbers[0].Get<Sprite>(), itemCount / 10);
            }
        }

        public void SetNumberSheet(Sprite spr, int itemCount)
        {
            SpriteSheet[] sheets = new[]
            {
                _loader.GetSheet(ZeldaSpriteSheet.ZeroCount),
                _loader.GetSheet(ZeldaSpriteSheet.OneCount),
                _loader.GetSheet(ZeldaSpriteSheet.TwoCount),
                _loader.GetSheet(ZeldaSpriteSheet.ThreeCount),
                _loader.GetSheet(ZeldaSpriteSheet.FourCount),
                _loader.GetSheet(ZeldaSpriteSheet.FiveCount),
                _loader.GetSheet(ZeldaSpriteSheet.SixCount),
                _loader.GetSheet(ZeldaSpriteSheet.SevenCount),
                _loader.GetSheet(ZeldaSpriteSheet.EightCount),
                _loader.GetSheet(ZeldaSpriteSheet.NineCount),
            };

            spr.Sheet = sheets[itemCount];
        }

        public void SetActiveItems(Menu menu)
        {
            Sprite SpriteB = menu.ActiveItems[0].Get<Sprite>();

            ItemType ActiveItem = _inventorySystem.GetActiveItem();

            switch (ActiveItem)
            {
                case ItemType.MagicBoomerang:
                    SpriteB.Sheet = _loader.GetSheet(ZeldaSpriteSheet.ActiveMagicBoomerang);
                    break;
                case ItemType.WoodenBoomerang:
                    SpriteB.Sheet = _loader.GetSheet(ZeldaSpriteSheet.ActiveWoodenBoomerang);
                    break;
                case ItemType.Bow:
                    if (_inventorySystem.HasKeyItem(ItemType.MagicArrow))
                    {
                        SpriteB.Sheet = _loader.GetSheet(ZeldaSpriteSheet.ActiveMagicArrow);
                    }
                    else if (_inventorySystem.HasKeyItem(ItemType.Arrow))
                    {
                        SpriteB.Sheet = _loader.GetSheet(ZeldaSpriteSheet.ActiveWoodenArrow);
                    }
                    break;
                case ItemType.Bomb:
                    SpriteB.Sheet = _loader.GetSheet(ZeldaSpriteSheet.ActiveBomb);
                    break;
                case ItemType.PortalGun:
                    SpriteB.Sheet = _loader.GetSheet(ZeldaSpriteSheet.ActivePortalGun);
                    break;
                case ItemType.Candle:
                    SpriteB.Sheet = _loader.GetSheet(ZeldaSpriteSheet.ActiveCandle);
                    break;
                case ItemType.None:
                    SpriteB.Sheet = _loader.GetSheet(ZeldaSpriteSheet.ActiveNoItem);
                    break;
                default:
                    SpriteB.Sheet = _loader.GetSheet(ZeldaSpriteSheet.ActiveNoItem);
                    break;
            }
        }

        private Vector2 SetIconPos(RoomName room)
        {
            int XFactor = room.ToString()[1] - '0',
                YFactor = room.ToString()[3] - '0';

            return new Vector2(
                    Map.MapCoords.X + (Map.XScale * MAP_SCALE * XFactor),
                    Map.MapCoords.Y + (Map.YScale * MAP_SCALE * YFactor)
                )
                + Map.Offset * MAP_SCALE;
        }

        public void HighlightCurrentRoom()
        {
            RoomName CurrentRoom = RoomGenerationSystem.GetRoomData().Name;
            Transform trans = RoomHighlight.Get<Transform>();

            trans.Position = SetIconPos(CurrentRoom);
        }
    }
}
