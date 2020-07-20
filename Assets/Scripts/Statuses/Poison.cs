using Base_Classes;

namespace Statuses
{
    /// <summary>
    ///   This status gives the character movement impairment and deals damage every second
    /// </summary>
    public class Poison : Status
  {
    public Poison(float time) : base("poison", time)
    {
      attackSpeedMultiplier *= 0.7f;
      damage = 1;
      canStack = true;
    }
  }
}