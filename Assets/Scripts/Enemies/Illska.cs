using Base_Classes;
using Heroes;
using Misc;
using Statuses;
using UnityEngine;

namespace Enemies
{
  public class Illska : Enemy
  {
    //The radius for different abilities
    private readonly float chasingRadius = 3;
    private readonly float crushRadius = 1;
    private readonly float kickDistance = 2;
    private readonly float swingRadius = 1;

    [SerializeField] private AreaChecker kickShape;

    // Start is called before the first frame update
    private new void Awake()
    {
      base.Awake();

      //Kick, Illska kicks the player, knocking him over and causing damage
      Ability kick = new Ability("Kick", 10, 5, 0, 0.5f, Kick, 0.1f);

      //Swing, Illska swings her sword, causing bleed and damage
      Ability swing = new Ability("Swing", 15, 7, 0, 1f, Swing);

      //Crush, Illska tries to crush the player with her fist
      //causing root and damage
      Ability crush = new Ability("Crush", 10, 5, 0, 1f, Crush, 0.5f);

      abilityList = new[] {kick, swing, crush};

      //Set Illska's variables
      SetVariables("Illska", 150, 1, 1, true);
    } //Awake

    // Update is called once per frame
    private new void Update()
    {
      base.Update();

      if (!IsAlive()) return;

      // Getting the distance between the player and itself
      var distance = Vector2.Distance(transform.position,
        player.transform.position);

      if (distance < chasingRadius) currentTarget = player;

      // Checks if the player is within crush radius and if the ability is ready
      if (distance < crushRadius && !isCasting) GetAbility("Crush").StartAbility(this);

      //Check if the player is within swing distance
      if (distance < swingRadius && !isCasting) GetAbility("Swing").StartAbility(this);

      if (distance < kickDistance && !isCasting) GetAbility("Kick").StartAbility(this);
    } //Update

    private void Kick()
    {
      //Checks if the player is in range
      var playersInRanger = kickShape.GetCharactersInRange<Player>();

      if (playersInRanger.Length > 0)
      {
        //Hits the player
        currentTarget = playersInRanger[0];

        //Deals damage to the player
        currentTarget.TakeDamage(GetAbility("Kick").GetDamage());
      }

      GetAbility("Kick").Complete(this);
    } //Kick

    private void Swing()
    {
      //Checks if the plaeyr is in range
      var playersInRange = kickShape.GetCharactersInRange<Player>();

      if (playersInRange.Length > 0)
      {
        //Hits the player
        currentTarget = playersInRange[0];

        //Deals damage to the player
        currentTarget.TakeDamage(GetAbility("Swing").GetDamage());

        //Give the player the root status
        currentTarget.AddStatus(new Bleed(2));
      }

      //Start the hit ability cooldown
      GetAbility("Swing").SetCoolDown();

      isCasting = false;
    } //Swing

    private void Crush()
    {
      //Deal damage to the player
      player.TakeDamage(GetAbility("Crush").GetDamage());

      //Give the player the root status
      player.AddStatus(new Root(1));

      //Begin the wind down timer
      GetAbility("Crush").Complete(this);
    } //Crush
  }
} //Illska