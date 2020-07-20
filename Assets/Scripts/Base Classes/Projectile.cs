// This class defines all the non-basic attack projectiles in the game
// They will all have a hit area and a death timer to ensure they don't live 
// forever

using Heroes;
using Misc;
using UnityEngine;

namespace Base_Classes
{
  public class Projectile : MonoBehaviour
  {
    private AreaChecker areaChecker;

    // The damage dealt by the projectile
    [SerializeField] protected int damage;

    // The death timer
    [SerializeField] private float deathResetTime;

    // The current timing for the death
    private float deathTimer = Mathf.Infinity;

    // Changes whether the projectile is against players or enemies
    protected bool isAgainstPlayer = true;

    // The speed for the projectile
    [SerializeField] private float speed;

    // The array of statuses that the character in contact will suffer
    private Status[] statuses = new Status[0];

    private void Awake() { areaChecker = GetComponent<AreaChecker>(); }

    // On each update, reduces the death timer and deletes the gameObject if the
    // timer hits zero
    public void Update()
    {
      deathTimer -= Time.deltaTime;

      // Moves the projectile is the direction of its up
      var t = transform;
      t.position += t.up * (speed * Time.deltaTime);

      if (deathTimer <= 0) Destroy(gameObject);
    }

    // Checks if a character is in range and damages it if so
    public void FixedUpdate() { CheckCollision(); }

    // Setting the statuses
    public void SetStatuses(Status[] statuses) { this.statuses = statuses; }

    protected void CheckCollision()
    {
      var characters = areaChecker.GetCharactersInRange<Character>();

      // Looping throughout all the characters
      foreach (Character character in characters)
        // attacking the character if its against that type
        if (isAgainstPlayer && character is Player
            || !isAgainstPlayer && character is Enemy)
        {
          // Adds all the statuses to the character
          foreach (Status status in statuses) character.AddStatus(status);
          Attack(character);
        }
    }


    // Throws the projectile in a direction
    public void Throw(Vector2 throwPosition)
    {
      var t = transform;
      t.up = throwPosition - (Vector2) t.position;
      areaChecker.SetDirection(t.up);
      deathTimer = deathResetTime;
    }

    // The method that does damage and adds any statuses to the player
    protected virtual void Attack(Character otherCharacter)
    {
      otherCharacter.TakeDamage(damage);
      Destroy(gameObject);
    }

    // Changing whether the player is the target
    public void IsTargetPlayer(bool isAgainstPlayer) { this.isAgainstPlayer = isAgainstPlayer; }
  }
}