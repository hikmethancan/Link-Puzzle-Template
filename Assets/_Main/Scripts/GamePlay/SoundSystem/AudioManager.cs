using System;
using _Main.Scripts.Utilities.Singletons;
using UnityEngine;

namespace _Main.Scripts.GamePlay.SoundSystem
{
    public class AudioManager : Singleton<AudioManager>
    {
        public Sound[] sounds;

        protected override void Awake()
        {
            base.Awake();
        
            foreach (var sound in sounds)
            {
                sound.source = gameObject.AddComponent<AudioSource>();

                sound.source.clip = sound.clip;
                sound.source.volume = sound.volume;
                sound.source.pitch = sound.pitch;
                sound.source.playOnAwake = sound.playOnAwake;
                sound.source.loop = sound.loop;
                if (sound.playOnAwake) sound.source.Play();
            }
        }

        public void Play(string name)
        {
            var s = Array.Find(sounds, sound => sound.name == name);
            if (s != null)
                s.source.Play();
        }

        public void AdjustVolume(string name, float value)
        {
            var s = Array.Find(sounds, sound => sound.name == name);
            if (s != null)
                s.source.volume = value;
        }

        public void Stop(string name)
        {
            var s = Array.Find(sounds, sound => sound.name == name);
            if (s != null)
                s.source.Stop();
        }

        public void StopAll()
        {
            foreach (var sound in sounds)
            {
                sound.source.Stop();
            }
        }
    }
}