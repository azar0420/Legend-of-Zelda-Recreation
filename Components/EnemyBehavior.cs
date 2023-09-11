using amongus3902.Components.EnemyActions;
using amongus3902.ContentMetadata;

namespace amongus3902.Components
{
    internal class EnemyBehavior : IComponent
    {
        //action, direction, duration(ms)
        public IEnemyAction[] Behaviors { get; set; }

        //milliseconds
        public int TimeSinceLastUpdate { get; set; } = 0;

        public int BehaviorIndex { get; set; } = 0;

        public IEnemyAction CurrentAction { get; set; }

        public bool TakingDamage { get; set; }
        public int InvincibleMS { get; set; } = 0;

        public LootGroup LootGroup { get; set; }

        public Enemys EnemyType { get; set; } = Enemys.OldMan;

        public EnemyBehavior(IEnemyAction[] Behaviors, LootGroup group = LootGroup.X)
        {
            this.Behaviors = Behaviors;
            this.LootGroup = group;
            this.CurrentAction = Behaviors[0];
        }

        public EnemyBehavior Duplicate()
        {
            return (EnemyBehavior)this.MemberwiseClone();
        }
    }
}
