using amongus3902.Components;
using amongus3902.ContentMetadata;
using amongus3902.Data;
using amongus3902.Factories;
using amongus3902.MetaClasses;
using amongus3902.Systems.Interfaces;
using amongus3902.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace amongus3902.Systems
{
    internal struct RoomStorage
    {
        public Entity BG { get; set; }

        public Dictionary<Directions, DoorStorage> Doors { get; set; }

        public List<Entity> Entities { get; set; }

        public event Action RoomEvent;

        public RoomStorage()
        {
            BG = new();
            Doors = new();
            Entities = new();
            RoomEvent = () => { };
        }

        public void InvokeRoomEvent()
        {
            RoomEvent();
        }
    }

    internal struct DoorStorage
    {
        public Entity DoorTop { get; set; }

        public Entity DoorFrame { get; set; }

        public Entity TransitionCollider { get; set; }

        public DoorState InitialDoorState { get; set; }

        public DoorStorage()
        {
            DoorTop = new();
            DoorFrame = new();
            TransitionCollider = new();
            InitialDoorState = DoorState.None;
        }
    }

    internal class RoomGenerationSystem : ISystem, IAlwaysActiveSystem
    {
        private World _world;
        private LoadSystem _loader;
        private SoundSystem _sound;
        private InventorySystem _inv;
        private RoomStorage _currentRoomStorage;
        private Dictionary<RoomName, RoomStorage> _madeRooms;
        private List<Entity> _players;

        private RoomBorderFactory _borderFact;
        private BlockFactory _blockFact;
        private ItemFactory _itemFact;
        private EnemyFactory _enemyFact;
        private TextMaker _textMaker;
        static RoomData CurrentRoomData;
        static RoomData PrevRoomData;
        private Entity CurrentSkeleton;

        private readonly List<RoomName> spawnItemRooms = new List<RoomName>()
        {
            RoomName.e2_0,
            RoomName.e2_3,
            RoomName.e1_5,
            RoomName.e3_2,
            RoomName.e4_1,
        };

        public void Start(World world)
        {
            _world = world;
            _loader = _world.GetSystem<LoadSystem>();
            _sound = _world.GetSystem<SoundSystem>();
            _inv = _world.GetSystem<InventorySystem>();
            _currentRoomStorage = new RoomStorage();
            _madeRooms = new();

            if (world.MODE == WorldMode.Multiplayer)
            {
                _players = LinkUtil.MakeLinks(5, _loader.GetSheet);
            }
            else
            {
                _players = LinkUtil.MakeLinks(1, _loader.GetSheet);
            }

            _borderFact = new RoomBorderFactory(world);
            _blockFact = new BlockFactory(world);
            _itemFact = new ItemFactory(world);
            _enemyFact = new EnemyFactory(world);
            _textMaker = new TextMaker(world);

        }

        public void SwitchRoom(RoomName room, Vector2 linkDir)
        {
            _inv.RemoveAllConsumables(ItemType.Clock);
            DestroyCurrentRoom();
            SetCurrentRoom(room, linkDir);
            CreateCurrentRoom();
        }

        public void AddToCurrentRoom(Entity e)
        {
            _currentRoomStorage.Entities.Add(e);
        }

        private void CreateCurrentRoom()
        {
            _world.AddEntity(_currentRoomStorage.BG);
            foreach (DoorStorage store in _currentRoomStorage.Doors.Values)
            {
                _world.AddEntity(store.DoorTop);
                _world.AddEntity(store.DoorFrame);
                _world.AddEntity(store.TransitionCollider);
            }
            foreach (Entity e in _currentRoomStorage.Entities)
            {
                if (e != null)
                {
                    _world.AddEntity(e);
                }
            }
            foreach (Entity e in _players)
            {
                if (e != null)
                {
                    _world.AddEntity(e);
                }
            }
        }

        private void DestroyCurrentRoom()
        {
            if (_world.HasEntity(_currentRoomStorage.BG.UniqueID))
            {
                _world.RemoveEntity(_currentRoomStorage.BG.UniqueID);
            }
            foreach (DoorStorage store in _currentRoomStorage.Doors.Values)
            {
                _world.TryRemoveEntity(store.DoorTop.UniqueID, out Entity _);
                _world.TryRemoveEntity(store.DoorFrame.UniqueID, out Entity _);
                _world.TryRemoveEntity(store.TransitionCollider.UniqueID, out Entity _);
            }
            List<Entity> despawned = new();
            foreach (Entity e in _currentRoomStorage.Entities)
            {
                if (_world.HasEntity(e.UniqueID))
                {
                    _world.RemoveEntity(e.UniqueID);
                }
                else
                {
                    despawned.Add(e);
                }
            }
            foreach (Entity e in _players)
            {
                if (_world.HasEntity(e.UniqueID))
                {
                    _world.RemoveEntity(e.UniqueID);
                }
            }
            foreach (Entity e in despawned)
            {
                if (
                    !(
                        e.Has<EnemySpawnerInfo>()
                        && _world.HasEntity(e.Get<EnemySpawnerInfo>().Child.UniqueID)
                    )
                )
                {
                    _currentRoomStorage.Entities.Remove(e);
                }
            }
            DestroyEntitiesWithType(typeof(Projectile));
            DestroyEntitiesWithType(typeof(Pickup));
            DestroyEntitiesWithType(typeof(EnemyBehavior));
            CurrentSkeleton = null;

            PrevRoomData = CurrentRoomData;
        }

        private void DestroyEntitiesWithType(Type t)
        {
            List<Entity> entities = _world.GetEntitiesWithComponentOfTypes(t);
            foreach (Entity e in entities)
            {
                _world.RemoveEntity(e.UniqueID);
            }
        }

        private void SetCurrentRoom(RoomName roomName, Vector2 linkDir)
        {
            RoomData roomData = RoomParser.GetRoomData(roomName);
            CurrentRoomData = roomData;

            if (_madeRooms.ContainsKey(roomName))
            {
                SetExistingRoom(roomName, linkDir);
                OpenDoorComingThrough(linkDir);
                return;
            }

            _currentRoomStorage = _borderFact.CreateBorder(
                roomData.Doors["north"],
                roomData.Doors["west"],
                roomData.Doors["east"],
                roomData.Doors["south"],
                roomData.Name,
                SwitchRoom,
                out Vector2 tileOrigin,
                out float scale,
                roomName == RoomName.basement
            );

            OpenDoorComingThrough(linkDir);

            Vector2 linkWorldPos = CalculateTransitionLinkWorldPos(linkDir, tileOrigin, scale);

            //special case for entering basement
            if (roomName == RoomName.basement)
            {
				linkWorldPos = SetWorldPos(new Vector2(2, 0), tileOrigin, scale);
			}

            UpdateLinks(linkDir, linkWorldPos, scale);

            AddEntitiesOfType(roomData.Blocks, tileOrigin, scale);
            AddEntitiesOfType(roomData.Enemies, tileOrigin, scale);
            AddEntitiesOfType(roomData.Items, tileOrigin, scale);
            AddEntitiesOfType(roomData.Text, tileOrigin, scale);

            _madeRooms.Add(roomName, _currentRoomStorage);
            }

        private void SetExistingRoom(RoomName roomName, Vector2 linkDir)
        {
            _borderFact.GetBackground(out Vector2 tileOrigin, out float scale);
            _currentRoomStorage = _madeRooms[roomName];
            Vector2 linkWorldPos = CalculateTransitionLinkWorldPos(linkDir, tileOrigin, scale);

            //special link position case for entering basement
            if (roomName == RoomName.basement)
            {
                linkWorldPos = SetWorldPos(new Vector2(2, 0), tileOrigin, scale);
            }
            //special link position case for leaving basement
            if (roomName == RoomName.e1_0 && PrevRoomData.Name == RoomName.basement)
            {
                linkWorldPos = SetWorldPos(new Vector2(4, 5), tileOrigin, scale);
            }

            UpdateLinks(linkDir, linkWorldPos, scale);

            foreach (Entity e in _currentRoomStorage.Entities)
            {
                if (e.Has<EnemySpawnerInfo>() && e.Has<Projectile>() && e.Has<ProjBackup>())
                {
                    e.Get<ProjBackup>().Regenerate(e.Get<Projectile>());
                }
            }
        }

        private void OpenDoorComingThrough(Vector2 linkDir)
        {
            if (linkDir.Length() < 0.9)
            {
                return;
            }

            bool isDoor = _currentRoomStorage.Doors.TryGetValue(
                Direction.VectorToDirection(linkDir),
                out DoorStorage doorComingThrough
            );

            if (isDoor)
            {
                DoorUtils.OpenDoor(
                    doorComingThrough.InitialDoorState,
                    Direction.VectorToDirection(linkDir),
                    doorComingThrough.DoorTop,
                    doorComingThrough.DoorFrame
                );
            }
        }

        private void UpdateLinks(Vector2 linkDir, Vector2 linkWorldPos, float scale)
        {
            foreach (Entity e in _players)
            {
                e.Replace(
                    new CharacterController(
                        CharacterAction.Look,
                        Direction.VectorToDirection(-linkDir),
                        e.Get<CharacterController>().Controls
                    )
                );
                e.Replace(new Transform(linkWorldPos, TransformData.PLAYER_DEPTH, scale));
                e.Replace(
                    AnimationData.LINK_ANIMATIONS[
                        (CharacterAction.Look, Direction.VectorToDirection(-linkDir))
                    ]
                );
                e.Get<Sprite>().SpriteTint = Color.White;
            }
        }

        private void AddEntitiesOfType<T>(
            Dictionary<T, List<Vector2>> entities,
            Vector2 tileOrigin,
            float scale
        )
        {
            foreach (KeyValuePair<T, List<Vector2>> p in entities)
            {
                foreach (Vector2 gridPos in p.Value)
                {
                    Entity entity = GenerateEntityOfCorrectType(p.Key);
                    Transform eTransform = entity.Get<Transform>();
                    eTransform.Position = SetWorldPos(gridPos, tileOrigin, scale);
                    eTransform.Scale = scale;

                    if (
                        p.Key.GetType() == typeof(Enemys) && (Enemys)(object)p.Key == Enemys.Stalfos
                    )
                    {
                        CurrentSkeleton = entity.Get<EnemySpawnerInfo>().Child;
                    }

                    if (p.Key.GetType() == typeof(ItemType))
                    {
                        HandleItems(entity, (ItemType)(object)p.Key);
                    }
                    else
                    {
                        _currentRoomStorage.Entities.Add(entity);
                    }
                }
            }
        }

        private void HandleItems(Entity entity, ItemType type)
        {
            if (spawnItemRooms.Contains(CurrentRoomData.Name))
            {
                foreach (Entity e in _currentRoomStorage.Entities)
                {
                    if (e.Has<EnemySpawnerInfo>())
                    {
                        Entity enemy = e.Get<EnemySpawnerInfo>().Child;

                        if (enemy.Has<HurtBox>())
                        {
                            enemy.Get<HurtBox>().Killed += (_) =>
                            {
                                CheckSpawnItems(entity);
                            };
                        }
                    }
                }
                return;
            }
            else if (CurrentSkeleton is not null && type == ItemType.Key)
            {
                entity.Replace(CurrentSkeleton.Get<Transform>());
            }
            _currentRoomStorage.Entities.Add(entity);
        }

        private void CheckSpawnItems(Entity item)
        {
            List<Entity> l = _world.GetEntitiesWithComponentOfTypes(
                typeof(EnemyBehavior),
                typeof(Team)
            );
            if (l.Count == 0)
            {
                _world.AddEntity(item);
                _sound.PlaySound(ZeldaSound.KeyAppear);
            }
        }

        private Entity GenerateEntityOfCorrectType<T>(T enumType)
        {
            object objectType = enumType;

            return enumType switch
            {
                BlockType => _blockFact.CreateBlock((BlockType)objectType),
                ItemType => _itemFact.CreateItem((ItemType)objectType),
                Enemys => _enemyFact.CreateEnemyInSmoke((Enemys)objectType),
                TextType => _textMaker.CreateText((TextType)objectType),
                _ => throw new NotImplementedException(),
            };
            ;
        }

        private Vector2 SetWorldPos(Vector2 gridPos, Vector2 gridOrigin, float scale)
        {
            return gridOrigin + gridPos * RoomConstants.TILE_SIZE * scale;
        }

        private Vector2 CalculateTransitionLinkWorldPos(
            Vector2 linkDir,
            Vector2 tileOrigin,
            float scale
        )
        {
            int linkPosX = (int)(
                linkDir.X * RoomConstants.TILE_GRID_WIDTH / 2 + RoomConstants.TILE_GRID_WIDTH / 2
            );
            int linkPosY = (int)(
                linkDir.Y * RoomConstants.TILE_GRID_HEIGHT / 2 + RoomConstants.TILE_GRID_HEIGHT / 2
            );
            Vector2 linkWorldPos = SetWorldPos(new Vector2(linkPosX, linkPosY), tileOrigin, scale);

            if (linkDir * Vector2.UnitX == Vector2.Zero && RoomConstants.TILE_GRID_WIDTH % 2 == 0)
            {
                linkWorldPos -=
                    _loader.GetSheet(ZeldaSpriteSheet.LinkGreen).FrameSize
                    / 2
                    * scale
                    * Vector2.UnitX;
            }

            if (linkDir * Vector2.UnitY == Vector2.Zero && RoomConstants.TILE_GRID_HEIGHT % 2 == 0)
            {
                linkWorldPos -=
                    _loader.GetSheet(ZeldaSpriteSheet.LinkGreen).FrameSize
                    / 2
                    * scale
                    * Vector2.UnitY;
            }

            if (linkDir.X + linkDir.Y < 0)
            {
                linkWorldPos += linkDir * RoomConstants.TILE_SIZE * scale;
            }

            return linkWorldPos;
        }

        public static RoomData GetRoomData()
        {
            return CurrentRoomData;
        }

        public void InvokeCurrentRoomEvent()
        {
            _currentRoomStorage.InvokeRoomEvent();
        }

        public List<RoomName> GetMadeRooms()
        {
            return new List<RoomName>(_madeRooms.Keys);
        }

    }
}
