using System;
using UnityEngine;

namespace BB.Data
{
    /// <summary>Which player input triggers an attack.</summary>
    public enum AttackInput
    {
        Jab,
        ForwardTilt,
        UpTilt,
        DownTilt,
        DashAttack,
        NeutralAir,
        ForwardAir,
        BackAir,
        UpAir,
        DownAir,
        NeutralSpecial,
        SideSpecial,
        UpSpecial,
        DownSpecial,
    }

    /// <summary>
    /// A capsule hitbox active for a tick range within an attack, positioned
    /// relative to a named socket (bone/empty) on the fighter prefab.
    /// Tick frame data here is the timing source of truth — animation is
    /// presentation only.
    /// </summary>
    [Serializable]
    public struct HitboxWindow
    {
        [Tooltip("First attack tick (from attack start, inclusive) this hitbox is active.")]
        public int startTick;
        [Tooltip("Last attack tick (inclusive) this hitbox is active.")]
        public int endTick;

        [Tooltip("Transform path inside the fighter prefab the capsule is relative to. Empty = fighter root.")]
        public string socket;
        public Vector3 offset;
        public float radius;
        [Tooltip("Capsule length along its local axis; 0 = sphere.")]
        public float length;

        [Header("On hit")]
        public float damage;
        [Tooltip("Launch angle in degrees; 0 = straight forward (mirrored by facing), 90 = straight up. 361 = Sakurai angle.")]
        public float angleDegrees;
        public float baseKnockback;
        [Tooltip("Knockback growth (per-attack scaling with victim percent), Smash-style, ~100 baseline.")]
        public float knockbackGrowth;
        public int hitstopTicks;

        [Header("Presentation ids")]
        public string hitSfxId;
        public string hitVfxId;
    }

    [CreateAssetMenu(menuName = "Backyard Battle/Attack Definition", fileName = "Attack_")]
    public class AttackDefinition : ScriptableObject
    {
        public string id;
        public string displayName;
        public AttackInput input;

        [Header("Frame data (simulation ticks @ 60 Hz)")]
        [Min(0)] public int startupTicks = 6;
        [Min(1)] public int activeTicks = 4;
        [Min(0)] public int recoveryTicks = 12;

        public HitboxWindow[] hitboxes;

        [Header("Presentation")]
        [Tooltip("Animation clip/state name on the fighter's animator. Presentation only.")]
        public string animationState;

        /// <summary>Total duration of the attack in ticks.</summary>
        public int TotalTicks => startupTicks + activeTicks + recoveryTicks;

        void OnValidate()
        {
            if (hitboxes == null) return;
            for (int i = 0; i < hitboxes.Length; i++)
            {
                // Hitboxes must live inside the active window of the attack.
                hitboxes[i].startTick = Mathf.Clamp(hitboxes[i].startTick, 0, TotalTicks);
                hitboxes[i].endTick = Mathf.Clamp(hitboxes[i].endTick, hitboxes[i].startTick, TotalTicks);
            }
        }
    }
}
