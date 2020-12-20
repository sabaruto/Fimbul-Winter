using Base_Classes;
using Heroes;
using Misc;
using Statuses;
using UnityEngine;

namespace Enemies
{
  /// <summary>
  ///     Nid will be the tutorial giant. His name means the loss of honour
  ///     He is a coward that has ran from the battlefield of Ragnarok
  ///     His abilities deal low damage.
  ///     Abilities: Hit, Yell and Crush
  /// </summary>
  public class Nid : Enemy
    {
        [SerializeField] private AreaChecker hitShape;

        //The radius for different abilities
        private readonly float chasingRadius = 3;
        private readonly float crushDistance = 1;
        private readonly float hitDistance = 1;
        private readonly float yellRadius = 2;

        private new void Awake()
        {
            base.Awake();

            //Hit, Nid hits the player with his club, knocking him over(rooting him)
            var hit = new Ability("Hit", 5, 5, 0, 0.7f, Hit, 0.2f);

            //Yell, Nid yells at the player and adds the fear effect
            //if the player is in range
            var yell = new Ability("Yell", 0, 10, 0, 2f, Yell);

            //Crush, Nid tries to Crush the player with his foot,
            //knocking him over and rooting him if it hits
            var crush = new Ability("Crush", 20, 10, 0, 3f, Crush, 0.3f);

            abilityList = new[] {hit, yell, crush};

            //Set Nid's variables
            SetVariables("Nid", 75, 1, 1, true);
        } //Awake

        // Update is called once per frame
        private new void Update()
        {
            base.Update();

            // Getting the distance between the player and itself
            var distance = Vector2.Distance(transform.position,
                player.transform.position);

            if (distance < chasingRadius) currentTarget = player;

            // Checks if the player is within yell radius and if the ability is ready
            if (distance < yellRadius && !isCasting) GetAbility("Yell").StartAbility(this);

            //Check if the player is within stab distance
            if (distance < crushDistance && !isCasting) GetAbility("Crush").StartAbility(this);

            if (distance < hitDistance && !isCasting) GetAbility("Hit").StartAbility(this);
        } //Update

        private void Hit()
        {
            //Checks if the player is in range
            var playersInRange = hitShape.GetCharactersInRange<Player>();

            if (playersInRange.Length > 0)
            {
                //Hits the player
                currentTarget = playersInRange[0];

                //Deals damage to the player
                currentTarget.TakeDamage(GetAbility("Hit").GetDamage());

                //Give the player the root status
                currentTarget.AddStatus(new Root(1));
            }

            GetAbility("Hit").Complete(this);
        } //Hit

        private void Yell()
        {
            //Add the fear status to the player
            player.AddStatus(new Fear(3));

            //Start the yell ability cooldown
            GetAbility("Yell").SetCoolDown();

            isCasting = false;
        } //Yell

        private void Crush()
        {
            //Deal damage to the player
            player.TakeDamage(GetAbility("Crush").GetDamage());

            //Give the player the root effect
            player.AddStatus(new Root(1.5f));

            //Begin the wind down timer
            GetAbility("Crush").Complete(this);
        } //Crush
    }
} //Nid