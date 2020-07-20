using Base_Classes;
using Heroes;
using Misc;
using Statuses;
using UnityEngine;

namespace Enemies
{
  public class Thief : Enemy
  {
    private readonly float backstabDistance = 2;

    // The chasing period
    private readonly float chasingPeriod = 5;

    // The chasing radius
    private readonly float chasingRadius = 3;

    // private float stabDistance = 1;

    [SerializeField] private AreaChecker stabShape;
    private static readonly int IsStabbing = Animator.StringToHash("isStabbing");

    private new void Awake()
    {
      base.Awake();

      //Backstab, deal damage to the player and apply a bleed effect
      Ability backstab = new Ability("Backstab", 10, 15, 20, 0.25f, Backstab);

      abilityList = new[] {backstab};

      //Set the variables of the thief
      SetVariables("Thief", 20, 40, 1, true);
    } //Awake

    // Update is called once per frame
    private new void Update()
    {
      base.Update();

      //Check if the thief is still alive
      if (!IsAlive()) return;

      // Getting the distance between the player and itself
      var distance = Vector2.Distance(transform.position,
        player.transform.position);

      if (distance < chasingRadius)
      {
        currentTarget = player;
        Chase();
      } //if

      // Checks if the player is within backstab radius and if the ability is ready
      if (distance < backstabDistance && !isCasting)
        if (GetAbility("Backstab").StartAbility(this))
        {
          animator.SetBool(IsStabbing, true);
          Debug.Log("Stabbing");
        }
    } //Update

    //This method uses the backstab ability. 
    private void Backstab()
    {
      //Checks if the plaeyr is in range
      var playersInRange = stabShape.GetCharactersInRange<Player>();

      if (playersInRange.Length > 0)
      {
        //Hits the player
        currentTarget = playersInRange[0];

        //Deals damage to the player
        currentTarget.TakeDamage(GetAbility("Backstab").GetDamage());

        //Give the player the root status
        currentTarget.AddStatus(new Bleed(5));
      }

      //Start the hit ability cooldown
      GetAbility("Backstab").Complete(this);
      animator.SetBool(IsStabbing, false);
      Debug.Log("Done stabbing");
    } //Backstab

    // This method starts the chasing
    private void Chase() { AddStatus(new Hunt(chasingPeriod)); } //Chase
  }
} //Thief