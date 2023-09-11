namespace amongus3902.Components
{
    internal class EnemySpawnerInfo : IComponent
    {
        public Entity Child { get; set; }

        public EnemySpawnerInfo(Entity child)
        {
            Child = child;
        }
    }
}
