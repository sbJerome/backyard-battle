using System.Collections.Generic;
using BB.Data;
using BB.Simulation;
using UnityEngine;

namespace BB.Core
{
    public enum MatchPhase : byte { Idle, Countdown, Fight, Results }

    /// <summary>
    /// Owns one match end-to-end: spawns fighters from PlayerSlots, runs the
    /// countdown, steps the MatchSimulation at fixed tick, and hands results
    /// to the award layer. Online (M4) the server owns this and clients mirror
    /// its phase. Scene flow above it (Lobby scene → stage scene → back) is
    /// GameManager's job.
    /// </summary>
    public class MatchController : MonoBehaviour
    {
        [Header("Rules")]
        [Min(1)] public int stocks = 3;
        [Min(0)] public float countdownSeconds = 3f;

        public MatchPhase Phase { get; private set; } = MatchPhase.Idle;
        public MatchSimulation Simulation { get; private set; }

        readonly List<PlayerSlot> _slots = new();
        int _countdownTicks;

        public event System.Action<MatchPhase> OnPhaseChanged;
        public event System.Action<HitEvent> OnHit;
        public event System.Action<FighterController> OnKO;
        /// <summary>Placement order, winner first. Fires once per match.</summary>
        public event System.Action<List<FighterController>> OnMatchEnd;

        /// <summary>Spawn everything and start the countdown. Slots must have Fighter + InputSource set.</summary>
        public void StartMatch(StageDefinition stage, IReadOnlyList<PlayerSlot> slots)
        {
            Simulation = new MatchSimulation(stage);
            Simulation.Hits.OnHit += e => OnHit?.Invoke(e);
            Simulation.OnKO += f => OnKO?.Invoke(f);
            Simulation.OnMatchEnd += HandleMatchEnd;

            _slots.Clear();
            for (int i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];
                _slots.Add(slot);

                var go = Instantiate(slot.Fighter.fighterPrefab);
                go.name = $"{slot.Fighter.displayName} (P{slot.Index + 1})";
                var fighter = go.GetComponent<FighterController>();
                if (fighter == null) fighter = go.AddComponent<FighterController>();
                fighter.Init(slot.Fighter, slot.Index, stage.GetSpawnPoint(i), (byte)stocks);

                var source = slot.InputSource ?? new BotInputSource();
                Simulation.AddFighter(fighter, source);
                if (source is ISimAwareInput aware) aware.Bind(fighter, Simulation);
            }

            // Pickups placed in the stage scene join the simulation.
            foreach (var pickup in FindObjectsByType<PickupItem>(FindObjectsSortMode.None))
                Simulation.RegisterPickup(pickup);

            _countdownTicks = Mathf.RoundToInt(countdownSeconds * FighterController.TickRate);
            SetPhase(MatchPhase.Countdown);
        }

        void FixedUpdate()
        {
            switch (Phase)
            {
                case MatchPhase.Countdown:
                    if (--_countdownTicks <= 0) SetPhase(MatchPhase.Fight);
                    break;
                case MatchPhase.Fight:
                    Simulation.Step();
                    break;
            }
        }

        void HandleMatchEnd(List<FighterController> placements)
        {
            SetPhase(MatchPhase.Results);
            OnMatchEnd?.Invoke(placements);
        }

        void SetPhase(MatchPhase phase)
        {
            if (Phase == phase) return;
            Phase = phase;
            OnPhaseChanged?.Invoke(phase);
        }
    }
}
