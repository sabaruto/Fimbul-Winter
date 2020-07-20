using System;
using UnityEngine;

namespace Dialogue
{
  /// <summary>
  ///   This class holds all the components of each dialogue page and
  ///   which is the next dialogue to go to
  /// </summary>
  [Serializable]
  public class SpeechSlide
  {
    // The basic information about the character
    [SerializeField] private string name;

    [SerializeField] private SpeechSlide nextDialogue;

    // The speech the person is talking about
    [SerializeField] [TextArea(1, 3)] private string sentence;

    // The sprite of the character
    [SerializeField] private Sprite sprite;

    public string Name => name;
    public string Sentence => sentence;

    public SpeechSlide NextDialogue { get => nextDialogue; set => nextDialogue = value; }

    public Sprite Sprite => sprite;
  }
}