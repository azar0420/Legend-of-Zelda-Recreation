using amongus3902.Components;
using Microsoft.Xna.Framework;
using System;
using System.Data.Common;
using System.Diagnostics;

namespace amongus3902.Utils
{
    internal class AnimationChanger
    {
        public static void SetAnimRowKeepColumn(Entity entity, int row)
        {
            if (entity.Has<Animation>() && entity.Has<Sprite>())
            {
                Animation anim = entity.Get<Animation>();
                Sprite sprite = entity.Get<Sprite>();
                int columns = sprite.Sheet.Columns;
                anim.StartFrame = row * columns;
                anim.EndFrame = anim.StartFrame + columns - 1;
                int oldFrame = sprite.CurrentFrame;
                int oldRow = oldFrame / columns;
                sprite.CurrentFrame += (row-oldRow) * columns;
            }
        }

        public static void SetAnimRowResetColumn(Entity entity, int row)
        {
            if (entity.Has<Animation>() && entity.Has<Sprite>())
            {
                Animation anim = entity.Get<Animation>();
                Sprite sprite = entity.Get<Sprite>();
                int columns = sprite.Sheet.Columns;
                anim.StartFrame = row * columns;
                anim.EndFrame = anim.StartFrame + columns - 1;
                sprite.CurrentFrame = row*columns;
                anim.TimeSinceLastFrame = 0;
            }
        }

        public static void MatchAnimToVelocity(Entity entity)
        {
            if (entity.Has<Animation>() && entity.Has<Sprite>() && entity.Has<PhysicsBody>())
            {
                Sprite sprite = entity.Get<Sprite>();
                Vector2 normalizedVel = Vector2.Normalize(entity.Get<PhysicsBody>().Velocity);
                if (
                    sprite.Direction != Directions.None
                    && !normalizedVel.Equals(Direction.DirectionToVector(sprite.Direction))
                )
                {
                    sprite.Direction = Direction.VectorToDirection(normalizedVel);
                    int columns = sprite.Sheet.Columns;
                    int startFrame = 0 + (int)sprite.Direction * columns;
                    double delay = entity.Get<Animation>().Delay;
                    Animation a = new Animation(startFrame, startFrame + columns - 1, delay);
                    a.TimeSinceLastFrame = a.Delay;
                    entity.Replace(a);
                }
            }
        }

        public static void AccelerateAnimation(
            Entity entity,
            float percentDone,
            double minDelay,
            bool isDecelerating
        )
        {
            if (entity.Has<Animation>())
            {
                if (isDecelerating)
                {
                    entity.Get<Animation>().Delay =
                        (percentDone / (1 - percentDone)) * minDelay + minDelay;
                }
                else
                {
                    entity.Get<Animation>().Delay = minDelay * (1 / percentDone);
                }
            }
        }
    }
}
