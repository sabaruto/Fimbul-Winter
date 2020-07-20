/// <Summary>
/// This class changes the level only when it is safe to do so, i.e. all the
/// enemies in the scene are killed nothing happens if not
/// </summary>

using Heroes;
using Managers;

namespace Triggers
{
  public class SafeLevelTrigger : LevelTrigger
  {
    protected override void TryMoveScene(Player player)
    {
      // Finds the GameMaster object
      GameMaster gameMaster = FindObjectOfType<GameMaster>();

      // Checks whether all the enemies in the game are dead
      if (gameMaster.EnemiesDead()) gameMaster.MoveScene(levelRef);
    }
  }
}