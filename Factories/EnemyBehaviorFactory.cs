using amongus3902.Components;
using amongus3902.Components.EnemyActions;
using amongus3902.ContentMetadata;
using amongus3902.Utils;
using Microsoft.Xna.Framework;
using System;

namespace amongus3902.Factories
{
    //creates an array of actions specific to an enemytype (factored out of enemyfactory due to length)
    internal class EnemyBehaviorFactory
    {
        private readonly int KEESE_VELOCITY = 1;
        private readonly int STALFOS_VELOCITY = 1;
        private readonly int GORIYA_VELOCITY = 1;
        private readonly int GEL_VELOCITY = 2;
        private readonly int AQUAMENTUS_VELOCITY = 1;
        private readonly float SUS_BURST_MAX_VEL = 1;
        private readonly float SUS_BURST_MIN_VEL = 0.5f;
        private readonly float SUS_OHIO_BEAM_VEL = (float)Math.Sqrt(2);
        private readonly float SUS_OHIO_BEAM_CORNER_VEL = (float)Math.Sqrt(2.75);
        private readonly float SUS_SPIRAL_VEL = 0.85f;
        private readonly int HARMLESS_OLD_MAN_VELOCITY = 5;

        private readonly int KEESE_DURATION = 1920;
        private readonly int ROOK_TILE_DURATION_BASE = 960;
        private readonly int BOOMERANG_RETURN_WAIT = 2000;
        private readonly int GEL_WAIT_DURATION = 250;
        private readonly int AQUAMENTUS_MOVE_DURATION = 2000;
        private readonly int AQUAMENTUS_SHOOT_DURATION = 2000;
        private readonly int OLD_MAN_WAIT_TIME = 250;
        private readonly int SUS_BURST_TIME = 1500;
        private readonly int SUS_OHIO_BEAM_TIME = 1500;
        private readonly int SUS_OHIO_BEAM_COOLDOWN = 500;
        private readonly int SUS_ASPID_DURATION = 1500;
        private readonly int SUS_SPIRAL_DURATION = 1500;
        private readonly int SUS_SPIRAL_COOLDOWN = 500;
        private readonly int SUS_WARP_TIME = 750;
        private readonly int SUS_WEAK_TIME = 1500;
        private readonly int OLD_MAN_MOVE_TIME = 50;

        private readonly int SUS_BURST_NUM = 5;
        private readonly int SUS_ASPID_SHOTS = 3;
        private readonly int SUS_ASPID_ANGLE = 45;
        private readonly int SUS_ASPID_CORNER_ANGLE = 25;
        private readonly double SUS_SPIRAL_ANGLE = 45.75;

        public EnemyBehavior CreateBehavior(Enemys type)
        {
            return type switch
            {
                Enemys.Keese => CreateKeeseBehavior(),
                Enemys.Stalfos => CreateStalfosBehavior(),
                Enemys.Goriya => CreateGoriyaBehavior(),
                Enemys.Gel => CreateGelBehavior(),
                Enemys.WallMaster => CreateWallMasterBehavior(),
                Enemys.Trap => CreateTrapBehavior(),
                Enemys.Aquamentus => CreateAquamentusBehavior(),
                Enemys.OldMan => CreateOldManBehavior(),
                Enemys.AmogusSus => CreateAmogusSusBehavior(),
                Enemys.HarmlessOldMan => CreateHarmlessOldManBehavior(),
                _ => throw new NotImplementedException(),
            };
        }

        private EnemyBehavior CreateKeeseBehavior()
        {
            return new EnemyBehavior(
                new IEnemyAction[]
                {
                    new KeeseAccelerationAction(KEESE_VELOCITY, false, KEESE_DURATION),
                    new ChangeAccelerationAction(Vector2.Zero, 0),
                    new RandomQueenVelocityAction(KEESE_VELOCITY, KEESE_DURATION),
                    new RandomQueenVelocityAction(KEESE_VELOCITY, KEESE_DURATION),
                    new RandomQueenVelocityAction(KEESE_VELOCITY, KEESE_DURATION),
                    new RandomQueenVelocityAction(KEESE_VELOCITY, KEESE_DURATION),
                    new RandomQueenVelocityAction(KEESE_VELOCITY, KEESE_DURATION),
                    new KeeseAccelerationAction(KEESE_VELOCITY, true, KEESE_DURATION),
                    new ChangeAccelerationAction(Vector2.Zero, 0),
                    new ChangeVelocityAction(Vector2.Zero, KEESE_DURATION)
                }
            );
        }

