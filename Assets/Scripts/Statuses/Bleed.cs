using Base_Classes;
using UnityEngine;

namespace Statuses
{
  /// <summary>
  ///     This status gives the character movement imparment and damages the player
  /// </summary>
  public class Bleed : Status
    {
        public Bleed(float time, int damage) : base("bleed", time)
        {
            speedMultiplier *= 0.75f;
            this.damage = damage;
            canStack = true;
            statusColor = Color.red;
        }

        public Bleed(float time) : this(time, 1)
        {
        }
    }
}