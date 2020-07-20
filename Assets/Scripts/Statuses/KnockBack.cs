using System.Linq;
using Base_Classes;
using UnityEngine;

namespace Statuses
{
  public class KnockBack : Status
  {
    // The direction the character is being hit
    private readonly Vector2 dir;

    // The speed of the knock back
    private readonly float speed;

    // Finding the direction at which the enemy is hit
    public KnockBack(float time, Vector2 dir, float speed = 20) : base("KnockBack",
      time)
    {
      this.dir = dir;
      this.speed = speed;
    }

    public override float[] Effect(Character character)
    {
      // Causing the character to be casting as to not allow any abilities
      // or auto attacking
      character.SetCasting(true);

      // The value for the character position
      Vector2 charPos = character.transform.position;
      var hit2D = new RaycastHit2D[50];

      Physics2D.CircleCastNonAlloc(charPos, 0.5f, dir, hit2D, 10f * Time.deltaTime);
      hit2D =  hit2D.Where(x => x).ToArray();

      Debug.DrawLine(charPos, charPos + dir * (speed * Time.deltaTime));

      // Checks if there is a wall in that position
      if (hit2D.Length > 1)
        // Checks for all the colliders with a different name than the character
        // with the knockback ability
        foreach (RaycastHit2D charHit in hit2D)
        {
          if (charHit.transform == null) continue;

          // Getting the character component of the player
          Character hitChar = charHit.transform.GetComponent<Character>();

          // Checking 
          if (hitChar != null && hitChar != character)
          {
            // Checking, if the character is an enemy, if the other character is
            // an enemy
            if (character is Enemy && hitChar is Enemy aEnemy)
              // Checks if the enemy has been knocked back already
              if (aEnemy.GetStatus<KnockBack>() == null)
              {
                // Finding the direction between itself and the enemy
                Vector2 enemyDir = charHit.point - charPos;

                // Adds the knockback effect to the next character
                aEnemy.AddStatus(new KnockBack(currentTimer, enemyDir));
              }
          }

          // If not, checks if the collider is a wall
          else if (charHit.transform.CompareTag("Obstacle"))
          {
            currentTimer = 0;
            character.AddStatus(new Root(2));
          }
        }

      if (currentTimer != 0)
        // Pushes the character a certain distance backwards
        character.transform.position = Vector2.MoveTowards(charPos, charPos + dir,
          speed * Time.deltaTime);

      // Calling the base effect method
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