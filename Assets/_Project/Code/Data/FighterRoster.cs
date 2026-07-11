using System;
using UnityEngine;

namespace BB.Data
{
    /// <summary>A greyed-out "coming soon" slot on the character select screen.</summary>
    [Serializable]
    public struct ComingSoonFighter
    {
        public string displayName;
        [TextArea] public string teaser;
        public Sprite silhouette;
    }

    /// <summary>
    /// The list the character-select UI iterates. Adding a fighter to the game
    /// is: add their FighterDefinition asset here.
    /// </summary>
    [CreateAssetMenu(menuName = "Backyard Battle/Fighter Roster", fileName = "Roster")]
    public class FighterRoster : ScriptableObject
    {
        public FighterDefinition[] fighters;
        public ComingSoonFighter[] comingSoon;
    }
}
