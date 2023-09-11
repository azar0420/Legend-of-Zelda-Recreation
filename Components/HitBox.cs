using Microsoft.Xna.Framework;
using System;

namespace amongus3902.Components
{
    internal class HitBox : IComponent
    {
        public Vector2 Size { get; set; } = Vector2.Zero;
        public Vector2 Offset { get; set; } = Vector2.Zero;

        public int DamageAmount { get; set; }
        public event Action<Entity, Entity> DidDamage;

        public bool Enabled { get; set; } = true;

        public HitBox() { }

        public HitBox(Vector2 hitBoxSize, Vector2 hitBoxOffset, int damageAmount)
        {
            Size = hitBoxSize;
            Offset = hitBoxOffset;
            DamageAmount = damageAmount;
        }

        public HitBox(
            Vector2 hitBoxSize,
            Vector2 hitBoxOffset,
            int damageAmount,
            Action<Entity, Entity> didDamage
        )
        {
            Size = hitBoxSize;
            Offset = hitBoxOffset;
            DamageAmount = damageAmount;
            DidDamage = didDamage;
        }

        public void DoDamageResponse(Entity doer, Entity reciever)
        {
            DidDamage?.Invoke(doer, reciever);
        }
    }
}
