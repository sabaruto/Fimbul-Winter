using Base_Classes;
using Heroes;
using Misc;
using Statuses;
using UnityEngine;

namespace Enemies
{
  public class Viddarot : Wolf
  {
    //The claw distance
    private readonly float clawDistance = 2;

    //The root distance
    private readonly float rootDistance = 4;

    [SerializeField] private AreaChecker clawShape;

    // [SerializeField] private AreaChecker rootsShape;

    private new void Awake()
    {
      base.Awake();

      //Root, Viddarot makes roots rise from the ground, rooting the enemy
      Ability roots = new Ability("Roots", 10, 15, 0, 0.3f, Roots, 0.4f);

      //Claw, Viddarot rushes at the player and tries to claw him
      Ability claw = new Ability("Claw", 15, 10, 0, 0.3f, Claw);

      //Ram, Viddarot rushes at the player and tries to ram him with his antlers
      Ability ram = new Ability("Ram", 20, 10, 0, 0.3f, Ram, 0.4f);

      abilityList = new[] {claw, ram, roots};

      //Setting the variables for viddarot
      SetVariables("Viddarot", 150, 1, 1, true);
    } //Awake

    private void Start() { statuses.Add(new Stalk(stalkPeriod)); } //Start

    // Update is called once per frame
    private new void Update()
    {
      base.Update();

      if (!IsAlive()) return;

      // Getting the distance between the player and itself
      var distance = Vector2.Distance(transform.position,
        player.transform.position);

      if (distance < chasingRadius)
      {
        currentTarget = player;
        Chase();
      } //if

      if (distance < clawDistance && !isCasting) GetAbility("Claw").StartAbility(this);

      if (distance < rootDistance && !isCasting) GetAbility("Roots").StartAbility(this);
    } //Update

    private void Claw()
    {
      Debug.Log("Im clawing");
      //Checks if the player is in range
      var playersInRange = clawShape.GetCharactersInRange<Player>();

      if (playersInRange.Length > 0)
      {
        //Hits the player
        currentTarget = playersInRange[0];

        //Deals damage to the player
        currentTarget.TakeDamage(GetAbility("Claw").GetDamage());

        //Adds the bleed effect to the player
        currentTarget.AddStatus(new Bleed(3));
      }

      //Set the kick ability cooldown
      GetAbility("Claw").SetCoolDown();

      isCasting = false;
    } //Claw

    private void Ram() { } //Ram

    private void Roots()
    {
      //Checks if the player is in range
      var playersInRange = clawShape.GetCharactersInRange<Player>();

      if (playersInRange.Length > 0)
      {
        //Hits the player
        currentTarget = playersInRange[0];

        //Deals damage to the player
        currentTarget.TakeDamage(GetAbility("Root").GetDamage());

        //Adds the bleed effect to the player
        currentTarget.AddStatus(new Root(3));
      }

      GetAbility("Claw").Complete(this);
    } //Root
  }
} //Viddarot