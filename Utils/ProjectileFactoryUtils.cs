using amongus3902.Components;
using amongus3902.ContentMetadata;
using amongus3902.Data;
using amongus3902.Factories;
using amongus3902.MetaClasses;
using amongus3902.Systems;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace amongus3902.Utils
{
    internal class ProjectileFactoryUtils
    {
        private ProjectileFactory _fact;
        private LoadSystem _loader;
        private SoundSystem _sound;
        private World _world;
        private PortalFactory _pfact;

        private readonly float PROJ_NOCOLLISION_BORDER = 1;

        public ProjectileFactoryUtils(
            ProjectileFactory fact,
            LoadSystem loader,
            SoundSystem sound,
            World world
        )
        {
            _fact = fact;
            _loader = loader;
            _sound = sound;
            _world = world;
            _pfact = new PortalFactory(_loader, _world);
        }

        public Entity[] SwordBeamDeathProjectiles(Entity source)
        {
            Entity[] spawnOnKill =
            {
                _fact.CreateProjectile(
                    ProjectileType.SwordBeamExplosion,
                    null,
                    new Vector2(-1, -1),
                    source
                ),
                _fact.CreateProjectile(
                    ProjectileType.SwordBeamExplosion,
                    null,
                    new Vector2(1, -1),
                    source
                ),
                _fact.CreateProjectile(
                    ProjectileType.SwordBeamExplosion,
                    null,
                    new Vector2(-1, 1),
                    source
                ),
                _fact.CreateProjectile(
                    ProjectileType.SwordBeamExplosion,
                    null,
                    new Vector2(1, 1),
                    source
                ),
            };
            return spawnOnKill;
        }

        public Entity[] BombDeathProjectiles(Entity source)
        {
            Entity[] spawnOnKill =
            {
                _fact.CreateProjectile(ProjectileType.BombExplosion, null, Vector2.Zero, source)
            };
            return spawnOnKill;
        }

        public Entity[] SafeSmokeDeathProjectile(Entity source)
        {
            Entity[] spawnOnKill =
            {
                _fact.CreateProjectile(ProjectileType.SafeSmoke, null, Vector2.Zero, source)
            };
            return spawnOnKill;
        }

        public int DiagonalSpriteRow(int x, int y)
        {
            x /= Math.Abs(x);
            y /= Math.Abs(y);
            if (x < 0)
                x = 0;
            if (y < 0)
                y = 0;
            return x + 2 * y;
        }

        public Sprite SelectDirectionalSheet(
            ZeldaSpriteSheet DownUp,
            ZeldaSpriteSheet RightLeft,
            Directions direction
        )
        {
            int castDirection = (int)direction;
            if (castDirection == 1 || castDirection == 2)
            {
                return new Sprite(_loader.GetSheet(RightLeft), castDirection - 1, direction);
            }
            else
            {
                return new Sprite(_loader.GetSheet(DownUp), castDirection / 3, direction);
            }
        }

        public void CreateCollision(PhysicsBody body, Projectile proj, Vector2 spriteSize)
        {
            body.CanCollide = true;
            body.ColliderSize = spriteSize - Vector2.One * PROJ_NOCOLLISION_BORDER * 2;
            body.ColliderOffset = Vector2.One * PROJ_NOCOLLISION_BORDER;
        }

        public void SetupHitbox(HitBox hitbox, Vector2 spriteSize)
        {
            hitbox.Size = spriteSize;
            hitbox.Offset = Vector2.Zero;
        }

        public static void DefaultProjectileDespawnCollideResponse(Entity proj, Entity hitEntity)
        {
            proj.Get<Projectile>().Collided = true;
        }

        public void BoomerangCollideResponse(Entity proj, Entity hitEntity)
        {
            _sound.PlaySound(ZeldaSound.Block);
            _fact.SpawnProjectile(
                ProjectileType.Clink,
                proj.Get<Transform>().Duplicate(),
                Vector2.Zero,
                proj
            );
            Projectile projectileBehavior = proj.Get<Projectile>();
            projectileBehavior.Collided = true;

            PhysicsBody body = proj.Get<PhysicsBody>();
            body.CanCollide = false;
            body.Velocity = Vector2.Zero;
        }

        public static void DefaultProjectileDidDamageResponse(Entity proj, Entity damagedEntitty)
        {
            proj.Get<Projectile>().DidDamage = true;
        }

        public void CreateDespawnProjectiles(Entity d, Entity[] projs)
        {
            Transform dTransform = d.Get<Transform>().Duplicate();
            foreach (Entity e in projs)
            {
                e.Replace(dTransform.Duplicate());
                _world.AddEntity(e);
            }
        }

        public void BoomerangRemoveAction()
        {
            _sound.RemoveAudioSourceOfType(ZeldaSound.BoomerangFly);
        }

        public void BombDespawnAction(Entity actor) =>
            CreateDespawnProjectiles(actor, BombDeathProjectiles(actor));

        public void SwordBeamDespawnAction(Entity actor) =>
            CreateDespawnProjectiles(actor, SwordBeamDeathProjectiles(actor));

        public void FireDespawnAction(Entity actor) =>
            CreateDespawnProjectiles(actor, SafeSmokeDeathProjectile(actor));

        public void ArrowCollideAction(Entity proj, Entity hitEntity)
        {
            Transform projTrans = proj.Get<Transform>().Duplicate();
            Sprite arrowSprite = proj.Get<Sprite>();
            Vector2 projDirection = Direction.DirectionToVector(arrowSprite.Direction);
            if (projDirection.X + projDirection.Y > 0)
            {
                projTrans.Position += arrowSprite.Sheet.FrameSize * projDirection * projTrans.Scale;
            }
            _sound.PlaySound(ZeldaSound.Block);
            _fact.SpawnProjectile(ProjectileType.Clink, projTrans, Vector2.Zero, proj);
            DefaultProjectileDespawnCollideResponse(proj, hitEntity);
        }

        public void OrangePortalProjectileCollideAction(Entity proj, Entity hitEntity)
        {
            if (hitEntity.Get<Components.PhysicsBody>().CollisionGroup == Collidables.Wall)
            {
                _pfact.SpawnOrangePortal(proj, _pfact.OrangePortalCollideAction);
            }
       
            _world.TryRemoveEntity(proj.UniqueID, out proj);

	
		}

		public void BluePortalProjectileCollideAction(Entity proj, Entity hitEntity)
		{
			if (hitEntity.Get<Components.PhysicsBody>().CollisionGroup == Collidables.Wall)
			{
                _pfact.SpawnBluePortal(proj, _pfact.BluePortalCollideAction);
			
			}

			_world.TryRemoveEntity(proj.UniqueID, out proj);

		}

	}
}
