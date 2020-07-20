using System.Collections;
using Base_Classes;
using UnityEngine;

namespace Misc
{
  public class MousePointer : MonoBehaviour
  {
    [SerializeField] private float counter = 400;

    private float currentCounter;

    private bool hasRightClicked;

    // The sprite renderer for the pointer
    private SpriteRenderer spriteRenderer;
    
    // The main camera
    private Camera mainCamera;

    private void Start()
    {
      spriteRenderer = GetComponent<SpriteRenderer>();
      currentCounter = counter;
      mainCamera = Camera.main;
    }

    // Update is called once per frame
    private void Update()
    {
      if (Input.GetMouseButtonDown(1)) hasRightClicked = true;

      if (!hasRightClicked) currentCounter -= Time.deltaTime;


      if (currentCounter < 0)
      {
        Debug.Log("Right click to move");
        StopAllCoroutines();
        spriteRenderer.enabled = true;
        StartCoroutine(ShowImage());
        currentCounter = counter;
      }
      
      Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

      RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

      if (hit.collider == null) return;

      Enemy e = hit.collider.GetComponent<Enemy>();
      if (e != null) Debug.Log("Enemy!");
    }

    private IEnumerator ShowImage()
    {
      yield return new WaitForSeconds(2);
      spriteRenderer.enabled = false;
      yield return null;
    }

    public void LateUpdate()
    {
      transform.position =
        (Vector2) mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }
  }
}