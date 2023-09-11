using amongus3902.Components;
using amongus3902.ContentMetadata;
using amongus3902.Factories;
using amongus3902.MetaClasses;
using amongus3902.Systems.Interfaces;
using System;
using System.Collections.Generic;

namespace amongus3902.Systems
{
    internal class LootSystem: ISystem
    {
        private Dictionary<LootGroup, (float chance, ItemType[] table)> _lootData;
        private World _world;
        private ItemFactory _iFact;
        private Random _rand;

        private int counter = 0;

        public void Start(World world) {
            _world = world;
            _iFact = new ItemFactory(world);
            _lootData = LootData.LootDetails;
            _rand = new Random();
        }


        public void RollLoot(LootGroup lootGroup, Transform trans) {
            counter = (counter + 1) % LootData.TableLength;
            if (lootGroup != LootGroup.X)
            {
                int roll = _rand.Next(0, 100) +1;
                if(roll - (100 * _lootData[lootGroup].chance) >= 0)
                {
                    //spawn the item at the appropriate spot in the loot table
                    Entity item = _iFact.CreateItem(_lootData[lootGroup].table[counter]);
                    item.Replace(trans);
                    _world.AddEntity(item);
                }
            }
        }

    }
}
