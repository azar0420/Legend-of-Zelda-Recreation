using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amongus3902.Components.EnemyActions
{
    internal class SpiralAction : ITimedEnemyAction
    {
        public int Duration { get; set; }

        public float Velocity { get; set; }

        public int Cooldown { get; set; }

        public double AngleDiff { get; set; }

        public int NextUpdate { get; set; }

        public double CurrentAngle { get; set; }

        public int Direction { get; set; } = 1;

        public SpiralAction(int duration, float velocity, int cooldown, double angleDiff)
        {
            Duration = duration;
            Velocity = velocity;
            Cooldown = cooldown;
            AngleDiff = angleDiff;
        }
    }
}
