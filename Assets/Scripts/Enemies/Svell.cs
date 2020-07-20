using Base_Classes;
using Heroes;
using Misc;
using Statuses;
using UnityEngine;

namespace Enemies
{
  public class Svell : Enemy
  {
    //The chasing radius
    private readonly float chasingRadius = 3;

    //The freeze radius
    private readonly float freezeRadius = 2;

    private readonly float iceRadius = 1;

    // The spike radius
    private readonly float spikeRadius = 2;

    // The area detector for the enemy
    [SerializeField] private AreaChecker spikeDetector;

    private new void Awake()
    {
      base.Awake();

      //Freeze, Svell puts his hands on the ground, causing the area around 
      //him to freeze and rooting the player as well as freezing him
      Ability freeze = new Ability("Freeze", 20, 10, 0, 2f, Freeze, 0.4f);

      //Ice, Svell hits the player with his ice sword, causing him to bleed
      //and be afraid
      Ability ice = new Ability("Ice", 15, 5, 0, 1f, Ice, 0.2f);

      //Spikes, Svell throws his sword on the ground and creates spikes in front of him
      //If the spikes hit the player, he will be damaged and bleed
      Ability spikes = new Ability("Spikes", 15, 10, 0, 1f, Spikes, 0.4f);

      abilityList = new[] {freeze, ice, spikes};

      //Set Svell'svariables
      SetVariables("Svell", 200, 100, 1, true);
    } //Awake

    private new void Update()
    {
      base.Update();

      //Check if the thief is still alive
      if (!IsAlive()) return;

      // Getting the distance between the player and itself
      var distance = Vector2.Distance(transform.position,
        player.transform.position);

      if (distance < chasingRadius) currentTarget = player;

      if (distance < freezeRadius && !isCasting)
        GetAbility("Freeze").StartAbility(this);

      else if (distance < iceRadius && !isCasting) GetAbility("Ice").StartAbility(this);

      if (distance < spikeRadius)
        if (GetAbility("Spikes").StartAbility(this))
        {
          // If the ability is started, point the spikes at the player
          Vector2 dir = player.transform.position - transform.position;
          spikeDetector.SetDirection(dir);
        }
    } //Update

    //This method uses the freeze ability
    private void Freeze()
    {
      var distance = Vector2.Distance(transform.position,
        player.transform.position);

      // Checks if the player is in range
      if (distance < freezeRadius)
      {
        //Deal damage to the player
        player.TakeDamage(GetAbility("Freeze").GetDamage());

        //Give the player the root effect
        player.AddStatus(new Root(1));

        //Give the player the freeze effect
        player.AddStatus(new Freeze(2));
      }

      //Begin the wind down timer
      GetAbility("Freeze").Complete(this);
    } //Freeze

    private void Ice()
    {
      var distance = Vector2.Distance(transform.position,
        player.transform.position);

      if (distance < iceRadius)
      {
        //Deal damage to the player
        player.TakeDamage(GetAbility("Ice").GetDamage());

        //Give the player the bleed effect
        player.AddStatus(new Bleed(5));

        //Give the player the fear effect
        player.AddStatus(new Fear(3));
      }

      //Begin the wind down timer
      GetAbility("Ice").Complete(this);
    } //Ice

    private void Spikes()
    {
      // Checking if the player is in range
      var players = spikeDetector.GetCharactersInRange<Player>();

      if (players.Length > 0)
      {
        players[0].TakeDamage(GetAbility("Spikes").GetDamage());
        players[0].AddStatus(new Bleed(2));
      }

      //Begin the wind down timer
      GetAbility("Spikes").Complete(this);
    } //Spikes
  }
} //Svell