using amongus3902.Components;
using amongus3902.MetaClasses;
using amongus3902.Systems.Interfaces;
using amongus3902.Utils;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace amongus3902.Systems
{
    internal class DamageSystem : IUpdateSystem
    {
        private World _world;
        private List<Entity> hurtBoxEntities,
            hitBoxEntities;

        public void Start(World world)
        {
            _world = world;
        }

        public void Update(GameTime gameTime)
        {
            hitBoxEntities = _world.GetEntitiesWithComponentOfTypes(typeof(HitBox), typeof(Team));
            hurtBoxEntities = _world.GetEntitiesWithComponentOfTypes(typeof(HurtBox));

            foreach (Entity hitEntity in hitBoxEntities)
            {
                foreach (Entity hurtEntity in hurtBoxEntities)
                {
                    if (
                        Geometry.DamageOverlaps(hitEntity, hurtEntity)
                        && (
                            !hurtEntity.Has<Team>()
                            || hitEntity.Get<Team>().AlliedTo != hurtEntity.Get<Team>().AlliedTo
                        )
                    )
                    {
                        HitBox hitBox = hitEntity.Get<HitBox>();
                        HurtBox hurtBox = hurtEntity.Get<HurtBox>();

                        if (hitBox.Enabled && hurtBox.Enabled)
                        {
                            if (!HasIFrames(hurtEntity))
                            {
                                hurtBox.Damage(hitBox.DamageAmount, hitEntity);
                                hitBox.DoDamageResponse(hitEntity, hurtEntity);
                            }

                            if (hurtBox.Health <= 0 && !hurtBox.IsDead)
                            {
                                hurtBox.Kill(hitEntity);
                                hurtBox.IsDead = true;
                            }
                        }
                    }
                }
            }
        }

        private static bool HasIFrames(Entity hurtEntity)
        {
            bool isEnemy = hurtEntity.Has<EnemyBehavior>();
            bool isPlayer = hurtEntity.Has<CharacterController>();

            return (isEnemy && hurtEntity.Get<EnemyBehavior>().TakingDamage)
                || (
                    isPlayer
                    && hurtEntity.Get<CharacterController>().CurrentAction
                        == CharacterAction.Damaged
                );
        }
    }
}
