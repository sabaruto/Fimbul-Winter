// This class increases the speed of the attack and poisons the character's
// attack

using Base_Classes;
using UnityEngine;

namespace Statuses
{
    public class Poisoner : Status
    {
        private readonly int attackDamage;
        private bool addedDamage;

        public Poisoner(float time, int attackDamage = 0, float attackSpeed = 3) : base("Poisoner", time)
        {
            statusColor = Color.yellow;
            attackSpeedMultiplier *= attackSpeed;
            this.attackDamage = attackDamage;
        }

        public override float[] Effect(Character character)
        {
            // Increases the damage of the character
            if (!addedDamage)
            {
                character.AddAttackDamage(attackDamage);
                addedDamage = true;
            }

            character.SetAttackStatus(new Poison(2));
            return base.Effect(character);
        }

        public override void Clear(Character character)
        {
            base.Clear(character);
            character.AddAttackDamage(-attackDamage);
            character.SetAttackStatus(null);
        }
    }
}