using amongus3902.Factories;

namespace amongus3902.Components.EnemyActions
{
    internal class ShootProjectileAction : IEnemyAction
    {
        public ProjectileType Projectile { get; set; }

        public ShootProjectileAction(ProjectileType projectile)
        {
            Projectile = projectile;
        }
    }
}
