using amongus3902.ContentMetadata;
using Microsoft.Xna.Framework.Audio;

namespace amongus3902.Components
{
    internal class Sound
    {
        public SoundEffect Audio { get; set; }

        public bool Looping { get; set; }

        public float Volume { get; set; }

        public float Pitch { get; set; }

        public float Pan { get; set; }

        public ZeldaSound Type { get; set; }


        public Sound(
            SoundEffect sound,
            ZeldaSound type,
            bool looping = false,
            float volume = 1,
            float pitch = 0,
            float pan = 0
        )
        {
            Audio = sound;
            Type = type;
            Looping = looping;
            Volume = volume;
            Pitch = pitch;
            Pan = pan;
        }
    }
}
