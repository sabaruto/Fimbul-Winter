using System.Collections.Generic;
using System.Linq;
using Base_Classes;
using Enums;
using Managers;
using Misc;
using Statuses;
using UnityEngine;
using UnityEngine.Serialization;

namespace Heroes
{
    public class Player : Character, IInformation
    {
        // The ability delegate
        public delegate void AbilityDelegate(Ability ability);

        // The trigger format for the abilities

        // The time period for the poison buff
        private const float PoisonUpTime = 3;
        private static readonly int Stunned = Animator.StringToHash("isStunned");
        private static readonly int Volley = Animator.StringToHash("Volley");
        private static readonly int Property = Animator.StringToHash("Piercing Arrow");

        // The piercing arrow
        [FormerlySerializedAs("piercingArrow")] [SerializeField]
        private GameObject piercingArrowObject;

        // The game object for the piercing arrow attack
        [SerializeField] private GameObject piercingArrowVisual;


        [SerializeField] private GameObject playerProjectile;

        // The sprite for the player
        [SerializeField] private Sprite sprite;

        // The sprite renderer for the stun sprite
        [SerializeField] private GameObject stunSprite;

        // The gameObject for the volley attack
        [SerializeField] private GameObject volleyVisual;

        private readonly float extendedVolleyResetTime = 4f;

        // The size for the volley
        private readonly float volleyRadius = 2f;

        private readonly float volleyResetTime = 2f;

        // The current helper the palyer is moving to
        private Helper currentHelper;

        // The direction for the dash
        private Vector2 dashDir;

        // The game master
        private GameMaster gameMaster;
        private string holder = "";

        // Checking if the player is in conversation
        private bool isBusy;

        // Main Camera
        private Camera mainCamera;

        // The direction for the piecing arrow
        private Vector2 piercingDir;

        // The skill points for the player
        private int skillPoints;

        // The statuses for the player's skills
        private Dictionary<string, SkillState> skillStatus =
            new Dictionary<string, SkillState>
            {
                {"Volley", SkillState.Default},
                {"Piercing Arrow", SkillState.Default},
                {"Poison", SkillState.Default}
            };

        // The timer for the stamina
        private float staminaTimer;

        // The position of the volley
        private Vector2 volleyPosition;

        // The timer for the volley 
        private float volleyTimer = Mathf.Infinity;


        // Changing the values for the health
        public new void Awake()
        {
            base.Awake();

            // Gets the game master
            gameMaster = FindObjectOfType<GameMaster>();

            mainCamera = Camera.main;
            Debug.Assert(mainCamera != null);

            // Abilities
            var volley = new Ability("Volley", 1, 5, 20, 0.5f, StartVolley, 0.25f);
            var piercingArrow = new Ability("Piercing Arrow", 5, 1, 5, 10f / 24f,
                PiercingArrow, 0.125f);
            var poison = new Ability("Poison", 0, 5f, 10, 0, Poison);
            var dash = new Ability("Dash", 0, 2, 0, 0, Dash);

            abilityList = new[] {dash, volley, poison, piercingArrow};
            SetVariables("Player", 100, 100, 8, true);
            projectile = playerProjectile;
        }

        public new void Update()
        {
            base.Update();

            if (!isBusy && IsAlive() && !isCasting)
            {
                CheckAbilities();
                Movement();
            }

            CheckVolley();

            // Slowly increases the stamina of the player
            CheckStamina();

            // Checks if the player is stunned and tells the animator if so
            CheckStunned();
        }

        public void LateUpdate()
        {
            ShowGui();
        }

        public void OnDrawGizmos()
        {
            // Checking if the volley is running
            if (volleyTimer < Mathf.Infinity) Gizmos.DrawWireSphere(volleyPosition, volleyRadius);
        }

        // setting the name and sprite for the player
        public string Name => name;
        public Sprite Sprite => sprite;

        // Checking when the specific delegate is used
        public event AbilityDelegate AbilitiesDelegate;

        protected override void OnDeath()
        {
            base.OnDeath();
            FindObjectOfType<AudioManager>().ChangeTheme("Death");
        }

        private void CheckStamina()
        {
            // Checks if the second has past
            if ((int) (3 * staminaTimer) != (int) (3 * (staminaTimer - Time.deltaTime)))
            {
                IncreaseStamina(1);
                staminaTimer = 1;
            }

            staminaTimer -= Time.deltaTime;
        }

        private void CheckStunned()
        {
            animator.SetBool(Stunned, isStunned);
            stunSprite.SetActive(isStunned);
        }

