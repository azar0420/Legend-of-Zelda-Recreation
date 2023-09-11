using amongus3902.Components;
using amongus3902.Systems;
using amongus3902.Data;
using Microsoft.Xna.Framework;
using System;
using amongus3902.MetaClasses;
using amongus3902.ContentMetadata;
using amongus3902.Utils;
using amongus3902.Components.DespawnConditions;
using System.Collections.Generic;

namespace amongus3902.Factories
{
    public enum ProjectileType
    {
        Fireball,
        WoodenBoomerang,
        MagicBoomerang,
        WoodenArrow,
        MagicArrow,
        Bomb,
        BombExplosion,
        SafeSmoke,
        SusSmoke,
        Fire,
        WoodenSword,
        SwordBeam,
        SwordBeamExplosion,
        DeathSparkle,
        Clink,
        BlueProjectile,
        OrangeProjectile,
        None
    }

    internal class ProjectileFactory
    {
        private readonly int FIREBALL_VEL_MODIFIER = 5;
        private readonly int BOOMERANG_VEL_MODIFIER = 10;
        private readonly float BOOMERANG_ACC_MODIFIER = -0.2f;
        private readonly int MAGIC_BOOMERANG_VEL_MODIFIER = 15;
        private readonly float MAGIC_BOOMERANG_ACC_MODIFIER = -0.3f;
        private readonly float ARROW_VEL_MODIFIER = 10;
        private readonly float SWORDBEAM_VEL_MODIFIER = 10;
        private readonly float SWORDBEAMEXPLOSION_VEL_MODIFIER = 10;
        private readonly float FIRE_VEL_MODIFIER = 3;
        private readonly float PORTAL_VEL_MODIFIER = 5;

        private LoadSystem _loader;
        private SoundSystem _sound;
        private World _world;

        private ProjectileFactoryUtils _utils;

        Dictionary<ProjectileType, int> ProjectileDamage = new Dictionary<ProjectileType, int>
        {
            { ProjectileType.Fireball, 1 },
            { ProjectileType.WoodenBoomerang, 1 },
            { ProjectileType.MagicBoomerang, 1 },
            { ProjectileType.WoodenArrow, 2 },
            { ProjectileType.MagicArrow, 4 },
            { ProjectileType.Bomb, 0 },
            { ProjectileType.BombExplosion, 4 },
            { ProjectileType.Fire, 1 },
            { ProjectileType.WoodenSword, 1 },
            { ProjectileType.SwordBeam, 1 },
            { ProjectileType.SwordBeamExplosion, 1 },
            { ProjectileType.SafeSmoke, 0 },
        };

        Vector2 defaultHitBoxSize = new(0, 0);
        Vector2 defaultHitBoxOffset = new(0, 0);

        public ProjectileFactory(World world)
        {
            _world = world;
            _loader = world.GetSystem<LoadSystem>();
            _sound = world.GetSystem<SoundSystem>();
            _utils = new ProjectileFactoryUtils(this, _loader, _sound, _world);
        }

        public void SpawnProjectile(
            ProjectileType type,
            Transform transform,
            Vector2 direction,
            Entity source
        )
        {
            _world.AddEntity(CreateProjectile(type, transform, direction, source));
        }

        public Entity CreateProjectile(
            ProjectileType type,
            Transform transform,
            Vector2 direction,
            Entity source
        )
        {
            Entity projectile = MakeProjectileOfType(type, direction, source);

            transform ??= new Transform();
            projectile.Attach(transform);

            if (source.Has<Team>() && !projectile.Has<Team>())
            {
                projectile.Attach(new Team(source.Get<Team>().AlliedTo));
            }

            return projectile;
        }

