using Microsoft.Xna.Framework;

namespace amongus3902.Components.EnemyActions
{
    public enum TrapState
    {
        Searching,
        Attacking,
        Returning
    }

    internal class TrapAction : IEnemyAction
    {
        public Vector2 HomePos { get; set; }

        public Vector2 TargetPos { get; set; }
        public Vector2 TargetSize { get; set; }

        public float AttackSpeed { get; set; } = 4;

        public float ReturnSpeed { get; set; } = 2;

        public TrapState State { get; set; } = TrapState.Searching;

        public TrapAction() { }
    }
}
