using amongus3902.Components;
using amongus3902.Systems;
using amongus3902.Data;
using Microsoft.Xna.Framework;
using System;
using amongus3902.MetaClasses;
using amongus3902.ContentMetadata;
using amongus3902.Utils;
using System.Linq;

namespace amongus3902.Factories
{
    public enum BlockType
    {
        Square,
        BlueGap,
        Stairs,
        WhiteBrick,
        Ladder,
        LadderTeleporter,
        BlueFloor,
        BlueSand,
        StatueLeft,
        StatueRight,

        PushableAll,
        PushableLeftRight,
        PushableUpDownLeft,

        InvisTileCollider,
        AboveBasementFloorCollider,

        MusicBlocker,
    }

    internal class BlockFactory
    {
        private readonly World _world;
        private readonly LoadSystem _loader;
        private readonly SpriteSheet _blocksheet;
        private readonly SoundSystem _sound;

        public BlockFactory(World world)
        {
            _world = world;
            _loader = world.GetSystem<LoadSystem>();
            _sound = world.GetSystem<SoundSystem>();
            _blocksheet = _loader.GetSheet(ZeldaSpriteSheet.Blocks);
        }

        public Entity CreateBlock(BlockType type)
        {
            return type switch
            {
                BlockType.PushableAll
                    => CreatePushableBlock(
                        Directions.Up,
                        Directions.Down,
                        Directions.Left,
                        Directions.Right
                    ),
                BlockType.PushableUpDownLeft
                    => CreatePushableBlock(Directions.Up, Directions.Down, Directions.Left),
                BlockType.PushableLeftRight
                    => CreatePushableBlock(Directions.Left, Directions.Right),

                BlockType.InvisTileCollider => CreateInvisCollider(),
                BlockType.AboveBasementFloorCollider => CreateInvisAboveFloorCollider(),

                BlockType.Square => CreateSquareBlock(),
                BlockType.BlueGap => CreateBlueGapBlock(),
                BlockType.Stairs => CreateStairsBlock(),
                BlockType.WhiteBrick => CreateWhiteBrickBlock(),
                BlockType.Ladder => CreateLadderBlock(),
                BlockType.LadderTeleporter => CreateLadderTeleporterBlock(),
                BlockType.BlueFloor => CreateBlueFloorBlock(),
                BlockType.BlueSand => CreateBlueSandBlock(),
                BlockType.StatueLeft => CreateStatueLeftBlock(),
                BlockType.StatueRight => CreateStatueRightBlock(),
                BlockType.MusicBlocker => CreateAmongusMusicOverride(),
                _ => throw new NotImplementedException(),
            };
        }

        private Entity MakeBlock(
            int spriteIndex,
            bool collides,
            Collidables collisionGroup = Collidables.Block
        )
        {
            Entity block = new();

            if (spriteIndex >= 0)
            {
                block.Attach(new Sprite(_blocksheet, spriteIndex));
            }

            return block
                .Attach(new Transform(Vector2.Zero, TransformData.BLOCK_DEPTH))
                .Attach(
                    new PhysicsBody(collides, _blocksheet.FrameSize, Vector2.Zero, collisionGroup)
                );
        }

        public Entity CreateSquareBlock() => MakeBlock(1, true);

        public Entity CreateBlueGapBlock() => MakeBlock(6, true, Collidables.Pit);

        public Entity CreateWhiteBrickBlock() => MakeBlock(8, true);

        public Entity CreateLadderBlock() => MakeBlock(9, false);

        public Entity CreateBlueFloorBlock() => MakeBlock(0, false);

        public Entity CreateBlueSandBlock() => MakeBlock(5, false);

        public Entity CreateStatueLeftBlock() => MakeBlock(3, true);

        public Entity CreateStatueRightBlock() => MakeBlock(2, true);

        public Entity CreatePushableBlock(params Directions[] dirs)
        {
            Entity pushable = MakeBlock(1, true);

            // put over top of other tiles
            pushable.Get<Transform>().LayerDepth = TransformData.PUSHABLE_BLOCK_DEPTH;

            bool gotPushed = false;

            // check for pushes when touched
            pushable.Get<PhysicsBody>().Touched += (Entity player) =>
            {
                if (
                    !(
                        EntityDetection.IsPlayer(player)
                        && !EntityDetection.AreKillableEnemiesInWorld(_world)
                        && EntityDetection.IsPlayerPushing(player, pushable)
                        && !gotPushed
                        && dirs.Contains(player.Get<CharacterController>().CurrentDirection)
                    )
                )
                {
                    return;
                }

                gotPushed = true;

                Physics.PushBlock(
                    pushable,
                    Direction.DirectionToVector(player.Get<CharacterController>().CurrentDirection),
                    (int)Geometry.SpriteSizeInPixels(pushable).X
                );

                _world.GetSystem<RoomGenerationSystem>().InvokeCurrentRoomEvent();
            };

            return pushable;
        }

        public Entity CreateStairsBlock()
        {
            Entity stairs = MakeBlock(7, true);
            stairs.Get<PhysicsBody>().Touched += (Entity player) =>
            {
                _sound.PlaySound(ZeldaSound.Stairs);
                BasementUtil.GoToBasement(_world.GetSystem<RoomGenerationSystem>().SwitchRoom);
            };

            return stairs;
        }

        public Entity CreateLadderTeleporterBlock()
        {
            Entity teleporter = MakeBlock(9, true);
            teleporter.Get<PhysicsBody>().Touched += (Entity toucher) =>
            {
                //keese are NOT allowed to leave the basement !!!!
                if (toucher.Has<CharacterController>())
                {
                    _sound.PlaySound(ZeldaSound.Stairs);
                    BasementUtil.LeaveBasement(_world.GetSystem<RoomGenerationSystem>().SwitchRoom);
                }
            };
            return teleporter;
        }

        public Entity CreateInvisCollider() => MakeBlock(-1, true, Collidables.Pit);

        public Entity CreateInvisAboveFloorCollider()
        {
            Entity block = MakeBlock(-1, true, Collidables.Pit);

            block.Get<PhysicsBody>().ColliderSize = new Vector2(
                RoomConstants.TILE_SIZE,
                RoomConstants.TILE_SIZE / 2 - 1
            );

            return block;
        }

        public Entity CreateAmongusMusicOverride()
        {
            Entity result = new Entity();
            result.Attach(new Transform());

            result.OnAdd += () => { _sound.RemoveAudioSourceOfType(ZeldaSound.DungeonMusic); };
            result.OnRemove += () => { _sound.PlaySound(ZeldaSound.DungeonMusic); };

            return result;
        }
    }
}
