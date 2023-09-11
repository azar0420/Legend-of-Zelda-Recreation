using amongus3902.Factories;
using System.Collections.Generic;

namespace amongus3902.ContentMetadata
{
    //https://www.zeldadungeon.net/zelda-runners-examining-random-and-forced-drops-and-chatting-with-zant/
    //source of loot table data
    public enum LootGroup
    {
        A,
        B,
        C,
        D,
        X
    }

    internal class LootData
    {
        public readonly static int TableLength = 10;

        public readonly static Dictionary<LootGroup, (float chance, ItemType[] table)> LootDetails =
            new()
            {
                {
                    LootGroup.A,
                    (
                        0.31f,
                        new[]
                        {
                            ItemType.Rupee,
                            ItemType.Heart,
                            ItemType.Rupee,
                            ItemType.Fairy,
                            ItemType.Rupee,
                            ItemType.Heart,
                            ItemType.Heart,
                            ItemType.Rupee,
                            ItemType.Rupee,
                            ItemType.Heart
                        }
                    )
                },
                {
                    LootGroup.B,
                    (
                        0.41f,
                        new[]
                        {
                            ItemType.Bomb,
                            ItemType.Rupee,
                            ItemType.Clock,
                            ItemType.Rupee,
                            ItemType.Heart,
                            ItemType.Bomb,
                            ItemType.Rupee,
                            ItemType.Bomb,
                            ItemType.Heart,
                            ItemType.Heart
                        }
                    )
                },
                {
                    LootGroup.C,
                    (
                        0.59f,
                        new[]
                        {
                            ItemType.Rupee,
                            ItemType.Heart,
                            ItemType.Rupee,
                            ItemType.BigRupee,
                            ItemType.Heart,
                            ItemType.Clock,
                            ItemType.Rupee,
                            ItemType.Rupee,
                            ItemType.Rupee,
                            ItemType.BigRupee
                        }
                    )
                },
                {
                    LootGroup.D,
                    (
                        0.41f,
                        new[]
                        {
                            ItemType.Heart,
                            ItemType.Fairy,
                            ItemType.Rupee,
                            ItemType.Heart,
                            ItemType.Fairy,
                            ItemType.Heart,
                            ItemType.Heart,
                            ItemType.Heart,
                            ItemType.Rupee,
                            ItemType.Heart
                        }
                    )
                },
            };
    }
}
