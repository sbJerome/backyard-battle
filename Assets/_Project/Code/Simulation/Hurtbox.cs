using UnityEngine;

namespace BB.Simulation
{
    /// <summary>
    /// Marks a collider as hittable and points back at its fighter. Fighters
    /// carry 1–3 of these on bones (Noodle's tail segments use the multiplier).
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class Hurtbox : MonoBehaviour
    {
        public FighterController owner;
        [Tooltip("Damage taken through this hurtbox is scaled by this (Noodle's tail: 0.6).")]
        public float damageMultiplier = 1f;
    }
}
