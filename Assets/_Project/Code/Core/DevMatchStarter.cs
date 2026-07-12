using BB.Data;
using BB.Simulation;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BB.Core
{
    /// <summary>
    /// Development harness: drop in a stage scene and it starts a match on
    /// Play — player 1 on keyboard/first gamepad, the rest as dummy bots.
    /// This is the M1 "one fighter + training dummy" loop. Replaced by the
    /// real Lobby flow at M2/M3.
    /// </summary>
    [RequireComponent(typeof(MatchController))]
    public class DevMatchStarter : MonoBehaviour
    {
        public StageDefinition stage;
        public FighterDefinition player1Fighter;
        public FighterDefinition dummyFighter;
        [Range(0, 5)] public int dummyCount = 1;
        public InputActionAsset controls;

        void Start()
        {
            if (stage == null || player1Fighter == null)
            {
                Debug.LogError("DevMatchStarter: assign stage and player1Fighter.");
                return;
            }

            var slots = new System.Collections.Generic.List<PlayerSlot>();

            var p1 = new PlayerSlot(0) { Fighter = player1Fighter };
            if (controls != null)
            {
                var map = controls.FindActionMap("Fighter", throwIfNotFound: true);
                map.Enable();
                p1.InputSource = new DeviceInputSource(map);
            }
            else
            {
                p1.InputSource = new BotInputSource();
            }
            slots.Add(p1);

            for (int i = 0; i < dummyCount; i++)
            {
                slots.Add(new PlayerSlot(i + 1)
                {
                    Fighter = dummyFighter != null ? dummyFighter : player1Fighter,
                    InputSource = new BotInputSource(),
                });
            }

            var match = GetComponent<MatchController>();
            match.StartMatch(stage, slots);
            // BrawlCamera self-acquires targets from the running MatchController.
        }
    }
}