        private Entity MakeProjectile(
            Sprite sprite,
            Projectile projectile,
            HitBox hitbox = null,
            PhysicsBody movement = null,
            Action<Entity, Entity> collideAction = null,
            Animation anim = null,
            Sound sound = null,
            Action<Entity> despawnAction = null,
            Action onRemoveAction = null,
            Team team = null
        )
        {
            Entity result = new Entity();
            result.Attach(sprite);
            result.Attach(projectile);

            if (movement != null)
            {
                result.Attach(movement);
                if (movement.CanCollide)
                    _utils.CreateCollision(movement, projectile, sprite.Sheet.FrameSize);
            }

            if (collideAction != null)
            {
                movement.Touched += (Entity e) => collideAction(result, e);
            }

            if (despawnAction != null)
            {
                projectile.Despawned += () => despawnAction(result);
            }

            if (onRemoveAction != null)
            {
                result.OnRemove += onRemoveAction;
            }

            if (anim != null)
                result.Attach(anim);

            if (sound != null)
                result.OnAdd += () => _sound.PlaySound(sound);

            if (team != null)
                result.Attach(team);

            if (hitbox != null)
            {
                _utils.SetupHitbox(hitbox, sprite.Sheet.FrameSize);
                result.Attach(hitbox);
            }

            return result;
        }

        private Entity MakeProjectileOfType(
            ProjectileType projectileType,
            Vector2 direction,
            Entity source
        )
        {
            return projectileType switch
            {
                ProjectileType.Fireball => MakeFireball(direction, source),
                ProjectileType.WoodenBoomerang => MakeWoodenBoomerang(direction, source),
                ProjectileType.MagicBoomerang => MakeMagicBoomerang(direction, source),
                ProjectileType.WoodenSword => MakeWoodenSword(direction, source),
                ProjectileType.SwordBeam => MakeSwordBeam(direction, source),
                ProjectileType.SwordBeamExplosion => MakeSwordBeamExplosion(direction, source),
                ProjectileType.Bomb => MakeBomb(direction, source),
                ProjectileType.BombExplosion => MakeBombSmoke(direction, source),
                ProjectileType.SafeSmoke => MakeSafeSmoke(direction, source),
                ProjectileType.SusSmoke => MakeSusSmoke(direction, source),
                ProjectileType.WoodenArrow => MakeWoodenArrow(direction, source),
                ProjectileType.MagicArrow => MakeMagicArrow(direction, source),
                ProjectileType.Fire => MakeFire(direction, source),
                ProjectileType.DeathSparkle => MakeDeathSparkle(direction, source),
                ProjectileType.Clink => MakeClink(direction, source),
                ProjectileType.BlueProjectile => MakeBlueProjectile(direction, source),
                ProjectileType.OrangeProjectile => MakeOrangeProjectile(direction, source),
                _ => throw new NotImplementedException(),
            };
        }

        private Entity MakeFireball(Vector2 direction, Entity source) =>
            MakeProjectile(
                new Sprite(_loader.GetSheet(ZeldaSpriteSheet.Fireball), 1),
                new Projectile(
                    ProjectileType.Fireball,
                    source,
                    new IDespawnCondition[] { new CollideDespawn(), new DoDamageDespawn() }
                ),
                new HitBox(
                    defaultHitBoxSize,
                    defaultHitBoxOffset,
                    ProjectileDamage[ProjectileType.Fireball],
                    ProjectileFactoryUtils.DefaultProjectileDidDamageResponse
                ),
                new PhysicsBody(
                    direction * FIREBALL_VEL_MODIFIER,
                    Vector2.Zero,
                    Collidables.Projectile
                ),
                ProjectileFactoryUtils.DefaultProjectileDespawnCollideResponse,
                new Animation(0, 3, 75f)
            );

        private Entity MakeWoodenBoomerang(Vector2 direction, Entity source) =>
            MakeProjectile(
                new Sprite(_loader.GetSheet(ZeldaSpriteSheet.WoodenBoomerang), 1),
                new Projectile(
                    ProjectileType.WoodenBoomerang,
                    source,
                    new IDespawnCondition[] { new OnReturnDespawn() }
                ),
                new HitBox(
                    defaultHitBoxSize,
                    defaultHitBoxOffset,
                    ProjectileDamage[ProjectileType.WoodenBoomerang]
                ),
                new PhysicsBody(
                    direction * BOOMERANG_VEL_MODIFIER,
                    direction * BOOMERANG_ACC_MODIFIER,
                    Collidables.Projectile
                ),
                _utils.BoomerangCollideResponse,
                new Animation(0, 7, 50f),
                _loader.GetSound(ZeldaSound.BoomerangFly),
                null,
                _utils.BoomerangRemoveAction
            );

