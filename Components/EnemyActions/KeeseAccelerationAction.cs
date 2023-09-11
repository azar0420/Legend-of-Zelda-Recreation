namespace amongus3902.Components.EnemyActions
{
    internal class KeeseAccelerationAction : ITimedEnemyAction
    {
        public float Magnitude { get; set; }
        public bool IsDecelerating { get; set; }
        public int Duration { get; set; }

        public double MinDelay = 75f;

        public KeeseAccelerationAction(float Magnitude, bool IsDecelerating, int Duration)
        {
            this.Magnitude = Magnitude;
            this.IsDecelerating = IsDecelerating;
            this.Duration = Duration;
        }
    }
}
