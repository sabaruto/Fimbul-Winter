using Base_Classes;
using Statuses;
using UnityEngine;

namespace Enemies
{
  public class Troll : Enemy
  {
    //The chasing radius of the troll
    private readonly float chasingRadius = 3;

    // The damage dealt by rattle per second
    private readonly int rattleDamage = 2;

    //The rattle radius
    private readonly float rattleRadius = 2;
    private readonly float rattleResetTime = 3;

    //The punch smash of the troll
    private readonly float smashRadius = 2;

    // Checking whether the troll is rattling
    private bool isRattling;

    // The area checker for the punch
    // [SerializeField] private AreaChecker punchDetector;

    // The rattle timer
    private float rattleTimer;

    [SerializeField] private GameObject rockProjectile;
    private static readonly int IsThrowing = Animator.StringToHash("isThrowing");
    private static readonly int IsSmashing = Animator.StringToHash("isSmashing");
    private static readonly int IsRumble = Animator.StringToHash("isRumble");

    private new void Awake()
    {
      base.Awake();

      //Smash, the troll throws his arms on the ground, causing the earth to tremble and
      //dealing damage to the player and knocking him over
      //(add root)
      Ability smash = new Ability("Smash", 20, 5, 0, 1f, Smash, 0.583f);

      //Rattle, troll hits the ground, causing it to shake and slowing the player if he is
      //in the area of damage
      Ability rattle = new Ability("Rattle", 0, 5, 0, 0.3f, StartRattle);

      //This ability hurls a boulder at the player
      Ability rockThrow = new Ability("Rock Throw", 2, 5, 0, 0.667f, RockThrow, 2f);

      abilityList = new[] {smash, rattle, rockThrow};

      //Set the variables of the troll
      SetVariables("Troll", 200, 100, 2, true);

      // Setting the rattle timer to the max timer
      rattleTimer = rattleResetTime;
    } //Awake

    // Update is called once per frame
    private new void Update()
    {
      base.Update();

      //Check if the troll is still alive
      if (!IsAlive()) return;

      // Getting the distance between the player and itself
      var distance = Vector2.Distance(transform.position,
        player.transform.position);

      //Check if the player is within chasing radius. If s0, start chasing them
      if (distance < chasingRadius) currentTarget = player;

      
      
      if (distance < smashRadius && GetAbility("Smash").StartAbility(this)) animator.SetBool(IsSmashing, true);

      if (distance < rattleRadius) GetAbility("Rattle").StartAbility(this);

      // Starts the rock throw animation if the troll starts throwing the rock
      if (GetAbility("Rock Throw").StartAbility(this)) animator.SetBool(IsThrowing, true);

      // Checks if the troll is rattling and rattle if so
      if (isRattling) Rattle();

      // Updates the animation for the troll
      animator.SetBool(IsRumble, isRattling);
    } //Update

    //This method uses the smash ability
    private void Smash()
    {
      // Check the distance between the enemy and player
      var distance = Vector2.Distance(transform.position,
        player.transform.position);

      if (distance <= smashRadius)
      {
        //Deal damage to the player
        player.TakeDamage(GetAbility("Smash").GetDamage());

        //Give the player the root effect
        player.AddStatus(new Root(1));

        //If the player is no longer rooted, slow them
        player.AddStatus(new Fear(3));
      }

      //Begin the wind down timer
      GetAbility("Smash").Complete(this);
    } //Smash


    // This starts the rattle ability
    private void StartRattle()
    {
      isRattling = true;
      GetAbility("Rattle").SetCoolDown();
      Debug.Log("is Rattling");
    }

    //This method uses the rattle ability
    private void Rattle()
    {
      // Decreases the timing of the rattle and damage the player if he is
      // in the zone at any second
      if ((int) (2 * (rattleTimer - Time.deltaTime)) != (int) (2 * rattleTimer) &&
          Vector2.Distance(player.transform.position, transform.position) <
          rattleRadius)
        player.TakeDamage(rattleDamage);

      rattleTimer -= Time.deltaTime;

      //Give the player the fear effect
      player.AddStatus(new Fear(3));

      // Puts the ability on cooldown if the rattle timer is finished
      if (rattleTimer <= 0)
      {
        rattleTimer = rattleResetTime;
        isCasting = false;
        isRattling = false;
      }
    } //Rattle

    private void RockThrow()
    {
      // Instantiates and throws the rock object
      GameObject rock = Instantiate(rockProjectile, transform.position,
        Quaternion.identity);

      rock.GetComponent<Projectile>().Throw(movement.GetDir());
      GetAbility("Rock Throw").Complete(this);
    }

    // Stops the animation of the rock throw
    public void StopThrow() { animator.SetBool(IsThrowing, false); }

    // Stops the animation of the smash
    public void StopSmash() { animator.SetBool(IsSmashing, false); }

    public void OnDrawGizmos()
    {
      // Checking if the rattle ability is running
      if (isRattling)
      {
        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(transform.position, rattleRadius);
      }
    }
  }
} //Troll