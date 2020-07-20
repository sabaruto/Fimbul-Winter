using System;
using Base_Classes;
using UnityEngine;

namespace Managers
{
  public class SoundManager : MonoBehaviour
  {
    [SerializeField] protected Sound[] sounds;

    public void Awake()
    {
      // Assiging all the sounds to a source in the audio manage class
      foreach (Sound s in sounds)
      {
        s.source = gameObject.AddComponent<AudioSource>();
        s.SetVariables();
      }
    }

    /// <summary>
    ///   Plays the sound with name given. No sound is played if there is no sound
    ///   with that name
    /// </summary>
    public void Play(string name)
    {
      // Finding the sound with the given name
      Sound s = FindSound(name);
      s?.Play();
    }

    public void Stop(string name)
    {
      Sound s = FindSound(name);
      s?.Stop();
    }

    public bool IsPlaying(string name)
    {
      Sound s = FindSound(name);
      if (s != null) return s.IsPlaying;
      return false;
    }

    public Sound FindSound(string name)
    {
      Sound s = Array.Find(sounds, sound => sound.name == name);

      if (s == null)
      {
        Debug.LogError("No sound called: " + name + " has been found");
        return null;
      }

      return s;
    }
  }
}