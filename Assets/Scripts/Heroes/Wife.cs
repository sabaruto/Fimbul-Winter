using Base_Classes;
using Dialogue;
using Managers;
using Misc;
using UnityEngine;

namespace Heroes
{
  /// <summary>
  ///   This class holds all the wifes' conversations and her access to the
  ///   skill tree for the player
  /// </summary>
  public class Wife : Helper
  {
    // The trigger to check if the wife has spoken previously
    private bool hasSpoken;

    // The upgrade menu
    [SerializeField] private GameObject upgradeMenu;

    public new void Awake()
    {
      base.Awake();
      dialogue.Speak += StartConvo;
    }

    public void Update()
    {
      // Checks whether the conversation
      if (!DialogueManager.IsSpeaking() && hasSpoken)
      {
        // Opens the upgrade menu
        upgradeMenu.SetActive(true);

        // Pauses the game
        PauseHandler.Pause("skillTree");

        // Change the beginning of the speech
        dialogue.SetStartingContents(reuseDialogue.StartSpeech());

        hasSpoken = false;
      }
    }

    private void StartConvo(SpeechSlide slide) { hasSpoken = true; }
  }
}