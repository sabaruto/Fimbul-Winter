using UnityEngine;

namespace Dialogue
{
  /// <summary>
  ///     This class holds and triggers the dialogue system
  /// </summary>
  public class DialogueTrigger : MonoBehaviour
    {
        public delegate void Dialogue(SpeechSlide contents);

        protected SpeechSlide startingContents;
        public event Dialogue Speak;

        public void StartConvo()
        {
            Speak(startingContents);
        }

        public void SetStartingContents(SpeechSlide contents)
        {
            startingContents = contents;
        }
    }
}