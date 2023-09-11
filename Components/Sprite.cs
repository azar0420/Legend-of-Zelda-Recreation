using amongus3902.Utils;
using Microsoft.Xna.Framework;

namespace amongus3902.Components
{
    internal class Sprite : IComponent
    {
        public SpriteSheet Sheet { get; set; }
        public int CurrentFrame { get; set; }
        public Directions Direction { get; set; }

        public Color SpriteTint = Color.White;

        public Sprite(SpriteSheet sheet, int frame = 0, Directions direction = Directions.None)
        {
            Sheet = sheet;
            CurrentFrame = frame;
            Direction = direction;
        }
    }
}