        private EnemyBehavior CreateStalfosBehavior()
        {
            int stalfosDurationOne = ROOK_TILE_DURATION_BASE / STALFOS_VELOCITY;
            int stalfosDurationTwo = stalfosDurationOne * 2;
            return new EnemyBehavior(
                new IEnemyAction[]
                {
                    new RandomRookVelocityAction(STALFOS_VELOCITY, stalfosDurationOne),
                    new RandomRookVelocityAction(STALFOS_VELOCITY, stalfosDurationTwo)
                },
                LootGroup.C
            );
        }

        private EnemyBehavior CreateGoriyaBehavior()
        {
            int goriyaDurationOne = ROOK_TILE_DURATION_BASE / GORIYA_VELOCITY;
            int goriyaDurationTwo = goriyaDurationOne * 2;
            return new EnemyBehavior(
                new IEnemyAction[]
                {
                    new RandomRookVelocityAction(GORIYA_VELOCITY, goriyaDurationOne),
                    new RandomRookVelocityAction(GORIYA_VELOCITY, goriyaDurationTwo),
                    new RandomRookVelocityAction(GORIYA_VELOCITY, goriyaDurationOne),
                    new RandomRookVelocityAction(GORIYA_VELOCITY, goriyaDurationTwo),
                    new ShootProjectileAction(ProjectileType.WoodenBoomerang),
                    new ChangeVelocityAction(Vector2.Zero, BOOMERANG_RETURN_WAIT)
                },
                LootGroup.B
            );
        }

        private EnemyBehavior CreateGelBehavior()
        {
            return new EnemyBehavior(
                new IEnemyAction[]
                {
                    new RandomRookVelocityAction(
                        GEL_VELOCITY,
                        ROOK_TILE_DURATION_BASE / GEL_VELOCITY
                    ),
                    new ChangeVelocityAction(Vector2.Zero, GEL_WAIT_DURATION)
                }
            );
        }

        private EnemyBehavior CreateWallMasterBehavior() => CreateStalfosBehavior();

        private EnemyBehavior CreateTrapBehavior()
        {
            return new EnemyBehavior(new IEnemyAction[] { new TrapAction() });
        }

        private EnemyBehavior CreateAquamentusBehavior()
        {
            Vector2 leftAquamentusVel = -Vector2.UnitX * AQUAMENTUS_VELOCITY;
            Vector2 rightAquamentusVel = Vector2.UnitX * AQUAMENTUS_VELOCITY;
            int shootSpriteRow = 0;
            int normalSpriteRow = 1;
            return new EnemyBehavior(
                new IEnemyAction[]
                {
                    new ChangeVelocityAction(leftAquamentusVel, AQUAMENTUS_MOVE_DURATION),
                    new ChangeVelocityAction(rightAquamentusVel, AQUAMENTUS_MOVE_DURATION),
                    new ChangeAnimRowAction(shootSpriteRow),
                    new AquamentusShootProjectileSpreadAction(),
                    new ChangeVelocityAction(Vector2.Zero, AQUAMENTUS_SHOOT_DURATION),
                    new ChangeAnimRowAction(normalSpriteRow),
                    new ChangeVelocityAction(leftAquamentusVel, AQUAMENTUS_MOVE_DURATION),
                    new ChangeVelocityAction(rightAquamentusVel, AQUAMENTUS_MOVE_DURATION * 2),
                    new ChangeVelocityAction(leftAquamentusVel, AQUAMENTUS_MOVE_DURATION),
                    new ChangeAnimRowAction(shootSpriteRow),
                    new AquamentusShootProjectileSpreadAction(),
                    new ChangeVelocityAction(Vector2.Zero, AQUAMENTUS_SHOOT_DURATION),
                    new ChangeAnimRowAction(normalSpriteRow),
                    new ChangeVelocityAction(rightAquamentusVel, AQUAMENTUS_MOVE_DURATION),
                    new ChangeVelocityAction(leftAquamentusVel, AQUAMENTUS_MOVE_DURATION),
                },
                LootGroup.D
            );
        }

