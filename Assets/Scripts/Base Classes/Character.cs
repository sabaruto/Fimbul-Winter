using System.Collections;
using System.Collections.Generic;
using Heroes;
using Managers;
using Misc;
using UnityEngine;

namespace Base_Classes
{
    public class Character : MonoBehaviour
    {
        private static readonly int TimeVar = Animator.StringToHash("Time");
        private static readonly int IsAttacking = Animator.StringToHash("isAttacking");

        // How fast the character basic attacks
        [SerializeField] protected float attackSpeed;

        // The damage of the basic attacks
        [SerializeField] protected int basicDamage;

        [SerializeField] protected float speed;
        protected Ability[] abilityList;

        //Boolean to check if the Character is still alive
        protected bool alive = true;

        // The animator for the character
        protected Animator animator;

        // The current timer for the attack
        protected float currentAttackTimer;

        // The current target for basic attacks of the character
        protected Character currentTarget;
        protected int health;

        // Checking if the character is casting 
        protected bool isCasting;
        protected bool isStunned;
        protected int maxHealth;
        protected int maxStamina;
        protected Movement movement;

        // The old position the character was moving towards
        protected Vector2 preAttackPos;

        // The GameObject for the projectile if the character is ranged
        protected GameObject projectile;

        // The status the projectile gives to the target
        protected Status projStatus;
        protected int range;

        protected SoundManager sound;

        // The multipliers for the speed and attackspeed from the different statuses
        protected float speedInc = 1, attackSpeedInc = 1;

        // The sprite renderer
        protected SpriteRenderer spriteRenderer;
        protected int stamina;

        // The current status of the character
        protected List<Status> statuses = new List<Status>();
        protected bool usesAbility;

        protected void Awake()
        {
            // Adding the movement component to the current gameObject
            movement = gameObject.AddComponent<Movement>();
            movement.SetSpeed(speed);

            // Getting the animator component
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            // Getting the audio manager component
            sound = GetComponent<SoundManager>();
        }

        protected void Update()
        {
            // Stops moving if the character is dead
            if (!IsAlive())
            {
                Die();
                return;
            }

            CompleteStatus();

            if (!isCasting) MoveToAttack();

            // Decreasing the attack timer
            if (currentAttackTimer >= 0)
            {
                currentAttackTimer -= Time.deltaTime;
                // Finding the maximaum value for the attack speed

                if (animator != null)
                {
                    var attackTime = 1 / attackSpeed;
                    animator.SetFloat(TimeVar, ((attackTime - currentAttackTimer)
                        / attackTime + 0.5f) % 1);
                }
            }

            // Checks if the player is casting and if so, stops movement
            if (isCasting) movement.StopMoving();
        }

        protected virtual void Die()
        {
            if (usesAbility) foreach (var ability in abilityList) ability.Terminate();

            movement.SetSpeed(0);
        }

        // This method is called the moment the character dies
        protected virtual void OnDeath()
        {
            health = 0;
            alive = false;
        }

        //Method for the Character to take damage
        public virtual void TakeDamage(int damage)
        {
            health -= damage;

            // Shows the character being hit
            spriteRenderer.color = Color.red;
            StartCoroutine(ClearColour());

            //If the health is less than one, the Character dies
            if (health < 1 && alive) OnDeath();
        } //takeDamage

        // Changing the aniamtion back to white
        private IEnumerator ClearColour()
        {
            yield return new WaitForSeconds(0.1f);

            spriteRenderer.color = Color.white;
        }

        //Method to check if the Character is still alive
        public bool IsAlive()
        {
            return alive;
        } //isAlive

        public bool HasAbility()
        {
            return usesAbility;
        } //hasAbility

        //Method to reduce the stamina of a character
        public void ReduceStamina(int requiredAmount)
        {
            stamina -= requiredAmount;

            if (stamina < 0) stamina = 0;
        } //reduceStamina

