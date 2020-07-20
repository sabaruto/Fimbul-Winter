using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///   This class holds the grid for the A* algorithm and executes the algorithm
/// </summary>
public class MyGrid
{
  // The list of nodes that have been found
  private readonly List<Node> foundNodes;

  // The array of nodes
  private readonly Node[] grid;

  // The height and length of the grid
  private readonly int length, height;

  // The scale of the grid, so how far one grid distance is
  private readonly float scale;

  // The position for the centre of the grid
  private Vector2 position;

  // Temporary variable: The current Route
  private List<Node> route = new List<Node>();

  /// <summary>
  ///   Initialising the grid with empty nodes
  /// </summary>
  public MyGrid(int length, int height, float scale)
  {
    grid = new Node[height * length];
    this.length = length;
    this.height = height;
    this.scale = scale;

    foundNodes = new List<Node>();
  }

  /// <summary>
  ///   Finds the scaled position of the node given the node in the index of the
  ///   node
  /// </summary>
  private Vector2 NodeVector(IReadOnlyList<int> index)
  {
    Vector2 nodePosition = position;
    var centeredX = index[0] - length / 2;
    var centeredY = index[1] - height / 2;
    nodePosition += new Vector2(scale * centeredX, scale * centeredY);

    return nodePosition;
  }

  /// <summary>
  ///   Finds the index of a vector2 position
  /// </summary>
  public int[] VectorCoordinates(Vector2 nodeVector)
  {
    nodeVector -= position;
    nodeVector /= scale;

    var nodeCoordinates = new int[2];

    nodeCoordinates[0] = Mathf.RoundToInt((float) length / 2 + nodeVector.x);
    nodeCoordinates[1] = Mathf.RoundToInt((float) height / 2 + nodeVector.y);

    return nodeCoordinates;
  }

  /// <summary>
  ///   Finding the coordinates of the node depending on the index
  /// </summary>
  private int[] Coordinates(int index)
  {
    var coordinates = new int[2];
    // Finds the number of rows that go into the index, which would be the row
    // number
    coordinates[1] = index / length;
    // Finds the left over 
    coordinates[0] = index - coordinates[1] * length;
    return coordinates;
  }

  private int Index(int[] coordinates) { return coordinates[0] + coordinates[1] * length; }

  /// <summary>
  ///   Creating a obstacle node
  /// </summary>
  public void SetObstacle(int x, int y)
  {
    // Find the position of the node
    Vector2 nodePosition = NodeVector(new[] {x, y});
    var index = Index(new[] {x, y});
    grid[index] = new Node(nodePosition, index);
  }

  /// <summary>
  ///   Creating an obstacle node from the vector position
  /// </summary>
  public void SetObstacle(Vector2 pos)
  {
    // The coordinates of the position
    var coordinates = VectorCoordinates(pos);

    // Checking if the point is on the grid
    if (isOnGrid(coordinates)) SetObstacle(coordinates[0], coordinates[1]);
  }

  /// <summary>
  ///   Clears the board of all nodes. Can be chosen to also delete the barriers
  /// </summary>
  public void ClearGrid(bool clearObstacles = false)
  {
    // Looping through all the different nodes
    for (var gridIndex = 0; gridIndex < grid.Length; gridIndex++)
    {
      // Ignoring values which are already null
      if (grid[gridIndex] == null) continue;

      if (clearObstacles || !grid[gridIndex].IsObstacle()) grid[gridIndex] = null;
    }

    // Clear the values in the found Nodes
    foundNodes.Clear();
  }

