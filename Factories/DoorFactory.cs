using amongus3902.Components;
using amongus3902.ContentMetadata;
using amongus3902.Data;
using amongus3902.Systems;
using amongus3902.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace amongus3902.Factories
{
    public enum DoorState
    {
        None = 0,
        Open = 1,
        Locked = 2,
        Closed = 3,
        Bombed = 4,
    }

    internal class DoorFactory
    {
        private static readonly HashSet<DoorState> LINK_COLLIDEABLE_DOORS =
            new() { DoorState.None, DoorState.Locked, DoorState.Closed };
        private static readonly HashSet<DoorState> OPENABLE_DOORS =
            new() { DoorState.Bombed, DoorState.Locked, DoorState.Closed };

        private readonly Entity _roomBackground;
        private readonly float _scale;
        private readonly Dictionary<Directions, DoorStorage> _doorDict;

        private readonly InventorySystem _inventory;
        private readonly Action<Action> _bindToRoomEvent;
        private readonly Action<RoomName, Vector2> _switchRoom;

        private readonly Dictionary<Directions, SpriteSheet> _doorSheets;

        private SoundSystem _sound;

        public DoorFactory(
            Entity roomBackground,
            Dictionary<Directions, DoorStorage> doorDictionary,
            InventorySystem inventory,
            Action<Action> bindToRoomEvent,
            Action<RoomName, Vector2> switchRoom,
            (
                SpriteSheet northDoor,
                SpriteSheet southDoor,
                SpriteSheet eastDoor,
                SpriteSheet westDoor
            ) doorSpriteSheets,
            SoundSystem sound
        )
        {
            _roomBackground = roomBackground;
            _scale = _roomBackground.Get<Transform>().Scale;
            _doorDict = doorDictionary;
            _inventory = inventory;
            _bindToRoomEvent = bindToRoomEvent;
            _switchRoom = switchRoom;

            _doorSheets = new()
            {
                { Directions.Up, doorSpriteSheets.northDoor },
                { Directions.Down, doorSpriteSheets.southDoor },
                { Directions.Right, doorSpriteSheets.eastDoor },
                { Directions.Left, doorSpriteSheets.westDoor },
            };

            _sound = sound;
        }

        public void AddDoorToRoom(DoorState doorState, RoomName dest, Directions dirFromCenter)
        {
            DoorState currentState = doorState;
            if (doorState == DoorState.Bombed)
            {
                currentState = DoorState.None;
            }

            DoorStorage storage = new() { InitialDoorState = doorState };

            (Sprite doorTopSprite, Sprite doorFrameSprite) = DoorUtils.GetDoorSprites(
                _doorSheets[dirFromCenter],
                currentState,
                dirFromCenter
            );
            storage.DoorTop.Attach(doorTopSprite);
            storage.DoorFrame.Attach(doorFrameSprite);

            (Vector2 framePos, Vector2 topPos) = GetDoorPositions(
                Direction.DirectionToVector(dirFromCenter),
                (doorTopSprite, doorFrameSprite)
            );
            storage.DoorTop.Attach(new Transform(topPos, TransformData.DOOR_TOP_DEPTH, _scale));
            storage.DoorFrame.Attach(
                new Transform(framePos, TransformData.DOOR_FRAME_DEPTH, _scale)
            );

            if (dest != RoomName.none)
            {
                storage.TransitionCollider = MakeTransitionCollider(
                    dest,
                    dirFromCenter,
                    doorTopSprite.Sheet
                );
            }

            AttachDoorCollision(storage.DoorFrame, currentState, dirFromCenter);

            if (OPENABLE_DOORS.Contains(doorState))
            {
                AttachDoorOpening(storage.DoorTop, storage.DoorFrame, doorState, dirFromCenter);
            }

            _doorDict.Add(dirFromCenter, storage);
        }

        private (Vector2 door1Pos, Vector2 door2Pos) GetDoorPositions(
            Vector2 dirFromCenter,
            (Sprite top, Sprite frame) doorSprites
        )
        {
            Vector2 edgePos = GetDoorEdgePos(dirFromCenter);

            (Vector2 topSize, Vector2 frameSize) = (
                doorSprites.top.Sheet.FrameSize * _scale,
                doorSprites.frame.Sheet.FrameSize * _scale
            );

            Vector2 doorTopPos =
                edgePos - frameSize / 2 - frameSize * dirFromCenter / 2 - topSize * dirFromCenter;
            Vector2 doorFramePos = edgePos - topSize / 2 - topSize * dirFromCenter / 2;

            return (doorTopPos, doorFramePos);
        }

        private static void AttachDoorCollision(
            Entity doorFrame,
            DoorState doorState,
            Directions dirFromCenter
        )
        {
            Vector2 doorColliderSize = RoomConstants.TILE_SIZE * new Vector2(2, 1);

            if (Math.Abs(Direction.DirectionToVector(dirFromCenter).X) == 1)
                doorColliderSize = RoomConstants.TILE_SIZE * new Vector2(1, 2);

            doorFrame.Attach(
                new PhysicsBody(
                    true,
                    doorColliderSize,
                    Vector2.Zero,
                    LINK_COLLIDEABLE_DOORS.Contains(doorState) ? Collidables.Wall : Collidables.Door
                )
            );
        }

        private Entity MakeTransitionCollider(
            RoomName dest,
            Directions dirFromCenter,
            SpriteSheet doorTopSheet
        )
        {
            Vector2 dirVector = Direction.DirectionToVector(dirFromCenter);
            Vector2 doorTopSheetSize = new(doorTopSheet.Width, doorTopSheet.Height);
            Vector2 edgePos = GetDoorEdgePos(dirVector);

            Vector2 doorTopPixelSize = doorTopSheetSize * _scale;
            Vector2 colliderPos = edgePos - doorTopPixelSize / 2 + doorTopPixelSize * dirVector / 2;

            Entity roomTransition = Physics.CreateRectCollision(
                colliderPos,
                doorTopSheetSize,
                _scale
            );

            roomTransition.Get<PhysicsBody>().Touched += (e) =>
            {
                if (e.Has<CharacterController>())
                {
                    _switchRoom(dest, -dirVector);
                }
            };

            return roomTransition;
        }

        private Vector2 GetDoorEdgePos(Vector2 dirFromCenter)
        {
            Vector2 bgSize = _roomBackground.Get<Sprite>().Sheet.FrameSize * _scale;
            Vector2 bgCenter = (2 * _roomBackground.Get<Transform>().Position + bgSize) / 2;
            return bgCenter + bgSize / 2 * dirFromCenter;
        }

        private void AttachDoorOpening(
            Entity doorTop,
            Entity doorFrame,
            DoorState doorState,
            Directions dirFromCenter
        )
        {
            (Sprite openTop, Sprite openFrame) = DoorUtils.GetDoorSprites(
                _doorSheets[dirFromCenter],
                DoorUtils.GetOpenState(doorState),
                dirFromCenter
            );

            Action<Entity, Entity, Sprite, Sprite> doorOpenMethod = doorState switch
            {
                DoorState.Locked => OpenDoorWithKeyOnLinkTouch,
                DoorState.Bombed => OpenDoorOnBombTouch,
                DoorState.Closed => OpenDoorOnRoomEvent,
                _ => throw new NotImplementedException(),
            };
            doorOpenMethod(doorTop, doorFrame, openTop, openFrame);
        }

        private void OpenDoorWithKeyOnLinkTouch(
            Entity doorTop,
            Entity doorFrame,
            Sprite openDoorTop,
            Sprite openDoorFrame
        )
        {
            void open(Entity e)
            {
                if (e.Has<CharacterController>() && _inventory.ConsumablesCount(ItemType.Key) > 0)
                {
                    _inventory.RemoveConsumables(ItemType.Key, 1);
                    DoorUtils.OpenDoor(doorTop, doorFrame, openDoorTop, openDoorFrame);
                    _sound.PlaySound(ZeldaSound.DoorUnlock);
                    doorFrame.Get<PhysicsBody>().Touched -= open;
                }
            }

            doorFrame.Get<PhysicsBody>().Touched += open;
        }

        private void OpenDoorOnBombTouch(
            Entity doorTop,
            Entity doorFrame,
            Sprite openDoorTop,
            Sprite openDoorFrame
        )
        {
            HurtBox hurt =
                new(
                    doorFrame.Get<PhysicsBody>().ColliderSize,
                    doorFrame.Get<PhysicsBody>().ColliderOffset,
                    int.MaxValue
                );
            doorFrame.Attach(hurt);

            void open(Entity e)
            {
                if (e.Has<Projectile>() && e.Get<Projectile>().Type == ProjectileType.BombExplosion)
                {
                    DoorUtils.OpenDoor(doorTop, doorFrame, openDoorTop, openDoorFrame);
                    _sound.PlaySound(ZeldaSound.SecretFind);
                    hurt.Damaged -= open;
                }
            }

            hurt.Damaged += open;
        }

        private void OpenDoorOnRoomEvent(
            Entity doorTop,
            Entity doorFrame,
            Sprite openDoorTop,
            Sprite openDoorFrame
        )
        {
            _bindToRoomEvent(() =>
            {
                DoorUtils.OpenDoor(doorTop, doorFrame, openDoorTop, openDoorFrame);
                _sound.PlaySound(ZeldaSound.DoorUnlock);
            });
        }
    }
}
