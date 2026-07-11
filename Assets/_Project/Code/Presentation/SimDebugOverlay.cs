using BB.Simulation;
using UnityEngine;

namespace BB.Presentation
{
    /// <summary>
    /// Week-one debugging: draws hitboxes, hurtboxes, and motor state as
    /// gizmos. "Animation lies; hitboxes never do" — this overlay is how we
    /// keep that pillar honest. Attach next to a MatchController-driven scene
    /// and it draws every fighter.
    /// </summary>
    public class SimDebugOverlay : MonoBehaviour
    {
        public bool drawHurtboxes = true;
        public bool drawHitboxes = true;
        public bool drawState = true;

        void OnDrawGizmos()
        {
            foreach (var fighter in FindObjectsByType<FighterController>(FindObjectsSortMode.None))
            {
                if (fighter.Definition == null) continue;

                if (drawHurtboxes)
                {
                    Gizmos.color = fighter.State.intangibleTicks > 0
                        ? new Color(0.5f, 0.5f, 1f, 0.5f)
                        : new Color(1f, 0.9f, 0.2f, 0.6f);
                    fighter.GetCapsuleWorld(out var p1, out var p2, out float r);
                    Gizmos.DrawWireSphere(p1, r);
                    Gizmos.DrawWireSphere(p2, r);
                    Gizmos.DrawLine(p1 + Vector3.left * r, p2 + Vector3.left * r);
                    Gizmos.DrawLine(p1 + Vector3.right * r, p2 + Vector3.right * r);
                }

                if (drawHitboxes && fighter.State.stateId == FighterStateId.Attack)
                {
                    var attack = fighter.CurrentAttack;
                    if (attack?.hitboxes != null)
                    {
                        int tick = fighter.State.stateTicks;
                        for (int i = 0; i < attack.hitboxes.Length; i++)
                        {
                            ref readonly var w = ref attack.hitboxes[i];
                            bool active = tick >= w.startTick && tick <= w.endTick;
                            Gizmos.color = active ? Color.red : new Color(1f, 0f, 0f, 0.15f);
                            HitResolver.GetHitboxCapsule(fighter, in w, out var h1, out var h2);
                            Gizmos.DrawWireSphere(h1, w.radius);
                            if ((h2 - h1).sqrMagnitude > 0.0001f) Gizmos.DrawWireSphere(h2, w.radius);
                        }
                    }
                }

#if UNITY_EDITOR
                if (drawState)
                {
                    UnityEditor.Handles.Label(
                        fighter.transform.position + Vector3.up * (fighter.Definition.capsuleHeight + 0.4f),
                        $"{fighter.State.stateId} {fighter.State.percent:0}%  stocks:{fighter.State.stocks}");
                }
#endif
            }
        }
    }
}
