using amongus3902.Components;
using amongus3902.ContentMetadata;
using amongus3902.MetaClasses;
using amongus3902.Systems.Interfaces;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System.Diagnostics;

namespace amongus3902.Systems
{
    internal class SoundSystem : IAlwaysActiveSystem
    {
        private LoadSystem _loader;
        private Dictionary<
            ZeldaSound,
            (int sources, SoundEffectInstance playableSound)
        > loopingSounds = new();
        private bool loopPaused = false;

        public void Start(World world)
        {
            _loader = world.GetSystem<LoadSystem>();

            // stop sounds when world ends
            world.OnWorldEnd += ClearLoopingSounds;
        }

        public void PlaySound(ZeldaSound soundType)
        {
            Sound sound = _loader.GetSound(soundType);
            if (sound.Looping)
            {
                TryStartLoopingSound(sound, soundType);
            }
            else
            {
                SoundEffectInstance playableAudio = CreatePlayableSound(sound);
                playableAudio.Play();
            }
        }

        public void PlaySound(Sound sound)
        {
            if (sound.Looping)
            {
                TryStartLoopingSound(sound, sound.Type);
            }
            else
            {
                SoundEffectInstance playableAudio = CreatePlayableSound(sound);
                playableAudio.Play();
            }
        }

        public void RemoveAudioSourceOfType(ZeldaSound soundType)
        {
            if (loopingSounds.ContainsKey(soundType))
            {
                (int sources, SoundEffectInstance playableSound) = loopingSounds[soundType];
                loopingSounds[soundType] = (sources - 1, playableSound);
                if (sources - 1 <= 0)
                {
                    playableSound.Stop();
                    loopingSounds.Remove(soundType);
                }
            }
        }

        public void ForceLoopingPause()
        {
            loopPaused = true;
            foreach ((int number, SoundEffectInstance playableSound) pair in loopingSounds.Values)
            {
                pair.playableSound.Pause();
            }
        }

        public void ForceLoopingResume()
        {
            if (loopPaused)
            {
                foreach (
                    (int number, SoundEffectInstance playableSound) pair in loopingSounds.Values
                )
                {
                    pair.playableSound.Play();
                }
                loopPaused = false;
            }
        }

        public void ClearLoopingSounds()
        {
            foreach ((int number, SoundEffectInstance playableSound) pair in loopingSounds.Values)
            {
                pair.playableSound.Stop();
            }
            loopingSounds = new();
        }

        private SoundEffectInstance CreatePlayableSound(Sound sound)
        {
            SoundEffectInstance playableAudio = sound.Audio.CreateInstance();
            return playableAudio;
        }

        private void TryStartLoopingSound(Sound sound, ZeldaSound soundType)
        {
            if (loopingSounds.ContainsKey(soundType))
            {
                (int sources, SoundEffectInstance playableSound) = loopingSounds[soundType];
                loopingSounds[soundType] = (sources + 1, playableSound);
                if (!loopPaused)
                    playableSound.Play();
            }
            else
            {
                SoundEffectInstance playableAudio = CreatePlayableSound(sound);
                playableAudio.IsLooped = true;
                loopingSounds.Add(soundType, (1, playableAudio));
                if (!loopPaused)
                    playableAudio.Play();
            }
        }
    }
}
