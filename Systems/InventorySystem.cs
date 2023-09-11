using amongus3902.Factories;
using amongus3902.MetaClasses;
using amongus3902.Systems.Interfaces;
using amongus3902.Utils;
using System.Collections.Generic;

namespace amongus3902.Systems
{
    internal class InventorySystem : ISystem
    {
        private List<ItemType> keyItems;
        private Dictionary<ItemType, int> consumableItems;
        private ItemType activeItem;
        private static ProjectileType lastUsed;

        public void Start(World world)
        {
            keyItems = new();
            consumableItems = new();
            activeItem = ItemType.None;
        }

        public bool HasKeyItem(ItemType type)
        {
            if (keyItems.Contains(type))
            {
                return true;
            }
            return false;
        }

        public List<ItemType> GetKeyItems()
        {
            return keyItems;
        }

        public void AddKeyItem(ItemType type)
        {
            if (!keyItems.Contains(type))
            {
                keyItems.Add(type);
            }
        }

        public void RemoveKeyItem(ItemType type)
        {
            if (keyItems.Contains(type))
            {
                keyItems.Remove(type);

              
            }
        }

        public bool HasConsumable(ItemType type)
        {
            return consumableItems.ContainsKey(type);
        }

        public int ConsumablesCount(ItemType type)
        {
            if (consumableItems.ContainsKey(type))
            {
                return consumableItems[type];
            }
            return 0;
        }

        public void AddConsumables(ItemType type, int count)
        {
            if (consumableItems.ContainsKey(type))
            {
                consumableItems[type] += count;
            }
            else
            {
                consumableItems.Add(type, count);
            }
            //Debug.WriteLine(string.Join(Environment.NewLine, consumableItems));
        }

        public void RemoveConsumables(ItemType type, int count)
        {
            if (consumableItems.ContainsKey(type))
            {
               consumableItems[type] -= count;
                if (consumableItems[type] < 0)
                {
                    consumableItems[type] = 0;
                }
            }
        }

        public void RemoveAllConsumables(ItemType type)
        {
            if (consumableItems.ContainsKey(type))
            {
                consumableItems[type] = 0;
            }
        }

        public bool TrySetActiveItem(ItemType type)
        {
            if (keyItems.Contains(type) || consumableItems.ContainsKey(type))
            {
                activeItem = type;
                return true;
            }
            return false;
        }

        public ItemType GetActiveItem()
        {
            return activeItem;
        }

        public ProjectileType ActiveItemProjectileType()
        {
            return activeItem switch
            {
                ItemType.Bow => DecideArrowType(),
                ItemType.WoodenBoomerang => ProjectileType.WoodenBoomerang,
                ItemType.MagicBoomerang => ProjectileType.MagicBoomerang,
                ItemType.Bomb => ProjectileType.Bomb,
                ItemType.PortalGun => GetPortalColor(),
                ItemType.Candle => ProjectileType.Fire,
                _ => ProjectileType.None
            };
        }

        public ProjectileType DecideArrowType()
        {
            if (keyItems.Contains(ItemType.MagicArrow))
            {
                return ProjectileType.MagicArrow;
            }
            if (keyItems.Contains(ItemType.Arrow))
            {
                return ProjectileType.WoodenArrow;
            }
            return ProjectileType.None;
        }

		public ProjectileType GetPortalColor()
		{
			ProjectileType type = ProjectileType.OrangeProjectile;
			if (lastUsed == ProjectileType.OrangeProjectile)
			{
				type = ProjectileType.BlueProjectile;
			}
			lastUsed = type;
			return type;
		}


	}
}
