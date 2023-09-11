using Microsoft.Xna.Framework.Input;
using System;

namespace amongus3902.Gamepads
{
    internal class P4Gamepad : IGamepad
    {
        public Keys[] MoveUp { get; } = { Keys.NumPad8 };
        public Keys[] MoveDown { get; } = { Keys.NumPad5 };
        public Keys[] MoveLeft { get; } = { Keys.NumPad4 };
        public Keys[] MoveRight { get; } = { Keys.NumPad6 };

        public Keys[] Attack { get; } = { Keys.NumPad7 };
        public Keys[] UseItem { get; } = { Keys.NumPad9 };

        public Keys[] TEMPItemUse1 { get; } = Array.Empty<Keys>();
        public Keys[] TEMPItemUse2 { get; } = Array.Empty<Keys>();
        public Keys[] TEMPItemUse3 { get; } = Array.Empty<Keys>();
        public Keys[] TEMPItemUse4 { get; } = Array.Empty<Keys>();
        public Keys[] TEMPItemUse5 { get; } = Array.Empty<Keys>();
        public Keys[] TEMPItemUse6 { get; } = Array.Empty<Keys>();
        public Keys[] TEMPItemUse7 { get; } = Array.Empty<Keys>();
    }
}
