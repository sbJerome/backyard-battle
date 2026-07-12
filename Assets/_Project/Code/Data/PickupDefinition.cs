using UnityEngine;

namespace BB.Data
{
    /// <summary>
    /// A backyard pickup (acorn, rubber band, firecracker...). Power Stone
    /// rule: items are the mid-match drama. Walk over to grab; Attack throws.
    /// </summary>
    [CreateAssetMenu(menuName = "Backyard Battle/Pickup Definition", fileName = "Pickup_")]
    public class PickupDefinition : ScriptableObject
    {
        public string id;
        public string displayName;
        [TextArea] public string flavor;

        [Header("Throw")]
        public float throwSpeed = 14f;
        public float throwUpward = 3f;
        public float flightGravity = 25f;
        [Tooltip("Contact radius of the thrown projectile.")]
        public float radius = 0.25f;

        [Header("On hit (same knockback pipeline as attacks)")]
        public float damage = 9f;
        public float angleDegrees = 40f;
        public float baseKnockback = 55f;
        public float knockbackGrowth = 110f;
        public int hitstopTicks = 5;

        [Header("Lifecycle")]
        public float respawnSeconds = 8f;
    }
}
