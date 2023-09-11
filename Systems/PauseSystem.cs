using amongus3902.Components;
using amongus3902.ContentMetadata;
using amongus3902.Data;
using amongus3902.Factories;
using amongus3902.MetaClasses;
using amongus3902.Systems.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace amongus3902.Systems
{
    internal class PauseSystem : IUpdateSystem, IAlwaysActiveSystem
    {
        private World _world;
        private LoadSystem _loader;
        private InputSystem _inputSystem;
        private RoomGenerationSystem _roomGen;
        private RoomData _roomData;

        private InventorySystem _inventorySystem;
        private List<ItemType> _inventory;

        private Entity activeItem;
        private Entity pauseSprite;
        private Entity selectSquare;

        private float SCALE;

        private int _itemIndex = 0;
        private int YValue = -510;

        private bool _pauseMenuActive = false;

        private List<Entity> _pauseScreenEntities = new();
        private List<Entity> _menuList = new();
        public void Start(World world)
        {
            _world = world;
            _loader = _world.GetSystem<LoadSystem>();
            _inventorySystem = _world.GetSystem<InventorySystem>();
            _inputSystem = _world.GetSystem<InputSystem>();
            _roomGen = _world.GetSystem<RoomGenerationSystem>();

            SCALE = world.SCREEN_WIDTH / Game1.NES_SCREEN_SIZE.X;

            //create pause screen
            pauseSprite = new Entity()
                .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.PauseScreen), 0))
                .Attach(new Transform(new Vector2(0, YValue), TransformData.PAUSE_MENU_BG_DEPTH, SCALE));
            _world.AddEntity(pauseSprite);
        }

        public void Update(GameTime gameTime)
        {
            if (_pauseMenuActive)
            {
                ScrollDown();
            }
            if (!_pauseMenuActive)
            {
                ScrollUp();
            }
        }

        public void TogglePauseScreen()
        {
            if (_pauseMenuActive)
            {
                RemovePauseScreen();
            }
            else
            {
                _world.IsPaused = true;
                MakePauseScreen();
                _pauseMenuActive = true;
            }
        }

        public void ToggleActiveItems()
        {
            if (_pauseMenuActive)
            {
                _itemIndex++;
                SwitchItem();
            }
        }

        private void ScrollDown()
        {
            if (YValue < 0)
            {
                YValue += 8;
                pauseSprite.Replace(new Transform(new Vector2(0, YValue), TransformData.PAUSE_MENU_BG_DEPTH, SCALE));

                //move menu
                _menuList = _world.GetEntitiesWithComponentOfTypes(typeof(Menu));
                foreach (Entity e in _menuList)
                {
                    Transform current = e.Get<Transform>();
                    float x = current.Position.X;
                    float y = current.Position.Y;
                    float layer = current.LayerDepth;
                    float scale = current.Scale;
                    y += 8;
                    e.Replace(new Transform(new Vector2(x, y), layer, scale));
                }
            }
            else
            {
                AddPauseScreen();
            }
        }

        private void ScrollUp()
        {
            if (YValue > -510)
            {
                YValue -= 8;
                pauseSprite.Replace(new Transform(new Vector2(0, YValue), TransformData.PAUSE_MENU_BG_DEPTH, SCALE));

                //move menu
                _menuList = _world.GetEntitiesWithComponentOfTypes(typeof(Menu));
                foreach (Entity e in _menuList)
                {
                    Transform current = e.Get<Transform>();
                    float x = current.Position.X;
                    float y = current.Position.Y;
                    float layer = current.LayerDepth;
                    float scale = current.Scale;
                    y -= 8;
                    e.Replace(new Transform(new Vector2(x, y), layer, scale));
                }
            }
            else
            {
                _world.IsPaused = false;
            }
        }

        private void AddPauseScreen()
        {
            foreach (Entity e in _pauseScreenEntities)
            {
                if (!_world.HasEntity(e.UniqueID))
                {
                    _world.AddEntity(e);
                }
            }
            _pauseMenuActive = true;
        }

        private void RemovePauseScreen()
        {
            foreach (Entity e in _pauseScreenEntities)
            {
                if (_world.HasEntity(e.UniqueID))
                {
                    _world.RemoveEntity(e.UniqueID);
                }
            }
            _pauseMenuActive = false;
        }

        private void MakePauseScreen()
        {
            _pauseScreenEntities.Clear();

            ItemType ActiveItem = _inventorySystem.GetActiveItem();

            //active item
            _pauseScreenEntities.Add(
                activeItem = new Entity()
                    .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.ActiveNoItem), 0))
                    .Attach(new Transform(new Vector2(211, 150), TransformData.PAUSE_MENU_ADDONS_DEPTH, SCALE))
            );

            _pauseScreenEntities.Add(
                selectSquare = new Entity()
                    .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.ItemSelect), 1))
                    .Attach(new Transform())
                    .Attach(new Animation(0, 1, 50))
            );

            SwitchItem();

            //display map and compass
            if (_inventorySystem.HasKeyItem(ItemType.Map))
            {
                _pauseScreenEntities.Add(
                    new Entity()
                        .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.Map), 0))
                        .Attach(new Transform(new Vector2(135, 340), TransformData.PAUSE_MENU_ADDONS_DEPTH, SCALE))
                );
            }
            if (_inventorySystem.HasKeyItem(ItemType.Compass))
            {
                _pauseScreenEntities.Add(
                    new Entity()
                        .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.Compass), 0))
                        .Attach(new Transform(new Vector2(135, 460), TransformData.PAUSE_MENU_ADDONS_DEPTH, SCALE))
                );
            }

            //display current inventory items
            if (_inventorySystem.HasConsumable(ItemType.Bomb))
            {
                _pauseScreenEntities.Add(
                    new Entity()
                        .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.Bomb), 0))
                        .Attach(new Transform(new Vector2(465, 145), TransformData.PAUSE_MENU_ADDONS_DEPTH, SCALE))
                );
            }
            if (
                (
                    _inventorySystem.HasKeyItem(ItemType.Arrow)
                    || _inventorySystem.HasKeyItem(ItemType.MagicArrow)
                ) && _inventorySystem.HasKeyItem(ItemType.Bow)
            )
            {
                if (_inventorySystem.HasKeyItem(ItemType.MagicArrow))
                {
                    _pauseScreenEntities.Add(
                        new Entity()
                            .Attach(
                                new Sprite(_loader.GetSheet(ZeldaSpriteSheet.ActiveMagicArrow), 0)
                            )
                            .Attach(new Transform(new Vector2(530, 146), TransformData.PAUSE_MENU_ADDONS_DEPTH, SCALE))
                    );
                }
                else
                {
                    _pauseScreenEntities.Add(
                        new Entity()
                            .Attach(
                                new Sprite(_loader.GetSheet(ZeldaSpriteSheet.ActiveWoodenArrow), 0)
                            )
                            .Attach(new Transform(new Vector2(530, 146), TransformData.PAUSE_MENU_ADDONS_DEPTH, SCALE))
                    );
                }

                _pauseScreenEntities.Add(
                    new Entity()
                        .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.Bow), 0))
                        .Attach(new Transform(new Vector2(550, 146), TransformData.PAUSE_MENU_ADDONS_DEPTH, SCALE))
                );
            }
            if (_inventorySystem.HasKeyItem(ItemType.MagicBoomerang))
            {
                _pauseScreenEntities.Add(
                    new Entity()
                        .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.ActiveMagicBoomerang), 0))
                        .Attach(new Transform(new Vector2(400, 146), TransformData.PAUSE_MENU_ADDONS_DEPTH, SCALE))
                );
            }
            else if (_inventorySystem.HasKeyItem(ItemType.WoodenBoomerang))
            {
                _pauseScreenEntities.Add(
                    new Entity()
                        .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.ActiveWoodenBoomerang), 0))
                        .Attach(new Transform(new Vector2(400, 146), TransformData.PAUSE_MENU_ADDONS_DEPTH, SCALE))
                );
            }
            if (_inventorySystem.HasKeyItem(ItemType.PortalGun))
            {
                _pauseScreenEntities.Add(
                    new Entity()
                        .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.PortalGun), 0))
                        .Attach(new Transform(new Vector2(600, 140), TransformData.PAUSE_MENU_ADDONS_DEPTH, SCALE))
                );
            }
            if (_inventorySystem.HasKeyItem(ItemType.Candle))
            {
                _pauseScreenEntities.Add(
                    new Entity()
                        .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.ActiveCandle), 0))
                        .Attach(new Transform(new Vector2(400, 194), TransformData.PAUSE_MENU_ADDONS_DEPTH, SCALE))
                );
            }

            UpdateMapPiece(_roomGen.GetMadeRooms()); 
        }

        private void UpdateMapPiece(List<RoomName> made)
        {
            foreach (var item in made)
            {
                switch (item)
                {
                    case RoomName.e1_5:
                        _pauseScreenEntities.Add(new Entity()
                        .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.PauseMapPieces), 1))
                        .Attach(new Transform(new Vector2(436, 459), TransformData.PAUSE_MENU_ADDONS_DEPTH, SCALE)));
                        break;
                    case RoomName.e2_5:
                        _pauseScreenEntities.Add(new Entity()
                        .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.PauseMapPieces), 15))
                        .Attach(new Transform(new Vector2(460, 459), TransformData.PAUSE_MENU_ADDONS_DEPTH, SCALE)));
                        break;
                    case RoomName.e3_5:
                        _pauseScreenEntities.Add(new Entity()
                        .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.PauseMapPieces), 2))
                        .Attach(new Transform(new Vector2(483, 459), TransformData.PAUSE_MENU_ADDONS_DEPTH, SCALE)));
                        break;
                    case RoomName.e2_4:
                        _pauseScreenEntities.Add(new Entity()
                        .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.PauseMapPieces), 12))
                        .Attach(new Transform(new Vector2(460, 436), TransformData.PAUSE_MENU_ADDONS_DEPTH, SCALE)));
                        break;
                    case RoomName.e2_3:
                        _pauseScreenEntities.Add(new Entity()
                        .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.PauseMapPieces), 11))
                        .Attach(new Transform(new Vector2(460, 412), TransformData.PAUSE_MENU_ADDONS_DEPTH, SCALE)));
                        break;
                    case RoomName.e1_3:
                        _pauseScreenEntities.Add(new Entity()
                        .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.PauseMapPieces), 8))
                        .Attach(new Transform(new Vector2(436, 411), TransformData.PAUSE_MENU_ADDONS_DEPTH, SCALE)));
                        break;
                    case RoomName.e1_2:
                        _pauseScreenEntities.Add(new Entity()
                        .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.PauseMapPieces), 5))
                        .Attach(new Transform(new Vector2(436, 388), TransformData.PAUSE_MENU_ADDONS_DEPTH, SCALE)));
                        break;
                    case RoomName.e0_2:
                        _pauseScreenEntities.Add(new Entity()
                        .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.PauseMapPieces), 1))
                        .Attach(new Transform(new Vector2(411, 388), TransformData.PAUSE_MENU_ADDONS_DEPTH, SCALE)));
                        break;
                    case RoomName.e2_2:
                        _pauseScreenEntities.Add(new Entity()
                        .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.PauseMapPieces), 11))
                        .Attach(new Transform(new Vector2(460, 388), TransformData.PAUSE_MENU_ADDONS_DEPTH, SCALE)));
                        break;
                    case RoomName.e3_2:
                        _pauseScreenEntities.Add(new Entity()
                        .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.PauseMapPieces), 3))
                        .Attach(new Transform(new Vector2(484, 388), TransformData.PAUSE_MENU_ADDONS_DEPTH, SCALE)));
                        break;
                    case RoomName.e4_2:
                        _pauseScreenEntities.Add(new Entity()
                        .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.PauseMapPieces), 10))
                        .Attach(new Transform(new Vector2(506, 388), TransformData.PAUSE_MENU_ADDONS_DEPTH, SCALE)));
                        break;
                    case RoomName.e4_1:
                        _pauseScreenEntities.Add(new Entity()
                        .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.PauseMapPieces), 4))
                        .Attach(new Transform(new Vector2(506, 364), TransformData.PAUSE_MENU_ADDONS_DEPTH, SCALE)));
                        break;
                    case RoomName.e2_1:
                        _pauseScreenEntities.Add(new Entity()
                        .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.PauseMapPieces), 12))
                        .Attach(new Transform(new Vector2(460, 364), TransformData.PAUSE_MENU_ADDONS_DEPTH, SCALE)));
                        break;
                    case RoomName.e2_0:
                        _pauseScreenEntities.Add(new Entity()
                        .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.PauseMapPieces), 4))
                        .Attach(new Transform(new Vector2(460, 340), TransformData.PAUSE_MENU_ADDONS_DEPTH, SCALE)));
                        break;
                    case RoomName.e1_0:
                        _pauseScreenEntities.Add(new Entity()
                        .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.PauseMapPieces), 1))
                        .Attach(new Transform(new Vector2(436, 340), TransformData.PAUSE_MENU_ADDONS_DEPTH, SCALE)));
                        break;
                    case RoomName.e5_1:
                        _pauseScreenEntities.Add(new Entity()
                        .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.PauseMapPieces), 2))
                        .Attach(new Transform(new Vector2(536, 364), TransformData.PAUSE_MENU_ADDONS_DEPTH, SCALE)));
                        break;
                    case RoomName.e3_3:
                        _pauseScreenEntities.Add(new Entity()
                        .Attach(new Sprite(_loader.GetSheet(ZeldaSpriteSheet.PauseMapPieces), 10))
                        .Attach(new Transform(new Vector2(484, 412), TransformData.PAUSE_MENU_ADDONS_DEPTH, SCALE)));
                        break;

                }
            }
             
        }

        public void SwitchItem()
        {
            //set up list depending on what is in Link's inventory
            _inventory = _inventorySystem.GetKeyItems();

            Dictionary<int, ItemType> inventoryOrder = new();
            const int MAX_INVENTORY_COUNT = 5;

            if (_inventory.Contains(ItemType.MagicBoomerang))
            {
                inventoryOrder.Add(0, ItemType.MagicBoomerang);
            }
            else if (_inventory.Contains(ItemType.WoodenBoomerang))
            {
                inventoryOrder.Add(0, ItemType.WoodenBoomerang);
            }
            if (_inventorySystem.HasConsumable(ItemType.Bomb))
            {
                inventoryOrder.Add(1, ItemType.Bomb);
            }
            if (
                _inventory.Contains(ItemType.Bow)
                && (_inventory.Contains(ItemType.Arrow) || _inventory.Contains(ItemType.MagicArrow))
            )
            {
                inventoryOrder.Add(2, ItemType.Bow);
            }
            if (_inventory.Contains(ItemType.PortalGun))
            {
                inventoryOrder.Add(3, ItemType.PortalGun);
            }
            if (_inventory.Contains(ItemType.Candle))
            {
                inventoryOrder.Add(4, ItemType.Candle);
            }

            // dont do anything if no items
            if (inventoryOrder.Count == 0)
            {
                return;
            }

            if (inventoryOrder.Count > 0)
            {
                if (_itemIndex >= MAX_INVENTORY_COUNT)
                {
                    _itemIndex = 0;
                }

                bool hasItem = inventoryOrder.TryGetValue(_itemIndex, out ItemType currentItem);

                // try switching to next item if this one isnt available
                if (!hasItem)
                {
                    _itemIndex++;
                    SwitchItem();
                    return;
                }

                Sprite current = new Sprite(_loader.GetSheet(ZeldaSpriteSheet.ActiveNoItem), 0);
                Transform inventorySelect = new();

                switch (currentItem)
                {
                    case ItemType.MagicBoomerang:
                        current.Sheet = _loader.GetSheet(ZeldaSpriteSheet.ActiveMagicBoomerang);
                        inventorySelect = new Transform(new Vector2(390, 145), TransformData.PAUSE_MENU_RETICLE_DEPTH, SCALE);
                        break;
                    case ItemType.WoodenBoomerang:
                        current.Sheet = _loader.GetSheet(ZeldaSpriteSheet.ActiveWoodenBoomerang);
                        inventorySelect = new Transform(new Vector2(390, 145), TransformData.PAUSE_MENU_RETICLE_DEPTH, SCALE);
                        break;

                    case ItemType.Bow:
                        if(_inventorySystem.HasKeyItem(ItemType.MagicArrow))
                        {
                            current.Sheet = _loader.GetSheet(ZeldaSpriteSheet.ActiveMagicArrow);
                            inventorySelect = new Transform(new Vector2(530, 145), TransformData.PAUSE_MENU_RETICLE_DEPTH, SCALE);
                        } 
                        else if (_inventorySystem.HasKeyItem(ItemType.Arrow))
                        {
                            current.Sheet = _loader.GetSheet(ZeldaSpriteSheet.ActiveWoodenArrow);
                            inventorySelect = new Transform(new Vector2(530, 145), TransformData.PAUSE_MENU_RETICLE_DEPTH, SCALE);
                        }
                        break;

                    case ItemType.Bomb:
                        current.Sheet = _loader.GetSheet(ZeldaSpriteSheet.ActiveBomb);
                        inventorySelect = new Transform(new Vector2(465, 145), TransformData.PAUSE_MENU_RETICLE_DEPTH, SCALE);
                        break;

                    case ItemType.PortalGun:
                        current.Sheet = _loader.GetSheet(ZeldaSpriteSheet.PortalGun);
                        inventorySelect = new Transform(new Vector2(600, 145), TransformData.PAUSE_MENU_RETICLE_DEPTH, SCALE);
                        break;
                    case ItemType.Candle:
                        current.Sheet = _loader.GetSheet(ZeldaSpriteSheet.Candle);
                        inventorySelect = new Transform(new Vector2(390, 193), TransformData.PAUSE_MENU_RETICLE_DEPTH, SCALE);
                        break;
                }

                activeItem.Replace(current);
                selectSquare.Replace(inventorySelect);
                _inventorySystem.TrySetActiveItem(currentItem);
            }
        }
    }
}
