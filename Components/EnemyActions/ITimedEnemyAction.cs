namespace amongus3902.Components.EnemyActions
{
    internal interface ITimedEnemyAction : IEnemyAction
    {
        int Duration { get; set; }
    }
}
