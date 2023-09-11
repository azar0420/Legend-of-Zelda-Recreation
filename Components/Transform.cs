using Microsoft.Xna.Framework;

namespace amongus3902.Components
{
    internal class Transform : IComponent
    {
        public Vector2 Position { get; set; }
        public float LayerDepth { get; set; }
        public float Scale { get; set; }

        private const float DEFAULT_SCALE = 3;

        public Transform()
            : this(Vector2.Zero) { }

        public Transform(Vector2 position, float layerDepth = 0, float scale = DEFAULT_SCALE)
        {
            Position = position;
            position.X = (int)position.X;
            position.Y = (int)position.Y;
            LayerDepth = layerDepth;
            Scale = scale;
        }

        public Transform Duplicate()
        {
            return (Transform)this.MemberwiseClone();
        }
    }
}
