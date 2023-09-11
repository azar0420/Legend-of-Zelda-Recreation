using amongus3902.Components;
using amongus3902.MetaClasses;
using amongus3902.Systems.Interfaces;
using amongus3902.Utils;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace amongus3902.Systems
{
    internal class PickupSystem : IUpdateSystem
    {
        private World _world;
        private PlayerSystem _playerSystem;

        public void Start(World world)
        {
            _world = world;
            _playerSystem = world.GetSystem<PlayerSystem>();
        }

        public void Update(GameTime gameTime)
        {
            List<Entity> links = _world.GetEntitiesWithComponentOfTypes(
                typeof(Transform),
                typeof(Sprite),
                typeof(CharacterController)
            );

            List<Entity> items = _world.GetEntitiesWithComponentOfTypes(typeof(Pickup));

            foreach (Entity link in links)
            {
                foreach (Entity item in items)
                {
                    HitItem(link, item);
                }
            }
        }

        private void HitItem(Entity link, Entity item)
        {
            if (Geometry.SpriteOverlaps(link, item))
            {
                _playerSystem.FindItem(link, item);
            }
        }
    }
}
