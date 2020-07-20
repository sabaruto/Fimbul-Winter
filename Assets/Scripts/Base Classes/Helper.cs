using Dialogue;
using Managers;
using UnityEngine;

namespace Base_Classes
{
  /// <summary>
  ///   This is the base class for all the family in the house for the player to
  ///   interact with. It would give the player a information prompt and would
  ///   allow the player to improve their abilities
  /// </summary>
  public class Helper : MonoBehaviour, IInformation
  {
    // The dialogue to be said by and to the dialogue
    protected DialogueTrigger dialogue;

    // The helper position for the character
    [SerializeField] protected Transform helperTransform;

    // The continuing dialogue the helper will talk about when you've had the 
    // inital flavour text for the day
    [SerializeField] protected Dialogue.Dialogue reuseDialogue;

    // The dialogue that would be chosen depending on the day
    [SerializeField] protected Dialogue.Dialogue[] specificDialogue;

    // The sprite of the helper
    [SerializeField] private Sprite sprite;

    public string Name => name;
    public Sprite Sprite => sprite;

    public void Awake()
    {
      dialogue = GetComponent<DialogueTrigger>();

      // Finds the current day
      var day = GameMaster.Day;
      dialogue.SetStartingContents(specificDialogue[day].StartSpeech());
    }

    // Gets the position of where the character should stand
    public Vector2 GetStandPosition() { return helperTransform.position; }

    // Tells the helper the player is ready for conversation
    public void ReadyToTalk() { dialogue.StartConvo(); }
  }
}