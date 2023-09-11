using Microsoft.Xna.Framework.Input;

namespace amongus3902.Gamepads
{
    internal interface IGamepad
    {
        public Keys[] MoveUp { get; }
        public Keys[] MoveDown { get; }
        public Keys[] MoveLeft { get; }
        public Keys[] MoveRight { get; }

        public Keys[] Attack { get; }
        public Keys[] UseItem { get; }

        public Keys[] TEMPItemUse1 { get; }
        public Keys[] TEMPItemUse2 { get; }
        public Keys[] TEMPItemUse3 { get; }
        public Keys[] TEMPItemUse4 { get; }
        public Keys[] TEMPItemUse5 { get; }
        public Keys[] TEMPItemUse6 { get; }
        public Keys[] TEMPItemUse7 { get; }
    }
}
