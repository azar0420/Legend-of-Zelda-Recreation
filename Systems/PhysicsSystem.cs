using amongus3902.Components;
using amongus3902.ContentMetadata;
using amongus3902.MetaClasses;
using amongus3902.Systems.Interfaces;
using amongus3902.Utils;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace amongus3902.Systems
{
    internal class PhysicsSystem : IUpdateSystem
    {
        private World _world;
        public const float MILLISECOND_MULTIPLIER = 0.05f;

        public void Start(World world)
        {
            _world = world;
            CollisionData.FillInGaps();
        }

        public void Update(GameTime gameTime)
        {
            List<Entity> entities = _world.GetEntitiesWithComponentOfTypes(
                typeof(Transform),
                typeof(PhysicsBody)
            );

            foreach (Entity entity in entities)
            {
                PhysicsBody body = entity.Get<PhysicsBody>();
                if (!DoesBodyMove(body))
                {
                    continue;
                }

                MoveInDirRespectingCollision(entity, entities, Vector2.UnitX, gameTime);
                MoveInDirRespectingCollision(entity, entities, Vector2.UnitY, gameTime);
            }
        }

        private void MoveInDirRespectingCollision(
            Entity entity,
            List<Entity> otherEntities,
            Vector2 unitDir,
            GameTime gameTime
        )
        {
            Transform transform = entity.Get<Transform>();
            PhysicsBody body = entity.Get<PhysicsBody>();

            Vector2 otherDir = Vector2.One - unitDir;

            Vector2 oldPos = transform.Position;
            Vector2 newPos = GetNextPosition(transform.Position, body.Velocity, gameTime);

            // change position only in the direction of unitdir
            transform.Position = unitDir * newPos + otherDir * oldPos;

            if (CollidesWithOtherEntities(entity, otherEntities, out Entity touched))
            {
                // revert pos
                transform.Position = oldPos;
                //store velocity
                Vector2 preTouchVel = body.Velocity;

                // touch entities
                body._touch(touched);
                if (!DoesBodyMove(touched.Get<PhysicsBody>()))
                    touched.Get<PhysicsBody>()._touch(entity);

                //check for touch changing velocity
                if (body.Velocity.Equals(preTouchVel))
                {
                    // hault velocity
                    body.Velocity *= otherDir;
                }
            }
            else
            {
                Vector2 newVel = GetNextVelocity(body.Velocity, body.Acceleration, gameTime);
                // change velocity only in the direction of unitdir
                body.Velocity = newVel * unitDir + body.Velocity * otherDir;
            }
        }

        private static bool DoesBodyMove(PhysicsBody body)
        {
            return body.Velocity != Vector2.Zero || body.Acceleration != Vector2.Zero;
        }

        private Vector2 GetNextPosition(Vector2 position, Vector2 velocity, GameTime gameTime)
        {
            return IncrementVector(position, velocity, gameTime.ElapsedGameTime.Milliseconds);
        }

        private Vector2 GetNextVelocity(Vector2 velocity, Vector2 acceleration, GameTime gameTime)
        {
            return IncrementVector(velocity, acceleration, gameTime.ElapsedGameTime.Milliseconds);
        }

        private Vector2 IncrementVector(Vector2 vector, Vector2 increment, float elapsedMs)
        {
            return vector + (increment * MILLISECOND_MULTIPLIER * elapsedMs);
        }

        public static bool CollidesWithOtherEntities(
            Entity entity,
            List<Entity> others,
            out Entity touched
        )
        {
            touched = null;

            if (!CanCollide(entity))
            {
                return false;
            }

            foreach (Entity entityToCheck in others)
            {
                if (Collides(entity, entityToCheck))
                {
                    touched = entityToCheck;
                    return true;
                }
            }

            return false;
        }

        public static bool Collides(Entity entity0, Entity entity1)
        {
            Collidables c0 = entity0.Get<PhysicsBody>().CollisionGroup,
                c1 = entity1.Get<PhysicsBody>().CollisionGroup;
            if (
                entity0 == entity1
                || !CanCollide(entity0)
                || !CanCollide(entity1)
                || CollisionData.CollisionGroupAssignments[(int)c0, (int)c1] == false
            )
            {
                return false;
            }

            return Geometry.ColliderOverlaps(entity0, entity1);
        }

        private static bool CanCollide(Entity entity)
        {
            return entity.Has<PhysicsBody>() && entity.Get<PhysicsBody>().CanCollide;
        }
    }
}
