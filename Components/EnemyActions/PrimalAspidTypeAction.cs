using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amongus3902.Components.EnemyActions
{
    internal class PrimalAspidTypeAction : ITimedEnemyAction
    {
        public int Duration { get; set; }

        public int Shots { get; set; }

        public int Angle { get; set; }

        public int NextShotTime { get; set; }

        public PrimalAspidTypeAction(int duration, int shots, int angle)
        {
            Duration = duration;
            Shots = shots;
            Angle = angle;
        }
    }
}
