using UnityEngine;

namespace BB.Simulation
{
    /// <summary>
    /// Input sources that need to see the simulation (bots, replays) get bound
    /// to their fighter after spawn by MatchController.
    /// </summary>
    public interface ISimAwareInput
    {
        void Bind(FighterController self, MatchSimulation sim);
    }

    /// <summary>
    /// A beatable sparring partner: walks at the nearest opponent, mixes jabs
    /// and tilts in range, jumps at higher targets, and tries to recover when
    /// knocked off. Deterministic — reads only sim state and its own tick
    /// counter, so it runs identically on a server (M4) or in a replay.
    /// </summary>
    public sealed class SimpleBotBrain : IInputSource, ISimAwareInput
    {
        [Range(0f, 1f)] public float aggression = 0.6f;

        FighterController _self;
        MatchSimulation _sim;
        int _tick;
        int _nextAttackTick;
        int _attackCount;

        public void Bind(FighterController self, MatchSimulation sim)
        {
            _self = self;
            _sim = sim;
        }

        public InputSnapshot ReadTick()
        {
            if (_self == null || _sim == null) return default;
            _tick++;

            var s = _self.State;
            var snap = new InputSnapshot();

            // --- recovery: off the stage or below ground level → get back ---
            float edgeX = _sim.Stage.blastZone.xMax - 6f;
            bool offStage = Mathf.Abs(s.position.x) > edgeX || s.position.y < -0.5f;
            if (offStage)
            {
                snap.moveX = Mathf.Sign(-s.position.x);
                // Pulse jump (edge-detected input needs releases between presses).
                if (s.velocity.y < 0f && (_tick / 6) % 2 == 0)
                    snap.held |= FighterButtons.Jump;
                return snap;
            }

            // --- find nearest living opponent ---
            FighterController target = null;
            float best = float.MaxValue;
            for (int i = 0; i < _sim.Fighters.Count; i++)
            {
                var f = _sim.Fighters[i];
                if (f == _self || f.State.stateId == FighterStateId.Dead) continue;
                float d = (f.State.position - s.position).sqrMagnitude;
                if (d < best) { best = d; target = f; }
            }
            if (target == null) return snap;

            Vector2 to = target.State.position - s.position;

            // --- pickups: grab a nearby free item; throw a held one ---
            if (_self.HeldItem == null)
            {
                PickupItem wanted = null;
                float bestPickup = float.MaxValue;
                for (int i = 0; i < _sim.Pickups.Count; i++)
                {
                    var p = _sim.Pickups[i];
                    if (p.CurrentState != PickupItem.PickupState.Available) continue;
                    float d = (p.Position - s.position).sqrMagnitude;
                    if (d < bestPickup) { bestPickup = d; wanted = p; }
                }
                // Detour for the item when it's closer than the opponent.
                if (wanted != null && bestPickup < to.sqrMagnitude)
                {
                    Vector2 toPickup = wanted.Position - s.position;
                    snap.moveX = Mathf.Abs(toPickup.x) > 0.3f ? Mathf.Sign(toPickup.x) : 0f;
                    if (toPickup.y > 1.5f && s.grounded && (_tick / 8) % 5 == 0)
                        snap.held |= FighterButtons.Jump;
                    return snap;
                }
            }
            else if (Mathf.Abs(to.y) < 1.5f && Mathf.Abs(to.x) < 9f && _tick >= _nextAttackTick)
            {
                // Face the target and let it fly.
                snap.moveX = Mathf.Sign(to.x);
                snap.held |= FighterButtons.Attack;
                _nextAttackTick = _tick + Mathf.RoundToInt(Mathf.Lerp(90f, 30f, aggression));
                return snap;
            }

            // --- approach ---
            if (Mathf.Abs(to.x) > 0.9f)
                snap.moveX = Mathf.Sign(to.x);

            // --- jump at higher targets, occasionally ---
            if (to.y > 1.5f && s.grounded && (_tick / 8) % 5 == 0)
                snap.held |= FighterButtons.Jump;

            // --- attack in range, on a cadence scaled by aggression ---
            bool inRange = Mathf.Abs(to.x) < 1.4f && Mathf.Abs(to.y) < 1.2f;
            if (inRange && _tick >= _nextAttackTick)
            {
                // One-tick press; alternate jab (neutral) and forward tilt.
                _attackCount++;
                if (_attackCount % 2 == 0) snap.moveX = Mathf.Sign(to.x);
                else snap.moveX = 0f;
                snap.held |= FighterButtons.Attack;
                _nextAttackTick = _tick + Mathf.RoundToInt(Mathf.Lerp(90f, 30f, aggression));
            }

            return snap;
        }
    }
}