        private void Movement()
        {
            var grid = movement.GetGrid();

            if (grid == null) return;

            // Finding the position of the mouse
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            // Find the corresponding index for the vector
            var mouseIndex = grid.VectorCoordinates(mousePosition);

            // If the player has a current helper, it would start moving to that helper
            if (currentHelper != null)
            {
                // Getting the player stand position
                var standPosition = currentHelper.GetStandPosition();

                // Checks if the player is close enough to the stand position
                if (Vector2.Distance(transform.position, standPosition) > 0.5f)
                {
                    movement.StartMoving(currentHelper.GetStandPosition());
                }
                else if (!isBusy)
                {
                    currentHelper.ReadyToTalk();
                    isBusy = true;
                }
            }

            // Check if the mouse is pressed
            if (Input.GetMouseButton(1))
            {
                // Clears the current target and helper for the player
                currentTarget = null;
                currentHelper = null;

                // Clears the holder
                holder = "";
                ClearDraw();

                // Checking if the mouse is within the confines of the map
                if (grid.isOnGrid(mouseIndex))
                    // Starts the A* algorithm for the given position
                    movement.StartMoving(mousePosition);
            }

            // Checks if the mouse has been pressed to find an enemy only when the
            // player isn't casting
            if (Input.GetMouseButtonDown(0) && !isCasting && !PauseHandler.IsPaused())
            {
                // Checks if there is an enemy around that point
                currentTarget = gameMaster.CheckPerson<Character>(mousePosition);
                if (currentTarget == null || !currentTarget.IsAlive()) currentTarget = null;

                // Checks if there is a helper around the point where you click
                currentHelper = gameMaster.CheckPerson<Helper>(mousePosition);
            }
        }

        protected override void Die()
        {
            base.Die();
            LoadLevelManager.LoadLevel("DemoScene");
            alive = true;
        }

        public void FinishConversation()
        {
            currentHelper = null;
            isBusy = false;
        }

        // Setting the skill statuses
        public void SetSkillStatus(Dictionary<string, SkillState> statuses)
        {
            skillStatus = statuses;
        }

        public Dictionary<string, SkillState> GetSkillStatus()
        {
            return skillStatus;
        }

        private void CheckAbilities()
        {
            var abilityName = "";
            // Hold the appropriate ability

            if (Input.GetKeyDown("q")) abilityName = "Volley";
            if (Input.GetKeyDown("w")) abilityName = "Piercing Arrow";
            if (Input.GetKeyDown("e")) abilityName = "Poison";
            if (Input.GetKeyDown("r")) abilityName = "Dash";

            if (abilityName != "")
            {
                DrawAbility(abilityName);
                holder = abilityName;
            }

            // choose the appropriate ability from the holder
            if (Input.GetMouseButton(0) && holder != ""
                                        && GetAbility(holder).StartAbility(this))
            {
                // Getting the position of the mouse
                var mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                switch (holder)
                {
                    case "Volley":
                        // Set the position of the mouse
                        volleyPosition = mousePos;

                        // Start the animation for the volley
                        animator.SetBool(Volley, true);

                        // Play the sound for the volley
                        sound.Play("Volley");
                        break;
                    case "Piercing Arrow":
                        piercingDir = mousePos;

                        // Start the animation for piercing arrow
                        animator.SetBool(Property, true);
                        break;
                    case "Dash":
                        dashDir = mousePos - transform.position;
                        break;
                }

                // Directs the player to the camera if the ability used isn't the
                // poison ability
                if (holder != "Poison") movement.UpdateDir(mousePos);

                ReduceStamina(GetAbility(holder).GetStamina());
                holder = "";
                ClearDraw();
            }
        }

        // Begins the timer for the volley
        private void StartVolley()
        {
            volleyTimer = skillStatus["Volley"] == SkillState.Left ? extendedVolleyResetTime : volleyResetTime;
            FinishAbility("Volley");
        }

