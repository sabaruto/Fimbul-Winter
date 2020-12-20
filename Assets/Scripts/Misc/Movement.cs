using UnityEngine;

namespace Misc
{
  /// <summary>
  ///     This class uses the Grid algorithm to find an optimal path to move when is
  ///     prompted
  /// </summary>
  public class Movement : MonoBehaviour
    {
        // The speed of the character
        [SerializeField] private float speed;

        // The curve of the circle
        private readonly float curveCircle = 0.25f;

        // The current node that the character is moving to
        private int currentNodeIndex;

        // The current array of nodes that the object is following
        private Node[] currentNodes;

        // The direction of the node
        private Vector2 dirPosition;

        // The grid that the movement is using
        private MyGrid grid;

        // boolean checking if the object is moving
        private bool isMoving;

        // Update is called once per frame
        private void Update()
        {
            if (isMoving) Move();

            UpdateCharacterDir();
        }

        public void OnDrawGizmosSelected()
        {
            // grid.OnDrawGizmosSelected();

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(dirPosition, Vector3.one);
        }

        // Sets the grid
        public void SetGrid(MyGrid grid)
        {
            this.grid = grid;
        }

        /// <summary>
        ///     Finding the nodes the movement will move through
        /// </summary>
        public void StartMoving(Vector2 destination)
        {
            // Clears the grid of movement

            grid.ClearGrid();
            var nodeList = grid.FindRoute(transform.position, destination);

            if (nodeList != null)
            {
                currentNodes = nodeList.ToArray();
                isMoving = true;
                currentNodeIndex = 0;

                // Sets the direction to the direction of the first node
                dirPosition = currentNodes[0].GetPosition();
            }
        }

        /// <summary>
        ///     Moving an object according to the current array of nodes
        /// </summary>
        private void Move()
        {
            // Checks if the character is close enough to the position of the end point
            // and if so, goes to that point and end the movement
            if (Vector2.Distance(transform.position,
                    currentNodes[currentNodes.Length - 1].GetPosition())
                < speed * Time.deltaTime)
            {
                transform.position = currentNodes[currentNodes.Length - 1].GetPosition();
                isMoving = false;
                currentNodeIndex = 0;
                return;
            }

            // Find the last node which the player has passed the "turn boundary" from
            // the current current node
            for (var nodeIndex = currentNodeIndex;
                nodeIndex < currentNodes.Length;
                nodeIndex++)
                if (Vector2.Distance(currentNodes[nodeIndex].GetPosition(),
                    transform.position) < curveCircle)
                    currentNodeIndex = nodeIndex == currentNodes.Length - 1 ? nodeIndex : nodeIndex + 1;
                else
                    break;

            // Turn towards the position and move to that point
            dirPosition = Vector2.MoveTowards(dirPosition,
                currentNodes[currentNodeIndex].GetPosition(),
                speed * Time.deltaTime);

            var t = transform;
            var position = t.position;

            position += ((Vector3) dirPosition - position).normalized * (Time.deltaTime * speed);
            t.position = position;
        }

        // Set the value of the speed
        public void SetSpeed(float newSpeed)
        {
            speed = newSpeed;
        }

        public float GetSpeed()
        {
            return speed;
        } //GetSpeed

        // Update the direction of the character
        private void UpdateCharacterDir()
        {
            var t = transform;
            // Changing the direction of the character to the movement
            if (dirPosition != (Vector2) t.position) t.up = dirPosition - (Vector2) t.position;
        }

        public Vector2 GetDir()
        {
            return dirPosition;
        }

        public void UpdateDir(Vector2 dirPosition)
        {
            this.dirPosition = dirPosition;
        }

        // Checks if the character is moving
        public bool IsMoving()
        {
            return isMoving;
        }

        // Stops the characters movement
        public void StopMoving()
        {
            isMoving = false;
            currentNodes = null;
        }

        // Checking if two points are in the same line of sight
        public bool LineOfSight(Vector2 startPosition, Vector2 endPosition)
        {
            return grid.LineOfSight(startPosition, endPosition);
        }

        // Gets the grid object
        public MyGrid GetGrid()
        {
            return grid;
        }
    }
}