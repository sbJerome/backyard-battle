using System.Collections.Generic;
using BB.Simulation;
using UnityEngine;

namespace BB.Presentation
{
    /// <summary>
    /// Frames every live fighter: fits their bounding box (led slightly by
    /// velocity), clamps to the stage's camera bounds, and zooms smoothly.
    /// Count-agnostic by construction — 2 or 20 fighters, same code.
    /// Long-lens perspective camera for the 2.5D look.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class BrawlCamera : MonoBehaviour
    {
        [Header("Framing")]
        public float padding = 3f;
        public float velocityLead = 0.25f;
        public float minHalfHeight = 4f;
        public float smoothTime = 0.35f;
        [Tooltip("Camera distance is derived from required frame size and FOV.")]
        public float minDistance = 12f;

        public Rect bounds = new Rect(-18f, -8f, 36f, 20f);

        readonly List<FighterController> _targets = new();
        Camera _camera;
        Vector3 _smoothVelocity;

        void Awake() => _camera = GetComponent<Camera>();

        public void SetTargets(IReadOnlyList<FighterController> fighters, Rect cameraBounds)
        {
            _targets.Clear();
            _targets.AddRange(fighters);
            bounds = cameraBounds;
        }

        void LateUpdate()
        {
            if (_targets.Count == 0) return;

            Vector2 min = new(float.MaxValue, float.MaxValue);
            Vector2 max = new(float.MinValue, float.MinValue);
            int alive = 0;

            foreach (var f in _targets)
            {
                if (f == null || f.State.stateId == FighterStateId.Dead) continue;
                alive++;
                Vector2 p = f.State.position + f.State.velocity * velocityLead;
                min = Vector2.Min(min, p);
                max = Vector2.Max(max, p);
            }
            if (alive == 0) return;

            Vector2 center = (min + max) * 0.5f;
            float halfHeight = Mathf.Max(minHalfHeight, (max.y - min.y) * 0.5f + padding);
            float halfWidth = (max.x - min.x) * 0.5f + padding;
            halfHeight = Mathf.Max(halfHeight, halfWidth / _camera.aspect);

            // Clamp the frame inside stage camera bounds.
            float clampedHalfHeight = Mathf.Min(halfHeight, bounds.height * 0.5f);
            float clampedHalfWidth = Mathf.Min(clampedHalfHeight * _camera.aspect, bounds.width * 0.5f);
            center.x = Mathf.Clamp(center.x, bounds.xMin + clampedHalfWidth, bounds.xMax - clampedHalfWidth);
            center.y = Mathf.Clamp(center.y, bounds.yMin + clampedHalfHeight, bounds.yMax - clampedHalfHeight);

            float distance = Mathf.Max(minDistance,
                clampedHalfHeight / Mathf.Tan(_camera.fieldOfView * 0.5f * Mathf.Deg2Rad));

            Vector3 target = new(center.x, center.y, -distance);
            transform.position = Vector3.SmoothDamp(transform.position, target, ref _smoothVelocity, smoothTime);
        }
    }
}
