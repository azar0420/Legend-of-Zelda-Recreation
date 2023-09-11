using amongus3902.Components;
using Microsoft.Xna.Framework;

namespace amongus3902.Utils
{
    internal class Geometry
    {
        private static float NEARLY_SAME_POS = 5;
        private static float WEAPON_OUT_SCALAR = 0.66f;

        public static Vector2 SpriteSizeInPixels(Entity entity)
        {
            return entity.Get<Sprite>().Sheet.FrameSize * entity.Get<Transform>().Scale;
        }

        public static Rectangle MakeRectangle(Vector2 topLeftPos, Vector2 size)
        {
            return new Rectangle((int)topLeftPos.X, (int)topLeftPos.Y, (int)size.X, (int)size.Y);
        }

        public static bool Overlaps(Rectangle a, Rectangle b)
        {
            // see theorem: https://stackoverflow.com/a/306332
            return a.Left < b.Right && a.Right > b.Left && a.Top < b.Bottom && a.Bottom > b.Top;
        }

        public static bool Overlaps(
            Vector2 topLeftPos0,
            Vector2 size0,
            Vector2 topLeftPos1,
            Vector2 size1
        )
        {
            return Overlaps(MakeRectangle(topLeftPos0, size0), MakeRectangle(topLeftPos1, size1));
        }

        public static bool ColliderOverlaps(Entity entity0, Entity entity1)
        {
            Transform trans0 = entity0.Get<Transform>();
            Transform trans1 = entity1.Get<Transform>();

            PhysicsBody body0 = entity0.Get<PhysicsBody>();
            PhysicsBody body1 = entity1.Get<PhysicsBody>();

            return Overlaps(
                trans0.Position + body0.ColliderOffset * trans0.Scale,
                trans0.Scale * body0.ColliderSize,
                trans1.Position + body1.ColliderOffset * trans1.Scale,
                trans1.Scale * body1.ColliderSize
            );
        }

        public static bool DamageOverlaps(Entity hitEntity, Entity hurtEntity)
        {
            Transform trans0 = hitEntity.Get<Transform>();
            Transform trans1 = hurtEntity.Get<Transform>();

            HitBox hitbox = hitEntity.Get<HitBox>();
            HurtBox hurtbox = hurtEntity.Get<HurtBox>();

            return Overlaps(
                trans0.Position + hitbox.Offset * trans0.Scale,
                trans0.Scale * hitbox.Size,
                trans1.Position + hurtbox.Offset * trans1.Scale,
                trans1.Scale * hurtbox.Size
            );
        }

        public static bool SpriteOverlaps(Entity entity0, Entity entity1)
        {
            Transform trans0 = entity0.Get<Transform>();
            Transform trans1 = entity1.Get<Transform>();

            Sprite sprite0 = entity0.Get<Sprite>();
            Sprite sprite1 = entity1.Get<Sprite>();

            return Overlaps(
                trans0.Position,
                trans0.Scale * sprite0.Sheet.FrameSize,
                trans1.Position,
                trans1.Scale * sprite1.Sheet.FrameSize
            );
        }

        public static Vector2 GetCenterPositionOfSpriteInWorld(Entity e)
        {
            Transform trans = e.Get<Transform>();
            Sprite sprite = e.Get<Sprite>();

            return trans.Position + sprite.Sheet.FrameSize / 2 * trans.Scale;
        }

        public static Vector2 GetCenterPositionAboveSpriteInWorld(Entity e)
        {
            Transform trans = e.Get<Transform>();
            Sprite sprite = e.Get<Sprite>();

            return GetCenterPositionOfSpriteInWorld(e)
                - sprite.Sheet.FrameSize * Vector2.UnitY * trans.Scale;
        }

        public static void CenterSpriteOnPosition(Entity toCenter, Vector2 toReference)
        {
            Transform trans = toCenter.Get<Transform>();
            Sprite sprite = toCenter.Get<Sprite>();

            trans.Position = toReference - sprite.Sheet.FrameSize / 2 * trans.Scale;
            //return (trans0.Position).Equals(trans1.Position);
        }

        public static bool EntityAtPos(Entity e, Vector2 targetPos)
        {
            Transform eTrans = e.Get<Transform>();
            float distance = Vector2.Distance(targetPos, eTrans.Position);
            if (distance < NEARLY_SAME_POS)
            {
                eTrans.Position = targetPos;
                return true;
            }
            return false;
        }

        public static bool IntervalOverlaps(float less1, float more1, float less2, float more2)
        {
            return less1 <= more2 && less2 <= more1;
        }

        public static Vector2 GetCenterPosForProjectile(Entity e, Vector2 direction, Entity proj)
        {
            Directions dir = Direction.VectorToDirection(direction);
            Sprite sprite = e.Get<Sprite>();
            Sprite weapon = proj.Get<Sprite>();
            Transform charTrans = e.Get<Transform>();
            float x = charTrans.Position.X;
            float y = charTrans.Position.Y;
            float scale = charTrans.Scale;

            switch (dir)
            {
                case Directions.Up:
                    x += (sprite.Sheet.Width / 2 - weapon.Sheet.Width / 2) * scale;
                    y -= weapon.Sheet.Height * scale * WEAPON_OUT_SCALAR;
                    break;
                case Directions.Down:
                    x += (sprite.Sheet.Width / 2 - weapon.Sheet.Width / 2) * scale;
                    y += weapon.Sheet.Height * scale * WEAPON_OUT_SCALAR;
                    break;
                case Directions.Left:
                    y += (sprite.Sheet.Height / 2 - weapon.Sheet.Height / 2) * scale;
                    x -= weapon.Sheet.Width * scale * WEAPON_OUT_SCALAR;
                    break;
                case Directions.Right:
                    y += (sprite.Sheet.Height / 2 - weapon.Sheet.Height / 2) * scale;
                    x += weapon.Sheet.Width * scale * WEAPON_OUT_SCALAR;
                    break;
            }

            Vector2 newVec = new Vector2((int)x, (int)y);

            return newVec;
        }
    }
}
