using Heroes;
using UnityEngine;

namespace Runes
{
  public class HealthRune : MonoBehaviour
  {
    // The health the player heals up by
    [SerializeField] private int health = 20;

    // Adding health to the player when the health rune was picked up
    public void OnTriggerEnter2D(Collider2D other)
    {
      Player player = other.GetComponent<Player>();
      // Checking if the collider is the player
      if (player == null) return;

      // Increase the health of the player and delete the gameobject
      player.Heal(health);
      Destroy(gameObject);
    }
  }
}