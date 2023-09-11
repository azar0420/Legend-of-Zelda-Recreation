using System;
using System.Collections.Generic;
using System.Linq;

namespace amongus3902.MetaClasses
{
    internal class EntityManager
    {
        // entities, indexed by UID
        private readonly Dictionary<string, Entity> _entitiesByUID = new();
        private readonly EntityCacher _cacher = new();

        public void Add(Entity entity)
        {
            _entitiesByUID.Add(entity.UniqueID, entity);
            _cacher.AddEntityToCache(entity);
        }

        public Entity Remove(string UID)
        {
            Entity entity = _entitiesByUID[UID];
            _entitiesByUID.Remove(UID);
            _cacher.RemoveEntityFromCache(entity);

            return entity;
        }

        public void Clear()
        {
            _cacher.ClearCache();
            _entitiesByUID.Clear();
        }

        public bool Has(string UID)
        {
            return _entitiesByUID.ContainsKey(UID);
        }

        public List<Entity> EntitiesWithComponentsOfTypes(params Type[] components)
        {
            if (_cacher.IsCached(components))
            {
                return _cacher.GetCached(components);
            }

            return _cacher.FilterAndCache(_entitiesByUID.Values.ToList(), components);
        }

        public List<Entity> GetAllEntities()
        {
            return _entitiesByUID.Values.ToList();
        }
    }
}
