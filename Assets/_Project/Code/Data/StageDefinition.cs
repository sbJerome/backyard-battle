using UnityEngine;

namespace BB.Data
{
    /// <summary>
    /// A stage is a scene plus the data the match systems need to run it.
    /// Spawn points are ordered and cycled, so any player count works (20+ later).
    /// </summary>
    [CreateAssetMenu(menuName = "Backyard Battle/Stage Definition", fileName = "Stage_")]
    public class StageDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string id;
        public string displayName;
        [TextArea] public string tagline;
        public Sprite thumbnail;
        public string sceneName;

        [Header("Gameplay geometry (world units, XY plane)")]
        [Tooltip("Fighters fully outside this rect are KO'd.")]
        public Rect blastZone = new Rect(-25f, -15f, 50f, 32f);
        [Tooltip("The camera never frames outside this rect.")]
        public Rect cameraBounds = new Rect(-18f, -8f, 36f, 20f);
        [Tooltip("Ordered spawn positions; cycled when player count exceeds the list.")]
        public Vector2[] spawnPoints;
        public Vector2 respawnPoint = new Vector2(0f, 6f);

        [Header("Hazards")]
        public HazardDefinition[] hazards;
        [Tooltip("True if the stage is legal with hazards disabled (tournament toggle).")]
        public bool tournamentLegal;

        [Header("Presentation")]
        public string musicId;

        public Vector2 GetSpawnPoint(int playerIndex)
        {
            if (spawnPoints == null || spawnPoints.Length == 0) return Vector2.zero;
            return spawnPoints[playerIndex % spawnPoints.Length];
        }
    }
}