  /// <summary>
  ///   Goes through all the different values around a node and instantiate them
  ///   If the node have already been seen, it will update it's parent to the
  ///   closest one
  /// </summary>
  private void Explore(int exploringNodeIndex, Vector2 endNodeVector)
  {
    // Check all the different nodes around its point
    var exploringNodeCoordinates = Coordinates(exploringNodeIndex);

    // Checking if the exploration has changed the ordering

    for (var xDiff = -1; xDiff <= 1; xDiff++)
    {
      for (var yDiff = -1; yDiff <= 1; yDiff++)
      {
        // Skipping the case where your checking your own node
        if (xDiff == 0 && yDiff == 0) continue;

        int[] checkingNodeCoor =
        {
          exploringNodeCoordinates[0] + xDiff,
          exploringNodeCoordinates[1] + yDiff
        };

        // Making sure the node is on the grid
        if (!isOnGrid(checkingNodeCoor)) continue;

        var checkingNodeIndex = Index(checkingNodeCoor);

        // Seeing if the node has been created before
        if (grid[checkingNodeIndex] == null)
        {
          // If so, creates a new node and set the parent node to this one
          grid[checkingNodeIndex] = new Node(NodeVector(checkingNodeCoor),
            checkingNodeIndex,
            endNodeVector,
            grid[exploringNodeIndex]);

          // We also add it to the list of found nodes
          foundNodes.InsertElementAscending(grid[checkingNodeIndex]);
        }

        // If the object is a obstacle we skip it
        else if (grid[checkingNodeIndex].IsObstacle()) { }

        else
        {
          // If not, update the grid position to find the best position
          var hasUpdated = grid[checkingNodeIndex]
            .CheckParentNode(grid[exploringNodeIndex]);

          // If the position was updated, remove the position and add an updated
          // one
          if (hasUpdated)
          {
            foundNodes.Remove(grid[checkingNodeIndex]);
            foundNodes.InsertElementAscending(grid[checkingNodeIndex]);
          }
        }
      }
    }

    // Assign the node to be explored
    grid[exploringNodeIndex].Explore();

    // Removes the node from the set of found nodes
    foundNodes.Remove(grid[exploringNodeIndex]);
  }

  /// <summary>
  ///   Checks if a set of coordinates are in the grid
  /// </summary>
  public bool isOnGrid(int[] coordinates)
  {
    return coordinates[0] >= 0 && coordinates[0] < length &&
           coordinates[1] >= 0 && coordinates[1] < height;
  }

  /// <summary>
  ///   Creates a start node, end node and executes the A* algorithm to find the
  ///   closest position
  /// </summary>
  private List<Node> FindRoute(int[] startNodePosition,
    int[] endNodePosition)
  {
    // Find the end node position
    Vector2 endNodeVector = NodeVector(endNodePosition);


    // Checking if the start position is on an obstacle
    if (grid[Index(startNodePosition)] != null) return null;

    // Create the start node
    grid[Index(startNodePosition)] = new Node(NodeVector(startNodePosition),
      Index(startNodePosition),
      endNodeVector, null);

    // Checking if the end position is equal to the start position or is an
    // obstacle
    if (grid[Index(endNodePosition)] != null) return null;

    // Checking if there is a line of sight between the end and start node
    if (LineOfSight(NodeVector(startNodePosition), NodeVector(endNodePosition)))
      return new List<Node>
      {
        new Node(NodeVector(endNodePosition),
          Index(endNodePosition),
          NodeVector(endNodePosition), null)
      };

    // Explore the start node
    Explore(Index(startNodePosition), endNodeVector);

    float minimumHCost;
    int minimumFCostIndex;

    // Looping through untill the program has found the end node
    do
    {
      // Find the minimum node for the f cost
      minimumFCostIndex = foundNodes[0].GetIndex();

      // Gets the minimum hcost
      minimumHCost = grid[minimumFCostIndex].HCost();

      // Explores the node with the smallest f cost
      Explore(minimumFCostIndex, endNodeVector);
    } while (Math.Abs(minimumHCost) > 0.0001);

    // Gets the Lineage and returns it as an array
    var fastestRoute = grid[minimumFCostIndex].GetLineage();
    route = PruneRoute(fastestRoute);

    return PruneRoute(fastestRoute);
  }

  /// <summary>
  ///   Picks the closest start and end node and executes the A* algorithm to
  ///   find the closest position
  /// </summary>
  public List<Node> FindRoute(Vector2 startPosition, Vector2 endPosition)
  {
    return FindRoute(VectorCoordinates(startPosition),
      VectorCoordinates(endPosition));
  }

