using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amongus3902.Components.EnemyActions
{
    internal class DoNothingAction : ITimedEnemyAction
    {
        public int Duration { get; set; }

        public DoNothingAction(int duration)
        {
            Duration = duration;
        }
    }
}
