using UnityEngine;

namespace BB.Data
{
    /// <summary>
    /// Everything that makes a fighter a fighter. Adding a new fighter to the
    /// game is: create one of these + a prefab + roster entry. No code.
    /// </summary>
    [CreateAssetMenu(menuName = "Backyard Battle/Fighter Definition", fileName = "Fighter_")]
    public class FighterDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string id;
        public string displayName;
        [TextArea] public string tagline;
        public Sprite icon;
        public GameObject fighterPrefab;

        [Header("Weight & gravity")]
        [Tooltip("Smash-style weight; 100 is baseline. Heavier = less knockback taken.")]
        public float weight = 100f;
        public float gravity = 60f;
        public float maxFallSpeed = 14f;
        public float fastFallSpeed = 22f;

        [Header("Ground movement")]
        public float walkSpeed = 4f;
        public float runSpeed = 7f;
        public float groundAcceleration = 60f;
        public float groundFriction = 40f;

        [Header("Air movement")]
        public float airSpeed = 5f;
        public float airAcceleration = 30f;
        public int extraJumps = 1;
        public float jumpVelocity = 14f;
        public float airJumpVelocity = 12f;

        [Header("Hurtbox")]
        public float capsuleRadius = 0.35f;
        public float capsuleHeight = 1.0f;

        [Header("Attacks")]
        public AttackDefinition[] attacks;

        [Header("Cosmetics")]
        [Tooltip("Transform path inside the fighter prefab where hats/accessories attach.")]
        public string accessoryAnchor = "AccessoryAnchor";
        public Material[] skins;

        [Header("Rumble Pile physical identity (future mode; part of the fighter's soul now)")]
        [Tooltip("Ragdoll density for the physics-brawler mode (Chip is very dense).")]
        public float rumbleDensity = 1f;
        [Tooltip("Bounciness for the physics-brawler mode (Wiener is very bouncy).")]
        [Range(0f, 1f)] public float rumbleRestitution = 0.1f;

        public AttackDefinition FindAttack(AttackInput input)
        {
            if (attacks == null) return null;
            for (int i = 0; i < attacks.Length; i++)
                if (attacks[i] != null && attacks[i].input == input)
                    return attacks[i];
            return null;
        }
    }
}