        // Method to increase the stamina of a character
        public void IncreaseStamina(int increaseValue)
        {
            stamina += increaseValue;
            if (stamina > maxStamina) stamina = maxStamina;
        }


        //Method to make setting the variables nicer
        protected void SetVariables(string name, int health, int stamina, int range,
            bool doesItUseAbility)
        {
            this.name = name;
            this.health = health;
            maxHealth = this.health;
            this.stamina = stamina;
            maxStamina = stamina;
            this.range = range;
            usesAbility = doesItUseAbility;
        } //setVariables

        public override string ToString()
        {
            return name + " health: " + health + " stamina: " + stamina + " range: " + range;
        } //toString

        public Ability[] GetAbilityList()
        {
            return abilityList;
        }

        // Finding out if the enemy is ranged or not depending on the size of the 
        // range
        public virtual bool IsRanged()
        {
            return projectile != null;
        }

        public bool IsStunned()
        {
            return isStunned;
        }

        public void SetStunned(bool requiredStunned)
        {
            isStunned = requiredStunned;
        }

        //Increase the health of the character
        protected void IncreaseHealth(int requiredHealth)
        {
            health += requiredHealth;

            if (health > maxHealth) health = maxHealth;
        } //increaseHealth

        // Choose an enemy to target and attack
        public void StartAttacking(Character target)
        {
            // Sets the new target 
            currentTarget = target;
        }

        /// <summary>
        ///     This method, when given a target, moves the character into the range of
        ///     the target and when doing so, attacks the character with its basic
        ///     attacks
        /// </summary>
        private void MoveToAttack()
        {
            // Tells the animator the character has stopped attacking
            if (animator != null) animator.SetBool(IsAttacking, false);

            // Runs only when there is a target for the character
            if (currentTarget == null)
            {
                preAttackPos = Vector2.positiveInfinity;
                return;
            }

            // Removes the target if it is dead
            if (!currentTarget.IsAlive())
            {
                currentTarget = null;
                preAttackPos = Vector2.positiveInfinity;
                return;
            }

            // The distance between the target and character
            var position = transform.position;
            var targetPosition = currentTarget.transform.position;
            var distance = Vector2.Distance(position, targetPosition);

            // whether the character can see the target
            var canSeeTarget = movement.GetGrid().LineOfSight(position,
                targetPosition);

            // Checking if the target is in range and if there if the target can
            // be reached
            if (distance <= range && canSeeTarget)
            {
                if (this is Enemy) Debug.Log("I'm going to attack");
                movement.StopMoving();

                preAttackPos = Vector2.positiveInfinity;
                // starts attacking the character
                Attack();

                // Tells the animator the character is attacking
                if (animator != null) animator.SetBool(IsAttacking, true);
            }
            else
            {
                // Checks if the character is moving and if not, starts moving
                if (Vector2.Distance(currentTarget.transform.position, preAttackPos) >
                    speed * Time.deltaTime * 5)
                {
                    var currentTargetPosition = currentTarget.transform.position;

                    movement.StartMoving(currentTargetPosition);
                    preAttackPos = currentTargetPosition;

                    // Debugging purposes
                    if (this is Enemy)
                    {
                        Debug.Log("I've changed attack ");
                        Debug.Log("My current target position is" + currentTargetPosition);
                        Debug.Log("The target object position is " + FindObjectOfType<Player>().transform.position);
                    }
                }
                else if (this is Enemy)
                {
                    Debug.Log("I'm I moving?: " + movement.IsMoving());
                    Debug.Log("Can I see the target?: " + canSeeTarget);
                }
            }
        }

