using amongus3902.Utils;
using System;

namespace amongus3902.Components.EnemyActions
{
    internal class ChangeAnimRowAction : IEnemyAction
    {
        public int Row { get; set; }

        public Action<Entity, int> SwitchType { get; set; } 

        public bool Looping { get; set; }

        public ChangeAnimRowAction(int row, Action<Entity,int> type = null, bool looping = true)
        {
            Row = row;
            Looping = looping;
            if (type == null)
            {
                SwitchType = AnimationChanger.SetAnimRowKeepColumn;
            }
            else
            {
                SwitchType = type;
            }

        }
    }
}
