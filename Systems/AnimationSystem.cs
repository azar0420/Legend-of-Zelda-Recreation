using amongus3902.Components;
using amongus3902.MetaClasses;
using amongus3902.Systems.Interfaces;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;

namespace amongus3902.Systems
{
    internal class AnimationSystem : IUpdateSystem
    {
        private World _world;

        public void Start(World world)
        {
            _world = world;
        }

        public void Update(GameTime gameTime)
        {
            List<Entity> entities = _world.GetEntitiesWithComponentOfTypes(
                typeof(Sprite),
                typeof(Animation)
            );

            foreach (Entity entity in entities)
            {
                Animation anim = entity.Get<Animation>();
                Sprite spr = entity.Get<Sprite>();

                if (ShouldFrameAdvance(gameTime.ElapsedGameTime.Milliseconds, anim, spr.CurrentFrame))
                {
                    spr.CurrentFrame = AdvanceFrame(anim, spr);
                    anim.TimeSinceLastFrame = 0;
                }
                else if (!anim.Looping && spr.CurrentFrame == anim.EndFrame)
                {
                    if (!anim.ReachedEnd)
                    {
                        anim.ReachedEnd = true;
                        anim.AnimEnd();
                    }
                }
            }
        }

        private bool ShouldFrameAdvance(double elapsedTime, Animation anim, int currentFrame)
        {
            anim.TimeSinceLastFrame += elapsedTime;
            return anim.TimeSinceLastFrame >= anim.Delay && 
                !(!anim.Looping && !anim.ReturnToFirstFrame && currentFrame == anim.EndFrame);
        }

        private int AdvanceFrame(Animation anim, Sprite spr)
        {
            if (spr.CurrentFrame < anim.StartFrame)
                return anim.StartFrame;

            if (spr.CurrentFrame >= anim.EndFrame)
            {
                if (!anim.Looping)
                {
                    
                    return anim.EndFrame;
                }

                anim.AnimEnd();
                return anim.StartFrame;
            }

            return spr.CurrentFrame + 1;
        }
    }
}
