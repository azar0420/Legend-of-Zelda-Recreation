using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amongus3902.Components.EnemyActions
{
    internal class HexaburstAction : ITimedEnemyAction
    {
        public int Bursts { get; set; }

        public float MaxVel { get; set; }

        public float MinVel { get; set; }

        public int Duration { get; set; }

        public HexaburstAction(int duration, int bursts, float maxVel = 1, float minVel = 1)
        {
            Bursts = bursts;
            MaxVel = maxVel;
            MinVel = minVel;
            Duration = duration;
        }
    }
}
