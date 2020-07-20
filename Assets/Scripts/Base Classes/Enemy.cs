using Heroes;
using Managers;
using UnityEngine;

namespace Base_Classes
{
  public class Enemy : Character
  {
    private static readonly int IsMoving = Animator.StringToHash("isMoving");

    // The range for the enemies
    [SerializeField] private float fightingRange = 8;

    // Checking if the enemy has been hit
    private bool isHit;

    // The player character
    protected Player player;

    // The drop rate for the rune
    [SerializeField] protected float runeDropRate = 0.2f;

    protected new void Awake()
    {
      base.Awake();
      // Finding the player character
      player = FindObjectOfType<Player>();
      // Setting the variables
      SetVariables("enemy", 100, 10, 2, false);
    }

    // Update is called once per frame
    protected new void Update()
    {
      base.Update();

      // Check if alive
      if (!IsAlive()) return;

      // Checks whether the enemy has seen the player
      if (currentTarget == null) LocateTarget();

      // If the enemy has an animator, change it to showing the walking animation
      if (animator != null) animator.SetBool(IsMoving, movement.IsMoving());
    }

    // Making the enemies likely to drop a health token
    protected override void Die()
    {
      base.Die();
      if (Random.value < runeDropRate)
        Instantiate(FindObjectOfType<GameMaster>().healthRune,
          transform.position, Quaternion.identity);
      gameObject.SetActive(false);
    }

    // Tries to see if the target is in view
    protected void LocateTarget()
    {
      // Finding the distance between the enemy and the player
      var currentDistance = Vector2.Distance(transform.position,
        player.transform.position);

      // Seeing if the player is in fighting range
      if (currentDistance < fightingRange)
        // Checking if the player is in sight
        if (movement.LineOfSight(transform.position, player.transform.position))
          ExecuteAttack();
    }

    // Executes its attack pattern when the player is found
    protected void ExecuteAttack()
    {
      // Start attacking the player
      currentTarget = player;
    }

    // Making enemies attack the player when they've been attacked
    public override void TakeDamage(int damage)
    {
      var currentHealth = health;

      base.TakeDamage(damage);

      if (currentHealth != health && !isHit)
      {
        isHit = true;
        currentTarget = player;
      }
    }
  }
}