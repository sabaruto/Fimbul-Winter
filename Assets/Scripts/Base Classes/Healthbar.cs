using UnityEngine;

// This class shows the health of the enemies and change accordingly
namespace Base_Classes
{
    public class Healthbar : MonoBehaviour
    {
        // The enemy connected
        [SerializeField] private Enemy enemy;

        // The initaial health of the enemy
        private float initialHealth;

        // The initial length of the bar
        private float intialBarLength;

        // The sprite renderer
        private SpriteRenderer sprite;

        public void Start()
        {
            intialBarLength = transform.localScale.x;
            initialHealth = enemy.GetHealth();
            sprite = GetComponent<SpriteRenderer>();
            sprite.enabled = false;
        }

        public void LateUpdate()
        {
            // Changes the length of the bar to reflect the health
            if (enemy.GetHealth() != initialHealth)
            {
                sprite.enabled = true;
                DrawHealth();
            }

            // Keeps the rotation of the healthbar the same
            var t = transform;

            t.rotation = Quaternion.identity;
            t.position = enemy.transform.position + Vector3.up;
        }

        private void DrawHealth()
        {
            var localScale = transform.localScale;

            localScale = new Vector3(intialBarLength *
                                     (enemy.GetHealth() / initialHealth), localScale.y,
                localScale.z);
            transform.localScale = localScale;
        }
    }
}