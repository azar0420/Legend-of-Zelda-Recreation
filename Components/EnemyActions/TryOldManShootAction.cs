using amongus3902.Utils;

namespace amongus3902.Components.EnemyActions
{

    internal class TryOldManShootAction : ITimedEnemyAction
    {
        public int Duration { get; set; }

        public Directions Side { get; set; }

        public bool TryAddOldManText { get; set; }

        public TryOldManShootAction(int duration, Directions side, bool tryAddOldManText)
        {
            Duration = duration;
            Side = side;
            TryAddOldManText = tryAddOldManText;
        }
    }
}
