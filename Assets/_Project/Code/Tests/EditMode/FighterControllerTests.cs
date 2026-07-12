using BB.Data;
using BB.Simulation;
using NUnit.Framework;
using UnityEngine;

namespace BB.Tests
{
    public class FighterControllerTests
    {
        GameObject _go;
        FighterController _fighter;

        [SetUp]
        public void SetUp()
        {
            var jab = ScriptableObject.CreateInstance<AttackDefinition>();
            jab.id = "test_jab";
            jab.input = AttackInput.Jab;
            jab.startupTicks = 4;
            jab.activeTicks = 3;
            jab.recoveryTicks = 8;
            jab.hitboxes = new[] { new HitboxWindow { startTick = 4, endTick = 7, radius = 0.4f, damage = 5f, angleDegrees = 45f, baseKnockback = 20f, knockbackGrowth = 80f } };

            var def = ScriptableObject.CreateInstance<FighterDefinition>();
            def.id = "test";
            def.attacks = new[] { jab };

            _go = new GameObject("TestFighter");
            _fighter = _go.AddComponent<FighterController>();
            _fighter.groundMask = 0; // no stage geometry in tests
            _fighter.Init(def, 0, Vector2.zero, 3);
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(_go);

        static InputSnapshot Press(FighterButtons b) => new() { held = b };

        [Test]
        public void AttackPress_EntersAndHoldsAttackState()
        {
            _fighter.Tick(default); // establish previousInput with nothing held
            _fighter.Tick(Press(FighterButtons.Attack));

            Assert.AreEqual(FighterStateId.Attack, _fighter.State.stateId,
                "attack state must survive the tick it was started on");

            // Attack persists through its full duration even with no input held.
            for (int i = 0; i < 5; i++) _fighter.Tick(default);
            Assert.AreEqual(FighterStateId.Attack, _fighter.State.stateId);
        }

        [Test]
        public void Attack_EndsAfterTotalTicks()
        {
            _fighter.Tick(default);
            _fighter.Tick(Press(FighterButtons.Attack));
            for (int i = 0; i < 20; i++) _fighter.Tick(default);
            Assert.AreNotEqual(FighterStateId.Attack, _fighter.State.stateId);
            Assert.AreEqual(-1, _fighter.State.attackIndex);
        }

        [Test]
        public void AttackActivationId_IncrementsPerAttack()
        {
            ushort before = _fighter.State.attackActivationId;
            _fighter.Tick(default);
            _fighter.Tick(Press(FighterButtons.Attack));
            Assert.AreEqual(before + 1, _fighter.State.attackActivationId);
        }

        [Test]
        public void ApplyHit_LaunchesAndAccumulatesPercent()
        {
            var hit = new HitboxWindow { damage = 10f, angleDegrees = 45f, baseKnockback = 30f, knockbackGrowth = 100f, hitstopTicks = 3 };
            _fighter.ApplyHit(in hit, 1);

            Assert.AreEqual(10f, _fighter.State.percent);
            Assert.AreEqual(FighterStateId.Launched, _fighter.State.stateId);
            Assert.Greater(_fighter.State.velocity.x, 0f);
            Assert.Greater(_fighter.State.velocity.y, 0f);
        }

        [Test]
        public void HandleKO_DecrementsStockAndRespawnsIntangible()
        {
            _fighter.HandleKO(new Vector2(0f, 6f));
            Assert.AreEqual(2, _fighter.State.stocks);
            Assert.AreEqual(FighterStateId.Respawning, _fighter.State.stateId);
            Assert.Greater(_fighter.State.intangibleTicks, 0);
            Assert.AreEqual(0f, _fighter.State.percent);
        }
    }
}
