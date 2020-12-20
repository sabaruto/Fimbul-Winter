namespace Base_Classes
{
    public class Ability
    {
        // The trigger format for the abilities
        public delegate void StartDelegate();

        // The cast time of the ability
        private readonly float castResetTime;

        //The final maximum cooldown variable and the one that will keep track of the count down
        private readonly float maxCoolDown;
        private readonly string name;
        private readonly int stamina;

        // The wind down time needed before you can use the ability again
        private readonly float windDownResetTime;

        // The boolean to check if the ability is casting
        private bool castingAbility;

        private float castTimer;
        private float coolDown;

        // The current character using this ability
        private Character currentCharacter;
        private int damage;

        // The boolean to check if the ability is finishing
        private bool isFinishing;
        private float windDownTime;

        public Ability(string theName, int theDamage, float theMaxCoolDown,
            int requiredStamina, float castTime = 1, StartDelegate startAbility = null, float windDownResetTime = 0)
        {
            name = theName;
            damage = theDamage;
            maxCoolDown = theMaxCoolDown;
            stamina = requiredStamina;
            castTimer = castTime;
            castResetTime = castTime;
            this.windDownResetTime = windDownResetTime;
            windDownTime = windDownResetTime;
            PlayAbility += startAbility;
        } //Ability constructor

        // The specific event of this delegate
        private event StartDelegate PlayAbility;

        //Method to set the Cool Down of the ability
        public void SetCoolDown()
        {
            coolDown = maxCoolDown;
        } //setCoolDown

        //Method to get the amount of mana you need in order to use the ability
        public int GetStamina()
        {
            return stamina;
        }


        //Method to get the current Cool Down value
        public float GetCoolDown()
        {
            return coolDown;
        }

        //Method to check if the coolDown is 0 and if the character has enough 
        //stamina so that the ability is ready to be used again
        //This method also reduces the stamina of the character if the ability can be used
        public bool IsAbilityReady(Character character)
        {
            if (coolDown == 0 && character.GetStamina() > stamina &&
                !character.IsStunned())
                return true;
            return false;
        } //isAbilityReady

        public bool StartAbility(Character character)
        {
            // Checks if the ability can be cast
            if (!character.GetCasting())
            {
                castingAbility = IsAbilityReady(character);

                if (castingAbility)
                {
                    character.SetCasting(true);
                    return true;
                }
            }

            return false;
        }

        // Starts the wind down timer
        public void Complete(Character character)
        {
            isFinishing = true;
            currentCharacter = character;
        }

        // Finishes the ability instantly
        public void Terminate()
        {
            isFinishing = false;
            if (currentCharacter != null) currentCharacter.SetCasting(false);
            SetCoolDown();
        }


        //Method to decrease the Cool Down of the Ability
        public void DecreaseCoolDown(float deltaTime)
        {
            coolDown -= deltaTime;
            if (coolDown < 0) coolDown = 0;

            // If the ability has started casting, it will decrease the casting timer
            // also. This also starts the ability when the timer hits zero
            if (castingAbility)
            {
                castTimer -= deltaTime;

                if (castTimer <= 0)
                {
                    castTimer = castResetTime;
                    PlayAbility?.Invoke();
                    castingAbility = false;
                }
            }

            // Starts reducing the timing for the ability wind down and calls the finish
            // clause when the timer hits zero
            if (isFinishing)
            {
                windDownTime -= deltaTime;

                if (windDownTime <= 0)
                {
                    windDownTime = windDownResetTime;
                    Terminate();
                }
            }
        } //decreaseCoolDown

        //Method that returns the value of damage of the ability
        public int GetDamage()
        {
            return damage;
        }

        //Method to increase or decrease the damage of an ability
        public void AddDamage(int plusDamage)
        {
            damage += plusDamage;
        }

        //Method to get the name of the ability
        public string GetName()
        {
            return name;
        }

        // Getting the time needed for the ability to cool down
        public float GetWindDownTimer()
        {
            return windDownResetTime;
        }

        // Checking if the ability has finished its cooldown period
        public bool IsFinishing()
        {
            return isFinishing;
        }

        public override string ToString()
        {
            return "name: " + name + " damage: " + damage + " cooldown: " + maxCoolDown;
        } //ToString
    }
} //Ability