        // Executes all the effects from the statuses
        private void CompleteStatus()
        {
            // Setting the speed and attack speed back to the original values
            speed /= speedInc;
            attackSpeed /= attackSpeedInc;

            // Setting the multipliers back to 1
            speedInc = 1;
            attackSpeedInc = 1;

            // Looping through all the current statuses
            for (var statusIndex = 0; statusIndex < statuses.Count; statusIndex++)
            {
                var status = statuses[statusIndex];
                var multipliers = status.Effect(this);

                // Checks if the status is complete
                if (multipliers == null)
                {
                    // Removes the status if so
                    status.Clear(this);
                    statuses.Remove(status);
                    statusIndex--;
                    continue;
                }

                // If not, increase the values of the speed and attack speed 
                // multipliers accordingly
                speedInc *= multipliers[0];
                attackSpeedInc *= multipliers[1];
            }

            // Change the speed and attack speed according to the multipliers
            speed *= speedInc;
            attackSpeed *= attackSpeedInc;

            // Checks that the current timer is not larger than the current timer
            if (currentAttackTimer > 1 / attackSpeed) currentAttackTimer = 1 / attackSpeed;

            movement.SetSpeed(speed);
        }

        // Adds a status to the character
        public void AddStatus<T>(T newStatus) where T : Status
        {
            if (!newStatus.CanStack())
            {
                // Checks if the character is already being afflicted by this attack
                var repeatedStatus = GetStatus<T>();

                // If so, resets the status back to full
                if (repeatedStatus != null)
                {
                    repeatedStatus.Reset();
                    return;
                }
            }

            statuses.Add(newStatus);
        }

        // Checks if a certain status is occuring and if so, gives the first
        // occurance of it
        public T GetStatus<T>() where T : Status
        {
            foreach (var status in statuses)
                if (status.GetType() == typeof(T))
                    return (T) status;

            return null;
        }

        // Gives all the different statuses affecting the character
        public List<Status> GetStatuses()
        {
            return statuses;
        }

        // Finds a certain ability from the list
        public Ability GetAbility(string name)
        {
            foreach (var ability in abilityList)
                if (ability.GetName() == name)
                    return ability;

            return null;
        }

        // Getting the max health and stamina of the character
        public int GetMaxHealth()
        {
            return maxHealth;
        }

        public int GetMaxStamina()
        {
            return maxStamina;
        }

        // Gets the health of the character
        public int GetHealth()
        {
            return health;
        }

        // Heals the player a certain amount
        public void Heal(int health)
        {
            this.health += health;
            if (this.health > maxHealth) this.health = maxHealth;
        }

        // Gets the stamina of the character
        public int GetStamina()
        {
            return stamina;
        }

        // Sets the attack damage of the character
        public void AddAttackDamage(int damage)
        {
            basicDamage += damage;
        }


        // Gets the casting boolean
        public bool GetCasting()
        {
            return isCasting;
        }

        // Sets the casting boolean
        public void SetCasting(bool isCasting)
        {
            this.isCasting = isCasting;
        }

        // Changing the status given by the auto attack
        public void SetAttackStatus(Status status)
        {
            projStatus = status;
        }

        private void Attack()
        {
            // Updates the direction to the opponent
            movement.UpdateDir(currentTarget.transform.position);

            // Only attacks when the timer is zero
            if (currentAttackTimer > 0) return;

            // Checks if the character is ranged or not
            if (IsRanged())
            {
                // Instantiate the projectile and set it's target to the current
                // target
                var aProjectile = Instantiate(projectile, transform.position,
                    Quaternion.identity);

                var projectileAttack =
                    aProjectile.GetComponent<BasicAttackProjectile>();

                projectileAttack.SetTarget(currentTarget);
                projectileAttack.SetDamage(basicDamage);
                projectileAttack.SetAttackStatus(projStatus);
            }

            // If not straight up damages the target
            else
            {
                currentTarget.TakeDamage(basicDamage);
                if (projStatus != null) currentTarget.AddStatus(projStatus);
            }

            // Creates the audio effect if the character has it
            if (sound != null) sound.Play("AutoAttack");

            // Increase the attack timer to the start
            currentAttackTimer = 1 / attackSpeed;
        }
    }
} //Character