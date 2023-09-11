using amongus3902.ContentMetadata;
using Microsoft.Xna.Framework;
using System;

namespace amongus3902.Components
{
    internal class PhysicsBody : IComponent
    {
        //TODO: simplify constructors
        public Vector2 Velocity { get; set; } = Vector2.Zero;
        public Vector2 Acceleration { get; set; } = Vector2.Zero;
        public bool CanCollide { get; set; } = false;
        public Vector2 ColliderOffset { get; set; } = Vector2.Zero;
        public Vector2 ColliderSize { get; set; } = Vector2.Zero;
        public event Action<Entity> Touched;

        public Collidables CollisionGroup { get; set; }

        public PhysicsBody() { }

        public PhysicsBody(
            bool collides,
            Vector2 colliderSize,
            Vector2 colliderOffset,
            Collidables collisionGroup
        )
        {
            ColliderSize = colliderSize;
            ColliderOffset = colliderOffset;
            CanCollide = collides;
            CollisionGroup = collisionGroup;
        }

        public PhysicsBody(Vector2 velocity, Vector2 acceleration, Collidables collisionGroup)
        {
            Velocity = velocity;
            Acceleration = acceleration;
            CollisionGroup = collisionGroup;
            CanCollide = true;
        }

        public PhysicsBody(Vector2 velocity, Vector2 acceleration)
        {
            Velocity = velocity;
            Acceleration = acceleration;
        }

        // DO NOT TOUCH UNLESS YOU ARE THE PHYSICS SYSTEM
        public void _touch(Entity toucher)
        {
            Touched?.Invoke(toucher);
        }
    }
}
