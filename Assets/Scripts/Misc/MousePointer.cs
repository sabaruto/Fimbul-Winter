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

        // The main camera
        private Camera mainCamera;

        // The sprite renderer for the pointer
        private SpriteRenderer spriteRenderer;

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

            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            var hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

            if (hit.collider == null) return;

            var e = hit.collider.GetComponent<Enemy>();
            if (e != null) Debug.Log("Enemy!");
        }

        public void LateUpdate()
        {
            transform.position =
                (Vector2) mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        private IEnumerator ShowImage()
        {
            yield return new WaitForSeconds(2);
            spriteRenderer.enabled = false;
            yield return null;
        }
    }
}