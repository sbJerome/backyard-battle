using System.Collections.Generic;
using BB.Data;
using BB.Simulation;
using UnityEngine;

namespace BB.Core
{
    /// <summary>
    /// The Shoebox's ears: a pure subscriber that turns match events into
    /// ledger entries. Modes emit gameplay events; NOTHING else ever writes
    /// Caps. This contract is what lets Rumble Pile and Recess share one
    /// progression system later.
    /// </summary>
    public class AwardTracker
    {
        readonly AwardLedger _ledger;
        readonly RewardTable _rewards;
        readonly AwardDefinition[] _brags;
        readonly string _mode;

        public AwardTracker(AwardLedger ledger, RewardTable rewards, AwardDefinition[] brags, string mode)
        {
            _ledger = ledger;
            _rewards = rewards;
            _brags = brags ?? System.Array.Empty<AwardDefinition>();
            _mode = mode;
        }

        public void Attach(MatchController match)
        {
            match.OnKO += _ => { /* per-KO stats land with the M6 sticker pass */ };
            match.OnMatchEnd += placements => OnMatchEnd(match, placements);
        }

        void OnMatchEnd(MatchController match, List<FighterController> placements)
        {
            string matchId = System.Guid.NewGuid().ToString("N");
            long now = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Demo scope: reward the local profile based on player 0's placement.
            int placement = placements.Count;
            for (int i = 0; i < placements.Count; i++)
                if (placements[i].PlayerIndex == 0) { placement = i + 1; break; }

            int caps = _rewards.participationCaps + _rewards.GetPlacementBonus(placement);
            Earn(LedgerEventTypes.CapsEarned, null, caps, now, matchId);
            Earn(LedgerEventTypes.XpGained, null, 10 + (placement == 1 ? 15 : 0), now, matchId);
            Earn(LedgerEventTypes.StatIncremented, "match.played", 1, now, matchId);
            if (placement == 1)
                Earn(LedgerEventTypes.StatIncremented, "wins", 1, now, matchId);

            EvaluateBrags(now, matchId);
            SaveSystem.Save(_ledger);
        }

        void EvaluateBrags(long now, string matchId)
        {
            var wallet = _ledger.Fold();
            foreach (var brag in _brags)
            {
                if (brag == null || wallet.Brags.Contains(brag.id)) continue;
                wallet.Stats.TryGetValue(brag.statKey, out long value);
                if (value < brag.threshold) continue;

                Earn(LedgerEventTypes.BragUnlocked, brag.id, 0, now, matchId);
                Earn(LedgerEventTypes.CapsEarned, brag.id, brag.capsReward, now, matchId);
                if (!string.IsNullOrEmpty(brag.shinyId))
                    Earn(LedgerEventTypes.ShinyEarned, brag.shinyId, 1, now, matchId);
            }
        }

        void Earn(string type, string id, long amount, long now, string matchId)
        {
            _ledger.Append(new LedgerEntry
            {
                type = type,
                id = id,
                amount = amount,
                timestampUnix = now,
                matchId = matchId,
                mode = _mode,
            });
        }
    }
}
