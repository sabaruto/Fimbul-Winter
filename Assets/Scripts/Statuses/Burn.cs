using Base_Classes;
using UnityEngine;

namespace Statuses
{
    /// <summary>
    ///   This status gives the character movement impairment and damages them
    /// </summary>
    public class Burn : Status
  {
    public Burn(float time) : base("burn", time)
    {
      statusColor = new Color(0.8f, 0.1f, 0.1f, 1);
      speedMultiplier *= 0.5f;
      damage = 2;
    } //Burn
  }
}