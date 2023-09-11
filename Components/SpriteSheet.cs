using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace amongus3902.Components
{
    internal class SpriteSheet
    {
        public Texture2D Texture { get; set; }
        public int Rows;
        public int Columns;

        public Vector2 FrameSize
        {
            get { return new(Texture.Width / Columns, Texture.Height / Rows); }
        }

        public int Height
        {
            get { return Texture.Height / Rows; }
        }

        public int Width
        {
            get { return Texture.Width / Columns; }
        }

        public SpriteSheet(Texture2D sheet, int rows, int cols)
        {
            Texture = sheet;
            Rows = rows;
            Columns = cols;
        }
    }
}
