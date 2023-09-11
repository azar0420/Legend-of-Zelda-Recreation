using System;
using System.Collections.Generic;
using System.Linq;

namespace amongus3902.MetaClasses
{
    // caches entities by their components
    internal class EntityCacher
    {
        // much use comparer because otherwise a big ol memory leak happens :)!
        Dictionary<HashSet<Type>, List<Entity>> _cachedEntitiesByComponents =
            new(HashSet<Type>.CreateSetComparer());

        public List<Entity> FilterAndCache(List<Entity> unfiltered, Type[] components)
        {
            if (IsCached(components))
            {
                return GetCached(components);
            }

            List<Entity> filtered = unfiltered
                .Where(e =>
                {
                    // somehow null values show up - TODO fix later
                    if (e is null)
                    {
                        return false;
                    }

                    // check that the entity has all component types
                    foreach (Type t in components)
                    {
                        if (!e.Has(t))
                            return false;
                    }

                    return true;
                })
                .ToList();

            HashSet<Type> componentSet = new(components);
            _cachedEntitiesByComponents[componentSet] = filtered;

            // copy the list so they dont have access to the cached version
            return filtered.ToList();
        }

        public bool IsCached(Type[] components)
        {
            HashSet<Type> componentSet = new(components);
            return _cachedEntitiesByComponents.ContainsKey(componentSet);
        }

        public List<Entity> GetCached(Type[] components)
        {
            HashSet<Type> componentSet = new(components);
            // copy the list so they dont have access to the cached version
            return _cachedEntitiesByComponents[componentSet].ToList();
        }

        private static bool AreComponentTypesIncludedInEntity(
            Entity entity,
            HashSet<Type> components
        )
        {
            foreach (Type t in components)
            {
                if (!entity.Has(t))
                {
                    return false;
                }
            }

            return true;
        }

        public void AddEntityToCache(Entity entity)
        {
            foreach (var pair in _cachedEntitiesByComponents)
            {
                if (AreComponentTypesIncludedInEntity(entity, pair.Key))
                {
                    pair.Value.Add(entity);
                }
            }
        }

        public void RemoveEntityFromCache(Entity entity)
        {
            // remove entities
            foreach (var pair in _cachedEntitiesByComponents)
            {
                if (AreComponentTypesIncludedInEntity(entity, pair.Key))
                {
                    pair.Value.Remove(entity);
                }
            }

            // remove empty lists as a result
            foreach (var key in _cachedEntitiesByComponents.Keys)
            {
                if (_cachedEntitiesByComponents[key].Count == 0)
                {
                    _cachedEntitiesByComponents.Remove(key);
                }
            }
        }

        public void ClearCache()
        {
            _cachedEntitiesByComponents.Clear();
        }
    }
}
