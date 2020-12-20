using Base_Classes;
using Heroes;
using Misc;
using Statuses;
using UnityEngine;

namespace Enemies
{
    public class Drunk : Enemy
    {
        [SerializeField] private AreaChecker stabShape;

        //The chasing radius
        private readonly float chasingRadius = 3;

        private readonly float kickRadius = 2;

        private new void Awake()
        {
            base.Awake();

            //Stab, the belligerent drunk stabs the player, it adds bleed
            var stab = new Ability("Stab", 3, 10, 0, 0.2f, Stab);

            //Kick, the belligerent drunk kiks the player
            var kick = new Ability("Kick", 2, 5, 10, 0.5f, Kick, 0.2f);

            abilityList = new[] {stab, kick};

            SetVariables("Belligerent Drunk", 50, 20, 1, true);
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

            // Checks if the player is within backstab radius and if the ability is ready
            if (distance < kickRadius && !isCasting) GetAbility("Kick").StartAbility(this);

            if (distance < kickRadius && !isCasting) GetAbility("Stab").StartAbility(this);
        } //Update

        //This method uses the stab ability
        private void Stab()
        {
            //Checks if the plaeyr is in range
            var playersInRange = stabShape.GetCharactersInRange<Player>();

            if (playersInRange.Length > 0)
            {
                //Hits the player
                currentTarget = playersInRange[0];

                //Deals damage to the player
                currentTarget.TakeDamage(GetAbility("Stab").GetDamage());

                currentTarget.AddStatus(new Bleed(3));
            }

            //Start the hit ability cooldown
            GetAbility("Stab").SetCoolDown();

            isCasting = false;
        } //Stab

        //This method uses the kick ability
        private void Kick()
        {
            if (GetAbility("Kick").IsAbilityReady(this))
            {
                //Deal damage to the player
                player.TakeDamage(GetAbility("Kick").GetDamage());

                //Begin the wind down timer
                GetAbility("Kick").Complete(this);
            } //if
        } //Kick
    }
} //Drunk