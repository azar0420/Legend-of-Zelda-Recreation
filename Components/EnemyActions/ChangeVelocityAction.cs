using Microsoft.Xna.Framework;

namespace amongus3902.Components.EnemyActions
{
    internal class ChangeVelocityAction : ITimedEnemyAction
    {
        public Vector2 Velocity { get; set; }
        public int Duration { get; set; }

        public ChangeVelocityAction(Vector2 Velocity, int Duration)
        {
            this.Velocity = Velocity;
            this.Duration = Duration;
        }
    }
}
