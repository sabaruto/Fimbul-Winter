using System;
using System.Collections.Generic;
using Enums;
using Heroes;

// This class holds all the relevant information about the game state to be
// saved and reloaded at any point
namespace Base_Classes
{
  [Serializable]
  public class Data
  {
    public readonly int day;

    // The data being used
    public readonly int health;
    public readonly string sceneRef;
    public readonly int skillPoints;
    public readonly int stamina;
    public Dictionary<string, SkillState> skillStatus;

    public Data(Player player, string sceneRef, int day)
    {
      health = player.GetHealth();
      stamina = player.GetStamina();
      skillPoints = player.GetSkillPoints();
      this.day = day;
      this.sceneRef = sceneRef;
      skillStatus = player.GetSkillStatus();
    }
  }
}