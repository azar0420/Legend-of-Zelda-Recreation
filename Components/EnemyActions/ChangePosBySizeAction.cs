using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amongus3902.Components.EnemyActions
{
    internal class ChangePosBySizeAction : IEnemyAction
    {
        public Vector2 Change { get; set; }

        public ChangePosBySizeAction(Vector2 change)
        {
            Change = change;
        }
    }
}
