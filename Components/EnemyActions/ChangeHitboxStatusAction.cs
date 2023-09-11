using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amongus3902.Components.EnemyActions
{
    internal class ChangeHitboxStatusAction : IEnemyAction
    {
        public bool Value { get; set; }

        public ChangeHitboxStatusAction(bool value)
        {
            Value = value;
        }
    }
}
