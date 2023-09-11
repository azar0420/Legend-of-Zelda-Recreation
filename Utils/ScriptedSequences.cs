using amongus3902.Components;
using amongus3902.Data;
using amongus3902.Factories;
using amongus3902.MetaClasses;
using amongus3902.Systems;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace amongus3902.Utils
{
    internal static class ScriptedSequences
    {
        // i'm sorry for my sins, this is a long scripted sequence
        public static async void PlayLinkDeathAnimation(Entity character, World gameWorld)
        {
            DisableSystemsForCutscene(gameWorld);
            RemoveAllEnemiesAndPlayers(gameWorld);

            LoadSystem loader = gameWorld.GetSystem<LoadSystem>();

            // get sprites to fade
            List<Entity> spriteEntities = gameWorld.GetEntitiesWithComponentOfTypes(typeof(Sprite));

            // add a dummy player, just with the sprite and a spinny animation
            Entity spinny = LinkUtil
                .MakeSpinnyLink(character.Get<Sprite>().Sheet)
                .Attach(character.Get<Transform>().Duplicate());

            gameWorld.AddEntity(spinny);
            await FadeSprites(spriteEntities, new Color(255, 50, 50), Color.Black, 5, 200);
            gameWorld.RemoveEntity(spinny.UniqueID);

            // why are you white
            Entity whiteLink = LinkUtil
                .MakeWhiteLink(loader.GetSheet(ZeldaSpriteSheet.WhiteLink))
                .Attach(character.Get<Transform>().Duplicate());
            gameWorld.AddEntity(whiteLink);

            await Task.Delay(500);

            // pop out of existence
            gameWorld.TryRemoveEntity(whiteLink.UniqueID, out Entity _);
            new ProjectileFactory(gameWorld).SpawnProjectile(
                ProjectileType.DeathSparkle,
                character.Get<Transform>().Duplicate(),
                Vector2.Zero,
                spinny
            );

            await Task.Delay(500);

            DisplayHeaderText(new TextMaker(gameWorld).CreateGameOverText(), gameWorld);
        }

        // i'm sorry for my sins, this is a long scripted sequence
        public static async void PlayLinkTriforceWinAnimation(World gameWorld)
        {
            DisableSystemsForCutscene(gameWorld);

            List<Entity> backgroundTiles = gameWorld.GetEntitiesWithComponentOfTypes(
                typeof(Sprite)
            );

            // copy the list so we can remove items from the original
            foreach (Entity entity in backgroundTiles.ToList())
            {
                if (entity.Has<CharacterController>() || entity.Has<Pickup>())
                {
                    backgroundTiles.Remove(entity);
                }
            }

            await FadeSprites(backgroundTiles, Color.White, Color.Black, 5, 200);

            await Task.Delay(500);

            DisplayHeaderText(new TextMaker(gameWorld).CreateGameWinText(), gameWorld);
        }

        public static void DisableSystemsForCutscene(World gameWorld)
        {
            gameWorld.GetSystem<DebugRoomSwitcher>().Disable();
            gameWorld
                .GetEntitiesWithComponentOfTypes(typeof(CharacterController))
                .ForEach(e =>
                {
                    e.Get<CharacterController>().InputsLocked = true;
                });
        }

        public static void RemoveAllEnemiesAndPlayers(World gameWorld)
        {
            List<Entity> enemies = gameWorld.GetEntitiesWithComponentOfTypes(typeof(EnemyBehavior));
            List<Entity> players = gameWorld.GetEntitiesWithComponentOfTypes(
                typeof(CharacterController)
            );

            enemies.ForEach(e => gameWorld.TryRemoveEntity(e.UniqueID, out var _));
            players.ForEach(e => gameWorld.TryRemoveEntity(e.UniqueID, out var _));
        }

        public static async Task FadeSprites(
            List<Entity> spriteEntities,
            Color startColor,
            Color endColor,
            int transitionStates,
            int msDelayBetweenStates
        )
        {
            for (int i = 0; i <= transitionStates; i++)
            {
                Color currentTint = Color.Lerp(startColor, endColor, (float)i / transitionStates);

                foreach (Entity entity in spriteEntities)
                {
                    entity.Get<Sprite>().SpriteTint = currentTint;
                }
                await Task.Delay(msDelayBetweenStates);
            }
        }

        public static void DisplayHeaderText(Entity text, World gameWorld)
        {
            // display game over text and prompt restart when link animation ends (factor out of here)
            Vector2 renderSize = text.Get<Transform>().Scale * text.Get<Sprite>().Sheet.FrameSize;

            // position at a bit higher than the center of screen
            text.Get<Transform>().Position =
                new Vector2(gameWorld.SCREEN_WIDTH, gameWorld.SCREEN_HEIGHT * 0.8f) / 2
                - renderSize / 2;

            gameWorld.AddEntity(text);
        }
    }
}
