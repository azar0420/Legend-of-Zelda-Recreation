using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amongus3902.Components.EnemyActions
{
    internal class RandomTimedAction : ITimedEnemyAction
    {

        public ITimedEnemyAction CurrentAction { get; set; }

        public ITimedEnemyAction[] PossibleActions { get; set; }

        public int Duration { get; set; }

        public RandomTimedAction(ITimedEnemyAction[] acts)
        {
            PossibleActions = acts;
        }
    }
}
