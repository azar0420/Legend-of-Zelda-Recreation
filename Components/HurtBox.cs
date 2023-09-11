using Microsoft.Xna.Framework;
using System;

namespace amongus3902.Components
{
    internal class HurtBox : IComponent
    {
        public Vector2 Size { get; set; } = Vector2.Zero;
        public Vector2 Offset { get; set; } = Vector2.Zero;

        public bool IsDead { get; set; } = false;

        public bool Enabled { get; set; } = true;

        public int Health;
        public int MaxHealth;
        public event Action Healed;
        public event Action<Entity> Damaged;
        public event Action<Entity> Killed;

        public HurtBox() { }

        public HurtBox(Vector2 hurtBoxSize, Vector2 hurtBoxOffset, int health)
        {
            Size = hurtBoxSize;
            Offset = hurtBoxOffset;
            Health = health;
            MaxHealth = health;
        }

        public void Heal(int hitpoints)
        {
            Health = Math.Min(MaxHealth, Health + hitpoints);
            Healed?.Invoke();
        }

        public void Damage(int hitpoints, Entity byEntity)
        {
            Health -= hitpoints;
            Damaged?.Invoke(byEntity);
        }

        public void Kill(Entity byEntity)
        {
            Killed?.Invoke(byEntity);
        }
    }
}
