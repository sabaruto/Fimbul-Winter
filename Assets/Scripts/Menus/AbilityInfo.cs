﻿using Base_Classes;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using Debug = System.Diagnostics.Debug;

namespace Menus
{
    public class AbilityInfo : MonoBehaviour, IBoxInformation
    {
        // The information about the ability

        [SerializeField] [TextArea(3, 6)] private string description;

        [SerializeField] private Sprite sprite;

        public Ability Ability { get; set; }

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

        // Giving accessors for the infomation about the ability
        public string Name => Ability.GetName();
        public Sprite Sprite => sprite;
        public string Description => description;

        public Hud.DisplayDelegate Delegate
        {
            get => DisplayInfo;
            set => DisplayInfo = value;
        }

        private event Hud.DisplayDelegate DisplayInfo;
    }
}