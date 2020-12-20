using NUnit.Framework;
using Base_Classes;

namespace Tests.EditMode.Base_Classes
{
    [TestFixture("Test Name", 10, 10, 10, 3, null, 1)]
    [TestFixture("Test", 1, 3, 5, 3, null, 1)]
    public class AbilityTests
    {
        private readonly string name;
        private readonly int damage;
        private readonly float maxCooldown;
        private readonly int stamina;
        private readonly float castTime;
        private readonly Ability.StartDelegate startAbility;
        private readonly float windDownResetTime;

        private Ability testAbility;

        public AbilityTests(string name, int damage, float maxCooldown, int stamina,
            float castTime, Ability.StartDelegate startAbility, float windDownResetTime)
        {
            this.name = name;
            this.damage = damage;
            this.maxCooldown = maxCooldown;
            this.stamina = stamina;
            this.castTime = castTime;
            this.startAbility = startAbility;
            this.windDownResetTime = windDownResetTime;
        }

        [SetUp]
        public void Setup()
        {
            testAbility = new Ability(name, damage, maxCooldown, stamina, castTime, startAbility, windDownResetTime);
        }

        [Test]
        public void CreateAbilityTest()
        {
            Assert.That(testAbility != null);
        }

        [Test]
        public void GetAbilityAttributesTest()
        {
            Assert.AreEqual(name, testAbility.GetName());
            Assert.AreEqual(damage, testAbility.GetDamage());
            Assert.AreEqual(0, testAbility.GetCoolDown());
            Assert.AreEqual(stamina, testAbility.GetStamina());
            Assert.AreEqual(windDownResetTime, testAbility.GetWindDownTimer());
        }

        [Test]
        public void SetCooldownTests()
        {
            Assert.AreEqual(0, testAbility.GetCoolDown());
            testAbility.SetCoolDown();
            Assert.AreEqual(maxCooldown, testAbility.GetCoolDown());
        }
    }
}