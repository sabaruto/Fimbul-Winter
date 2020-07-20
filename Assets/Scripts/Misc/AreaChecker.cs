using System.Collections.Generic;
using Base_Classes;
using Heroes;
using UnityEngine;

// This class finds and returns the objects hit by its collider

namespace Misc
{
  public class AreaChecker : MonoBehaviour
  {
    // The boolean to check if the image is always on
    [SerializeField] private bool alwaysDraw = true;

    // The collider for the area checker
    private Collider2D coll2D;

    // The direction the checker should go
    private Vector2 direction;

    // The boolean to say if the checker is being drawn at this point
    private bool draw;

    // The boolean checking if the draw function should be pointing at the player

    private Transform playerTransform;

    // boolean to check whether or not the object orientates to the cursor
    [SerializeField] private bool pointsToCursor = true;

    // boolean to check whether or not the object orientates to the player
    [SerializeField] private bool pointsToPlayer;

    // The sprite renderer for the area checker
    private SpriteRenderer sprRenderer;

    public void Start()
    {
      coll2D = GetComponent<Collider2D>();
      sprRenderer = GetComponent<SpriteRenderer>();

      if (sprRenderer != null) sprRenderer.enabled = alwaysDraw;

      // Chooses either the player or cursor transform to follow
      if (pointsToPlayer) playerTransform = FindObjectOfType<Player>().transform;
    }

    public T[] GetCharactersInRange<T>() where T : Character
    {
      var results = new List<Collider2D>();
      coll2D.OverlapCollider(new ContactFilter2D(), results);

      var charResults = new List<T>();

      foreach (Collider2D result in results)
      {
        // The character connected to the collider
        T colCharacter = result.GetComponent<T>();

        if (colCharacter != null) charResults.Add(colCharacter);
      }

      return charResults.ToArray();
    }

    public void SetDirection(Vector2 direction) { this.direction = direction; }

    public void LateUpdate()
    {
      Transform t = transform;
      if (pointsToCursor || draw && sprRenderer != null)
      {
        Debug.Assert(Camera.main != null);
        t.up = (Vector2) (Camera.main.ScreenToWorldPoint(Input.mousePosition) - t.position);
      }
      else if (pointsToPlayer)
        t.up = (Vector2) (playerTransform.position - t.position);
      else
        t.up = direction;

      // resets the up transform if the area checker is a circle
      if (coll2D is CircleCollider2D) transform.up = Vector2.up;
    }

    // Sets whether to draw in refrence to the position of checking or cursor
    // public void DrawToCursor(bool lookCursor) { }

    // Set the draw function for the area checker
    public void SetDraw(bool canDraw)
    {
      draw = canDraw;
      if (sprRenderer != null) sprRenderer.enabled = draw;
    }
  }
}