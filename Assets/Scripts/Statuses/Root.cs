using Base_Classes;


//This status roots the character. It makes them move and attack very slowly
namespace Statuses
{
  public class Root : Status
  {
    //slow the character's attack speed and movement
    public Root(float time) : base("root", time)
    {
      speedMultiplier *= 0.01f;
      attackSpeedMultiplier *= 0.5f;
    } //Stun

    //Set the characted as stunned
    public override float[] Effect(Character character)
    {
      character.SetStunned(true);

      return base.Effect(character);
    }

    //Set the character as not stunned
    public override void Clear(Character character)
    {
      base.Clear(character);
      character.SetStunned(false);
    }
  }
}