using System;
using UnityEngine;

namespace BB.Data
{
    /// <summary>
    /// A Style Sticker: end-of-match comedy bonus computed from match stats.
    /// "max" stickers go to the highest value of the stat, "min" to the lowest.
    /// </summary>
    [Serializable]
    public struct StyleStickerDefinition
    {
        public string id;
        public string displayName;
        [Tooltip("Match stat key, e.g. \"airtimeTicks\", \"taunts\", \"selfDestructs\".")]
        public string statKey;
        public bool awardToLowest;
        public int caps;
    }

    /// <summary>
    /// Payout rules shared by every mode. Modes report placements/stats;
    /// this table decides Caps. Lives in data so tuning is not a code change.
    /// </summary>
    [CreateAssetMenu(menuName = "Backyard Battle/Reward Table", fileName = "RewardTable")]
    public class RewardTable : ScriptableObject
    {
        [Header("Base payout")]
        public int participationCaps = 10;
        [Tooltip("Bonus by final placement, index 0 = 1st place.")]
        public int[] placementBonus = { 50, 30, 20, 10, 5, 5 };
        [Tooltip("Cap multiplier for the first win of the day (the Juice Box).")]
        public float juiceBoxMultiplier = 3f;
        [Tooltip("Minimum on-stage activity (input ticks) to earn anything — anti-idle.")]
        public int minActivityTicks = 600;

        [Header("Style stickers")]
        public StyleStickerDefinition[] stickers;

        public int GetPlacementBonus(int placement)
        {
            if (placementBonus == null || placementBonus.Length == 0) return 0;
            int idx = Mathf.Clamp(placement - 1, 0, placementBonus.Length - 1);
            return placementBonus[idx];
        }
    }
}
