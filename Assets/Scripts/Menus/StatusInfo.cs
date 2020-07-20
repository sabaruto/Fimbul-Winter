using Base_Classes;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Menus
{
  public class StatusInfo : MonoBehaviour, IBoxInformation
  {
    // The information about the status
    public Status Status { get; set; }

    public void OnPointerEnter(PointerEventData eventData) { DisplayInfo(this, true); }

    public void OnPointerExit(PointerEventData eventData) { DisplayInfo(this, false); }

    // Giving accessors for the infomation about the status
    public string Name => Status.GetName();
    public Sprite Sprite { get; set; }
    public string Description => Status.GetDescription();

    public Hud.DisplayDelegate Delegate { get => DisplayInfo; set => DisplayInfo = value; }

    private event Hud.DisplayDelegate DisplayInfo;
  }
}