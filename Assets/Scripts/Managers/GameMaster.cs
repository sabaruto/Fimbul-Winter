using System.Collections.Generic;
using System.IO;
using Base_Classes;
using Heroes;
using Misc;
using UnityEngine;

namespace Managers
{
    public class GameMaster : MonoBehaviour
    {
        // The game object for health runes
        public GameObject healthRune;

        // The size for the current map
        [SerializeField] private int length = 1000, height = 1000;
        // The current day of the game

        // Adding all the characters in the screen
        private readonly List<GameObject> characters = new List<GameObject>();

        // All the clickable gameObjects in the screen
        private readonly List<GameObject> clickableObjects = new List<GameObject>();

        // The resolution of the grid
        private readonly float gridResolution = 0.25f;

        // The obstacles in the game
        private GameObject[] obstacles;

        public static int Day { get; private set; }

        private void Start()
        {
            var charScript = FindObjectsOfType<Character>();
            var helpScript = FindObjectsOfType<Helper>();

            foreach (var scr in charScript) characters.Add(scr.gameObject);

            clickableObjects.AddRange(characters);

            Data data = null;

            // Load the current information of the file for the player if the file
            // exists
            if (File.Exists(SaveManager.GetInfoPath()))
            {
                data = SaveManager.LoadGame();

                // Assign the correct day to the scene
                Day = data.day;
            }

            // Adding all the helper objects
            foreach (var scr in helpScript) clickableObjects.Add(scr.gameObject);

            // Finds all the objects in the scene
            obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

            // Adding a grid object to all the character classes with movement
            foreach (var obj in characters)
            {
                // Finding the movement component and setting a grid object to it
                var objMovement = obj.GetComponent<Movement>();
                objMovement.SetGrid(new MyGrid(length, height, gridResolution));

                // Checks if the given object is a player and if so, gives refrence
                // to that object
                if (obj.GetComponent<Character>() is Player player)
                    // Adds the information for the player if data was loaded
                    if (data != null)
                    {
                        player.SetHealth(data.health);
                        player.SetStamina(data.stamina);
                        player.SetSkillPoints(data.skillPoints);
                        player.SetSkillStatus(data.skillStatus);
                    }
            }

            // Setting all the different obstacles
            foreach (var obst in obstacles) SetObstacle(obst);

            // [TEMPORARY]
            FindObjectOfType<AudioManager>().ChangeTheme("Forest");
        }

        private void Update()
        {
            foreach (var character in characters)
            {
                var currentCharacter = character.GetComponent<Character>();
                if (currentCharacter.HasAbility())
                    foreach (var ability in currentCharacter.GetAbilityList())
                        ability.DecreaseCoolDown(Time.deltaTime);
            } //foreach
        }

        // Checks wether a person is in a given position
        public T CheckPerson<T>(Vector2 position) where T : MonoBehaviour
        {
            // The current closest enemy and distance
            T closestPerson = null;
            var distance = Mathf.Infinity;

            // Loops through all the gameObjects
            foreach (var character in clickableObjects)
            {
                var currentPerson = character.GetComponent<T>();
                // Checking if the character is a enemy
                if (currentPerson != null)
                {
                    // Seeing if the position of the enemy is close enough to
                    // the position
                    var currentDistance = Vector2.Distance(character.transform.position,
                        position);

                    if (currentDistance < 2f && currentDistance < distance)
                    {
                        distance = currentDistance;
                        closestPerson = currentPerson;
                    }
                }
            }

            return closestPerson;
        }

        // Checks if all the enemies in a given stage are dead
        public bool EnemiesDead()
        {
            // Looping through all the different characters in the scene
            foreach (var obj in characters)
                // checking if the character is an enemy
                if (obj.GetComponent<Character>() is Enemy enemy)
                    // Checks if the enemy is alive
                    if (enemy.IsAlive())
                        return false;

            return true;
        }


        // Setting a position on the grid to be an obstacle point
        public void SetObstaclePoint(int x, int y)
        {
            // Looping through all the characters
            foreach (var character in characters) character.GetComponent<Movement>().GetGrid().SetObstacle(x, y);
        }

        // Setting a vector position on the gid to be an obstacle point
        public void SetObstaclePoint(Vector2 position)
        {
            // Looping through all the characters
            foreach (var character in characters) character.GetComponent<Movement>().GetGrid().SetObstacle(position);
        }

        // Setting an area to be an obstacle
        private void SetObstacle(GameObject obstacle)
        {
            // Getting the collider of the barrier
            var currentCollider = obstacle.GetComponent<Collider2D>();

            // Find the bounds of the collider
            var currentBounds = currentCollider.bounds;

            // Getting the centre of the bounds
            Vector2 centre = currentBounds.center;

            // Getting the extends for the collider
            var xExtends = currentBounds.extents.x + 2 * gridResolution;
            var yExtends = currentBounds.extents.y + 2 * gridResolution;

            // Looping through all the points that could be in the bounds
            for (var xIndex = centre.x - xExtends;
                xIndex <= centre.x + xExtends;
                xIndex += gridResolution)
            for (var yIndex = centre.y - yExtends;
                yIndex <= centre.y + yExtends;
                yIndex += gridResolution)
            {
                // The obstacle point
                var obstaclePoint = new Vector2(xIndex, yIndex);

                // Checks if a point is on the grid
                if (currentCollider.OverlapPoint(obstaclePoint)) SetObstaclePoint(obstaclePoint);
            }
        }

        // This changes the scene from one place to another
        public void MoveScene(string sceneRef)
        {
            // Saves the information in the relevant file
            SaveManager.SaveGame(FindObjectOfType<Player>(), sceneRef, Day);

            // Changes the scene to the relevant file
            LoadLevelManager.LoadLevel(sceneRef);
        }
    }
} //GameMaster Class