        private Entity MakeMagicBoomerang(Vector2 direction, Entity source) =>
            MakeProjectile(
                new Sprite(_loader.GetSheet(ZeldaSpriteSheet.MagicBoomerang), 1),
                new Projectile(
                    ProjectileType.MagicBoomerang,
                    source,
                    new IDespawnCondition[] { new OnReturnDespawn() }
                ),
                new HitBox(
                    defaultHitBoxSize,
                    defaultHitBoxOffset,
                    ProjectileDamage[ProjectileType.MagicBoomerang]
                ),
                new PhysicsBody(
                    direction * MAGIC_BOOMERANG_VEL_MODIFIER,
                    direction * MAGIC_BOOMERANG_ACC_MODIFIER,
                    Collidables.Projectile
                ),
                _utils.BoomerangCollideResponse,
                new Animation(0, 7, 50),
                _loader.GetSound(ZeldaSound.BoomerangFly),
                null,
                _utils.BoomerangRemoveAction
            );

        private Entity MakeWoodenSword(Vector2 direction, Entity source) =>
            MakeProjectile(
                _utils.SelectDirectionalSheet(
                    ZeldaSpriteSheet.WoodenSwordDU,
                    ZeldaSpriteSheet.WoodenSwordRL,
                    source.Get<Sprite>().Direction
                ),
                new Projectile(
                    ProjectileType.WoodenSword,
                    source,
                    new IDespawnCondition[] { new TimeDurationDespawn(200) }
                ),
                new HitBox(
                    defaultHitBoxSize,
                    defaultHitBoxOffset,
                    ProjectileDamage[ProjectileType.WoodenSword]
                ),
                null,
                null,
                null,
                _loader.GetSound(ZeldaSound.SwordSwing)
            );

        private Entity MakeSwordBeam(Vector2 direction, Entity source) =>
            MakeProjectile(
                
                new Sprite(
                    _loader.GetSheet(ZeldaSpriteSheet.SwordBeam),
                    (int)source.Get<Sprite>().Direction * source.Get<Sprite>().Sheet.Columns
                ),
                new Projectile(
                    ProjectileType.SwordBeam,
                    source,
                    new IDespawnCondition[]
                    {
                        new TimeDurationDespawn(500),
                        new CollideDespawn(),
                        new DoDamageDespawn()
                    }
                ),
                new HitBox(
                    defaultHitBoxSize,
                    defaultHitBoxOffset,
                    ProjectileDamage[ProjectileType.SwordBeam],
                    ProjectileFactoryUtils.DefaultProjectileDidDamageResponse
                ),
                new PhysicsBody(
                    direction * SWORDBEAM_VEL_MODIFIER,
                    Vector2.Zero,
                    Collidables.Projectile
                ),
                ProjectileFactoryUtils.DefaultProjectileDespawnCollideResponse,
                new Animation(
                    (int)source.Get<Sprite>().Direction * 3,
                    ((int)source.Get<Sprite>().Direction * 3) + 2,
                    50
                ),
                _loader.GetSound(ZeldaSound.SwordBeam),
                _utils.SwordBeamDespawnAction
            );

