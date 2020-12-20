using Managers;
using UnityEngine.EventSystems;

namespace Base_Classes
{
  /// <summary>
  ///     Gives the ability for information about the object to be shown to the HUD
  /// </summary>
  public interface IBoxInformation : IPointerEnterHandler, IPointerExitHandler,
        IInformation
    {
        string Description { get; }
        Hud.DisplayDelegate Delegate { get; set; }
    }
}