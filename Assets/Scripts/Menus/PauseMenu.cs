using Managers;
using Misc;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Menus
{
  public class PauseMenu : MonoBehaviour
  {
    private static bool isPaused;

    // The pause ID
    private readonly string pauseId = "PauseMenu";

    // The pause menu UI element
    [FormerlySerializedAs("pauseMenuUI")] [SerializeField]
    private GameObject pauseMenuUi;

    // Update is called once per frame
    private void Update()
    {
      if (Input.GetKeyDown(KeyCode.Escape)) ChangePause();
    }

    public void ChangePause()
    {
      isPaused = !isPaused;

      // Turn the pause menu on or off depending on if the
      // game is paused or not
      pauseMenuUi.SetActive(isPaused);

      // Change the time scale also accordingly
      if (isPaused)
        PauseHandler.Pause(pauseId);
      else
        PauseHandler.RemovePause(pauseId);
    }

    // Changes the time scale back to normal
    public void UnPause(string id) { PauseHandler.RemovePause(id); }

    // Moving the scene to the main menu scene
    public void Menu()
    {
      SceneManager.LoadScene(LoadLevelManager.MainMenuRef);
      PauseHandler.RemovePause(pauseId);
    }

    public void QuitGame() { Application.Quit(); }

    public static bool IsPaused() { return isPaused; }
  }
}