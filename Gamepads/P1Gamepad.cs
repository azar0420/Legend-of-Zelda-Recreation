using Microsoft.Xna.Framework.Input;

namespace amongus3902.Gamepads
{
    internal class P1Gamepad : IGamepad
    {
        public Keys[] MoveUp { get; } = { Keys.W, Keys.Up };
        public Keys[] MoveDown { get; } = { Keys.S, Keys.Down };
        public Keys[] MoveLeft { get; } = { Keys.A, Keys.Left };
        public Keys[] MoveRight { get; } = { Keys.D, Keys.Right };

        public Keys[] Attack { get; } = { Keys.Q, Keys.N, Keys.Z };
        public Keys[] UseItem { get; } = { Keys.E, Keys.B, Keys.X };

        public Keys[] TEMPItemUse1 { get; } = { Keys.D1 };
        public Keys[] TEMPItemUse2 { get; } = { Keys.D2 };
        public Keys[] TEMPItemUse3 { get; } = { Keys.D3 };
        public Keys[] TEMPItemUse4 { get; } = { Keys.D4 };
        public Keys[] TEMPItemUse5 { get; } = { Keys.D5 };
        public Keys[] TEMPItemUse6 { get; } = { Keys.D6 };
        public Keys[] TEMPItemUse7 { get; } = { Keys.D7 };
    }
}