        private Entity MakeSwordBeamExplosion(Vector2 direction, Entity source) =>
            MakeProjectile(
                new Sprite(
                    _loader.GetSheet(ZeldaSpriteSheet.SwordBeamExplosion),
                    3 * _utils.DiagonalSpriteRow((int)direction.X, (int)direction.Y)
                ),
                new Projectile(
                    ProjectileType.SwordBeamExplosion,
                    source,
                    new IDespawnCondition[] { new TimeDurationDespawn(125) }
                ),
                new HitBox(
                    defaultHitBoxSize,
                    defaultHitBoxOffset,
                    ProjectileDamage[ProjectileType.SwordBeamExplosion]
                ),
                new PhysicsBody(SWORDBEAMEXPLOSION_VEL_MODIFIER * direction, Vector2.Zero),
                null,
                new Animation(
                    3 * _utils.DiagonalSpriteRow((int)direction.X, (int)direction.Y),
                    3 * _utils.DiagonalSpriteRow((int)direction.X, (int)direction.Y) + 2,
                    50
                )
            );

        private Entity MakeBomb(Vector2 direction, Entity source) =>
            MakeProjectile(
                new Sprite(_loader.GetSheet(ZeldaSpriteSheet.Bomb), 0),
                new Projectile(
                    ProjectileType.Bomb,
                    source,
                    new IDespawnCondition[] { new TimeDurationDespawn(2000) }
                ),
                null,
                null,
                null,
                null,
                _loader.GetSound(ZeldaSound.BombDrop),
                _utils.BombDespawnAction
            );

        private Entity MakeBombSmoke(Vector2 direction, Entity source) =>
            MakeProjectile(
                new Sprite(_loader.GetSheet(ZeldaSpriteSheet.BombSmoke), 0),
                new Projectile(
                    ProjectileType.BombExplosion,
                    source,
                    new IDespawnCondition[] { new TimeDurationDespawn(700) }
                ),
                new HitBox(
                    defaultHitBoxSize,
                    defaultHitBoxOffset,
                    ProjectileDamage[ProjectileType.BombExplosion]
                ),
                null,
                null,
                new Animation(0, 2, 250),
                _loader.GetSound(ZeldaSound.BombBlow),
                null,
                null,
                new Team(TeamType.Neutral)
            );

        private Entity MakeSafeSmoke(Vector2 direction, Entity source) =>
            MakeProjectile(
                new Sprite(_loader.GetSheet(ZeldaSpriteSheet.BombSmoke), 0),
                new Projectile(
                    ProjectileType.SafeSmoke,
                    source,
                    new IDespawnCondition[] { new TimeDurationDespawn(250) }
                ),
                new HitBox(
                    defaultHitBoxSize,
                    defaultHitBoxOffset,
                    ProjectileDamage[ProjectileType.SafeSmoke]
                ),
                null,
                null,
                new Animation(0, 2, 100)
            );

        private Entity MakeSusSmoke(Vector2 direction, Entity source) =>
            MakeProjectile(
                new Sprite(_loader.GetSheet(ZeldaSpriteSheet.SusSmoke), 0),
                new Projectile(
                    ProjectileType.SusSmoke,
                    source,
                    new IDespawnCondition[] { new TimeDurationDespawn(2000) }
                ),
                null,
                null,
                null,
                new Animation(0, 2, 222)
            );

        private Entity MakeWoodenArrow(Vector2 direction, Entity source) =>
            MakeProjectile(
                _utils.SelectDirectionalSheet(
                    ZeldaSpriteSheet.ArrowDU,
                    ZeldaSpriteSheet.ArrowRL,
                    source.Get<Sprite>().Direction
                ),
                new Projectile(
                    ProjectileType.WoodenArrow,
                    source,
                    new IDespawnCondition[]
                    {
                        new TimeDurationDespawn(500),
                        new CollideDespawn(),
                        new DoDamageDespawn()
                    }
                ),
                new HitBox(
                    defaultHitBoxSize,
                    defaultHitBoxOffset,
                    ProjectileDamage[ProjectileType.WoodenArrow],
                    ProjectileFactoryUtils.DefaultProjectileDidDamageResponse
                ),
                new PhysicsBody(
                    ARROW_VEL_MODIFIER * direction,
                    Vector2.Zero,
                    Collidables.Projectile
                ),
                _utils.ArrowCollideAction,
                null,
                _loader.GetSound(ZeldaSound.ArrowShoot)
            );

