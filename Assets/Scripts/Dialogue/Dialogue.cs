using UnityEngine;

namespace Dialogue
{
  /// <summary>
  ///   The contents of a dialogue sequence
  /// </summary>
  [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
  public class Dialogue : ScriptableObject
  {
    // The array of speech used for the dialogue
    [SerializeField] private SpeechSlide[] dialogueSequence;

    // Gets the start of the sentence
    public SpeechSlide StartSpeech()
    {
      // Connects all the speech slides together
      for (var speechIndex = 0; speechIndex < dialogueSequence.Length - 1;
        speechIndex++)
        dialogueSequence[speechIndex].NextDialogue =
          dialogueSequence[speechIndex + 1];

      // Checks if there is a beginning and sends it if it exists
      return dialogueSequence != null ? dialogueSequence[0] : null;
    }
  }
}