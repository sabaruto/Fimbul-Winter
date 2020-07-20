using System.Collections;
using Misc;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
  public class LoadLevelManager : MonoBehaviour
  {
    // All the scene variable names
    public static readonly string MainMenuRef = "MainMenu", HomeRef = "Home",
      BeginningRef = "CutScene";

    // The pause ID for the level loader
    private static readonly string PauseId = "LevelLoader";

    // The static slider loader
    private static GameObject staticSlider;

    // The static animator for the script
    private static Animator animator;

    // The static slider
    private static Slider staticLoadingSlider;

    // The static text percentage
    private static TextMeshProUGUI staticPercentage;

    // The loading task
    private static Task loadLevel;

    // The percentage for the level loader
    [SerializeField] private TextMeshProUGUI loadingPercentage;

    // The slider for the level value
    [SerializeField] private Slider loadingSlider;

    // The screen for the level loader
    [SerializeField] private GameObject sliderObject;
    private static readonly int FadeOut = Animator.StringToHash("FadeOut");

    // The public variable for other classes to access the progress

    // Clearing the pause at the start
    public void Start()
    {
      staticSlider = sliderObject;
      staticPercentage = loadingPercentage;
      staticLoadingSlider = loadingSlider;

      animator = GetComponent<Animator>();
      PauseHandler.RemovePause(PauseId);
      loadLevel?.Stop();
      sliderObject.SetActive(false);
    }

    public static void LoadLevel(string sceneRef)
    {
      // Starting the fade out animation
      animator.SetTrigger(FadeOut);

      // Create a new task starting the loading of the larger scene
      loadLevel = new Task(LoadAsynchronously(sceneRef), false);

      // Pause the game
      PauseHandler.Pause(PauseId);
    }

    public void StartLevelLoad() { loadLevel.Start(); }

    private static IEnumerator LoadAsynchronously(string sceneRef)
    {
      AsyncOperation operation = SceneManager.LoadSceneAsync(sceneRef);

      staticSlider.SetActive(true);

      while (!operation.isDone)
      {
        var currentProgress = operation.progress;

        Debug.Log(currentProgress);
        staticLoadingSlider.value = currentProgress / 0.9f;
        staticPercentage.text = currentProgress / 0.9f * 100 + "%";
        yield return null;
      }
    }
  }
}