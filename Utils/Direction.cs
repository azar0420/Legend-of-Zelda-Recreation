using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace amongus3902.Utils
{
    public enum Directions
    {
        Down = 0,
        Right = 1,
        Left = 2,
        Up = 3,
        None = -1
    }

    internal static class Direction
    {
        private static readonly Dictionary<Directions, Vector2> _dirToVelocity =
            new()
            {
                { Directions.Up, -Vector2.UnitY },
                { Directions.Down, Vector2.UnitY },
                { Directions.Left, -Vector2.UnitX },
                { Directions.Right, Vector2.UnitX },
                { Directions.None, Vector2.Zero }
            };

        public static Vector2 DirectionToVector(Directions dir)
        {
            return _dirToVelocity[dir];
        }

        public static Directions VectorToDirection(Vector2 vector)
        {
            return _dirToVelocity
                .FirstOrDefault(x => x.Value.Equals(Vector2.Normalize(vector)))
                .Key;
        }
    }
}
