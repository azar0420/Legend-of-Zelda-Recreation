using Microsoft.Xna.Framework;
using System;

namespace amongus3902.Utils
{
    internal class VectorUtils
    {
        private readonly Random _random = new();

        public Vector2 GenerateRandomRookVector(float vectorMagnitude)
        {
            return _random.Next(0, 4) switch
            {
                0 => new Vector2(vectorMagnitude, 0),
                1 => new Vector2(-vectorMagnitude, 0),
                2 => new Vector2(0, -vectorMagnitude),
                _ => new Vector2(0, vectorMagnitude),
            };
        }

        public Vector2 GenerateRandomBishopVector(float vectorMagnitude)
        {
            float magnitudeComponent = vectorMagnitude / (float)Math.Sqrt(2);

            return _random.Next(0, 4) switch
            {
                0 => new Vector2(magnitudeComponent, magnitudeComponent),
                1 => new Vector2(magnitudeComponent, -magnitudeComponent),
                2 => new Vector2(-magnitudeComponent, -magnitudeComponent),
                _ => new Vector2(-magnitudeComponent, magnitudeComponent),
            };
        }

        public Vector2 GenerateRandomQueenVector(float vectorMagnitude)
        {
            return _random.Next(0, 2) switch
            {
                0 => GenerateRandomRookVector(vectorMagnitude),
                _ => GenerateRandomBishopVector(vectorMagnitude),
            };
        }

        public static Vector2 GetIntVector(Vector2 v)
        {
            return new Vector2((int)v.X, (int)v.Y);
        }

        public static Vector2 SnapToClosestAxis(Vector2 v)
        {
            if (Math.Abs(v.X) > Math.Abs(v.Y))
            {
                return Vector2.UnitX * v.X;
            }

            return Vector2.UnitY * v.Y;
        }

        public static Vector2 SnapVectorToNormalizedOrthogonal(Vector2 vector)
        {
            return Vector2.Normalize(SnapToClosestAxis(vector));
        }
    }
}
