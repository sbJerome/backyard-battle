using BB.Data;
using BB.Simulation;
using NUnit.Framework;
using UnityEngine;

namespace BB.Tests
{
    public class SimpleBotBrainTests
    {
        GameObject _a, _b;
        MatchSimulation _sim;
        SimpleBotBrain _brain;

        [SetUp]
        public void SetUp()
        {
            var def = ScriptableObject.CreateInstance<FighterDefinition>();
            def.attacks = new AttackDefinition[0];

            var stage = ScriptableObject.CreateInstance<StageDefinition>();
            _sim = new MatchSimulation(stage);

            _a = new GameObject("Bot");
            var botFighter = _a.AddComponent<FighterController>();
            botFighter.groundMask = 0;
            botFighter.Init(def, 0, new Vector2(0f, 0f), 3);

            _b = new GameObject("Target");
            var targetFighter = _b.AddComponent<FighterController>();
            targetFighter.groundMask = 0;
            targetFighter.Init(def, 1, new Vector2(5f, 0f), 3);

            _brain = new SimpleBotBrain();
            _sim.AddFighter(botFighter, _brain);
            _sim.AddFighter(targetFighter, new BotInputSource());
            _brain.Bind(botFighter, _sim);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_a);
            Object.DestroyImmediate(_b);
        }

        [Test]
        public void Unbound_ReturnsNeutral()
        {
            Assert.AreEqual(default(InputSnapshot), new SimpleBotBrain().ReadTick());
        }

        [Test]
        public void WalksTowardNearestOpponent()
        {
            var snap = _brain.ReadTick();
            Assert.AreEqual(1f, snap.moveX, "target is to the right — bot should walk right");
        }

        [Test]
        public void AttacksWhenInRange()
        {
            // Move the target next to the bot, then poll for an attack press.
            var target = _sim.Fighters[1];
            target.State.position = new Vector2(0.8f, 0f);

            bool attacked = false;
            for (int i = 0; i < 120 && !attacked; i++)
                attacked = _brain.ReadTick().IsHeld(FighterButtons.Attack);
            Assert.IsTrue(attacked, "bot should attack an adjacent opponent within 2 seconds");
        }
    }
}
