using BB.Core;
using UnityEngine;

namespace BB.UI
{
    /// <summary>
    /// HUD skeleton: percent + stocks per fighter in Max's smudgy-marker style.
    /// Real widgets land at M2 with the couch-brawl milestone; this stub keeps
    /// the assembly compiling and marks the subscription pattern (read sim
    /// state, never poke it).
    /// </summary>
    public class MatchHud : MonoBehaviour
    {
        public MatchController match;

        void OnGUI()
        {
            if (match == null || match.Simulation == null) return;

            // Placeholder dev HUD until the M2 UGUI pass.
            for (int i = 0; i < match.Simulation.Fighters.Count; i++)
            {
                var f = match.Simulation.Fighters[i];
                GUI.Label(new Rect(20 + i * 140, Screen.height - 40, 130, 30),
                          $"P{f.PlayerIndex + 1} {f.State.percent:0}% ×{f.State.stocks}");
            }
        }
    }
}
