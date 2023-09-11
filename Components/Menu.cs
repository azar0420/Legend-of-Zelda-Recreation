using amongus3902.Data;
using System.Numerics;

namespace amongus3902.Components
{
    //primarily for type checking rather than storing data
    internal class Menu : IComponent
    {
        public char MapLevel;
        public bool HasMap;
        public ZeldaSpriteSheet[] HeartSpriteSheets;

        public Entity[] BombCount;
        public Entity[] RupeeCount;
        public Entity[] KeyCount;

        public Entity[] Hearts;
        public Entity[] ActiveItems;

        public static readonly int HeartInitX = 528,
            HeartInitY = 144,
            HeartEndX = 696,
            HeartEndY = HeartInitY - 24,
            HeartDiff = 24,
            ActiveScale = 72;
        public static readonly Vector2 RupeeCoords = new(288, 72),
            KeyCoords = new(288, 120),
            BombCoords = new(288, 144),
            ActiveCoords = new(384, 96);

        public Menu(char m = '!', bool hasMap = false)
        {
            MapLevel = m;
            HasMap = hasMap;
            HeartSpriteSheets = new ZeldaSpriteSheet[16];
            Hearts = new Entity[16];

            BombCount = new Entity[2];
            RupeeCount = new Entity[2];
            KeyCount = new Entity[2];
            ActiveItems = new Entity[2];
        }
    }
}
