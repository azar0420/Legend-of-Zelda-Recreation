using amongus3902.Systems.Interfaces;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace amongus3902.MetaClasses
{
    internal class SystemManager
    {
        private readonly List<ISystem> _systems = new();

        public void Add(ISystem system)
        {
            _systems.Add(system);
        }

        public T Get<T>()
            where T : ISystem
        {
            return (T)_systems.Find(s => s.GetType() == typeof(T));
        }

        public void Update(GameTime gameTime, bool isPaused = false)
        {
            _systems.ForEach(system =>
            {
                if (isPaused && system is not IAlwaysActiveSystem)
                {
                    return;
                }

                if (system is not IUpdateSystem)
                {
                    return;
                }

                ((IUpdateSystem)system).Update(gameTime);
            });
        }

        public void Draw(GameTime gameTime, bool isPaused = false)
        {
            _systems.ForEach(system =>
            {
                if (isPaused && system is not IAlwaysActiveSystem)
                {
                    return;
                }

                if (system is not IDrawSystem)
                {
                    return;
                }

                ((IDrawSystem)system).Draw(gameTime);
            });
        }
    }
}
