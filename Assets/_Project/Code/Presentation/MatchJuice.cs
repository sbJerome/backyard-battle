using BB.Core;
using BB.Simulation;
using UnityEngine;

namespace BB.Presentation
{
    /// <summary>
    /// Turns sim hit events into feel: screenshake scaled by damage and a quick
    /// hitspark flash at the contact point. (Hitstop is simulation-side — both
    /// fighters already freeze in FighterController.) SFX slots in here at M6.
    /// </summary>
    public class MatchJuice : MonoBehaviour
    {
        [Tooltip("Shake added per point of damage (clamped).")]
        public float shakePerDamage = 0.028f;
        public float maxShakePerHit = 0.45f;
        public float sparkDuration = 0.12f;
        public float sparkSize = 0.9f;

        MatchController _match;
        BrawlCamera _camera;
        Material _sparkMaterial;

        void Start()
        {
            _match = FindAnyObjectByType<MatchController>();
            _camera = FindAnyObjectByType<BrawlCamera>();
            if (_match != null) _match.OnHit += HandleHit;

            var shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null) shader = Shader.Find("Unlit/Color");
            _sparkMaterial = new Material(shader) { color = new Color(1f, 0.95f, 0.6f) };
        }

        void OnDestroy()
        {
            if (_match != null) _match.OnHit -= HandleHit;
        }

        void HandleHit(HitEvent e)
        {
            if (_camera != null)
                _camera.AddShake(Mathf.Min(maxShakePerHit, 0.12f + e.Hit.damage * shakePerDamage));

            Vector3 at = Vector3.Lerp(e.Attacker.transform.position, e.Victim.transform.position, 0.65f)
                         + Vector3.up * (e.Victim.Definition.capsuleHeight * 0.5f);
            SpawnSpark(at);
        }

        void SpawnSpark(Vector3 at)
        {
            var spark = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            spark.name = "HitSpark";
            Destroy(spark.GetComponent<Collider>());
            spark.transform.position = at;
            spark.transform.localScale = Vector3.one * 0.2f;
            spark.GetComponent<MeshRenderer>().sharedMaterial = _sparkMaterial;
            var anim = spark.AddComponent<HitSpark>();
            anim.duration = sparkDuration;
            anim.targetScale = sparkSize;
        }
    }

    /// <summary>Grows and dies. That's it. Replaced by real VFX at M6.</summary>
    public class HitSpark : MonoBehaviour
    {
        public float duration = 0.12f;
        public float targetScale = 0.9f;
        float _age;

        void Update()
        {
            _age += Time.deltaTime;
            float t = _age / duration;
            if (t >= 1f) { Destroy(gameObject); return; }
            transform.localScale = Vector3.one * Mathf.Lerp(0.2f, targetScale, t * t);
        }
    }
}
