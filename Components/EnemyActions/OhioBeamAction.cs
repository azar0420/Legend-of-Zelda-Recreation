using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amongus3902.Components.EnemyActions
{
    internal class OhioBeamAction : ITimedEnemyAction
    {
        public int Duration { get; set; }

        public float Velocity { get; set; }

        public int Cooldown { get; set; }

        public int NextUpdate { get; set; }

        public Entity Target { get; set; }

        public OhioBeamAction(int duration, float velocity, int cooldown)
        {
            Duration = duration;
            Velocity = velocity;
            Cooldown = cooldown;
        }
    }
}
