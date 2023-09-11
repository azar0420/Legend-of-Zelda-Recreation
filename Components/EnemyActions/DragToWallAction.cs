using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amongus3902.Components.EnemyActions
{
    internal class DragToWallAction : IEnemyAction
    {
        public Vector2 Direction { get; set; } = Vector2.Zero;

        public Entity Player { get; set; }

        public DragToWallAction(Entity player)
        {
            Player = player;
        }
    }
}
