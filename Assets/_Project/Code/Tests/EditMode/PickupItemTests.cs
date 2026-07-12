using BB.Data;
using BB.Simulation;
using NUnit.Framework;
using UnityEngine;

namespace BB.Tests
{
    public class PickupItemTests
    {
        GameObject _holderGo, _victimGo, _pickupGo;
        FighterController _holder, _victim;
        MatchSimulation _sim;
        PickupItem _pickup;

        [SetUp]
        public void SetUp()
        {
            var def = ScriptableObject.CreateInstance<FighterDefinition>();
            def.attacks = new AttackDefinition[0];

            var stage = ScriptableObject.CreateInstance<StageDefinition>();
            _sim = new MatchSimulation(stage);

            _holderGo = new GameObject("Holder");
            _holder = _holderGo.AddComponent<FighterController>();
            _holder.groundMask = 0;
            _holder.Init(def, 0, Vector2.zero, 3);

            _victimGo = new GameObject("Victim");
            _victim = _victimGo.AddComponent<FighterController>();
            _victim.groundMask = 0;
            _victim.Init(def, 1, new Vector2(3f, 0f), 3);

            _sim.AddFighter(_holder, new BotInputSource());
            _sim.AddFighter(_victim, new BotInputSource());

            var pdef = ScriptableObject.CreateInstance<PickupDefinition>();
            _pickupGo = new GameObject("Acorn");
            _pickupGo.transform.position = new Vector3(0f, 0.5f, 0f); // on top of the holder
            _pickup = _pickupGo.AddComponent<PickupItem>();
            _pickup.definition = pdef;
            _sim.RegisterPickup(_pickup);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_holderGo);
            Object.DestroyImmediate(_victimGo);
            Object.DestroyImmediate(_pickupGo);
        }

        [Test]
        public void WalkOver_GrabsPickup()
        {
            _pickup.TickPickup();
            Assert.AreEqual(PickupItem.PickupState.Held, _pickup.CurrentState);
            Assert.AreSame(_pickup, _holder.HeldItem);
        }

        [Test]
        public void AttackWhileHolding_Throws()
        {
            _pickup.TickPickup(); // grab
            _holder.Tick(default);
            _holder.Tick(new InputSnapshot { held = FighterButtons.Attack });

            Assert.AreEqual(PickupItem.PickupState.Thrown, _pickup.CurrentState);
            Assert.IsNull(_holder.HeldItem);
            Assert.AreNotEqual(FighterStateId.Attack, _holder.State.stateId,
                "throw must replace the normal attack");
        }

        [Test]
        public void ThrownPickup_HitsAndRespawns()
        {
            _pickup.TickPickup(); // grab (holder faces +x, victim at +3)
            _holder.Tick(default);
            _holder.Tick(new InputSnapshot { held = FighterButtons.Attack });

            for (int i = 0; i < 60 && _pickup.CurrentState == PickupItem.PickupState.Thrown; i++)
                _pickup.TickPickup();

            Assert.Greater(_victim.State.percent, 0f, "acorn should have hit the victim");
            Assert.AreEqual(FighterStateId.Launched, _victim.State.stateId);
            Assert.AreEqual(PickupItem.PickupState.Respawning, _pickup.CurrentState);
        }

        [Test]
        public void GettingHit_DropsHeldPickup()
        {
            _pickup.TickPickup(); // grab
            var hit = new HitboxWindow { damage = 8f, angleDegrees = 60f, baseKnockback = 40f, knockbackGrowth = 100f };
            _holder.ApplyHit(in hit, 1);

            Assert.IsNull(_holder.HeldItem);
            Assert.AreEqual(PickupItem.PickupState.Available, _pickup.CurrentState);
        }
    }
}
