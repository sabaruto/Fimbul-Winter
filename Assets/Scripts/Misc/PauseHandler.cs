using System.Collections.Generic;
using UnityEngine;

namespace Misc
{
  /// <summary>
  ///   This class holds all the different pauses and only starts the game once all
  ///   the pauses have been emptied
  /// </summary>
  public static class PauseHandler
  {
    // The list of classes pausing the game
    private static readonly List<string> PauseClaims = new List<string>();

    // Adds a pause to the list of pause claims
    public static void Pause(string name)
    {
      Time.timeScale = 0;
      PauseClaims.Add(name);
    }

    // Removes a pause claim and tries to unpause the game
    public static void RemovePause(string name)
    {
      PauseClaims.Remove(name);

      if (PauseClaims.Count == 0) Time.timeScale = 1;
    }

    public static bool IsPaused() { return PauseClaims.Count > 0; }
  }
}