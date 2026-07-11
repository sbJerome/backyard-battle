using System;
using UnityEngine;

namespace BB.Data
{
    [Serializable]
    public struct ComingSoonStage
    {
        public string displayName;
        [TextArea] public string teaser;
        public Sprite thumbnail;
    }

    /// <summary>The list the stage-select UI iterates.</summary>
    [CreateAssetMenu(menuName = "Backyard Battle/Stage Catalog", fileName = "StageCatalog")]
    public class StageCatalog : ScriptableObject
    {
        public StageDefinition[] stages;
        public ComingSoonStage[] comingSoon;
    }
}
