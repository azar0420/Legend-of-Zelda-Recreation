using Microsoft.Xna.Framework.Input;
using System;

namespace amongus3902.Gamepads
{
    internal class P3Gamepad : IGamepad
    {
        public Keys[] MoveUp { get; } = { Keys.Y };
        public Keys[] MoveDown { get; } = { Keys.H };
        public Keys[] MoveLeft { get; } = { Keys.G };
        public Keys[] MoveRight { get; } = { Keys.J };

        public Keys[] Attack { get; } = { Keys.T };
        public Keys[] UseItem { get; } = { Keys.U };

        public Keys[] TEMPItemUse1 { get; } = Array.Empty<Keys>();
        public Keys[] TEMPItemUse2 { get; } = Array.Empty<Keys>();
        public Keys[] TEMPItemUse3 { get; } = Array.Empty<Keys>();
        public Keys[] TEMPItemUse4 { get; } = Array.Empty<Keys>();
        public Keys[] TEMPItemUse5 { get; } = Array.Empty<Keys>();
        public Keys[] TEMPItemUse6 { get; } = Array.Empty<Keys>();
        public Keys[] TEMPItemUse7 { get; } = Array.Empty<Keys>();
    }
}
