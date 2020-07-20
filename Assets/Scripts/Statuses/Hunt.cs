using Base_Classes;

namespace Statuses
{
  /// <summary>
  ///   This status gives the character increased speed and attack speed
  /// </summary>
  public class Hunt : Status
  {
    public Hunt(float time) : base("hunt", time)
    {
      speedMultiplier *= 1.5f;
      attackSpeedMultiplier *= 1.5f;
    }
  }
}