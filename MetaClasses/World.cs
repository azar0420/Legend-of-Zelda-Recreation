using amongus3902.Components;
using amongus3902.Systems.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace amongus3902.MetaClasses
{
    enum WorldMode
    {
        Singleplayer,
        Multiplayer,
        Debug,
    }

    internal class World
    {
        private readonly EntityManager _entityManager = new();
        private readonly SystemManager _systemManager = new();

        // holy shit be careful with this guy | hours lost to him: 8
        public event Action<Entity> OnEntityAdded;
        public event Action<Entity> OnEntityRemoved;

        public event Action OnWorldEnd;

        public readonly int SCREEN_WIDTH;
        public readonly int SCREEN_HEIGHT;

        public readonly WorldMode MODE;

        public bool IsPaused { get; set; }

        public World(WorldMode mode, int screenWidth, int screenHeight)
        {
            SCREEN_WIDTH = screenWidth;
            SCREEN_HEIGHT = screenHeight;
            MODE = mode;
        }

        // adds the system to the world and starts it
        public World AddSystem(ISystem system)
        {
            _systemManager.Add(system);
            system.Start(this);
            return this;
        }

        public T GetSystem<T>()
            where T : ISystem
        {
            return _systemManager.Get<T>();
        }

        // calls update on all systems
        public void Update(GameTime gameTime)
        {
            _systemManager.Update(gameTime, IsPaused);
        }

        // calls draw on all systems
        public void Draw(GameTime gameTime)
        {
            _systemManager.Draw(gameTime, IsPaused);
        }

        // adds an entity to the world
        public void AddEntity(Entity entity)
        {
            Debug.Assert(
                !_entityManager.Has(entity.UniqueID),
                $"World already has entity {entity.UniqueID}"
            );

            Debug.Assert(entity != null, $"Entity is null!");

            _entityManager.Add(entity);

            entity._add();
            OnEntityAdded?.Invoke(entity);
        }

        public bool HasEntity(string UID)
        {
            return _entityManager.Has(UID);
        }

        public bool TryRemoveEntity(string UID, out Entity entity)
        {
            bool has = HasEntity(UID);
            entity = null;

            if (has)
            {
                entity = RemoveEntity(UID);
            }

            return has;
        }

        // removes an entity from the world
        public Entity RemoveEntity(string UID)
        {
            Debug.Assert(_entityManager.Has(UID), $"World does not have entity {UID}");

            Entity entity = _entityManager.Remove(UID);

            entity._remove();
            OnEntityRemoved?.Invoke(entity);

            return entity;
        }

        public void ClearEntities()
        {
            foreach (Entity e in _entityManager.GetAllEntities())
            {
                e._remove();
            }

            _entityManager.Clear();
        }

        public List<Entity> GetEntitiesWithComponentOfTypes(params Type[] types)
        {
            Array.ForEach(
                types,
                t => Debug.Assert(t.GetInterface(typeof(IComponent).Name) != null)
            );

            return _entityManager.EntitiesWithComponentsOfTypes(types);
        }

        public void End()
        {
            OnWorldEnd?.Invoke();
        }
    }
}
