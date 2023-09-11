using Microsoft.Xna.Framework.Input;
using System;

namespace amongus3902.Gamepads
{
    internal class P5Gamepad : IGamepad
    {
        public Keys[] MoveUp { get; } = { Keys.P };
        public Keys[] MoveDown { get; } = { Keys.OemSemicolon };
        public Keys[] MoveLeft { get; } = { Keys.L };
        public Keys[] MoveRight { get; } = { Keys.OemQuotes };

        public Keys[] Attack { get; } = { Keys.O };
        public Keys[] UseItem { get; } = { Keys.OemOpenBrackets };

        public Keys[] TEMPItemUse1 { get; } = Array.Empty<Keys>();
        public Keys[] TEMPItemUse2 { get; } = Array.Empty<Keys>();
        public Keys[] TEMPItemUse3 { get; } = Array.Empty<Keys>();
        public Keys[] TEMPItemUse4 { get; } = Array.Empty<Keys>();
        public Keys[] TEMPItemUse5 { get; } = Array.Empty<Keys>();
        public Keys[] TEMPItemUse6 { get; } = Array.Empty<Keys>();
        public Keys[] TEMPItemUse7 { get; } = Array.Empty<Keys>();
    }
}
