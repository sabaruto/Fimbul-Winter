// The wolf enemy, a stalker which switches to a hunter until killed or its
// stamina is lost

using Base_Classes;
using Statuses;
using UnityEngine;

namespace Enemies
{
  public class Wolf : Enemy
  {
    // The bit distnace
    private readonly float biteDistance = 2;

    // The radius of the wolf howl
    private readonly float howlRadius = 6;

    // The stalking radius
    private readonly float stalkingRadius = 20;

    // The chasing period
    protected float chasingPeriod = 5;

    // The chasing radius
    protected float chasingRadius = 5;

    // The stalking time period
    protected float stalkPeriod = 10;
    private static readonly int IsHunting = Animator.StringToHash("isHunting");

    private new void Awake()
    {
      base.Awake();

      // Stalking, allowing the wolf to be semi translucent but at a cost of
      // movement speed. If broken when in range of the player, it gains
      // a burst of movement speed and attack speed
      Ability stalking = new Ability("Stalking", 0, 25, 0);

      // Rip, bites into the player causing a bleed effect, making the player
      // constantly losing health for a few seconds
      Ability rip = new Ability("Rip", 10, 10, 0, 0, Rip);

      // Howl, the wolf calls for nearby wolves for help. All wolves within a 10
      // meter radius have their target become the player. This ability breaks
      // stalking
      Ability howl = new Ability("Howl", 0, 30, 0, 0.3f, Howl);


      abilityList = new[] {stalking, rip, howl};

      // Setting the variables for the enemy
      SetVariables("Wolf", 25, 10, 1, true);
    }

    private void Start() { statuses.Add(new Stalk(stalkPeriod)); }

    // The update method to call the howl method if the player is spotted by the
    // wolf
    private new void Update()
    {
      base.Update();

      if (!IsAlive()) return;

      if (currentTarget != null) GetAbility("Howl").StartAbility(this);

      // Getting the distance between the player and itself
      var distance = Vector2.Distance(transform.position,
        player.transform.position);

      // Checks if the player is in bite radius
      if (distance < biteDistance) GetAbility("Rip").StartAbility(this);

      // Adding the running animation if the wolf is hunting
      animator.SetBool(IsHunting, GetStatus<Hunt>() != null);

      // Checks if the player is in chasing radius
      if (distance < chasingRadius)
        Chase();

      // if not, checks if the player is in stalking distance
      else if (distance < stalkingRadius) Stalk();
    }

    protected void Howl()
    {
      // looks through all the game objects in a certain radius
      var wolves = FindObjectsOfType<Wolf>();

      // Checks if the colliders are connected to a wolf object
      foreach (Wolf wolf in wolves)
      {
        // Ignore all the wolves that are outside the howl radius
        if (Vector2.Distance(transform.position, wolf.transform.position)
            > howlRadius)
          continue;

        // Checking if the collider is refrenced to a game component and if the
        // wolf hasn't found a creature
        if (wolf != null && wolf.currentTarget == null) wolf.currentTarget = currentTarget;
      }

      isCasting = false;
      GetAbility("Howl").SetCoolDown();
    }

    // This method starts the stalking
    protected void Stalk()
    {
      // Find the stalk status and if there isn't any, create a new stalk status
      Stalk currentStalk = GetStatus<Stalk>();

      // Gets the stalk ability
      if (currentStalk == null && GetAbility("Stalking").IsAbilityReady(this))
      {
        AddStatus(new Stalk(stalkPeriod));
        GetAbility("Stalking").SetCoolDown();
      }

      // Stops the stalk if the wolf is wounded
      if (maxHealth != health && GetStatus<Stalk>() != null)
      {
        GetStatus<Stalk>().Clear(this);
        GetAbility("Stalking").SetCoolDown();
      }
    }

    // This method starts the chasing
    protected void Chase()
    {
      // ends the stalking status if it was running
      Stalk currentStalk = GetStatus<Stalk>();

      if (currentStalk != null)
      {
        currentStalk.Clear(this);
        AddStatus(new Hunt(chasingPeriod));
      }
    }

    // This method uses the rip ability
    private void Rip()
    {
      // Damages the player
      player.TakeDamage(GetAbility("Rip").GetDamage());

      // Gives the player the bleed effect
      player.AddStatus(new Bleed(5));

      // Starts the rip ability cooldown
      GetAbility("Rip").SetCoolDown();
      isCasting = false;
    }
  }
}