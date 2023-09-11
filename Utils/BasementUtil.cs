using amongus3902.Data;
using System;
using Microsoft.Xna.Framework;

namespace amongus3902.Utils
{
    internal class BasementUtil
    {
        public static void GoToBasement(Action<RoomName, Vector2> switchRoom)
        {
            switchRoom(RoomName.basement, Vector2.UnitY * -1);
        }

        public static void LeaveBasement(Action<RoomName, Vector2> switchRoom)
        {
            switchRoom(RoomName.e1_0, Vector2.UnitX);
        }
    }
}
