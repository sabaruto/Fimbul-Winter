using Base_Classes;
using UnityEngine;

namespace Statuses
{
  /// <summary>
  ///     This class holds the status for stalking the enemy for a certain amount of
  ///     time given at cast time
  /// </summary>
  public class Stalk : Status
    {
        // The gamma value of the stalking character
        private readonly float gamma;

        // The sprite of the character
        private SpriteRenderer sprite;

        // The color being used by the character sprite
        private Color spriteColor = Color.clear;

        public Stalk(float time, float gamma) : base("stalk", time)
        {
            speedMultiplier *= 0.5f;
            this.gamma = gamma;
            damage = 0;
        }

        // Creating a stalking object with just the stalk of half the gamma
        public Stalk(float time) : this(time, 0.1f)
        {
        }

        // Change the effect to also make the sprite less transparent
        public override float[] Effect(Character character)
        {
            sprite = character.GetComponent<SpriteRenderer>();

            // Getting the color of the sprite if it hasn't been obtained yet
            if (spriteColor == Color.clear)
            {
                spriteColor = sprite.color;
                spriteColor.a = gamma;
                sprite.color = spriteColor;
            }

            // Calling the base effect method
            return base.Effect(character);
        }

        // Changing the color of the character back
        public override void Clear(Character character)
        {
            base.Clear(character);
            if (spriteColor != Color.clear)
            {
                spriteColor.a = gamma;
                sprite.color = spriteColor;
            }

            currentTimer = 0;
        }
    }
}