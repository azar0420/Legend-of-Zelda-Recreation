namespace amongus3902.Components.EnemyActions
{
    internal class RandomRookVelocityAction : ITimedEnemyAction
    {
        public float Magnitude { get; set; }
        public int Duration { get; set; }

        public RandomRookVelocityAction(int Magnitude, int Duration)
        {
            this.Magnitude = Magnitude;
            this.Duration = Duration;
        }
    }
}