        private void CheckVolley()
        {
            // Stops if the volley timer is finished 
            if (volleyTimer > (skillStatus["Volley"] == SkillState.Left ? extendedVolleyResetTime : volleyResetTime))
                return;

            var colliders = new Collider2D[50];
            // Finds all the enemies in the area
            Physics2D.OverlapCircleNonAlloc(volleyPosition, volleyRadius, colliders);
            colliders = colliders.Where(x => x != null).ToArray();

            // Looping through all the enemies
            foreach (var c in colliders)
            {
                // Checking if it has a enemy component
                var enemy = c.GetComponent<Enemy>();

                if (enemy != null && (int) (5 * (volleyTimer - Time.deltaTime)) !=
                    (int) (5 * volleyTimer))
                {
                    // Deals damage to the enemy every second
                    enemy.TakeDamage(GetAbility("Volley").GetDamage());

                    // Add deals poison if the poison skill was chosen
                    switch (skillStatus["Volley"])
                    {
                        case SkillState.Right:
                            enemy.AddStatus(new Root(1));
                            break;
                    }
                }
            }

            // Decreases the volley timers
            volleyTimer -= Time.deltaTime;

            // Checks if the volley timer has ended
            if (volleyTimer < 0) volleyTimer = Mathf.Infinity;
        }

        private void PiercingArrow()
        {
            // Creates the arrow game object
            var arrow = Instantiate(piercingArrowObject, transform.position,
                Quaternion.identity);

            // Getting the projectile component
            var arrowProj = arrow.GetComponent<Projectile>();

            // Play the sound of the piercing arrow
            sound.Play("Piercing");

            // Add a status if some of the skills were chosen
            switch (skillStatus["Piercing Arrow"])
            {
                case SkillState.Left:
                    arrowProj.SetStatuses(new Status[] {new Bleed(2, 2)});
                    break;
                case SkillState.Right:
                    arrowProj.SetStatuses(new Status[] {new Freeze(2)});
                    break;
            }

            arrowProj.IsTargetPlayer(false);
            arrowProj.Throw(piercingDir);


            FinishAbility("Piercing Arrow");
        }

        private void Poison()
        {
            // Increase the damage if the associated skill was chosen
            var damageIncrease = skillStatus["Poison"] == SkillState.Right ? 5 : 0;

            // Increasing the attack speed multiplier
            float attackSpeedIncrease = skillStatus["Poison"] == SkillState.Left ? 5 : 3;

            AddStatus(new Poisoner(PoisonUpTime, damageIncrease, attackSpeedIncrease));
            FinishAbility("Poison");
        }

        private void Dash()
        {
            AddStatus(new Dash(0.1f, dashDir, 30));
            FinishAbility("Dash");
        }

        // Finishes the ability
        private void FinishAbility(string abilityName)
        {
            GetAbility(abilityName).Complete(this);
            AbilitiesDelegate?.Invoke(GetAbility(abilityName));
        }

        // Getting the value of the skill points
        public int GetSkillPoints()
        {
            return skillPoints;
        }

        // Increasing the value of the skill points
        public void IncreaseSkillPoints(int value)
        {
            skillPoints += value;
        }

        // Setting the skill points
        public void SetSkillPoints(int points)
        {
            skillPoints = points;
        }

        // Trying to pay for a skill using skill points
        public bool BuySkill(int price)
        {
            var hasEnoughMoney = price <= skillPoints;
            if (hasEnoughMoney) skillPoints -= price;
            return hasEnoughMoney;
        }

        // Setting the health and stamina
        public void SetHealth(int health)
        {
            this.health = health;
        }

        public void SetStamina(int stamina)
        {
            this.stamina = stamina;
        }

        /// <summary>
        ///     Clears all the attack shape drawings
        /// </summary>
        private void ClearDraw()
        {
            volleyVisual.SetActive(false);
            piercingArrowVisual.SetActive(false);
        }

        /// <summary>
        ///     Draws one of the abilities and removes all the other ones
        /// </summary>
        private void DrawAbility(string abilityName)
        {
            // Clears all the abilities
            ClearDraw();

            // Chooses the specific ability to be drawn
            switch (abilityName)
            {
                case "Volley":
                    // Set the volley visual as active
                    volleyVisual.SetActive(true);
                    break;
                case "Piercing Arrow":
                    piercingArrowVisual.SetActive(true);
                    break;
            }
        }

        private void ShowGui()
        {
            if (volleyVisual.activeSelf)
                // Move the object to the mouse of the game object
                volleyVisual.transform.position = (Vector2) mainCamera
                    .ScreenToWorldPoint(Input.mousePosition);

            if (piercingArrowVisual.activeSelf)
                // Direct the object to the mouse position
                piercingArrowVisual.transform.up = (Vector2) (mainCamera.ScreenToWorldPoint(Input.mousePosition)
                                                              - transform.position);

            // Stops the animation for each ability once its complete
            if (!isCasting)
            {
                if (animator.GetBool(Volley)) animator.SetBool(Volley, false);

                if (animator.GetBool(Property)) animator.SetBool(Property, false);
            }
        }
    }
}