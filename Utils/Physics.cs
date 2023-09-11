using amongus3902.Components;
using amongus3902.ContentMetadata;
using Microsoft.Xna.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace amongus3902.Utils
{
    internal static class Physics
    {
        private const float KNOCKBACK_SPEED = 15;

        public static void KnockbackFromEntity(
            Entity toEntity,
            Entity fromEntity,
            int knockbackTimeMs
        )
        {
            Vector2 knockbackDir = Vector2.Normalize(
                toEntity.Get<Transform>().Position - fromEntity.Get<Transform>().Position
            );

            PhysicsBody body = toEntity.Get<PhysicsBody>();
            body.Velocity = knockbackDir * KNOCKBACK_SPEED;

            new Task(() =>
            {
                Thread.Sleep(knockbackTimeMs);
                body.Velocity = Vector2.Zero;
            }).Start();
        }

        public static void PushBlock(Entity block, Vector2 direction, int distancePixels)
        {
            const int dt_ms = 20;

            Transform transform = block.Get<Transform>();
            Vector2 startPosition = transform.Position;

            PhysicsBody body = block.Get<PhysicsBody>();
            body.Velocity = direction;

            new Task(() =>
            {
                while ((transform.Position - startPosition).Length() < distancePixels)
                {
                    Thread.Sleep(dt_ms);
                }

                body.Velocity = Vector2.Zero;
                transform.Position = startPosition + direction * distancePixels;
            }).Start();
        }

        public static Entity CreateRectCollision(Vector2 pos, Vector2 size, float scale)
        {
            return new Entity()
                .Attach(new Transform(pos, 1, scale))
                .Attach(new PhysicsBody(true, size, Vector2.Zero, Collidables.Wall));
        }
    }
}
