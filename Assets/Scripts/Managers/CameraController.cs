using UnityEngine;

namespace Managers
{
  public class CameraController : MonoBehaviour
  {
    // The limit in which the camera can be moved
    [SerializeField] private Vector3 bounds = Vector3.one;

    // The camera component
    private Camera currentCamera;

    // conditionalizing if you have the camera locked on
    private bool isLockedOn;

    // The pan boarder thickness
    [SerializeField] private float panBoarderThickness = 10f;

    [SerializeField] private float panSpeed = 10f;

    // Gets the player position
    [SerializeField] private Transform playerTransform;

    // The minimum and maximum size the camera can scroll to
    [SerializeField] private float screenSizeMax = 10, screenSizeMin = 4;

    [SerializeField] private float scrollSpeed = 200f;

    // Conversion between pixels and world
    private float worldToPixels;
    
    // The main Camera
    private Camera mainCamera;

    private void Start()
    {
      currentCamera = GetComponent<Camera>();
      mainCamera = Camera.main;
    }

    private void Update()
    {
      // Allowing you to toggle the movement of the camera
      if (Input.GetKeyDown("y")) isLockedOn = !isLockedOn;

      Vector3 pos = transform.position;

      // If not, it moves only when the mouse is on the edges of the screen
      if (!isLockedOn)
      {
        if (Input.mousePosition.y >= Screen.height - panBoarderThickness) pos.y += panSpeed * Time.deltaTime;

        if (Input.mousePosition.y <= panBoarderThickness) pos.y -= panSpeed * Time.deltaTime;

        if (Input.mousePosition.x >= Screen.width - panBoarderThickness) pos.x += panSpeed * Time.deltaTime;

        if (Input.mousePosition.x <= panBoarderThickness) pos.x -= panSpeed * Time.deltaTime;

        // Clamping the position to being only to the edges of the boards
        pos.x = Mathf.Clamp(pos.x, -bounds.x, bounds.x);
        pos.y = Mathf.Clamp(pos.y, -bounds.y, bounds.y);

        transform.position = pos;
      }

      // Moving the size of the camera from the action
      var scroll = Input.GetAxis("Mouse ScrollWheel");

      var orthographicSize = currentCamera.orthographicSize;
      
      orthographicSize -= scroll * scrollSpeed * Time.deltaTime;
      currentCamera.orthographicSize = orthographicSize;

      // Clamping the size of the size
      currentCamera.orthographicSize = Mathf.Clamp(orthographicSize, screenSizeMin, screenSizeMax);
    }

    private void LateUpdate()
    {
      worldToPixels = Screen.height / 2.0f / mainCamera.orthographicSize;

      Vector3 pos = transform.position;
      Vector3 playerPos = playerTransform.position;

      // Making the controller move to the player if the camera is locked on
      if (isLockedOn)
      {
        pos = new Vector3(playerPos.x, playerPos.y, pos.z);

        pos.x = Mathf.Clamp(pos.x, -bounds.x, bounds.x);
        pos.y = Mathf.Clamp(pos.y, -bounds.y, bounds.y);
      }

      var width = Screen.width / worldToPixels;
      var height = Screen.height / worldToPixels;

      // Clamping the position to being only a certain distance from the player
      pos.x = Mathf.Clamp(pos.x, playerTransform.position.x - width / 2 + 1,
        playerPos.x + width / 2 - 1);

      pos.y = Mathf.Clamp(pos.y, playerPos.y - height / 2 + 1, playerPos.y + height / 2 - 1);

      transform.position = pos;
    }
  }
}