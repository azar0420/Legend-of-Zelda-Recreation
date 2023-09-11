using System;

namespace amongus3902.Components
{
    internal class Animation : IComponent
    {
        public int StartFrame { get; set; }
        public int EndFrame { get; set; }
        public double Delay { get; set; }
        public double TimeSinceLastFrame { get; set; }
        public bool Looping { get; set; }

        public bool ReturnToFirstFrame { get; set; }

        private const double DEFAULT_DELAY = 200; //milliseconds

        public event Action AnimEnded;
        public bool ReachedEnd { get; set; } = false;

        public Animation(
            int startFrame,
            int endFrame,
            double delay = DEFAULT_DELAY,
            bool toLoop = true, 
            bool returnToFirstFrame=true
        )
        {
            StartFrame = startFrame;
            EndFrame = endFrame;
            Delay = delay;
            Looping = toLoop;
            TimeSinceLastFrame = 0;
        }

        public Animation(int startFrame, int endFrame, bool looping, bool returnToFirstFrame=true)
            : this(startFrame, endFrame, DEFAULT_DELAY, looping, returnToFirstFrame) { }

        public Animation Clone()
        {
            return (Animation)MemberwiseClone();
        }

        public void AnimEnd()
        {
            AnimEnded?.Invoke();
        }
    }
}
