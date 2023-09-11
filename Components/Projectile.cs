using amongus3902.Components.DespawnConditions;
using amongus3902.Factories;
using System;

namespace amongus3902.Components
{
    internal class Projectile : IComponent
    {
        public ProjectileType Type { get; set; }
        public Entity Source { get; set; }

        public IDespawnCondition[] DespawnConditions { get; set; }

        //event despawn handlers (probably move these with above refactor)
        public bool Collided = false;
        public bool DidDamage = false;

        public event Action Despawned;

        public Projectile(ProjectileType type, Entity source, IDespawnCondition[] DestroyConditions)
        {
            Source = source;
            DespawnConditions = DestroyConditions;
            Type = type;
        }

        public void Despawn()
        {
            Despawned?.Invoke();
        }
    }
}
