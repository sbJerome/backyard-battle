using UnityEngine;

namespace BB.Data
{
    /// <summary>
    /// Config for a stage hazard. Pillar rule ("chaos is fair"): every hazard
    /// telegraphs for at least 2 seconds — enforced here, not by convention.
    /// </summary>
    [CreateAssetMenu(menuName = "Backyard Battle/Hazard Definition", fileName = "Hazard_")]
    public class HazardDefinition : ScriptableObject
    {
        public const float MinTelegraphSeconds = 2f;

        public string id;
        public string displayName;

        [Header("Timing (seconds; converted to ticks by the hazard system)")]
        [Tooltip("Average seconds between activations.")]
        public float periodSeconds = 25f;
        [Tooltip("Warning duration before the hazard hits. Clamped to >= 2s (game-feel pillar #1).")]
        public float telegraphSeconds = 2f;
        [Tooltip("How long the hazard stays dangerous once live.")]
        public float activeSeconds = 1f;

        [Header("On hit (same knockback pipeline as fighter attacks)")]
        public float damage = 10f;
        public float angleDegrees = 90f;
        public float baseKnockback = 40f;
        public float knockbackGrowth = 80f;

        [Header("Presentation ids")]
        public string telegraphSfxId;
        public string telegraphVfxId;
        public string hitSfxId;
        public string hitVfxId;

        void OnValidate()
        {
            telegraphSeconds = Mathf.Max(MinTelegraphSeconds, telegraphSeconds);
        }
    }
}
