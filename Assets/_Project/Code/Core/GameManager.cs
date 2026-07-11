using UnityEngine;

namespace BB.Core
{
    /// <summary>
    /// Boot-scene singleton: loads the profile, owns services that outlive
    /// scenes (Shoebox ledger, settings), and drives scene flow
    /// (MainMenu → Lobby → Stage_X → Lobby). Fleshed out at M2; the Boot scene
    /// is created in-editor during the first Unity session.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public AwardLedger Ledger { get; private set; }

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Ledger = SaveSystem.LoadOrCreate(System.Guid.NewGuid().ToString("N"));
            Application.targetFrameRate = 120;
            Time.fixedDeltaTime = 1f / BB.Simulation.FighterController.TickRate;
        }
    }
}
