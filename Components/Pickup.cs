using System;

namespace amongus3902.Components
{
    public enum PickupType
    {
        GameWinning,
        PogItem,
        NoPogItem,
        Consumable,
        Heart,
        Rupee
    }

    internal class Pickup : IComponent
    {
        public PickupType Type;

        public event Action Collected;

        public Pickup(PickupType type)
        {
            Type = type;
        }

        public void Collect()
        {
            Collected?.Invoke();
        }
    }
}
