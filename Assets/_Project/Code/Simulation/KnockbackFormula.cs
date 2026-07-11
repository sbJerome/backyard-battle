using UnityEngine;

namespace BB.Simulation
{
    /// <summary>
    /// Smash-style damage/knockback math. Pure functions — unit tested in
    /// EditMode with no scene required. Tune constants, not structure.
    /// </summary>
    public static class KnockbackFormula
    {
        /// <summary>World-units-per-second of launch speed per knockback unit.</summary>
        public const float LaunchSpeedPerKb = 0.35f;
        /// <summary>Hitstun ticks per knockback unit.</summary>
        public const float HitstunPerKb = 0.4f;
        /// <summary>The classic "Sakurai angle" sentinel value.</summary>
        public const float SakuraiAngle = 361f;
        public const float BaselineWeight = 100f;

        /// <summary>
        /// Knockback units for a hit.
        /// </summary>
        /// <param name="percentAfterHit">Victim percent AFTER damage is applied (0–999).</param>
        /// <param name="damage">Damage of the hit.</param>
        /// <param name="weight">Victim weight (~100 baseline; heavier = less knockback).</param>
        /// <param name="baseKnockback">Flat knockback of the attack.</param>
        /// <param name="knockbackGrowth">Attack scaling (~100 baseline).</param>
        public static float Knockback(float percentAfterHit, float damage, float weight,
                                      float baseKnockback, float knockbackGrowth)
        {
            float p = Mathf.Clamp(percentAfterHit, 0f, 999f);
            float scaling = (p / 10f + p * damage / 20f) * (200f / (weight + 100f)) * 1.4f + 18f;
            return scaling * knockbackGrowth / 100f + baseKnockback;
        }

        /// <summary>Launch velocity for a knockback value, angle and attacker facing.</summary>
        public static Vector2 LaunchVelocity(float knockback, float angleDegrees, int facing)
        {
            float angle = ResolveAngle(angleDegrees);
            float rad = angle * Mathf.Deg2Rad;
            var dir = new Vector2(Mathf.Cos(rad) * facing, Mathf.Sin(rad));
            return dir * (knockback * LaunchSpeedPerKb);
        }

        public static int HitstunTicks(float knockback)
            => Mathf.Max(0, Mathf.FloorToInt(knockback * HitstunPerKb));

        /// <summary>Sakurai angle resolves to 45° while launched (simplified for the demo).</summary>
        public static float ResolveAngle(float angleDegrees)
            => Mathf.Approximately(angleDegrees, SakuraiAngle) ? 45f : angleDegrees;
    }
}
