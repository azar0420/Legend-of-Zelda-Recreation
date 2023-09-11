using amongus3902.Components;
using amongus3902.Components.EnemyActions;
using amongus3902.MetaClasses;
using amongus3902.Systems.Interfaces;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace amongus3902.Systems
{
    //advances enemy to the next action if applicable and calls enemyactionsubsystem to execute the action
    internal class EnemySystem : IUpdateSystem
    {
        private World _world;
        private InventorySystem _inv;
        private EnemyActionSubsystem _subsystem;
        private bool Paused = false;

        public void Start(World world)
        {
            _world = world;
            _inv = world.GetSystem<InventorySystem>();
            _subsystem = new EnemyActionSubsystem(world);
        }

        public void Update(GameTime gameTime)
        {
            List<Entity> enemies = _world.GetEntitiesWithComponentOfTypes(
                typeof(Transform),
                typeof(PhysicsBody),
                typeof(EnemyBehavior)
            );
            if (_inv.ConsumablesCount(Factories.ItemType.Clock) == 0)
            {
                Paused = false;
                foreach (Entity enemy in enemies)
                {
                    UpdateEnemy(enemy, gameTime);
                }
            }
            else if (!Paused)
            {
                foreach (Entity enemy in enemies)
                {
                    PhysicsBody eBody = enemy.Get<PhysicsBody>();
                    eBody.Velocity = Vector2.Zero;
                    eBody.Acceleration = Vector2.Zero;
                }
                Paused = true;
            }
            foreach (Entity enemy in enemies)
            {
                UpdateIFrames(enemy, gameTime);
            }
        }

        private void UpdateEnemy(Entity enemy, GameTime gameTime)
        {
            UpdateEnemyBehavior(enemy, gameTime);
        }

        private void UpdateEnemyBehavior(Entity enemy, GameTime gameTime)
        {
            EnemyBehavior enemyBehavior = enemy.Get<EnemyBehavior>();
            if (enemyBehavior.Behaviors.Length > 0)
            {
                IEnemyAction currentAction = enemyBehavior.CurrentAction;
                _subsystem
                    .GetAction(currentAction.GetType())
                    .Invoke(enemy, enemyBehavior, currentAction);
                enemyBehavior.TimeSinceLastUpdate += gameTime.ElapsedGameTime.Milliseconds;

                if (ReadyForActionChange(enemyBehavior))
                {
                    UpdateBehavior(enemyBehavior);
                }
                
            }
        }

        private void UpdateIFrames(Entity enemy, GameTime gameTime)
        {
            EnemyBehavior eBehavior = enemy.Get<EnemyBehavior>();
            Sprite eSprite = enemy.Get<Sprite>();
            if (eBehavior.InvincibleMS > 0)
            {
                eSprite.SpriteTint = Color.Red;
                eBehavior.InvincibleMS -= gameTime.ElapsedGameTime.Milliseconds;
            }
            else
            {
                eSprite.SpriteTint = Color.White;
                eBehavior.TakingDamage = false;
            }
        }

        private static bool ReadyForActionChange(
            EnemyBehavior enemyBehavior
        )
        {
            if (enemyBehavior.CurrentAction is ITimedEnemyAction)
            {
                return enemyBehavior.TimeSinceLastUpdate
                    >= ((ITimedEnemyAction)enemyBehavior.CurrentAction).Duration;
            }
            else
            {
                return true;
            }
        }

        private void UpdateBehavior(EnemyBehavior enemyBehavior)
        {
            enemyBehavior.TimeSinceLastUpdate = 0;
            enemyBehavior.BehaviorIndex =
                (enemyBehavior.BehaviorIndex + 1) % enemyBehavior.Behaviors.Length;
            enemyBehavior.CurrentAction = enemyBehavior.Behaviors[enemyBehavior.BehaviorIndex];
        }
    }
}
