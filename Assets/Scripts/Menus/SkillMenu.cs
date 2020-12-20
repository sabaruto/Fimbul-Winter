using Enums;
using Heroes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menus
{
    public class SkillMenu : MonoBehaviour
    {
        // The skill points text
        [SerializeField] private TextMeshProUGUI pointsText;

        // The Buttons of the skillTree
        [SerializeField] private Button[] volleyButtons,
            piercingArrowButtons,
            poisonButtons;

        // The skill menu 
        [SerializeField] private GameObject skillMenu;

        // The game master object
        private Player player;

        // The previous state of the skill menu
        private bool wasMenuActive;

        public void Start()
        {
            player = FindObjectOfType<Player>();

            // TEMPORARY
            player.IncreaseSkillPoints(1);

            UpdateGui();
        }

        public void Update()
        {
            // Updates the GUI once skill menu is openned
            if (wasMenuActive != skillMenu.activeSelf)
            {
                // Checks if the menu was switched on
                if (!wasMenuActive)
                    // Updates the GUI
                    UpdateGui();

                // Updates the previous active menu
                wasMenuActive = skillMenu.activeSelf;
            }
        }

        public void UpdateGui()
        {
            // Gets the skill status from the player
            var skillStatus = player.GetSkillStatus();

            // Holding all the buttons for the GUI
            var buttons = new[]
            {
                volleyButtons, piercingArrowButtons,
                poisonButtons
            };

            pointsText.text = "Skill Points: " + player.GetSkillPoints();

            for (var skillIndex = 0; skillIndex < buttons.Length; skillIndex++)
            {
                var skillName = "";

                // Checking which skill to check with
                switch (skillIndex)
                {
                    case 0:
                        skillName = "Volley";
                        break;
                    case 1:
                        skillName = "Piercing Arrow";
                        break;
                    case 2:
                        skillName = "Poison";
                        break;
                }

                // Gets the skill state
                var state = skillStatus[skillName];

                // Checks if the current skill is default
                for (var buttonIndex = 0; buttonIndex < 2; buttonIndex++)
                {
                    var currentButton = buttons[skillIndex][buttonIndex];
                    currentButton.interactable = state == SkillState.Default;

                    if (buttonIndex == 1 && state == SkillState.Left ||
                        buttonIndex == 0 && state == SkillState.Right)
                        currentButton.image.color = Color.red;
                }

                // If not, only 
            }
        }

        // Tries to upgrade one of the skills for the player
        private void UpgradeSkill(string skillName, int upgradeState)
        {
            var newState = SkillState.Default;

            switch (upgradeState)
            {
                case 0:
                    newState = SkillState.Left;
                    break;
                case 2:
                    newState = SkillState.Right;
                    break;
            }

            // Gets the skill status from the player
            var skillStatus = player.GetSkillStatus();

            // Look for the specific skill status
            var status = skillStatus[skillName];

            // Checks if the status is not upgradable
            if (status != SkillState.Default)
            {
                Debug.LogError("Cannot Upgrade ability");
                return;
            }

            // If not, upgrades the ability to the specific upgrade value if there is 
            // enough skill points
            if (!player.BuySkill(1))
            {
                Debug.Log("Do not have enough skill points. Only have " + player.GetSkillPoints() + " points left.");
                return;
            }

            skillStatus[skillName] = newState;

            Debug.Log("Upgraded: " + skillName + "To: " + upgradeState);
            UpdateGui();
        }

        public void UpgradeSkillLeft(string skillName)
        {
            UpgradeSkill(skillName, 0);
        }

        public void UpgradeSkillMiddle(string skillName)
        {
            UpgradeSkill(skillName, 1);
        }

        public void UpgradeSkillRight(string skillName)
        {
            UpgradeSkill(skillName, 2);
        }
    }
}