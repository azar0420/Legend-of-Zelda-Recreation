using amongus3902.Components;
using amongus3902.Data;
using amongus3902.MetaClasses;
using amongus3902.Systems;
using amongus3902.Utils;
using Microsoft.Xna.Framework;
using System;

namespace amongus3902.Factories
{
    internal class RoomBorderFactory
    {
        private readonly LoadSystem _loader;
        private readonly SoundSystem _sound;
        private readonly InventorySystem _inventory;
        private readonly RoomGenerationSystem _roomGenerationSystem;
        private readonly Vector2 _screenSize;

        private readonly float BORDER_TILE_MULT = 2;

        public RoomBorderFactory(World world)
        {
            _loader = world.GetSystem<LoadSystem>();
            _inventory = world.GetSystem<InventorySystem>();
            _roomGenerationSystem = world.GetSystem<RoomGenerationSystem>();
            _screenSize = new Vector2(world.SCREEN_WIDTH, world.SCREEN_HEIGHT);
            _sound = world.GetSystem<SoundSystem>();
        }

        public RoomStorage CreateBorder(
            (DoorState state, RoomName dest) northDoor,
            (DoorState state, RoomName dest) westDoor,
            (DoorState state, RoomName dest) eastDoor,
            (DoorState state, RoomName dest) southDoor,
            RoomName roomName,
            Action<RoomName, Vector2> switchRoom,
            out Vector2 tileOrigin,
            out float scale,
            bool isBasement = false
        )
        {
            RoomStorage result = new();
            ZeldaSpriteSheet borderSprite = ZeldaSpriteSheet.RoomBorder;
            if (roomName == RoomName.basement)
            {
                borderSprite = ZeldaSpriteSheet.EmptyRoom;
            }
            Entity roomBackground = GetBackground(out tileOrigin, out scale, borderSprite);

            result.BG = roomBackground;

            if (roomName != RoomName.basement)
            {
                DoorFactory doorFact =
                    new(
                        roomBackground,
                        result.Doors,
                        _inventory,
                        (Action a) => result.RoomEvent += a,
                        switchRoom,
                        (
                            _loader.GetSheet(ZeldaSpriteSheet.NorthDoor),
                            _loader.GetSheet(ZeldaSpriteSheet.SouthDoor),
                            _loader.GetSheet(ZeldaSpriteSheet.EastDoor),
                            _loader.GetSheet(ZeldaSpriteSheet.WestDoor)
                        ),
                        _sound
                    );
                doorFact.AddDoorToRoom(northDoor.state, northDoor.dest, Directions.Up);
                doorFact.AddDoorToRoom(westDoor.state, westDoor.dest, Directions.Left);
                doorFact.AddDoorToRoom(eastDoor.state, eastDoor.dest, Directions.Right);
                doorFact.AddDoorToRoom(southDoor.state, southDoor.dest, Directions.Down);
            }

            RoomBorder.AddWallCollision(tileOrigin, scale, result.Entities, isBasement);

            return result;
        }

        public Entity GetBackground(
            out Vector2 tileOrigin,
            out float scale,
            ZeldaSpriteSheet borderSprite = ZeldaSpriteSheet.RoomBorder
        )
        {
            Entity bg = RoomBorder.CreateBackground(
                _loader.GetSheet(borderSprite),
                _screenSize,
                (int)_loader.GetSheet(ZeldaSpriteSheet.TopDisplay).FrameSize.Y
            );
            Transform bgTransform = bg.Get<Transform>();
            Vector2 roomOrigin = bgTransform.Position;
            scale = bgTransform.Scale;
            float borderOffset = RoomConstants.TILE_SIZE * BORDER_TILE_MULT * scale;
            tileOrigin = VectorUtils.GetIntVector(
                roomOrigin + new Vector2(borderOffset, borderOffset)
            );
            return bg;
        }
    }
}
