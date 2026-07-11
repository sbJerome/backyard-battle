using System.Collections.Generic;
using BB.Data;
using UnityEngine;

namespace BB.Simulation
{
    /// <summary>
    /// Drives every fighter at a fixed 60 Hz tick and owns match-level rules
    /// that live inside the sim: blast zones, KOs, match end. Everything above
    /// (menus, lobby, ceremony) is BB.Core's problem; everything visual is
    /// BB.Presentation's. At M4 this runs on the server, and clients predict
    /// their own fighter through the same code.
    /// </summary>
    public class MatchSimulation
    {
        public readonly StageDefinition Stage;
        public readonly HitResolver Hits = new();

        readonly List<FighterController> _fighters = new();
        readonly List<IInputSource> _inputs = new();

        public int Tick { get; private set; }
        public bool Finished { get; private set; }
        public IReadOnlyList<FighterController> Fighters => _fighters;

        public event System.Action<FighterController> OnKO;
        /// <summary>Raised once, with the surviving fighters in placement order (winner first).</summary>
        public event System.Action<List<FighterController>> OnMatchEnd;

        readonly List<FighterController> _koOrder = new(); // first KO'd = last place

        public MatchSimulation(StageDefinition stage)
        {
            Stage = stage;
        }

        public void AddFighter(FighterController fighter, IInputSource input)
        {
            _fighters.Add(fighter);
            _inputs.Add(input);
        }

        /// <summary>Advance the whole match one tick. Call from FixedUpdate at 60 Hz.</summary>
        public void Step()
        {
            if (Finished) return;
            Tick++;

            for (int i = 0; i < _fighters.Count; i++)
            {
                if (_fighters[i].State.stateId == FighterStateId.Dead) continue;
                _fighters[i].Tick(_inputs[i].ReadTick());
            }

            for (int i = 0; i < _fighters.Count; i++)
            {
                if (_fighters[i].State.stateId == FighterStateId.Attack)
                    Hits.ResolveTick(_fighters[i]);
            }

            CheckBlastZones();
            CheckMatchEnd();
        }

        void CheckBlastZones()
        {
            for (int i = 0; i < _fighters.Count; i++)
            {
                var f = _fighters[i];
                if (f.State.stateId == FighterStateId.Dead) continue;
                if (Stage.blastZone.Contains(f.State.position)) continue;

                bool hadStocks = f.State.stocks > 1;
                f.HandleKO(Stage.respawnPoint);
                if (!hadStocks) _koOrder.Add(f);
                OnKO?.Invoke(f);
            }
        }

        void CheckMatchEnd()
        {
            int alive = 0;
            FighterController last = null;
            for (int i = 0; i < _fighters.Count; i++)
            {
                if (_fighters[i].State.stateId != FighterStateId.Dead)
                {
                    alive++;
                    last = _fighters[i];
                }
            }

            // Solo testing (1 fighter) never "ends"; otherwise last one standing wins.
            if (_fighters.Count >= 2 && alive <= 1)
            {
                Finished = true;
                var placements = new List<FighterController>();
                if (last != null) placements.Add(last);
                for (int i = _koOrder.Count - 1; i >= 0; i--) placements.Add(_koOrder[i]);
                OnMatchEnd?.Invoke(placements);
            }
        }
    }
}
