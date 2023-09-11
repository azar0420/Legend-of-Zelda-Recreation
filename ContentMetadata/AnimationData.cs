using amongus3902.Components;
using amongus3902.Utils;
using System.Collections.Generic;

namespace amongus3902.ContentMetadata
{
    internal static class AnimationData
    {
        public static readonly Dictionary<
            (CharacterAction action, Directions direction),
            Animation
        > LINK_ANIMATIONS =
            new()
            {
                { (CharacterAction.Look, Directions.Up), new Animation(15, 15) },
                { (CharacterAction.Look, Directions.Down), new Animation(3, 3) },
                { (CharacterAction.Look, Directions.Left), new Animation(11, 11) },
                { (CharacterAction.Look, Directions.Right), new Animation(7, 7) },
                //
                { (CharacterAction.Walk, Directions.Up), new Animation(14, 15) },
                { (CharacterAction.Walk, Directions.Down), new Animation(2, 3) },
                { (CharacterAction.Walk, Directions.Left), new Animation(10, 11) },
                { (CharacterAction.Walk, Directions.Right), new Animation(6, 7) },
                //
                { (CharacterAction.Attack, Directions.Up), new Animation(13, 15, false) },
                { (CharacterAction.Attack, Directions.Down), new Animation(1, 3, false) },
                { (CharacterAction.Attack, Directions.Left), new Animation(9, 11, false) },
                { (CharacterAction.Attack, Directions.Right), new Animation(5, 7, false) },
                //
                { (CharacterAction.Item, Directions.None), new Animation(36, 37, 200, false) },
                //
                { (CharacterAction.Damaged, Directions.Down), new Animation(16, 19, 50) },
                { (CharacterAction.Damaged, Directions.Right), new Animation(20, 23, 50) },
                { (CharacterAction.Damaged, Directions.Left), new Animation(24, 27, 50) },
                { (CharacterAction.Damaged, Directions.Up), new Animation(28, 31, 50) },
                { (CharacterAction.Dance, Directions.None), new Animation(32,37)}
            };
    }
}