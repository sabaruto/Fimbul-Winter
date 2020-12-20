using System.Collections.Generic;
using System.Globalization;
using Base_Classes;
using Heroes;
using Menus;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Managers
{
  /// <summary>
  ///     This class draws all the HUD for the player's attributes
  /// </summary>
  public class Hud : MonoBehaviour
    {
        // The delegate format to display the information
        public delegate void DisplayDelegate(IBoxInformation info, bool display);


        // The text component for each ability
        [SerializeField] private TextMeshProUGUI[] abilityText = new TextMeshProUGUI[4];

        [SerializeField] private Canvas canvas;

        // The image for the information box
        [SerializeField] private Image infoImage;

        // The text fields for the information box
        [SerializeField] private TextMeshProUGUI infoName, infoDesc;

        // The game object holding the information box
        [SerializeField] private GameObject informationBox;

        // The image component for the abilities
        [SerializeField] private Image q, w, e, r;

        // The pivot that moves the status UI position
        [FormerlySerializedAs("statusUIPivot")] [SerializeField]
        private RectTransform statusUiPivot;

        // The spacing between each UI element
        [FormerlySerializedAs("statusUISpacing")] [SerializeField]
        private float statusUiSpacing;

        // All the different ability information
        [SerializeField] private AbilityInfo volleyInfo, piercingInfo, poisonInfo, dashInfo;

        // The list of abilites to decrease
        private readonly List<Ability> coolDownAbilities = new List<Ability>();


        // The current visualisations of the statuses
        private readonly List<GameObject> statusUi = new List<GameObject>();

        // Getting the UI image
        private Image healthbar;

        // Finding the dimensions for the health and stamina bar
        private Vector2 healthDims;

        // The list of abilities in hunter stance
        private Ability[] hunterAbilities;

        // Holding the starting values for the health and stamina
        private int initHealth, initStamina;

        // The player objectt
        private Player player;

        private Image staminaBar;
        private Vector2 staminaDims;

        public void Start()
        {
            // Getting the current player from the scene
            player = FindObjectOfType<Player>();
            var images = FindObjectsOfType<Image>();

            foreach (var image in images)
                if (image.name == "Health")
                    healthbar = image;
                else if (image.name == "Stamina") staminaBar = image;

            initHealth = player.GetMaxHealth();
            initStamina = player.GetMaxStamina();

            // Add all the abilities for each stance to the array of abilites
            hunterAbilities = new Ability[4];
            hunterAbilities[0] = volleyInfo.Ability = player.GetAbility("Volley");
            hunterAbilities[1] = piercingInfo.Ability
                = player.GetAbility("Piercing Arrow");
            hunterAbilities[2] = poisonInfo.Ability = player.GetAbility("Poison");
            hunterAbilities[3] = dashInfo.Ability = player.GetAbility("Dash");

            // Add the cooldwon method to the players ability delegate
            player.AbilitiesDelegate += AddCooldownAbilites;

            // Adding the display method to the ability information
            volleyInfo.Delegate += DisplayInfo;
            piercingInfo.Delegate += DisplayInfo;
            poisonInfo.Delegate += DisplayInfo;
            dashInfo.Delegate += DisplayInfo;

            // Setting the image for the player ability
            q.sprite = volleyInfo.Sprite;
            w.sprite = piercingInfo.Sprite;
            e.sprite = poisonInfo.Sprite;
            r.sprite = dashInfo.Sprite;

            healthDims = healthbar.GetComponent<RectTransform>().sizeDelta;
            staminaDims = staminaBar.GetComponent<RectTransform>().sizeDelta;
        }

        public void LateUpdate()
        {
            DrawHeathBar();
            DrawStatuses();
            ShowAbilityCooldown();
        }

        // Draws the health and stamina bar
        private void DrawHeathBar()
        {
            // Gets the health and stamina from the player
            var health = player.GetHealth();
            var stamina = player.GetStamina();

            // Getting the ratio of health and stamina lost
            var healthRatio = (float) health / initHealth;

            var staminaRatio = (float) stamina / initStamina;

            // Finds the dimensions of the health and stamina bar
            healthbar.GetComponent<RectTransform>().sizeDelta =
                new Vector2(healthDims.x * healthRatio,
                    healthDims.y);
            staminaBar.GetComponent<RectTransform>().sizeDelta =
                new Vector2(staminaDims.x * staminaRatio,
                    staminaDims.y);
        }

        // Draws all the different statuses the player is being affected by
        private void DrawStatuses()
        {
            // Getting the list of statuses
            var playerStatuses = player.GetStatuses().ToArray();

            // Looping through all the different statuses
            for (var statusIndex = 0;
                statusIndex < playerStatuses.Length;
                statusIndex++)
            {
                // Checks whether there is not a corresponding icon for the status
                if (statusUi.Count == statusIndex)
                {
                    // If there isn't create a new one
                    var statusGameObject = new GameObject("icon #" + statusIndex);
                    statusGameObject.AddComponent<Image>();

                    // Set the parent to the UI canvas
                    statusGameObject.transform.SetParent(canvas.transform, false);

                    // Move the object to the corresponding place
                    var orientation = statusGameObject.GetComponent<RectTransform>();

                    var size = statusUiPivot.sizeDelta.x;
                    orientation.pivot = new Vector2(0, 1);
                    orientation.anchorMax = orientation.anchorMin = new Vector2(0, 1);
                    orientation.anchoredPosition = statusUiPivot.anchoredPosition
                                                   + new Vector2((statusUiSpacing + size)
                                                                 * statusIndex, 0);
                    orientation.sizeDelta = new Vector2(size, size);

                    statusUi.Add(statusGameObject);
                }

                var statusIcon = statusUi[statusIndex].GetComponent<Image>();
                statusIcon.color = playerStatuses[statusIndex].GetColor();


                var statusInfo = statusUi[statusIndex].GetComponent<StatusInfo>();

                if (statusInfo == null) statusInfo = statusUi[statusIndex].AddComponent<StatusInfo>();

                statusInfo.Sprite = statusIcon.sprite;
                statusInfo.Status = playerStatuses[statusIndex];
                statusInfo.Delegate += DisplayInfo;
            }

            // Clear all the statuses that are not in use
            for (var emptyIconIndex = playerStatuses.Length;
                emptyIconIndex < statusUi.Count;
                emptyIconIndex++)
            {
                statusUi[emptyIconIndex].GetComponent<Image>().color = Color.clear;
                Destroy(statusUi[emptyIconIndex].GetComponent<StatusInfo>());
            }
        }

        // Shows the appropriate ability cooldown when needed
        private void ShowAbilityCooldown()
        {
            // Getting the appropriate ability array 
            var abilities = hunterAbilities;

            // Looping through all the abilities on show
            for (var abilityIndex = 0; abilityIndex < 4; abilityIndex++)
                // Checking if the ability has been used
                if (coolDownAbilities.Contains(abilities[abilityIndex]))
                {
                    // The current cooldown for the ability
                    var cooldown = abilities[abilityIndex].GetCoolDown();

                    // Checks if the player has been completed
                    if (cooldown <= 0)
                    {
                        abilityText[abilityIndex].text = "";

                        if (!abilities[abilityIndex].IsFinishing()) coolDownAbilities.Remove(abilities[abilityIndex]);
                    }
                    else

                        // If not, shows the ability to the correct text
                    {
                        abilityText[abilityIndex].text = cooldown > 1
                            ? ((int) cooldown + 1).ToString()
                            : ((float) (int) (cooldown * 10 + 1) / 10).ToString(CultureInfo.InvariantCulture);
                    }
                }

                // If not, clears the values on the screen if it hasn't
                else if (abilityText[abilityIndex].text != "")
                {
                    abilityText[abilityIndex].text = "";
                }
        }

        // This method draws the information about the ability or status
        private void DisplayInfo(IBoxInformation info, bool display)
        {
            informationBox.SetActive(display);

            if (display)
            {
                infoName.text = info.Name;
                infoImage.sprite = info.Sprite;
                infoDesc.text = info.Description;
            }
        }

        // Adds the ability to the list of abilites on cooldown
        private void AddCooldownAbilites(Ability ability)
        {
            coolDownAbilities.Add(ability);
        }
    }
}