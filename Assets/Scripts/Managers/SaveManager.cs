using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Base_Classes;
using Heroes;
using UnityEngine;

// This class saves and loads the relevant infomation into the game state
// between stages and playthroughs
namespace Managers
{
    public static class SaveManager
    {
        // The path where the information is stored
        private static string infoPath =
            Application.persistentDataPath + "/Save0.b";

        // Saves the game information into a safe part of the system's data
        public static void SaveGame(Player player, string sceneRef, int day)
        {
            // Creating the binary formatter which serialises the data
            var formatter = new BinaryFormatter();
            var stream = new FileStream(infoPath, FileMode.Create);

            // Get the data and seralise it
            var data = new Data(player, sceneRef, day);

            formatter.Serialize(stream, data);
            stream.Close();
        }

        // Loads the game information from the save file if it exists and causes an
        // error if the file isn't there
        public static Data LoadGame()
        {
            if (File.Exists(infoPath))
            {
                var formatter = new BinaryFormatter();
                var stream = new FileStream(infoPath, FileMode.Open);

                var data = formatter.Deserialize(stream) as Data;

                stream.Close();
                return data;
            }

            Debug.LogError("Save file not found in " + infoPath);
            return null;
        }

        public static string GetInfoPath()
        {
            return infoPath;
        }


        public static void ChooseSaveFile(int saveFileIndex)
        {
            // Checks if the file index is too high or too low
            if (saveFileIndex < 0 || saveFileIndex > 3)
                Debug.LogError("Not a save location");
            else
                // Changes the info file location accordingly
                infoPath = SaveFilePath(saveFileIndex);
        }

        // Deletes the file associated to this index
        public static void DeleteSaveFile(int saveFileIndex)
        {
            // Checks if the file index is too high or too low
            if (saveFileIndex <= 0 || saveFileIndex > 3) Debug.LogError("Not a save location");

            // Checks if the file exists
            if (File.Exists(SaveFilePath(saveFileIndex)))
                // If so, deletes the file
                File.Delete(SaveFilePath(saveFileIndex));
        }

        // Checks if a file exists
        public static bool FileExists(int saveFileIndex)
        {
            return File.Exists(SaveFilePath(saveFileIndex));
        }

        public static void DeleteCurrentSaveFile()
        {
            if (File.Exists(infoPath)) File.Delete(infoPath);
        }

        // Gives the file path from the index
        public static string SaveFilePath(int saveFileIndex)
        {
            return Application.persistentDataPath + "/Save" + saveFileIndex + ".b";
        }
    }
}