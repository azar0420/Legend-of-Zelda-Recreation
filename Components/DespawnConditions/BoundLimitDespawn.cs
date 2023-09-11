using Microsoft.Xna.Framework;

namespace amongus3902.Components.DespawnConditions
{
    internal class BoundLimitDespawn : IDespawnCondition
    {
        public Rectangle DespawnBounds { get; set; }

        public BoundLimitDespawn(Rectangle DespawnBounds)
        {
            this.DespawnBounds = DespawnBounds;
        }

    }
}
