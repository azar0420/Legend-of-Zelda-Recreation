using Microsoft.Xna.Framework;

namespace amongus3902.Components.EnemyActions
{
    internal class ChangeAccelerationAction : ITimedEnemyAction
    {
        public Vector2 Acceleration { get; set; }
        public int Duration { get; set; }

        public ChangeAccelerationAction(Vector2 Acceleration, int Duration)
        {
            this.Acceleration = Acceleration;
            this.Duration = Duration;
        }
    }
}
