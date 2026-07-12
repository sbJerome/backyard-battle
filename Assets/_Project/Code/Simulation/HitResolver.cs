using System.Collections.Generic;
using BB.Data;
using UnityEngine;

namespace BB.Simulation
{
    public readonly struct HitEvent
    {
        public readonly FighterController Attacker;
        public readonly FighterController Victim;
        public readonly HitboxWindow Hit;

        public HitEvent(FighterController attacker, FighterController victim, in HitboxWindow hit)
        {
            Attacker = attacker;
            Victim = victim;
            Hit = hit;
        }
    }

    /// <summary>
    /// Central hit detection: explicit overlap queries during an attack's
    /// active ticks (never OnTriggerEnter — queries are tick-aligned,
    /// deterministic-ordered, and prediction-compatible). One hit per
    /// attack-activation per victim. At M4 this runs server-side only.
    /// </summary>
    public class HitResolver
    {
        const int MaxOverlaps = 32;

        readonly Collider[] _overlaps = new Collider[MaxOverlaps];
        /// <summary>Victims already hit by the current activation, per attacker (keyed by activation id).</summary>
        readonly Dictionary<FighterController, (ushort activationId, HashSet<FighterController> victims)> _alreadyHit = new();

        /// <summary>Raised once per landed hit, after state is applied. Presentation and netcode subscribe.</summary>
        public event System.Action<HitEvent> OnHit;

        /// <summary>Raise a hit that resolved outside the hitbox query path (pickups, hazards).</summary>
        public void Raise(in HitEvent e) => OnHit?.Invoke(e);

        /// <summary>Call once per tick per attacking fighter.</summary>
        public void ResolveTick(FighterController attacker)
        {
            var attack = attacker.CurrentAttack;
            if (attack == null || attack.hitboxes == null) return;

            int attackTick = attacker.State.stateTicks;
            // New activation → drop the stale dedupe set (tick counts start at 1
            // by the time hits resolve, so never key this off attackTick == 0).
            if (_alreadyHit.TryGetValue(attacker, out var entry) &&
                entry.activationId != attacker.State.attackActivationId)
                _alreadyHit.Remove(attacker);

            for (int i = 0; i < attack.hitboxes.Length; i++)
            {
                ref readonly var window = ref attack.hitboxes[i];
                if (attackTick < window.startTick || attackTick > window.endTick) continue;
                QueryWindow(attacker, in attack.hitboxes[i]);
            }
        }

        void QueryWindow(FighterController attacker, in HitboxWindow window)
        {
            GetHitboxCapsule(attacker, in window, out Vector3 p1, out Vector3 p2);
            int count = Physics.OverlapCapsuleNonAlloc(p1, p2, window.radius, _overlaps,
                                                       ~0, QueryTriggerInteraction.Collide);
            for (int i = 0; i < count; i++)
            {
                var hurtbox = _overlaps[i].GetComponent<Hurtbox>();
                if (hurtbox == null || hurtbox.owner == null) continue;
                var victim = hurtbox.owner;
                if (victim == attacker) continue;

                if (!_alreadyHit.TryGetValue(attacker, out var entry2))
                    _alreadyHit[attacker] = entry2 = (attacker.State.attackActivationId, new HashSet<FighterController>());
                if (!entry2.victims.Add(victim)) continue; // already hit by this activation

                var scaled = window;
                scaled.damage *= hurtbox.damageMultiplier;
                victim.ApplyHit(in scaled, attacker.State.facing);
                attacker.State.hitstopTicks = window.hitstopTicks; // both parties freeze
                OnHit?.Invoke(new HitEvent(attacker, victim, in scaled));
            }
        }

        /// <summary>Hitbox capsule in world space: socket-relative, mirrored by facing.</summary>
        public static void GetHitboxCapsule(FighterController attacker, in HitboxWindow window,
                                            out Vector3 p1, out Vector3 p2)
        {
            Vector3 origin = new Vector3(attacker.State.position.x, attacker.State.position.y, 0f);
            if (!string.IsNullOrEmpty(window.socket))
            {
                var socket = attacker.transform.Find(window.socket);
                if (socket != null) origin = socket.position;
            }

            Vector3 offset = window.offset;
            offset.x *= attacker.State.facing;
            Vector3 center = origin + offset;

            Vector3 half = new Vector3(window.length * 0.5f * attacker.State.facing, 0f, 0f);
            p1 = center - half;
            p2 = center + half;
        }
    }
}
