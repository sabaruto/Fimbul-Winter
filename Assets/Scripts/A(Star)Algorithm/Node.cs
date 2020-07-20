using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///   The node of the A* Algorithm, holding all the specific values for the node
/// </summary>
public class Node : IComparable<Node>
{
  // The h and g cost finding the distance between the start and end of the path
  private readonly float hCost;

  // The index of the node
  private readonly int index;

  // Checking if the node is a obstacle
  private readonly bool isObstacle;
  private readonly Vector2 position;
  private float gCost;

  // Checking if the node has been explored
  private bool hasExplored;
  private Node parentNode;

  public Node(Vector2 position, int index, Vector2 endNodePosition,
    Node parentNode)
  {
    // Setting refrences to the position and parent node
    this.position = position;
    this.parentNode = parentNode;
    this.index = index;

    // Finding the hCost of the node

    // Getting the difference in position between the node and the end node
    Vector2 positionDifference = endNodePosition - position;

    // Finding the absolute values for the x and y position
    var xValue = Mathf.Abs(positionDifference.x);
    var yValue = Mathf.Abs(positionDifference.y);

    // Gets the difference between the x and y values
    var coordinateDifference = xValue - yValue;

    // If there is a larger x value, get the diagonal distance and add to the 
    // difference
    var diagonalDistance = coordinateDifference > 0 ? yValue : xValue;

    hCost = diagonalDistance * Mathf.Sqrt(2) + Mathf.Abs(coordinateDifference);

    FindGCost();
  }

  /// <summary>
  ///   Creating a obstacle node
  /// </summary>
  public Node(Vector2 position, int index)
  {
    this.position = position;
    isObstacle = true;
    this.index = index;
  }

  public int CompareTo(Node y)
  {
    var returnValue = ConvertToCompare(FCost() - y.FCost());
    if (returnValue == 0) returnValue = ConvertToCompare(hCost - y.hCost);
    return returnValue;
  }

  /// <summary>
  ///   When given a newley explored position, it checks if that position is a
  ///   better parent node and changes it's values accordingly if so. It returns
  ///   wheather or not the node updated
  /// </summary>
  public bool CheckParentNode(Node updatedNode)
  {
    // Finds the gCost using the new node
    var newGCost = updatedNode.gCost +
                   Mathf.Clamp(Vector2.Distance(updatedNode.position, position), -1.4f, 1.4f);


    if (newGCost < gCost)
    {
      parentNode = updatedNode;
      gCost = newGCost;

      return true;
    }

    return false;
  }

  /// <summary>
  ///   Finds the lineage of parent nodes in the form of a list of nodes
  /// </summary>
  public List<Node> GetLineage()
  {
    var lineage = new List<Node>();

    if (parentNode != null) lineage = parentNode.GetLineage();

    lineage.Add(this);

    return lineage;
  }

  /// <summary>
  ///   Finds the g cost of the node
  /// </summary>
  private void FindGCost()
  {
    if (parentNode != null)
      gCost = parentNode.gCost + Mathf.Clamp(Vector2
        .Distance(parentNode.position, position), -1.4f, 1.4f);
    else
      gCost = 0;
  }

  /// <summary>
  ///   Gets the h cost of the node
  /// </summary>
  public float HCost() { return hCost; }

  /// <summary>
  ///   Finds the f cost of the node
  /// </summary>
  public float FCost() { return hCost + gCost; }

  public Vector2 GetPosition() { return position; }

  public void Explore() { hasExplored = true; }

  public bool HasExplored() { return hasExplored; }

  public bool IsObstacle() { return isObstacle; }

  public int GetIndex() { return index; }

  public int Compare(object x, object y) { return ((Node) x).CompareTo((Node) y); }

  // Changing the values to become more distinct
  private int ConvertToCompare(float value)
  {
    if (value > 0) return 1;
    if (value < 0) return -1;
    return 0;
  }

  // Define the is greater than operator.
  public static bool operator >(Node operand1, Node operand2) { return operand1.CompareTo(operand2) == 1; }

  // Define the is less than operator.
  public static bool operator <(Node operand1, Node operand2) { return operand1.CompareTo(operand2) == -1; }

  // Define the is greater than or equal to operator.
  public static bool operator >=(Node operand1, Node operand2) { return operand1.CompareTo(operand2) >= 0; }

  // Define the is less than or equal to operator.
  public static bool operator <=(Node operand1, Node operand2) { return operand1.CompareTo(operand2) <= 0; }
}