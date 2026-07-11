using BB.Simulation;
using NUnit.Framework;
using UnityEngine;

namespace BB.Tests
{
    public class KnockbackFormulaTests
    {
        [Test]
        public void Knockback_IncreasesWithPercent()
        {
            float low = KnockbackFormula.Knockback(20f, 10f, 100f, 30f, 100f);
            float high = KnockbackFormula.Knockback(120f, 10f, 100f, 30f, 100f);
            Assert.Greater(high, low);
        }

        [Test]
        public void Knockback_HeavierTakesLess()
        {
            float light = KnockbackFormula.Knockback(80f, 10f, 80f, 30f, 100f);   // Pepper-ish
            float heavy = KnockbackFormula.Knockback(80f, 10f, 130f, 30f, 100f);  // Chip-ish
            Assert.Greater(light, heavy);
        }

        [Test]
        public void Knockback_BaseKnockbackIsFloor()
        {
            float kb = KnockbackFormula.Knockback(0f, 0f, 100f, 42f, 0f);
            Assert.GreaterOrEqual(kb, 42f);
        }

        [Test]
        public void Knockback_GrowthScalesScalingTermOnly()
        {
            float noGrowth = KnockbackFormula.Knockback(100f, 12f, 100f, 10f, 0f);
            Assert.AreEqual(10f, noGrowth, 0.001f, "growth 0 should leave only base knockback");
        }

        [Test]
        public void LaunchVelocity_RespectsFacing()
        {
            var right = KnockbackFormula.LaunchVelocity(100f, 45f, 1);
            var left = KnockbackFormula.LaunchVelocity(100f, 45f, -1);
            Assert.Greater(right.x, 0f);
            Assert.Less(left.x, 0f);
            Assert.AreEqual(right.y, left.y, 0.001f);
        }

        [Test]
        public void LaunchVelocity_StraightUpHasNoHorizontal()
        {
            var v = KnockbackFormula.LaunchVelocity(100f, 90f, 1);
            Assert.AreEqual(0f, v.x, 0.001f);
            Assert.Greater(v.y, 0f);
        }

        [Test]
        public void SakuraiAngle_ResolvesTo45()
        {
            Assert.AreEqual(45f, KnockbackFormula.ResolveAngle(KnockbackFormula.SakuraiAngle));
            Assert.AreEqual(30f, KnockbackFormula.ResolveAngle(30f));
        }

        [Test]
        public void Hitstun_ScalesWithKnockback()
        {
            Assert.AreEqual(0, KnockbackFormula.HitstunTicks(0f));
            Assert.Greater(KnockbackFormula.HitstunTicks(200f), KnockbackFormula.HitstunTicks(50f));
        }
    }
}
