using UnityEngine;

namespace Base_Classes
{
  /// <summary>
  ///   This base class holds the projectile and moves towards a target and damages
  ///   the enemy
  /// </summary>
  public class BasicAttackProjectile : MonoBehaviour
  {
    // The distance travelled by the projectile
    private readonly float speed = 0.7f;

    // The damage this basic attack yields
    private int damage;

    // The status given to the character when hit
    private Status status;

    // The character this projectile is moving to
    private Character target;

    // Moving towards the target until the distance is close enough then 
    // damages of the target 
    public void LateUpdate()
    {
      // Finds the position of the target
      Vector2 targetPosition = target.transform.position;

      Transform t = transform;

      // Finds the difference in position between yourself and the target
      Vector2 targetDifference = targetPosition - (Vector2) t.position;

      // Directs the projectile to the direction of the unit
      t.up = targetDifference;

      if (targetDifference.magnitude < speed)
      {
        target.TakeDamage(damage);

        // Gives the character a status if there is one to give
        if (status != null) target.AddStatus(status);
        Destroy(gameObject);
      }

      // If not, move closer to the target
      else
      {
        transform.position += (Vector3) targetDifference.normalized * speed;
      }
    }

    // Sets the target of the object
    public void SetTarget(Character character) { target = character; }

    // Sets the damage of the attack
    public void SetDamage(int value) { damage = value; }

    // Adds a status to the basic attack
    public void SetAttackStatus(Status newStatus) { status = newStatus; }
  }
}