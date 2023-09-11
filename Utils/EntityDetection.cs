using amongus3902.Components;
using amongus3902.MetaClasses;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace amongus3902.Utils
{
    internal class EntityDetection
    {
        public static bool IsPlayer(Entity entity)
        {
            return entity.Has<CharacterController>();
        }

        public static bool IsPlayerPushing(Entity player, Entity pushee)
        {
            CharacterController controller = player.Get<CharacterController>();
            Transform playerTrans = player.Get<Transform>();
            Transform pusheeTrans = pushee.Get<Transform>();

            Vector2 pushDirection = pusheeTrans.Position - playerTrans.Position;

            Vector2 gridVector = Vector2.Normalize(VectorUtils.SnapToClosestAxis(pushDirection));

            return (gridVector - Direction.DirectionToVector(controller.CurrentDirection)).Length()
                <= 0.01;
        }

        public static bool AreKillableEnemiesInWorld(World world)
        {
            List<Entity> possibleEnemies = world.GetEntitiesWithComponentOfTypes(
                typeof(Team),
                typeof(HurtBox)
            );

            foreach (Entity e in possibleEnemies)
            {
                if (e.Get<Team>().AlliedTo == TeamType.Foe)
                {
                    return true;
                }
            }

            return false;
        }

        public static Entity GetRandomPlayer(World world)
        {
            List<Entity> players = world.GetEntitiesWithComponentOfTypes(typeof(CharacterController), typeof(Transform));
            var random = new Random();
            return players[random.Next(players.Count)];

        }

        public static Vector2 GetVectorToNearestPlayer(World world, Vector2 pos)
        {
            List<Entity> players = world.GetEntitiesWithComponentOfTypes(
                typeof(CharacterController),
                typeof(Transform)
            );
            Vector2 result = players[0].Get<Transform>().Position - pos;
            for (int i = 1; i < players.Count; i++)
            {
                Vector2 playerPos = players[i].Get<Transform>().Position;
                Vector2 temp = playerPos - pos;

                if (temp.Length() < result.Length())
                {
                    result = temp;
                }
            }
            return Vector2.Normalize(result);
        }

        public static Vector2 GetOrthogonalVectorToPlayerCollider(
            World world,
            Entity entity,
            out Entity p
        )
        {
            List<Entity> players = world.GetEntitiesWithComponentOfTypes(
                typeof(CharacterController),
                typeof(Transform),
                typeof(PhysicsBody)
            );
            Vector2 entitySize = Geometry.SpriteSizeInPixels(entity);
            Vector2 entityPos = entity.Get<Transform>().Position;
            foreach (Entity player in players)
            {
                p = player;
                Transform playerTrans = player.Get<Transform>();
                PhysicsBody playerCol = player.Get<PhysicsBody>();
                Vector2 playerSize = playerCol.ColliderSize;
                Vector2 playerPos =
                    playerTrans.Position + playerCol.ColliderOffset * playerTrans.Scale;
                if (
                    Geometry.IntervalOverlaps(
                        playerPos.X,
                        playerPos.X + playerSize.X,
                        entityPos.X,
                        entitySize.X + entityPos.X
                    )
                )
                {
                    return Vector2.Normalize(new Vector2(0, playerPos.Y - entityPos.Y));
                }
                else if (
                    Geometry.IntervalOverlaps(
                        playerPos.Y,
                        playerPos.Y + playerSize.Y,
                        entityPos.Y,
                        entitySize.Y + entityPos.Y
                    )
                )
                {
                    return Vector2.Normalize(new Vector2(playerPos.X - entityPos.X, 0));
                }
            }
            p = null;
            return Vector2.Zero;
        }
    }
}
