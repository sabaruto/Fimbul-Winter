using Base_Classes;
using UnityEngine;

namespace Statuses
{
  /// <summary>
  ///     This status gives the character movement imparment and
  ///     decreases their attack speed
  /// </summary>
  public class Fear : Status
    {
        public Fear(float time) : base("fear", time)
        {
            statusColor = Color.black;
            speedMultiplier *= 0.5f;
            attackSpeedMultiplier *= 0.5f;
        } //Fear
    }
}