        private EnemyBehavior CreateOldManBehavior()
        {
            return new EnemyBehavior(
                new IEnemyAction[]
                {
                    new TryOldManShootAction(OLD_MAN_WAIT_TIME, Utils.Directions.Left, true),
                    new TryOldManShootAction(OLD_MAN_WAIT_TIME, Utils.Directions.Right, false),
                    
                }
            );
        }

        private EnemyBehavior CreateHarmlessOldManBehavior()
        {
            return new EnemyBehavior(
                new IEnemyAction[]
                {
                    new RandomRookVelocityAction(
                        HARMLESS_OLD_MAN_VELOCITY,
                        ROOK_TILE_DURATION_BASE / HARMLESS_OLD_MAN_VELOCITY
                    ),
                    new ChangeVelocityAction(Vector2.Zero, OLD_MAN_MOVE_TIME)
                }
            );
        }

        private EnemyBehavior CreateAmogusSusBehavior()
        {
            return new EnemyBehavior(
                new IEnemyAction[]
                {
                    new RandomTimedAction(
                        new ITimedEnemyAction[]
                        {
                            new HexaburstAction(
                                SUS_BURST_TIME,
                                SUS_BURST_NUM,
                                SUS_BURST_MAX_VEL,
                                SUS_BURST_MIN_VEL
                            ),
                            new OhioBeamAction(
                                SUS_OHIO_BEAM_TIME,
                                SUS_OHIO_BEAM_VEL,
                                SUS_OHIO_BEAM_COOLDOWN
                            ),
                            new PrimalAspidTypeAction(
                                SUS_ASPID_DURATION,
                                SUS_ASPID_SHOTS,
                                SUS_ASPID_ANGLE
                            ),
                            new SpiralAction(
                                SUS_SPIRAL_DURATION,
                                SUS_SPIRAL_VEL,
                                SUS_SPIRAL_COOLDOWN,
                                SUS_SPIRAL_ANGLE
                            ),
                        }
                    ),
                    new RandomTimedAction(
                        new ITimedEnemyAction[]
                        {
                            new HexaburstAction(
                                SUS_BURST_TIME,
                                SUS_BURST_NUM,
                                SUS_BURST_MAX_VEL,
                                SUS_BURST_MIN_VEL
                            ),
                            new OhioBeamAction(
                                SUS_OHIO_BEAM_TIME,
                                SUS_OHIO_BEAM_VEL,
                                SUS_OHIO_BEAM_COOLDOWN
                            ),
                            new PrimalAspidTypeAction(
                                SUS_ASPID_DURATION,
                                SUS_ASPID_SHOTS,
                                SUS_ASPID_ANGLE
                            ),
                            new SpiralAction(
                                SUS_SPIRAL_DURATION,
                                SUS_SPIRAL_VEL,
                                SUS_SPIRAL_COOLDOWN,
                                SUS_SPIRAL_ANGLE
                            ),
                        }
                    ),
                    new RandomTimedAction(
                        new ITimedEnemyAction[]
                        {
                            new HexaburstAction(
                                SUS_BURST_TIME,
                                SUS_BURST_NUM,
                                SUS_BURST_MAX_VEL,
                                SUS_BURST_MIN_VEL
                            ),
                            new SpiralAction(
                                SUS_SPIRAL_DURATION,
                                SUS_SPIRAL_VEL,
                                SUS_SPIRAL_COOLDOWN,
                                SUS_SPIRAL_ANGLE
                            ),
                        }
                    ),
                    new ChangeAnimRowAction(1, AnimationChanger.SetAnimRowResetColumn, false),
                    new ChangeHitboxStatusAction(false),
                    new DoNothingAction(SUS_WARP_TIME),
                    new ChangePosBySizeAction(new Vector2(-2.5f, -1)),
                    new ChangeAnimRowAction(2, AnimationChanger.SetAnimRowResetColumn, false),
                    new DoNothingAction(SUS_WARP_TIME),
                    new ChangeHitboxStatusAction(true),
                    new ChangeAnimRowAction(0, AnimationChanger.SetAnimRowResetColumn, true),
                    new RandomTimedAction(
                        new ITimedEnemyAction[]
                        {
                            new OhioBeamAction(
                                SUS_OHIO_BEAM_TIME,
                                SUS_OHIO_BEAM_CORNER_VEL,
                                SUS_OHIO_BEAM_COOLDOWN
                            ),
                            new PrimalAspidTypeAction(
                                SUS_ASPID_DURATION,
                                SUS_ASPID_SHOTS,
                                SUS_ASPID_CORNER_ANGLE
                            ),
                        }
                    ),
                    new ChangeAnimRowAction(1, AnimationChanger.SetAnimRowResetColumn, false),
                    new ChangeHitboxStatusAction(false),
                    new DoNothingAction(SUS_WARP_TIME),
                    new ChangePosBySizeAction(new Vector2(5, 2.5f)),
                    new ChangeAnimRowAction(2, AnimationChanger.SetAnimRowResetColumn, false),
                    new DoNothingAction(SUS_WARP_TIME),
                    new ChangeHitboxStatusAction(true),
                    new ChangeAnimRowAction(0, AnimationChanger.SetAnimRowResetColumn, true),
                    new RandomTimedAction(
                        new ITimedEnemyAction[]
                        {
                            new OhioBeamAction(
                                SUS_OHIO_BEAM_TIME,
                                SUS_OHIO_BEAM_CORNER_VEL,
                                SUS_OHIO_BEAM_COOLDOWN
                            ),
                            new PrimalAspidTypeAction(
                                SUS_ASPID_DURATION,
                                SUS_ASPID_SHOTS,
                                SUS_ASPID_CORNER_ANGLE
                            ),
                        }
                    ),
                    new ChangeAnimRowAction(1, AnimationChanger.SetAnimRowResetColumn, false),
                    new ChangeHitboxStatusAction(false),
                    new DoNothingAction(SUS_WARP_TIME),
                    new ChangePosBySizeAction(new Vector2(0, -2.5f)),
                    new ChangeAnimRowAction(2, AnimationChanger.SetAnimRowResetColumn, false),
                    new DoNothingAction(SUS_WARP_TIME),
                    new ChangeHitboxStatusAction(true),
                    new ChangeAnimRowAction(0, AnimationChanger.SetAnimRowResetColumn, true),
                    new RandomTimedAction(
                        new ITimedEnemyAction[]
                        {
                            new OhioBeamAction(
                                SUS_OHIO_BEAM_TIME,
                                SUS_OHIO_BEAM_CORNER_VEL,
                                SUS_OHIO_BEAM_COOLDOWN
                            ),
                            new PrimalAspidTypeAction(
                                SUS_ASPID_DURATION,
                                SUS_ASPID_SHOTS,
                                SUS_ASPID_CORNER_ANGLE
                            ),
                        }
                    ),
                    new ChangeAnimRowAction(1, AnimationChanger.SetAnimRowResetColumn, false),
                    new ChangeHitboxStatusAction(false),
                    new DoNothingAction(SUS_WARP_TIME),
                    new ChangePosBySizeAction(new Vector2(-5, 2.5f)),
                    new ChangeAnimRowAction(2, AnimationChanger.SetAnimRowResetColumn, false),
                    new DoNothingAction(SUS_WARP_TIME),
                    new ChangeHitboxStatusAction(true),
                    new ChangeAnimRowAction(0, AnimationChanger.SetAnimRowResetColumn, true),
                    new RandomTimedAction(
                        new ITimedEnemyAction[]
                        {
                            new OhioBeamAction(
                                SUS_OHIO_BEAM_TIME,
                                SUS_OHIO_BEAM_CORNER_VEL,
                                SUS_OHIO_BEAM_COOLDOWN
                            ),
                            new PrimalAspidTypeAction(
                                SUS_ASPID_DURATION,
                                SUS_ASPID_SHOTS,
                                SUS_ASPID_CORNER_ANGLE
                            )
                        }
                    ),
                    new ChangeAnimRowAction(1, AnimationChanger.SetAnimRowResetColumn, false),
                    new ChangeHitboxStatusAction(false),
                    new DoNothingAction(SUS_WARP_TIME),
                    new ChangePosBySizeAction(new Vector2(2.5f, -1.5f)),
                    new ChangeAnimRowAction(2, AnimationChanger.SetAnimRowResetColumn, false),
                    new DoNothingAction(SUS_WARP_TIME),
                    new ChangeHitboxStatusAction(true),
                    new ChangeAnimRowAction(0, AnimationChanger.SetAnimRowResetColumn, true),
                    new DoNothingAction(SUS_WEAK_TIME),
                }
            );
        }
    }
}
