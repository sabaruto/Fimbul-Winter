using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menus
{
    public class MainMenu : MonoBehaviour
    {
        // The load game button
        [SerializeField] private GameObject loadGame, newGame, deleteSave;

        // Deletes the save file of the current file and gives a new file
        public void NewGame()
        {
            // Deletes the current save file
            SaveManager.DeleteCurrentSaveFile();

            // Goes to the home for the beginning of the game
            SceneManager.LoadScene(LoadLevelManager.BeginningRef);
        }

        // Loads a new game
        public void LoadGame()
        {
            // Loads the current game file
            var data = SaveManager.LoadGame();

            // Goes to the first file location
            SceneManager.LoadScene(data.sceneRef);
        }

        public void ChooseSaveFile(int saveFileIndex)
        {
            // Checking if the chosen save file has been saved before
            var canContinue = SaveManager.FileExists(saveFileIndex);

            loadGame.SetActive(canContinue);
            deleteSave.SetActive(canContinue);
            newGame.SetActive(!canContinue);

            // Set the current file location to the empty file location
            SaveManager.ChooseSaveFile(saveFileIndex);
        }

        // Deletes the save file of the current file
        public void DeleteSaveFile()
        {
            SaveManager.DeleteCurrentSaveFile();
        }

        // Quits the game
        public void QuitGame()
        {
            Application.Quit();
        }
    }
}