using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Base_Classes
{
  /// <summary>
  ///     This defines a sound that can be played
  /// </summary>
  [Serializable]
    public class Sound
    {
        public AudioClip clip;
        public bool loop;
        public AudioMixerGroup mixerGroup;
        public string name;

        [HideInInspector] public AudioSource source;

        [Range(0f, 1f)] public float volume;

        public bool IsPlaying => source.isPlaying;

        public void SetVariables()
        {
            source.volume = volume;
            source.clip = clip;
            source.loop = loop;
            source.outputAudioMixerGroup = mixerGroup;
        }

        public void Play()
        {
            source.Play();
        }

        public void Stop()
        {
            source.Stop();
        }
    }
}