using UnityEngine;

namespace BB.Data
{
    public enum BragTier
    {
        Doodle,   // easy
        Marker,   // medium
        GoldStar, // hard — awards a Shiny
    }

    /// <summary>
    /// A Brag (achievement) in Max's notebook. Predicates are data: a stat key
    /// tracked by the award system reaching a threshold. Modes emit stats; the
    /// award layer is a pure subscriber — no mode ever writes Caps directly.
    /// </summary>
    [CreateAssetMenu(menuName = "Backyard Battle/Award Definition", fileName = "Brag_")]
    public class AwardDefinition : ScriptableObject
    {
        public string id;
        public string displayName;
        [TextArea] public string description;
        public BragTier tier = BragTier.Doodle;
        public Sprite badge;

        [Header("Predicate: statKey >= threshold")]
        [Tooltip("Stat tracked by AwardTracker, e.g. \"wins\", \"wins.chip\", \"ko.hazard\", \"match.played.rumble\".")]
        public string statKey;
        public long threshold = 1;

        [Header("Reward")]
        public int capsReward = 25;
        [Tooltip("Gold Star brags grant a named Shiny; empty for lower tiers.")]
        public string shinyId;
    }
}
