using amongus3902.Factories;
using amongus3902.Gamepads;
using amongus3902.Utils;
using System.Collections.Generic;

namespace amongus3902.Components
{
    enum CharacterAction
    {
        Look,
        Walk,
        Attack,
        Item,
        Damaged,
        Dance
    }

    internal class CharacterController : IComponent
    {
        public float PlayerSpeed { get; set; } = 4;
        public bool HasMagicShield { get; set; } = false;

        public CharacterAction CurrentAction { get; set; } = CharacterAction.Look;
        public Directions CurrentDirection { get; set; } = Directions.Down;

        public List<Directions> DirectionalPriority { get; } = new();

        public IGamepad Controls;

        public bool InputsLocked = false;

        public Dictionary<ProjectileType, int> LimitedProjCounts { get; set; } =
            new()
            {
                { ProjectileType.Bomb, 1 },
                { ProjectileType.WoodenBoomerang, 1 },
                { ProjectileType.MagicBoomerang, 1 },
                { ProjectileType.SwordBeam, 1 },
                { ProjectileType.Fire, 1 },
                { ProjectileType.WoodenSword, 1 },
                { ProjectileType.WoodenArrow, 1 },
                { ProjectileType.MagicArrow, 1 },
            };

        public CharacterController(CharacterAction action, Directions dir, IGamepad controls)
        {
            CurrentAction = action;
            CurrentDirection = dir;
            Controls = controls;
        }
    }
}
