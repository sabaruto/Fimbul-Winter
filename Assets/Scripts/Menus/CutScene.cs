using System.Collections;
using Managers;
using TMPro;
using UnityEngine;

namespace Menus
{
  public class CutScene : MonoBehaviour
  {
    // The text being shown to the screen
    [SerializeField] private TextMeshProUGUI currentSentence;

    // The list of sentences to be written to the board
    [SerializeField] [TextArea(1, 3)] private string[] sentences;

    // Starts the play of the cutscene
    private void Start() { StartCoroutine(RunTime()); }

    // Goes through all the sentences and waits a certain time to end it
    private IEnumerator RunTime()
    {
      // Clear the alpha for the sentence
      currentSentence.faceColor -= new Color(0, 0, 0, 1);

      // Looping through all the different sentences
      foreach (var sentence in sentences)
      {
        currentSentence.text = sentence;

        // Looping while the aplha of the sentence is not close
        // to one
        while (255 - currentSentence.faceColor.a > float.Epsilon)
        {
          yield return new WaitForSeconds(0.1f);
          currentSentence.faceColor += new Color(0, 0, 0, 0.1f);
        }

        yield return new WaitForSeconds(3f);

        while (currentSentence.faceColor.a > float.Epsilon)
        {
          yield return new WaitForSeconds(0.1f);
          currentSentence.faceColor -= new Color(0, 0, 0, 0.1f);
        }
      }

      // Moves the cutscene to the start
      SkipToHome();
    }

    // Skips the current cutscene and goes to a certain scene
    public void SkipToHome()
    {
      StopAllCoroutines();
      LoadLevelManager.LoadLevel(LoadLevelManager.HomeRef);
    }
  }
}