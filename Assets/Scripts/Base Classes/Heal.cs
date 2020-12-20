using UnityEngine;

namespace Base_Classes
{
    public class Heal : MonoBehaviour
    {
        // The heal position for the character
        [SerializeField] private Transform healTransform;

        // Gets the position of where the character should stand
        public Vector2 GetStandPosition()
        {
            return healTransform.position;
        }
    }
}