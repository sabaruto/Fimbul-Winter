using Heroes;
using Managers;
using UnityEngine;

namespace Triggers
{
  /// <summary>
  ///     This class changes the level once the player steps into this area
  /// </summary>
  public class LevelTrigger : MonoBehaviour
    {
        [SerializeField] protected string levelRef;

        public void OnTriggerEnter2D(Collider2D other)
        {
            // Checks if the collider has a player component
            var player = other.GetComponent<Player>();
            if (player != null) TryMoveScene(player);
        }

        // Executes the trigger
        protected virtual void TryMoveScene(Player player)
        {
            // Moves the scene to that index
            FindObjectOfType<GameMaster>().MoveScene(levelRef);
        }
    }
}