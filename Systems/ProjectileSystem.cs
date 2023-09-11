using amongus3902.Components;
using amongus3902.Components.DespawnConditions;
using amongus3902.MetaClasses;
using amongus3902.Systems.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace amongus3902.Systems
{
    internal class ProjectileSystem : IUpdateSystem
    {
        private World _world;

        //TODO: only needed in one despawn condition, seems a waste to pass
        //likely a better way?
        private int timeElapsedSinceLastUpdate = 0;
        private float PROJ_RETURN_THRESHHOLD = 10;
        private float VEL_STOPPED_THRESHHOLD = 0.2f;
        private Dictionary<Type, Func<Entity, IDespawnCondition, bool>> DespawnConditions;

        public void Start(World world)
        {
            _world = world;
            DespawnConditions = CreateConditionDict();
        }

        public void Update(GameTime gameTime)
        {
            timeElapsedSinceLastUpdate = gameTime.ElapsedGameTime.Milliseconds;
            List<Entity> projectiles = _world.GetEntitiesWithComponentOfTypes(
                typeof(Transform),
                typeof(Projectile)
            );

            foreach (Entity projectile in projectiles)
            {
                UpdateProjectile(projectile);
            }
        }

        private void UpdateProjectile(Entity projectile)
        {
            if (NeedsDespawn(projectile))
            {
                Projectile projBehavior = projectile.Get<Projectile>();
                projBehavior.Despawn();
                if (_world.HasEntity(projectile.UniqueID))
                {
                    _world.RemoveEntity(projectile.UniqueID);
                }
                
            }
        }

        private Dictionary<Type, Func<Entity, IDespawnCondition, bool>> CreateConditionDict()
        {
            return new()
            {
                {
                    typeof(OnReturnDespawn),
                    new Func<Entity, IDespawnCondition, bool>(BoomerangReturnDespawn)
                },
                {
                    typeof(TimeDurationDespawn),
                    new Func<Entity, IDespawnCondition, bool>(ShouldTimeDurationDespawn)
                },
                {
                    typeof(BoundLimitDespawn),
                    new Func<Entity, IDespawnCondition, bool>(ShouldBoundLimitDespawn)
                },
                {
                    typeof(CollideDespawn),
                    new Func<Entity, IDespawnCondition, bool>(ShouldCollideDespawn)
                },
                {
                    typeof(DoDamageDespawn),
                    new Func<Entity, IDespawnCondition, bool>(ShouldDoDamageDespawn)
                }
            };
        }

        private bool NeedsDespawn(Entity projectile)
        {
            bool result = false;
            IDespawnCondition[] conditions = projectile.Get<Projectile>().DespawnConditions;
            foreach (IDespawnCondition condition in conditions)
            {
                result =
                    result || DespawnConditions[condition.GetType()].Invoke(projectile, condition);
            }
            return result;
        }

        private bool BoomerangReturnDespawn(Entity projectile, IDespawnCondition condition)
        {
            Vector2 sourcePos = projectile.Get<Projectile>().Source.Get<Transform>().Position;
            bool isAtSource =
                Vector2.Distance(sourcePos, projectile.Get<Transform>().Position)
                < PROJ_RETURN_THRESHHOLD;
            bool result = false;
            OnReturnDespawn castCondition = (OnReturnDespawn)condition;
            if (!castCondition.HasLeft && !isAtSource)
            {
                castCondition.HasLeft = true;
            }
            else if (isAtSource && castCondition.HasLeft)
            {
                result = true;
            }
            if (CheckIfReturning(projectile, castCondition))
            {
                MoveAndAccelerateTowardsTarget(projectile, sourcePos);
            }
            return result;
        }

        private bool ShouldTimeDurationDespawn(Entity projectile, IDespawnCondition condition)
        {
            TimeDurationDespawn castCondition = (TimeDurationDespawn)condition;
            castCondition.TimeElapsed += timeElapsedSinceLastUpdate;
            return castCondition.TimeElapsed >= castCondition.DespawnTime;
        }

        private bool ShouldBoundLimitDespawn(Entity projectile, IDespawnCondition condition)
        {
            Rectangle rect = ((BoundLimitDespawn)condition).DespawnBounds;
            Vector2 projPos = projectile.Get<Transform>().Position;
            return !rect.Contains(projPos);
        }

        private bool ShouldCollideDespawn(Entity projectile, IDespawnCondition condition)
        {
            return projectile.Get<Projectile>().Collided;
        }

        private bool ShouldDoDamageDespawn(Entity projectile, IDespawnCondition condition)
        {
            return projectile.Get<Projectile>().DidDamage;
        }

        //TODO: move to different system?
        private void MoveAndAccelerateTowardsTarget(Entity projectile, Vector2 target)
        {
            Vector2 projectilePos = projectile.Get<Transform>().Position;
            PhysicsBody projectileMov = projectile.Get<PhysicsBody>();

            projectileMov.Velocity =
                Vector2.Normalize(target - projectilePos) * projectileMov.Velocity.Length();
            projectileMov.Acceleration =
                Vector2.Normalize(target - projectilePos) * projectileMov.Acceleration.Length();
        }

        private bool CheckIfReturning(Entity projectile, OnReturnDespawn condition)
        {
            if (condition.Returning)
            {
                return true;
            }
            PhysicsBody projectileMov = projectile.Get<PhysicsBody>();
            bool result = false;
            if (projectileMov.Velocity.Length() < VEL_STOPPED_THRESHHOLD || projectile.Get<Projectile>().Collided)
            {
                result = true;
                condition.Returning = true;
            }
            return result;
        }
    }
}