  /// <summary>
  ///   Prunes the route to only have points where the direction change
  /// </summary>
  private List<Node> PruneRoute(List<Node> fullRoute)
  {
    // The list of nodes that would be used
    var prunedRoute = new List<Node>();
    // Going through the whole list
    for (var nodeIndex = 0; nodeIndex < fullRoute.Count; nodeIndex++)
    {
      // Ignoring the beginning of the route
      if (nodeIndex == 0) continue;
      // Adding the point if the route is at the end
      if (nodeIndex == fullRoute.Count - 1)
      {
        prunedRoute.Add(fullRoute[nodeIndex]);
        continue;
      }

      // Checking the direction of the previous node to itself
      Vector2 previousDirection = fullRoute[nodeIndex].GetPosition() -
                                  fullRoute[nodeIndex - 1].GetPosition();
      previousDirection.Normalize();

      // Checking the direction of the future node
      Vector2 nextDirection = fullRoute[nodeIndex + 1].GetPosition() -
                              fullRoute[nodeIndex].GetPosition();
      nextDirection.Normalize();

      // If the two directions are different, add the node to the route
      if (previousDirection != nextDirection) prunedRoute.Add(fullRoute[nodeIndex]);
    }

    // Looping through all the differently angled routes
    for (var routeIndex = 1; routeIndex < prunedRoute.Count; routeIndex++)
    {
      // Looping through all the routes previous to it
      for (var oldRouteIndex = -1; oldRouteIndex < routeIndex; oldRouteIndex++)
      {
        // Gets the starting node if we start from -1
        Node oldNode = oldRouteIndex == -1
          ? fullRoute[0]
          : prunedRoute[oldRouteIndex];

        // Checks if there is a line of sight between this node and the old node
        if (LineOfSight(prunedRoute[routeIndex], oldNode))
        {
          // Deletes all the nodes between this node and the previous nodes
          var numberOfDeletions = routeIndex - 1 - oldRouteIndex;

          while (numberOfDeletions != 0)
          {
            prunedRoute.RemoveAt(oldRouteIndex + 1);
            numberOfDeletions--;
            routeIndex--;
          }

          break;
        }
      }
    }

    return prunedRoute;
  }

  /// <summary>
  ///   Sees if there is free motion from the start node to the end node without
  ///   hitting any blocked nodes
  /// </summary>
  public bool LineOfSight(Vector2 startPosition, Vector2 endPosition)
  {
    // Gets the positions of the start and end node on the grid
    var startCoor = VectorCoordinates(startPosition);
    var endCoor = VectorCoordinates(endPosition);

    // Converting the points to int vectors
    Vector2Int startVector = new Vector2Int(startCoor[0], startCoor[1]);
    Vector2Int endVector = new Vector2Int(endCoor[0], endCoor[1]);

    // Finding the direction
    Vector2 dir = endVector - startVector;
    dir.Normalize();

    // The current node we are checking
    Vector2 currentVector = startVector;

    // Looping until we find the end node
    while (Vector2Int.RoundToInt(currentVector) != endVector)
    {
      // Adds a dir to the currentVector
      currentVector += dir;

      // Finds the ceil and floor of that position
      Vector2Int roundVector = Vector2Int.RoundToInt(currentVector);

      // Looping through the values around the point
      for (var xDiff = -1; xDiff <= 1; xDiff++)
      {
        for (var yDiff = -1; yDiff <= 1; yDiff++)
        {
          var roundIndex = Index(new[]
          {
            roundVector.x + xDiff,
            roundVector.y + yDiff
          });

          // Checks if that point is a obstacle
          if (grid[roundIndex] != null && grid[roundIndex].IsObstacle()) return false;
        }
      }
    }

    return true;
  }

  // Using the line of sight tool for points
  public bool LineOfSight(Node startNode, Node endNode)
  {
    return LineOfSight(startNode.GetPosition(), endNode.GetPosition());
  }

  // Setting the position of the grid
  public void SetGridPosiiton(Vector2 pos) { position = pos; }

  // Getting the scale of the grid
  public float GetScale() { return scale; }

  // A draw function to draw out all the different nodes

  public void OnDrawGizmosSelected()
  {
    // Looping through all the different node positions
    for (var nodeIndex = 0; nodeIndex < grid.Length; nodeIndex++)
    {
      // Find the node position
      Vector2 nodeVector = NodeVector(Coordinates(nodeIndex));

      // Checks if the node is there or not
      if (grid[nodeIndex] == null) continue;

      // Checks if the node is in the route

      if (route.Contains(grid[nodeIndex]))
        Gizmos.color = Color.magenta;
      else if (grid[nodeIndex].HasExplored())
        Gizmos.color = Color.red;
      else if (grid[nodeIndex].IsObstacle())
        Gizmos.color = Color.gray;
      else
        Gizmos.color = Color.green;
      // Draw a sphere at the transforms position
      Gizmos.DrawWireCube(nodeVector, Vector3.one * 0.05f);
    }
  }
}