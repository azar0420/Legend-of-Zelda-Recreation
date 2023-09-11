using Microsoft.Xna.Framework;

namespace amongus3902.Components
{
    internal class Map : IComponent
    {
        //mostly for type checking rather than anything
        public static readonly int XScale = 8,
            YScale = 4;
        public static Vector2 Offset = new Vector2(2.5f, 0.5f),
            MapCoords = new(40, 54);
        public bool IsHighlighter;

        public Map(bool highlight = false)
        {
            IsHighlighter = highlight;
        }
    }
}
