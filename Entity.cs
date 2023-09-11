using amongus3902.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace amongus3902
{
    internal class Entity
    {
        public event Action OnRemove;
        public event Action OnAdd;

        public readonly Dictionary<Type, IComponent> Components = new();
        public readonly string UniqueID;

        public Entity()
        {
            UniqueID = Guid.NewGuid().ToString();
        }

        public Entity Attach<T>(T component)
            where T : IComponent
        {
            Type componentClass = component.GetType();

            //guard to check against adding two components of the same type
            Debug.Assert(!Has<T>(), $"Entity already has a component of type {componentClass}");

            Components.Add(componentClass, component);

            return this;
        }

        public Entity Detatch<T>()
            where T : IComponent
        {
            Components.Remove(typeof(T));
            return this;
        }

        public Entity Replace<T>(T component)
            where T : IComponent
        {
            Detatch<T>();
            Attach<T>(component);
            return this;
        }

        public T Get<T>()
            where T : IComponent
        {
            Debug.Assert(Has<T>(), $"Entity {UniqueID} does not have component {typeof(T)}");

            return (T)Components[typeof(T)];
        }

        public bool Has<T>()
            where T : IComponent
        {
            return Has(typeof(T));
        }

        public bool Has(Type type)
        {
            return Components.ContainsKey(type);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is not Entity)
                return false;

            return UniqueID == ((Entity)obj).UniqueID;
        }

        public override int GetHashCode()
        {
            return UniqueID.GetHashCode();
        }

        public static bool operator ==(Entity left, Entity right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !left.Equals(right);
        }

        // only for the world to call, DO NOT CALL UNLESS YOU ARE WORLD
        public void _remove()
        {
            OnRemove?.Invoke();
        }

        // same here probably
        public void _add()
        {
            OnAdd?.Invoke();
        }
    }
}
