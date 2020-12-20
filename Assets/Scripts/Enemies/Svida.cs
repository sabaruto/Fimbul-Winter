using Base_Classes;
using Statuses;
using UnityEngine;

namespace Enemies
{
    public class Svida : Enemy
    {
        // The rock projectile being thrown
        [SerializeField] private GameObject flameRockProjectile;
        private readonly float fireRadius = 2;

        // The range and radius in which Svida and shoot the hail of fire
        private readonly float fireRange = 4;

        // The time period for the fire
        private readonly float fireResetTime = 1;
        private readonly float lavaRadius = 2;

        // The range and radius of the lava shot
        private readonly float lavaRange = 4;

        // The time period for the lava
        private readonly float lavaResetTime = 1;

        // The range at which Svida will start throwing boulder
        private readonly float throwDistance = 3;

        // The current position of the fire Circle
        private Vector2 firePosition;
        private float fireTimer = Mathf.Infinity;

        // The current position of the lava Circle
        private Vector2 lavaPosition;
        private float lavaTimer = Mathf.Infinity;

        private new void Awake()
        {
            base.Awake();

            // Svida creates a circle of fire which burns the player if
            // he goes inside
            var rainOfFire = new Ability("Rain Of Fire", 2, 5, 0, 2f,
                StartRainOfFire, 0.4f);

            // Svida makes a geyser of lava which erupts from the ground, damaging 
            // and burning the player
            var burst = new Ability("Burst", 2, 5, 0, 2f, StartBurst, 0.3f);

            // Svida throws a flaming rock at the player, damaging and burning the
            // player
            var flameThrow = new Ability("Flame Throw", 10, 4, 0, 3, FlameThrow, 0.2f);

            abilityList = new[] {rainOfFire, burst, flameThrow};

            // Setting the attributes for the enemy
            SetVariables("Svida", 200, 100, 4, true);
        }

        private new void Update()
        {
            base.Update();

            if (!IsAlive()) return;

            // Getting the distance between the player and itself
            var distance = Vector2.Distance(transform.position,
                player.transform.position);

            // Attacks with the fire strom if it is in range
            if (distance < fireRange && !isCasting)
            {
                GetAbility("Rain Of Fire").StartAbility(this);
                firePosition = player.transform.position;
            }

            // If not, attacks with the lava gyser
            if (distance < lavaRange && !isCasting)
            {
                GetAbility("Burst").StartAbility(this);
                lavaPosition = player.transform.position;
            }

            if (distance < throwDistance) GetAbility("Flame Throw").StartAbility(this);

            // Checks the fire and lava abilities to ensure they have gone through it's
            // course
            if (fireTimer <= fireResetTime) RainOfFire();
            if (lavaTimer <= lavaResetTime) Burst();
        }


        // This is Unity's quick drawing method which allows you to draw simple 
        // shapes into the game for debugging. The shapes drawn are not seen in the
        // final render of the game
        public void OnDrawGizmos()
        {
            // Drawing the positions for the lava and fire positions
            if (firePosition != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(firePosition, 0.5f);
            }

            if (lavaPosition != null)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawWireSphere(lavaPosition, 0.5f);
            }

            // Checking if the fire or lava ability is in use
            if (fireTimer <= fireResetTime)
            {
                Gizmos.color = Color.red;

                // Draw the circle representing the fire circle
                Gizmos.DrawWireSphere(firePosition, fireRadius);
            }

            if (lavaTimer <= lavaResetTime)
            {
                Gizmos.color = Color.black;

                // Draw the circle representing the lava circle
                Gizmos.DrawWireSphere(lavaPosition, lavaRadius);
            }
        }


        // Starts the fire storm ability
        private void StartRainOfFire()
        {
            fireTimer = fireResetTime;
        }

        // Deals damage to the player if the fire storm is up and the player is in
        // the damage area
        private void RainOfFire()
        {
            // Start attacking the player if they are in range
            if ((int) (10 * (fireTimer - Time.deltaTime)) != (int) (10 * fireTimer) &&
                Vector2.Distance(firePosition, player.transform.position) < fireRadius)
            {
                player.TakeDamage(GetAbility("Rain Of Fire").GetDamage());
                player.AddStatus(new Root(1f));
            }

            // Reduces the timer
            fireTimer -= Time.deltaTime;

            // Stops the fire storm if the timer is up
            if (fireTimer <= 0)
            {
                fireTimer = Mathf.Infinity;

                //Begin the wind down timer
                GetAbility("Rain Of Fire").Complete(this);
            }
        }

        private void StartBurst()
        {
            lavaTimer = lavaResetTime;
        }

        // Deals damage to the player if the lava geyser is up and the player is in
        // the damage area
        private void Burst()
        {
            // If not, start attacking the player
            if ((int) (10 * (lavaTimer - Time.deltaTime)) != (int) (10 * lavaTimer) &&
                Vector2.Distance(lavaPosition, player.transform.position) < lavaRadius)
            {
                player.TakeDamage(GetAbility("Burst").GetDamage());
                player.AddStatus(new Burn(1));
            }

            // Reduces the rattle timer
            lavaTimer -= Time.deltaTime;

            // Stops the fire storm if the timer is up
            if (lavaTimer <= 0)
            {
                lavaTimer = Mathf.Infinity;

                //Begin the wind down timer
                GetAbility("Burst").Complete(this);
            }
        }

        private void FlameThrow()
        {
            var flameRock = Instantiate(flameRockProjectile, transform.position,
                Quaternion.identity);

            flameRock.GetComponent<Projectile>().SetStatuses(new Status[] {new Burn(2)});
            flameRock.GetComponent<Projectile>().Throw(player.transform.position);

            //Begin the wind down timer
            GetAbility("Flame Throw").Complete(this);
        }
    }
}