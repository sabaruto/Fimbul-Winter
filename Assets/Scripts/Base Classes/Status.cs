using UnityEngine;

namespace Base_Classes
{
  /// <summary>
  ///     The base class for all the status effects, changing the characters
  ///     abilities and reverting them once the effect is done
  /// </summary>
  public abstract class Status
    {
        // The timer for the status
        protected readonly float maxTime = 10;

        // The name of the status
        protected readonly string name = "status";

        // Whether the effect stacks
        protected bool canStack = false;
        protected float currentTimer;

        /// <summary>
        ///     The damage taken by the character each second
        /// </summary>
        protected int damage = 0;

        // The description of the status
        protected string description;

        // The speed and attack speed multipliers
        protected float speedMultiplier = 1, attackSpeedMultiplier = 1;

        // The colour for the status
        protected Color statusColor = Color.white;

        public Status(string name, float maxTime)
        {
            this.maxTime = maxTime;
            this.name = name;
            currentTimer = this.maxTime;
        }

        // Effect the character with the status boosts and debuffs if the status is
        public virtual float[] Effect(Character character)
        {
            // Finds the difference in integer timing between the time before and after
            if (damage > 0 &&
                (int) (currentTimer - Time.deltaTime) != (int) currentTimer)
                character.TakeDamage(damage);

            // Decrease the timer of the status
            currentTimer -= Time.deltaTime;

            // Checks if the timer is complete
            if (currentTimer <= 0) return null;

            // If not, return the multipliers
            return new[] {speedMultiplier, attackSpeedMultiplier};
        }

        // The remove function to reverse all the stuff completed to the character
        public virtual void Clear(Character character)
        {
            // character.GetComponent<SpriteRenderer>().color = Color.white;
        }

        // Get the name of the status
        public string GetName()
        {
            return name;
        }

        // Gets the knowledge of if a status is stacking
        public bool CanStack()
        {
            return canStack;
        }

        // Gets the color for the status
        public Color GetColor()
        {
            return statusColor;
        }

        // Resets the status back to full
        public void Reset()
        {
            currentTimer = maxTime;
        }

        // Gets the description of the status
        public string GetDescription()
        {
            return description;
        }
    }
}