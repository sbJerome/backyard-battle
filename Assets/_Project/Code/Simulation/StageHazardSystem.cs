using BB.Data;
using UnityEngine;

namespace BB.Simulation
{
    /// <summary>
    /// Base class for stage hazards. Deterministic: driven by the match tick
    /// (handed in by the stage's hazard system), never by wall-clock time or
    /// its own coroutines. Telegraph → active → idle, with the ≥2s telegraph
    /// guaranteed by HazardDefinition.
    ///
    /// Hazard hits go through the exact same ApplyHit pipeline as fighter
    /// attacks — one knockback formula for everything on the stage.
    /// </summary>
    public abstract class StageHazard : MonoBehaviour
    {
        public HazardDefinition definition;

        public enum Phase { Idle, Telegraph, Active }
        public Phase CurrentPhase { get; private set; } = Phase.Idle;

        int _phaseStartTick;
        int _cycleStartTick;

        public event System.Action<StageHazard> OnTelegraph;
        public event System.Action<StageHazard> OnActivate;

        int TelegraphTicks => Mathf.RoundToInt(definition.telegraphSeconds * FighterController.TickRate);
        int ActiveTicks => Mathf.RoundToInt(definition.activeSeconds * FighterController.TickRate);
        int PeriodTicks => Mathf.RoundToInt(definition.periodSeconds * FighterController.TickRate);

        /// <summary>Advance the hazard's timer state. Called once per sim tick.</summary>
        public void TickHazard(int matchTick, MatchSimulation sim)
        {
            switch (CurrentPhase)
            {
                case Phase.Idle:
                    if (matchTick - _cycleStartTick >= PeriodTicks)
                    {
                        CurrentPhase = Phase.Telegraph;
                        _phaseStartTick = matchTick;
                        OnTelegraph?.Invoke(this);
                    }
                    break;

                case Phase.Telegraph:
                    if (matchTick - _phaseStartTick >= TelegraphTicks)
                    {
                        CurrentPhase = Phase.Active;
                        _phaseStartTick = matchTick;
                        ActivationStarted();
                        OnActivate?.Invoke(this);
                    }
                    break;

                case Phase.Active:
                    ApplyDanger(sim);
                    if (matchTick - _phaseStartTick >= ActiveTicks)
                    {
                        CurrentPhase = Phase.Idle;
                        _cycleStartTick = matchTick;
                    }
                    break;
            }
        }

        /// <summary>Hit fighters inside the hazard's danger volume (called every active tick).</summary>
        protected abstract void ApplyDanger(MatchSimulation sim);

        /// <summary>Called once when the hazard transitions telegraph → active.</summary>
        protected virtual void ActivationStarted() { }

        /// <summary>Shared helper: hit one fighter with this hazard's stats.</summary>
        protected void HitFighter(FighterController victim)
        {
            var hit = new HitboxWindow
            {
                damage = definition.damage,
                angleDegrees = definition.angleDegrees,
                baseKnockback = definition.baseKnockback,
                knockbackGrowth = definition.knockbackGrowth,
                hitstopTicks = 4,
                hitSfxId = definition.hitSfxId,
                hitVfxId = definition.hitVfxId,
            };
            victim.ApplyHit(in hit, victim.State.position.x >= transform.position.x ? 1 : -1);
        }
    }

    /// <summary>
    /// The workhorse hazard: a box volume that erupts on a cycle
    /// (Grillmore flare-ups). Placed and sized in the stage scene.
    /// </summary>
    public class PeriodicVolumeHazard : StageHazard
    {
        [Tooltip("Local-space danger box, XY plane.")]
        public Vector2 size = new Vector2(2f, 4f);

        readonly System.Collections.Generic.HashSet<FighterController> _hitThisActivation = new();

        protected override void ActivationStarted() => _hitThisActivation.Clear();

        protected override void ApplyDanger(MatchSimulation sim)
        {
            var center = (Vector2)transform.position;
            var rect = new Rect(center - size * 0.5f, size);

            for (int i = 0; i < sim.Fighters.Count; i++)
            {
                var f = sim.Fighters[i];
                if (f.State.stateId == FighterStateId.Dead) continue;
                if (!rect.Contains(f.State.position)) continue;
                if (!_hitThisActivation.Add(f)) continue; // once per eruption
                HitFighter(f);
            }
        }
    }
}
