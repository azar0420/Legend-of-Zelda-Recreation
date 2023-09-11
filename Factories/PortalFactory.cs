using amongus3902.Components;
using amongus3902.ContentMetadata;
using amongus3902.MetaClasses;
using amongus3902.Systems;
using amongus3902.Utils;
using amongus3902.Data;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using amongus3902.Components.DespawnConditions;

namespace amongus3902.Factories
{
    internal class PortalFactory
    {
        Entity bluePortal;
        Entity orangePortal;
        LoadSystem _loader;
        World _world;
        bool orangePortalSpawned;
        bool bluePortalSpawned;

        public PortalFactory(LoadSystem loader, World world)
        {
            _loader = loader;
            _world = world;
        }

        public void SpawnOrangePortal(Entity proj, Action<Entity, Entity> collideAction)
        {
            //despawn old portal
            DespawnOrangePortal();

            Entity result = new();
            SpriteSheet sheet = _loader.GetSheet(ZeldaSpriteSheet.OrangePortal);
            result.Attach(new Sprite(sheet));

            Transform projTransform = proj.Get<Transform>();
            Vector2 position = projTransform.Position;
            result.Attach(new Transform(position, TransformData.ITEM_DEPTH));
			
            //add projectile component onto portal so it despawns when room switches
			result.Attach(new Projectile(ProjectileType.None, proj, new IDespawnCondition[] { }));

			result.OnRemove += OrangePortalOnRemove;

			PhysicsBody collider = new PhysicsBody(true, new Vector2(sheet.Width-10, sheet.Height-10), Vector2.Zero, Collidables.Wall);
            collider.Touched += (Entity e) => collideAction(e, result);
            result.Attach(collider);
            _world.AddEntity(result);

            orangePortal = result;
            orangePortalSpawned = true;
        }

        public void DespawnOrangePortal()
        {
            if (orangePortalSpawned)
            {
                _world.RemoveEntity(orangePortal.UniqueID);

			}
          
        }

        public void SpawnBluePortal(Entity proj, Action<Entity, Entity> collideAction)
        {
			//despawn old portal
			DespawnBluePortal();

			Entity result = new();
			SpriteSheet sheet = _loader.GetSheet(ZeldaSpriteSheet.BluePortal);
			result.Attach(new Sprite(sheet));

			Transform projTransform = proj.Get<Transform>();
			Vector2 position = projTransform.Position;
			result.Attach(new Transform(position, TransformData.ITEM_DEPTH));

            //add projectile component onto portal so it despawns when room switches
            result.Attach(new Projectile(ProjectileType.None, proj, new IDespawnCondition[] { }));

			result.OnRemove += BluePortalOnRemove;

			PhysicsBody collider = new PhysicsBody(true, new Vector2(sheet.Width-10, sheet.Height-10), Vector2.Zero, Collidables.Wall);
			collider.Touched += (Entity e) => collideAction(e, result);
			result.Attach(collider);
			_world.AddEntity(result);

      		bluePortal = result;
			bluePortalSpawned = true;
		}

		private void Result_OnRemove()
		{
			throw new NotImplementedException();
		}

		public void DespawnBluePortal()
        {
			if (bluePortalSpawned)
			{
				_world.RemoveEntity(bluePortal.UniqueID);

			}
		}

        public void OrangePortalCollideAction(Entity toucher, Entity portal)
        {
			if (bluePortalSpawned)
			{
				Transform trans = toucher.Get<Transform>();
				Transform portalTrans = bluePortal.Get<Transform>();
				trans.Position = portalTrans.Position;
			}
		}

        public void BluePortalCollideAction(Entity toucher, Entity portal)
        {
			if (orangePortalSpawned)
			{
				Transform trans = toucher.Get<Transform>();
				Transform portalTrans = orangePortal.Get<Transform>();
				trans.Position = portalTrans.Position;
			}
		}

        public void BluePortalOnRemove()
        {
            bluePortalSpawned = false;
        }
        
        public void OrangePortalOnRemove()
        {
            orangePortalSpawned = false;
        }
    }
}
