using Microsoft.Xna.Framework.Input;
using System;

namespace amongus3902.Gamepads
{
    internal class P2Gamepad : IGamepad
    {
        public Keys[] MoveUp { get; } = { Keys.Home };
        public Keys[] MoveDown { get; } = { Keys.End };
        public Keys[] MoveLeft { get; } = { Keys.Delete };
        public Keys[] MoveRight { get; } = { Keys.PageDown };

        public Keys[] Attack { get; } = { Keys.Insert };
        public Keys[] UseItem { get; } = { Keys.PageUp };

        public Keys[] TEMPItemUse1 { get; } = Array.Empty<Keys>();
        public Keys[] TEMPItemUse2 { get; } = Array.Empty<Keys>();
        public Keys[] TEMPItemUse3 { get; } = Array.Empty<Keys>();
        public Keys[] TEMPItemUse4 { get; } = Array.Empty<Keys>();
        public Keys[] TEMPItemUse5 { get; } = Array.Empty<Keys>();
        public Keys[] TEMPItemUse6 { get; } = Array.Empty<Keys>();
        public Keys[] TEMPItemUse7 { get; } = Array.Empty<Keys>();
    }
}
