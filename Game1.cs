using amongus3902.Components;
using amongus3902.ContentMetadata;
using amongus3902.Factories;
using amongus3902.ContentMetadata;
using amongus3902.MetaClasses;
using amongus3902.Systems;
using amongus3902.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace amongus3902
{
    public class Game1 : Game
    {
        private SpriteBatch _spriteBatch;
        private GraphicsDeviceManager _graphics;
        private World _gameWorld;

        public static Vector2 NES_SCREEN_SIZE = new(256, 240);
        int WINDOW_SCALE = 3;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.IsFullScreen = false;
          //  _graphics.PreferredBackBufferWidth = 768;
          //  _graphics.PreferredBackBufferHeight = 720;
          //  _graphics.ApplyChanges();

            _graphics.PreferredBackBufferWidth = (int)NES_SCREEN_SIZE.X * WINDOW_SCALE;
            _graphics.PreferredBackBufferHeight = (int)NES_SCREEN_SIZE.Y * WINDOW_SCALE;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            CreateWorld(WorldMode.Debug);
        }

        private void CreateWorld(WorldMode mode)
        {
            _gameWorld?.End();

            _gameWorld = new World(
                mode,
                _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight
            )
                .AddSystem(new LoadSystem(Content))
                .AddSystem(new SoundSystem())
                .AddSystem(new InventorySystem())
                .AddSystem(new LootSystem())
                .AddSystem(new InputSystem())
                .AddSystem(new RenderSystem(_spriteBatch, GraphicsDevice))
                .AddSystem(new PhysicsSystem())
                .AddSystem(new RoomGenerationSystem())
                .AddSystem(new AnimationSystem())
                .AddSystem(new EnemySystem())
                .AddSystem(new PlayerSystem())
                .AddSystem(new ProjectileSystem())
                .AddSystem(new DebugRoomSwitcher())
                .AddSystem(new PickupSystem())
                .AddSystem(new DamageSystem())
                .AddSystem(new MenuSystem())
                .AddSystem(new PauseSystem());

            // bind keys that affect the world
            InputSystem input = _gameWorld.GetSystem<InputSystem>();

            // bind restart keys
            input.Bind(Exit, Keys.F1);
            input.Bind(() => CreateWorld(mode), Keys.F2, PressType.onUp);
            input.Bind(() => CreateWorld(WorldMode.Debug), Keys.F3, PressType.onUp);
            input.Bind(() => CreateWorld(WorldMode.Singleplayer), Keys.F4, PressType.onUp);
            input.Bind(() => CreateWorld(WorldMode.Multiplayer), Keys.F5, PressType.onUp);

            // cheat codes
            CheatCode dancing =
                new(
                    CheatCodeData.DancingCheatCode,
                    () =>
                    {
                        List<Entity> links = _gameWorld.GetEntitiesWithComponentOfTypes(
                            typeof(CharacterController)
                        );
                        foreach (Entity link in links)
                        {
                            PlayerSystem.MakeLinkDance(link);
                        }
                    }
                );

            CheatCode coin =
                new(
                    CheatCodeData.CoinCheatCode,
                    () =>
                    {
                        InventorySystem inventory = _gameWorld.GetSystem<InventorySystem>();
                        int RupeeAdd = 99 - inventory.ConsumablesCount(ItemType.Rupee);

                        if (RupeeAdd > 0)
                        {
                            inventory.AddConsumables(ItemType.Rupee, RupeeAdd);
                        }
                    }
                );

            CheatCode MakeLinkDie =
                new(
                    CheatCodeData.DieCheatCode,
                    () =>
                    {
                        List<Entity> links = _gameWorld.GetEntitiesWithComponentOfTypes(
                            typeof(CharacterController),
                            typeof(HurtBox)
                        );
                        links.First().Get<HurtBox>().Kill(new Entity());
                    }
                );

            input.OnKeyDown += dancing.UpdateCheatCode;
            input.OnKeyDown += coin.UpdateCheatCode;
            input.OnKeyDown += MakeLinkDie.UpdateCheatCode;

            // bind pause screen
            input.Bind(_gameWorld.GetSystem<PauseSystem>().TogglePauseScreen, Keys.Space);

            //bind mute sounds
            input.Bind(() => SoundEffect.MasterVolume = 1 - SoundEffect.MasterVolume, Keys.M);

            // play the background music
            SoundSystem sound = _gameWorld.GetSystem<SoundSystem>();
            sound.PlaySound(ZeldaSound.DungeonMusic);

            //start Link with arrow
            InventorySystem inv = _gameWorld.GetSystem<InventorySystem>();
            inv.AddKeyItem(ItemType.Arrow);
            inv.AddConsumables(ItemType.Bomb, 1);
        }

        protected override void Update(GameTime gameTime)
        {
            _gameWorld.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _gameWorld.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
