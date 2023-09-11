namespace amongus3902.Components.EnemyActions
{
    internal class RandomQueenVelocityAction : ITimedEnemyAction
    {
        public float Magnitude { get; set; }
        public int Duration { get; set; }

        public RandomQueenVelocityAction(int Magnitude, int Duration)
        {
            this.Magnitude = Magnitude;
            this.Duration = Duration;

        }
    }
}
