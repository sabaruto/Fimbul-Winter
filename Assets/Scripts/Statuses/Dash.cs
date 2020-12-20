using Base_Classes;
using UnityEngine;

namespace Statuses
{
    public class Dash : Status
    {
        // The direction the character is moving in
        private readonly Vector2 dir;

        // The speed of the dash
        private readonly float speed = 20;

        public Dash(float time, Vector2 dir) : base("Dash", time)
        {
            this.dir = dir;
        }

        public Dash(float time, Vector2 dir, float speed) : this(time, dir)
        {
            this.speed = speed;
        }

        public override float[] Effect(Character character)
        {
            // Making sure the character cannot cast while dashing
            character.SetCasting(true);

            // The position of the character
            Vector2 charPos = character.transform.position;

            if (currentTimer != 0)
                // Pushes the character in the given direction
                character.transform.position = Vector2.MoveTowards(charPos, charPos + dir,
                    speed * Time.deltaTime);

            return base.Effect(character);
        }

        // Removing the casting state
        public override void Clear(Character character)
        {
            base.Clear(character);
            character.SetCasting(false);
        }
    }
}