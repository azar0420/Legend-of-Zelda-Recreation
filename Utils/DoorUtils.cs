using amongus3902.Components;
using amongus3902.ContentMetadata;
using amongus3902.Factories;
using Microsoft.Xna.Framework;

namespace amongus3902.Utils
{
    internal class DoorUtils
    {
        public static (Sprite doorTopSprite, Sprite doorFrameSprite) GetDoorSprites(
            SpriteSheet doorSheet,
            DoorState doorState,
            Directions dirFromCenter
        )
        {
            int doorFrameFrame = (int)doorState * 2 / doorSheet.Rows;
            int doorTopFrame = doorFrameFrame + (doorSheet.Rows - 1) * (doorSheet.Columns - 1) + 1;

            Vector2 dirVector = Direction.DirectionToVector(dirFromCenter);
            if (dirVector.X < 0 || dirVector.Y < 0)
            {
                (doorTopFrame, doorFrameFrame) = (doorFrameFrame, doorTopFrame);
            }

            return (new Sprite(doorSheet, doorTopFrame), new Sprite(doorSheet, doorFrameFrame));
        }

        public static DoorState GetOpenState(DoorState doorState)
        {
            return doorState switch
            {
                DoorState.Locked or DoorState.Closed => DoorState.Open,
                // bombed doesn't need an open state because its sprite is initially set to
                // DoorState.None, but stored as DoorState.Bombed to make tracking easier
                _ => doorState
            };
        }

        public static void OpenDoor(
            DoorState doorState,
            Directions dirFromCenter,
            Entity doorTop,
            Entity doorFrame
        )
        {
            DoorState newState = GetOpenState(doorState);
            // if new state is the same as the current state, the door was not opened, with bombed being the exception
            if (newState == doorState && doorState != DoorState.Bombed)
            {
                return;
            }

            (Sprite openDoorTop, Sprite openDoorFrame) = GetDoorSprites(
                doorFrame.Get<Sprite>().Sheet,
                newState,
                dirFromCenter
            );

            OpenDoor(doorTop, doorFrame, openDoorTop, openDoorFrame);
        }

        public static void OpenDoor(
            Entity doorTop,
            Entity doorFrame,
            Sprite openDoorTop,
            Sprite openDoorFrame
        )
        {
            doorFrame.Get<PhysicsBody>().CollisionGroup = Collidables.Door;
            doorTop.Replace(openDoorTop);
            doorFrame.Replace(openDoorFrame);
        }
    }
}