        private Entity MakeMagicArrow(Vector2 direction, Entity source) =>
            MakeProjectile(
                _utils.SelectDirectionalSheet(
                    ZeldaSpriteSheet.MagicArrowDU,
                    ZeldaSpriteSheet.MagicArrowRL,
                    source.Get<Sprite>().Direction
                ),
                new Projectile(
                    ProjectileType.MagicArrow,
                    source,
                    new IDespawnCondition[]
                    {
                        new TimeDurationDespawn(1000),
                        new CollideDespawn(),
                        new DoDamageDespawn()
                    }
                ),
                new HitBox(
                    defaultHitBoxSize,
                    defaultHitBoxOffset,
                    ProjectileDamage[ProjectileType.MagicArrow],
                    ProjectileFactoryUtils.DefaultProjectileDidDamageResponse
                ),
                new PhysicsBody(
                    ARROW_VEL_MODIFIER * direction,
                    Vector2.Zero,
                    Collidables.Projectile
                ),
                _utils.ArrowCollideAction,
                null,
                _loader.GetSound(ZeldaSound.ArrowShoot)
            );

        private Entity MakeFire(Vector2 direction, Entity source) =>
            MakeProjectile(
                new Sprite(_loader.GetSheet(ZeldaSpriteSheet.Fire), 1),
                new Projectile(
                    ProjectileType.Fire,
                    source,
                    new IDespawnCondition[] { new CollideDespawn() }
                ),
                new HitBox(
                    defaultHitBoxSize,
                    defaultHitBoxOffset,
                    ProjectileDamage[ProjectileType.Fire]
                ),
                new PhysicsBody(
                    direction * FIRE_VEL_MODIFIER,
                    Vector2.Zero,
                    Collidables.Projectile
                ),
                ProjectileFactoryUtils.DefaultProjectileDespawnCollideResponse,
                new Animation(0, 1, 100),
                _loader.GetSound(ZeldaSound.Candle),
                _utils.FireDespawnAction
            );

        private Entity MakeDeathSparkle(Vector2 direction, Entity source) =>
            MakeProjectile(
                new Sprite(_loader.GetSheet(ZeldaSpriteSheet.DeathSparkle), 0),
                new Projectile(
                    ProjectileType.DeathSparkle,
                    source,
                    new IDespawnCondition[] { new TimeDurationDespawn(275) }
                ),
                null,
                null,
                null,
                new Animation(0, 3, 75)
            );

        private Entity MakeClink(Vector2 direction, Entity source) =>
            MakeProjectile(
                new Sprite(_loader.GetSheet(ZeldaSpriteSheet.Clink), 0),
                new Projectile(
                    ProjectileType.Clink,
                    source,
                    new IDespawnCondition[] { new TimeDurationDespawn(200) }
                )
            );

        private Entity MakeOrangeProjectile(Vector2 direction, Entity source) =>
            MakeProjectile(
                new Sprite(_loader.GetSheet(ZeldaSpriteSheet.OrangeProjectile), 0),
                new Projectile(
                    ProjectileType.OrangeProjectile,
                    source,
                    new IDespawnCondition[] { new CollideDespawn() }
                ),
                null,
                new PhysicsBody(
                    direction * PORTAL_VEL_MODIFIER,
                    Vector2.Zero,
                    Collidables.PortalProjectile
				),
                _utils.OrangePortalProjectileCollideAction
            );

        private Entity MakeBlueProjectile(Vector2 direction, Entity source) =>
            MakeProjectile(
                new Sprite(_loader.GetSheet(ZeldaSpriteSheet.BlueProjectile), 0),
                new Projectile(
                    ProjectileType.BlueProjectile,
                    source,
                    new IDespawnCondition[] { }
                ),
                null,
                new PhysicsBody(
                    direction * PORTAL_VEL_MODIFIER,
                    Vector2.Zero,
                    Collidables.PortalProjectile
                ),
                _utils.BluePortalProjectileCollideAction
            );
    }
}
