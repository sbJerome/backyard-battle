using BB.Data;
using UnityEngine;

namespace BB.Simulation
{
    /// <summary>Something a fighter can hold and use with the Attack button.</summary>
    public interface IHeldItem
    {
        void ThrowFrom(FighterController holder, in InputSnapshot input);
        /// <summary>Release without throwing (holder got launched or KO'd).</summary>
        void Drop();
    }

    /// <summary>
    /// A pickup living in a stage: walk over it to grab (auto-pickup, Streets
    /// of Rage style), press Attack to throw it as an arcing projectile.
    /// Tick-driven by MatchSimulation — no physics components, no wall time —
    /// and thrown hits go through the same ApplyHit pipeline as fighter
    /// attacks, so hitstop/shake/knockback all apply.
    /// NOTE for M4: holder is tracked by object reference; needs an id-based
    /// scheme when state gets replicated.
    /// </summary>
    public class PickupItem : MonoBehaviour, IHeldItem
    {
        public PickupDefinition definition;
        [Tooltip("Visual root toggled off while respawning.")]
        public GameObject visual;

        public enum PickupState : byte { Available, Held, Thrown, Respawning }
        public PickupState CurrentState { get; private set; } = PickupState.Available;
        public FighterController Holder => _holder;

        MatchSimulation _sim;
        Vector2 _spawnPoint;
        Vector2 _position;
        Vector2 _velocity;
        FighterController _holder;
        FighterController _thrower;
        int _respawnTicks;

        const float GrabRadius = 0.9f;

        public Vector2 Position => _position;

        public void Register(MatchSimulation sim)
        {
            _sim = sim;
            _spawnPoint = transform.position;
            _position = _spawnPoint;
            CurrentState = PickupState.Available;
        }

        /// <summary>Advance one simulation tick. Called by MatchSimulation.</summary>
        public void TickPickup()
        {
            switch (CurrentState)
            {
                case PickupState.Available: TickAvailable(); break;
                case PickupState.Held: TickHeld(); break;
                case PickupState.Thrown: TickThrown(); break;
                case PickupState.Respawning:
                    if (--_respawnTicks <= 0)
                    {
                        _position = _spawnPoint;
                        CurrentState = PickupState.Available;
                    }
                    break;
            }

            transform.position = new Vector3(_position.x, _position.y, 0f);
            if (visual != null && visual.activeSelf == (CurrentState == PickupState.Respawning))
                visual.SetActive(CurrentState != PickupState.Respawning);
        }

        void TickAvailable()
        {
            for (int i = 0; i < _sim.Fighters.Count; i++)
            {
                var f = _sim.Fighters[i];
                if (f.State.stateId is FighterStateId.Dead or FighterStateId.Launched or FighterStateId.Tumble) continue;
                if (f.HeldItem != null) continue;
                Vector2 center = f.State.position + Vector2.up * (f.Definition.capsuleHeight * 0.5f);
                if ((center - _position).sqrMagnitude > GrabRadius * GrabRadius) continue;

                _holder = f;
                f.HeldItem = this;
                CurrentState = PickupState.Held;
                return;
            }
        }

        void TickHeld()
        {
            if (_holder == null || _holder.State.stateId == FighterStateId.Dead)
            {
                Drop();
                return;
            }
            // Float above the holder's head.
            _position = _holder.State.position + Vector2.up * (_holder.Definition.capsuleHeight + 0.45f);
        }

        void TickThrown()
        {
            float dt = FighterController.TickDelta;
            _velocity.y -= definition.flightGravity * dt;
            _position += _velocity * dt;

            // Hit the first fighter (other than the thrower) we touch.
            for (int i = 0; i < _sim.Fighters.Count; i++)
            {
                var f = _sim.Fighters[i];
                if (f == _thrower || f.State.stateId == FighterStateId.Dead) continue;
                Vector2 center = f.State.position + Vector2.up * (f.Definition.capsuleHeight * 0.5f);
                float hitRange = definition.radius + f.Definition.capsuleRadius + 0.15f;
                if ((center - _position).sqrMagnitude > hitRange * hitRange) continue;

                var hit = new HitboxWindow
                {
                    damage = definition.damage,
                    angleDegrees = definition.angleDegrees,
                    baseKnockback = definition.baseKnockback,
                    knockbackGrowth = definition.knockbackGrowth,
                    hitstopTicks = definition.hitstopTicks,
                };
                int facing = _velocity.x >= 0f ? 1 : -1;
                f.ApplyHit(in hit, facing);
                if (_thrower != null)
                    _sim.Hits.Raise(new HitEvent(_thrower, f, in hit));
                BeginRespawn();
                return;
            }

            // Flew out of the world.
            if (!_sim.Stage.blastZone.Contains(_position))
                BeginRespawn();
        }

        void BeginRespawn()
        {
            _thrower = null;
            _respawnTicks = Mathf.RoundToInt(definition.respawnSeconds * FighterController.TickRate);
            CurrentState = PickupState.Respawning;
        }

        // ---- IHeldItem -----------------------------------------------------

        public void ThrowFrom(FighterController holder, in InputSnapshot input)
        {
            if (CurrentState != PickupState.Held || holder != _holder) return;

            _thrower = holder;
            _holder.HeldItem = null;
            _holder = null;
            _velocity = new Vector2(definition.throwSpeed * holder.State.facing, definition.throwUpward);
            CurrentState = PickupState.Thrown;
        }

        public void Drop()
        {
            if (_holder != null)
            {
                _position = _holder.State.position + Vector2.up * 0.3f;
                _holder.HeldItem = null;
                _holder = null;
            }
            if (CurrentState == PickupState.Held)
                CurrentState = PickupState.Available;
        }
    }
}
