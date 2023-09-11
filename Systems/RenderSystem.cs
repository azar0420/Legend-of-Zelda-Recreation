using amongus3902.Components;
using amongus3902.MetaClasses;
using amongus3902.Systems.Interfaces;
using amongus3902.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace amongus3902.Systems
{
    internal class RenderSystem : IDrawSystem, IAlwaysActiveSystem
    {
        private World _world;
        private readonly SpriteBatch _spriteBatch;
        private readonly Texture2D _pixel;

        private bool _drawColliders = false;

        public RenderSystem(SpriteBatch batch, GraphicsDevice graphics)
        {
            _spriteBatch = batch;

            _pixel = new Texture2D(graphics, 1, 1);
            _pixel.SetData(new[] { new Color(1f, 1f, 1f, 0.01f) });
        }

        public void Start(World world)
        {
            _world = world;

            world.OnWorldEnd += _pixel.Dispose;

            if (world.MODE == WorldMode.Debug)
            {
                //Toggle Colliders
                world
                    .GetSystem<InputSystem>()
                    .Bind(() => _drawColliders = !_drawColliders, Keys.C);
            }
        }

        private static Rectangle MakeSourceRectangle(
            Texture2D texture,
            Vector2 sourceSize,
            int frame
        )
        {
            int frameColumn = (int)(texture.Width / sourceSize.X);
            int pixelColumn = (int)(frame % frameColumn * sourceSize.X);
            int pixelRow = (int)(frame / frameColumn * sourceSize.Y);

            return Geometry.MakeRectangle(new(pixelColumn, pixelRow), sourceSize);
        }

        private void DrawSprite(Sprite sprite, Transform drawAt)
        {
            Rectangle source = MakeSourceRectangle(
                sprite.Sheet.Texture,
                sprite.Sheet.FrameSize,
                sprite.CurrentFrame
            );

            Rectangle destination = Geometry.MakeRectangle(
                drawAt.Position,
                drawAt.Scale * sprite.Sheet.FrameSize
            );

            _spriteBatch.Draw(
                sprite.Sheet.Texture,
                destination,
                source,
                sprite.SpriteTint,
                0,
                Vector2.Zero,
                SpriteEffects.None,
                drawAt.LayerDepth
            );
        }

        private void DrawCollider(PhysicsBody collider, Transform drawAt, string UID)
        {
            if (!collider.CanCollide)
            {
                return;
            }

            Random r = new(UID.GetHashCode());

            _spriteBatch.Draw(
                _pixel,
                Geometry.MakeRectangle(
                    drawAt.Position + collider.ColliderOffset * drawAt.Scale,
                    drawAt.Scale * collider.ColliderSize
                ),
                new Color(r.Next(0, 200), r.Next(0, 200), r.Next(0, 200))
            );
        }

        private static void StartDrawing(SpriteBatch batch)
        {
            batch.Begin(
                SpriteSortMode.FrontToBack,
                BlendState.AlphaBlend,
                SamplerState.PointClamp, // Nearest Neighbor Zoom, other params are defaults
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise
            );
        }

        private static void StopDrawing(SpriteBatch batch)
        {
            batch.End();
        }

        public void Draw(GameTime gameTime)
        {
            List<Entity> entities = _world.GetEntitiesWithComponentOfTypes(
                typeof(Transform),
                typeof(Sprite)
            );

            StartDrawing(_spriteBatch);

            foreach (Entity e in entities)
            {
                DrawSprite(e.Get<Sprite>(), e.Get<Transform>());
            }

            StopDrawing(_spriteBatch);

            if (_drawColliders)
            {
                List<Entity> colliderEntities = _world.GetEntitiesWithComponentOfTypes(
                    typeof(Transform),
                    typeof(PhysicsBody)
                );

                StartDrawing(_spriteBatch);

                foreach (Entity e in colliderEntities)
                {
                    DrawCollider(e.Get<PhysicsBody>(), e.Get<Transform>(), e.UniqueID);
                }

                StopDrawing(_spriteBatch);
            }
        }
    }
}
