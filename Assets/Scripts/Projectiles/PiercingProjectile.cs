using System.Collections.Generic;
using Base_Classes;

// This projectile instead of deleting after being hit, remebers the character
// and doesn't hit that character again

namespace Projectiles
{
  public class PiercingProjectile : Projectile
  {
    // The list of game objects that hit the projectile
    private readonly List<Character> hitCharacters = new List<Character>();

    protected override void Attack(Character otherCharacter)
    {
      // Checks if the characters are in the list
      if (!hitCharacters.Contains(otherCharacter))
      {
        hitCharacters.Add(otherCharacter);
        otherCharacter.TakeDamage(damage);
      }
    }
  }
}