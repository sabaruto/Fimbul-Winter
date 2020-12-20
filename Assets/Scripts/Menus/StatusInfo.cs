using Base_Classes;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using Debug = System.Diagnostics.Debug;

namespace Menus
{
    public class StatusInfo : MonoBehaviour, IBoxInformation
    {
        // The information about the status
        public Status Status { get; set; }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Assert(DisplayInfo != null, nameof(DisplayInfo) + " != null");
            DisplayInfo(this, true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Assert(DisplayInfo != null, nameof(DisplayInfo) + " != null");
            DisplayInfo(this, false);
        }

        // Giving accessors for the information about the status
        public string Name => Status.GetName();
        public Sprite Sprite { get; set; }
        public string Description => Status.GetDescription();

        public Hud.DisplayDelegate Delegate
        {
            get => DisplayInfo;
            set => DisplayInfo = value;
        }

        private event Hud.DisplayDelegate DisplayInfo;
    }
}