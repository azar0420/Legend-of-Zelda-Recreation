using amongus3902.Components.DespawnConditions;
using System.Collections.Generic;

namespace amongus3902.Components
{
    internal class ProjBackup : IComponent
    {
        public Entity Source { get; set; }

        public List<IDespawnCondition> DespawnConditions { get; set; }

        //event despawn handlers (probably move these with above refactor)
        public bool Collided = false;
        public bool DidDamage = false;

        public ProjBackup(Projectile p)
        {
            Source = p.Source;
            DespawnConditions = new();
            foreach (IDespawnCondition condition in p.DespawnConditions)
            {
                DespawnConditions.Add(condition.Duplicate());
            }
            Collided = p.Collided;
            DidDamage = p.DidDamage;
        }

        public void Regenerate(Projectile p)
        {
            p.Source = Source;
            List<IDespawnCondition> copy = new();
            foreach (IDespawnCondition condition in DespawnConditions)
            {
                copy.Add(condition.Duplicate());
            }
            p.DespawnConditions = copy.ToArray();
            p.Collided = Collided;
            p.DidDamage = DidDamage;
        }
    }
}
