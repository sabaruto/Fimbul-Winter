using System.Collections;
using Dialogue;
using Heroes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
  public class DialogueManager : MonoBehaviour
  {
    private static bool isSpeaking;

    // The gameobject holding the choice objects
    // [SerializeField] private GameObject choiceButtons;

    // The sentences that would be shown to the diagloue box
    private SpeechSlide currentContents;

    [SerializeField] private string currentSentence;

    // The prompts for the button names
    // [SerializeField] private TextMeshProUGUI leftButton, rightButton;

    // The sprite for the talking of the characters
    [SerializeField] private Image speakingFace;

    // The name of the person talking and the words their saying
    [SerializeField] private TextMeshProUGUI speakingName, paragraph;

    // The animation for the dialogue box
    [SerializeField] private Animator textBoxAnimation;

    // The current routine
    private Task typingTask;
    private static readonly int IsTalking = Animator.StringToHash("isTalking");

    public void Start()
    {
      // Find all the dialogue triggers and subscribe the speak function to it
      var triggers = FindObjectsOfType<DialogueTrigger>();

      foreach (DialogueTrigger trigger in triggers) trigger.Speak += StartConversation;
    }

    public void Update()
    {
      // Allows progression of the dialogue through pressing the enter key
      if (Input.GetKeyDown(KeyCode.Return)) PushConversation();
    }

    private void StartConversation(SpeechSlide startingContents)
    {
      // Clear the previous conversation
      ClearBox();

      // Assign the current dialogue contents to the manager
      currentContents = startingContents;
      isSpeaking = true;

      textBoxAnimation.SetBool(IsTalking, true);
      PushConversation();
    }

    public void PushConversation()
    {
      // Checks if the current task is finished
      if (typingTask != null && typingTask.Running)
      {
        paragraph.text = currentSentence;
        typingTask.Stop();
        return;
      }

      if (currentContents == null)
      {
        // Finish the conversation when the last sentence is met
        StopAllCoroutines();
        EndConversation();
        return;
      }

      // Push the next sentence
      currentSentence = currentContents.Sentence;
      speakingName.text = currentContents.Name;
      speakingFace.sprite = currentContents.Sprite;
      currentContents = currentContents.NextDialogue;

      StopAllCoroutines();
      typingTask = new Task(TypeText(currentSentence));
    }

    private IEnumerator TypeText(string sentence)
    {
      paragraph.text = "";
      foreach (var letter in sentence.ToCharArray())
      {
        paragraph.text += letter;
        yield return new WaitForSeconds(0.05f);
      }
    }

    private void ClearBox()
    {
      currentContents = null;
      paragraph.text = "";
      speakingName.text = "";
    }

    private void EndConversation()
    {
      ClearBox();
      textBoxAnimation.SetBool(IsTalking, false);
      isSpeaking = false;

      // Allowing the player to move once again
      Player player = FindObjectOfType<Player>();
      player.FinishConversation();
    }

    // Checking if the dialogue manager is speaking
    public static bool IsSpeaking() { return isSpeaking; }
  }
}