using Base_Classes;
using Heroes;
using Misc;
using Statuses;
using UnityEngine;

namespace Enemies
{
    public class Draugr : Enemy
    {
        [SerializeField] private AreaChecker stabShape;

        // The chasing radius
        private readonly float chasingRadius = 2;

        private readonly float stabDistance = 1;

        private readonly float stenchRadius = 3;

        private new void Awake()
        {
            base.Awake();

            //Stab, this is pretty much a basic attack. The thief deals damage to the player
            var stab = new Ability("Stab", 2, 1, 0, 0.5f, Stab);

            //Stench, the draugr releases the stench of a dead corpse, stunning the player
            var stench = new Ability("Stench", 1, 10, 0, 1f, Stench, 0.1f);

            //Awaken, the draugr becomes larger. This increases their damage and health and lowers their attack speed
            var awaken = new Ability("Awaken", 0, 15, 0, 2.5f, Awaken);

            abilityList = new[] {stab, stench, awaken};

            //Set the variables of the draugr
            SetVariables("Draugr", 100, 1, 1, true);

            TakeDamage(20);
        } //Awake

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
                Awaken();
            } //if

            // Checks if the player is within stench radius and if the ability is ready
            if (distance < stenchRadius && !isCasting) GetAbility("Stench").StartAbility(this);

            //Check if the player is within stab distance
            if (distance < stabDistance) GetAbility("Stab").StartAbility(this);
        } //Update

        //This method uses the stab ability
        private void Stab()
        {
            //Checks if the player is in range
            var playersInRange = stabShape.GetCharactersInRange<Player>();

            if (playersInRange.Length > 0)
            {
                //Hits the player
                currentTarget = playersInRange[0];

                //Deals damage to the player
                currentTarget.TakeDamage(GetAbility("Stab").GetDamage());

                //Adds the bleed effect
                currentTarget.AddStatus(new Bleed(1));
            } //if

            //Start the hit ability cooldown
            GetAbility("Stab").SetCoolDown();

            isCasting = false;
        } //Stab

        //This method uses the awaken ability
        private void Awaken()
        {
            if (GetAbility("Awaken").IsAbilityReady(this))
            {
                //Add damage to the stab and stench ability
                GetAbility("Stab").AddDamage(5);
                GetAbility("Stench").AddDamage(3);

                //Increase the draugr's health
                IncreaseHealth(20);

                //Add the slow status to the draugr
                AddStatus(new Slow(10));

                //Start awaken ability cooldown
                GetAbility("Awaken").SetCoolDown();
            } //if

            isCasting = false;
        } //Awaken

        //This method uses the stench ability
        private void Stench()
        {
            //Deal damage to the players
            player.TakeDamage(GetAbility("Stench").GetDamage());

            //Give the player the root effect
            player.AddStatus(new Root(1));

            //Begin the wind down timer
            GetAbility("Stench").Complete(this);
        } //Stench
    }
} //Draugr