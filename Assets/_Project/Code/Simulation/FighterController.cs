using BB.Data;
using UnityEngine;

namespace BB.Simulation
{
    /// <summary>
    /// The kinematic fighter motor. Tick-driven: MatchSimulation calls
    /// <see cref="Tick"/> at a fixed 60 Hz with an InputSnapshot; nothing here
    /// reads devices, wall-clock time, or frame delta. All gameplay state lives
    /// in the <see cref="State"/> struct so it can be predicted/reconciled (M4).
    ///
    /// Simulation is locked to the XY plane (z = 0); visuals are full 3D.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class FighterController : MonoBehaviour
    {
        public const float TickRate = 60f;
        public const float TickDelta = 1f / TickRate;
        const float GroundSnapDistance = 0.08f;
        const float SkinWidth = 0.02f;
        const int RespawnIntangibleTicks = 120;

        public FighterDefinition Definition { get; private set; }
        public int PlayerIndex { get; private set; }
        public FighterState State;

        [Tooltip("Layers the motor collides with (stage geometry).")]
        public LayerMask groundMask = ~0;

        Rigidbody _body;
        CapsuleCollider _capsule;

        /// <summary>Raised when this fighter takes a hit (already applied to state).</summary>
        public event System.Action<FighterController, HitboxWindow> OnHitTaken;

        public void Init(FighterDefinition definition, int playerIndex, Vector2 spawnAt, byte stocks)
        {
            Definition = definition;
            PlayerIndex = playerIndex;
            State = FighterState.Spawn(spawnAt, stocks);

            _body = GetComponent<Rigidbody>();
            _body.isKinematic = true;
            _body.useGravity = false;

            _capsule = GetComponent<CapsuleCollider>();
            if (_capsule == null) _capsule = gameObject.AddComponent<CapsuleCollider>();
            _capsule.radius = definition.capsuleRadius;
            _capsule.height = definition.capsuleHeight;
            _capsule.center = new Vector3(0f, definition.capsuleHeight * 0.5f, 0f);

            SyncTransform();
        }

        /// <summary>Advance the fighter one simulation tick.</summary>
        public void Tick(in InputSnapshot input)
        {
            if (State.stateId == FighterStateId.Dead) return;

            // Hitstop freezes everything (both parties of a hit).
            if (State.hitstopTicks > 0)
            {
                State.hitstopTicks--;
                State.previousInput = input;
                return;
            }

            if (State.intangibleTicks > 0) State.intangibleTicks--;

            switch (State.stateId)
            {
                case FighterStateId.Launched:
                case FighterStateId.Tumble:
                    TickLaunched(input);
                    break;
                case FighterStateId.Attack:
                    TickAttack(input);
                    break;
                default:
                    TickMovement(input);
                    break;
            }

            State.stateTicks++;
            State.previousInput = input;
            SyncTransform();
        }

        // ---- states ------------------------------------------------------

        void TickMovement(in InputSnapshot input)
        {
            var def = Definition;

            // Facing follows intent on the ground.
            if (State.grounded && Mathf.Abs(input.moveX) > 0.2f)
                State.facing = (sbyte)(input.moveX > 0f ? 1 : -1);

            // Horizontal acceleration.
            float targetSpeed = input.moveX * (State.grounded ? def.runSpeed : def.airSpeed);
            float accel = State.grounded ? def.groundAcceleration : def.airAcceleration;
            if (State.grounded && Mathf.Approximately(input.moveX, 0f))
                accel = def.groundFriction;
            State.velocity.x = Mathf.MoveTowards(State.velocity.x, targetSpeed, accel * TickDelta);

            // Jumps.
            if (input.WasPressed(FighterButtons.Jump, State.previousInput))
            {
                if (State.grounded)
                {
                    State.velocity.y = def.jumpVelocity;
                    State.grounded = false;
                    State.fastFalling = false;
                }
                else if (State.jumpsRemaining > 0)
                {
                    State.jumpsRemaining--;
                    State.velocity.y = def.airJumpVelocity;
                    State.fastFalling = false;
                }
            }

            // Fast fall: flick down while falling.
            if (!State.grounded && State.velocity.y < 0f && input.moveY < -0.6f)
                State.fastFalling = true;

            ApplyGravity(def);
            MoveAndCollide();

            // Attacks (specials share the mapping table; see MapAttackInput).
            if (input.WasPressed(FighterButtons.Attack, State.previousInput) ||
                input.WasPressed(FighterButtons.Special, State.previousInput))
            {
                bool special = input.WasPressed(FighterButtons.Special, State.previousInput);
                var attack = def.FindAttack(MapAttackInput(input, special));
                if (attack != null) BeginAttack(attack);
            }

            SetState(State.grounded
                ? (Mathf.Abs(State.velocity.x) > 0.1f ? FighterStateId.Run : FighterStateId.Idle)
                : FighterStateId.Airborne);
        }

        void TickAttack(in InputSnapshot input)
        {
            var attack = CurrentAttack;
            if (attack == null) { SetState(FighterStateId.Idle); return; }

            // Attacks keep momentum but no steering (demo simplification;
            // aerial drift during attacks lands with M1 tuning).
            ApplyGravity(Definition);
            if (State.grounded)
                State.velocity.x = Mathf.MoveTowards(State.velocity.x, 0f, Definition.groundFriction * TickDelta);
            MoveAndCollide();

            if (State.stateTicks >= attack.TotalTicks)
            {
                State.attackIndex = -1;
                SetState(State.grounded ? FighterStateId.Idle : FighterStateId.Airborne);
            }
        }

        void TickLaunched(in InputSnapshot input)
        {
            var def = Definition;
            ApplyGravity(def);
            // Light air drag on launch trajectories so they arc, not orbit.
            State.velocity.x = Mathf.MoveTowards(State.velocity.x, 0f, 2f * TickDelta);
            MoveAndCollide();

            if (State.hitstunTicks > 0)
            {
                State.hitstunTicks--;
                if (State.hitstunTicks == 0)
                    SetState(FighterStateId.Tumble);
            }
            else if (State.grounded)
            {
                SetState(FighterStateId.Idle);
            }
            else if (State.stateId == FighterStateId.Tumble &&
                     input.WasPressed(FighterButtons.Jump, State.previousInput) &&
                     State.jumpsRemaining > 0)
            {
                // Escape tumble with a jump.
                State.jumpsRemaining--;
                State.velocity.y = def.airJumpVelocity;
                SetState(FighterStateId.Airborne);
            }
        }

        // ---- hits ----------------------------------------------------------

        public AttackDefinition CurrentAttack =>
            State.attackIndex >= 0 && State.attackIndex < Definition.attacks.Length
                ? Definition.attacks[State.attackIndex]
                : null;

        void BeginAttack(AttackDefinition attack)
        {
            State.attackIndex = (short)System.Array.IndexOf(Definition.attacks, attack);
            SetState(FighterStateId.Attack);
        }

        /// <summary>Apply a hit to this fighter. Called only by HitResolver (server-side at M4).</summary>
        public void ApplyHit(in HitboxWindow hit, int attackerFacing)
        {
            if (State.intangibleTicks > 0 || State.stateId == FighterStateId.Dead) return;

            State.percent = Mathf.Min(999f, State.percent + hit.damage);
            float kb = KnockbackFormula.Knockback(State.percent, hit.damage, Definition.weight,
                                                  hit.baseKnockback, hit.knockbackGrowth);
            State.velocity = KnockbackFormula.LaunchVelocity(kb, hit.angleDegrees, attackerFacing);
            State.hitstunTicks = KnockbackFormula.HitstunTicks(kb);
            State.hitstopTicks = hit.hitstopTicks;
            State.grounded = false;
            State.fastFalling = false;
            State.attackIndex = -1;
            SetState(FighterStateId.Launched);
            OnHitTaken?.Invoke(this, hit);
        }

        /// <summary>KO + respawn (or death when out of stocks). Called by MatchSimulation on blast-zone exit.</summary>
        public void HandleKO(Vector2 respawnAt)
        {
            if (State.stocks > 0) State.stocks--;
            if (State.stocks == 0)
            {
                SetState(FighterStateId.Dead);
                return;
            }

            State.position = respawnAt;
            State.velocity = Vector2.zero;
            State.percent = 0f;
            State.attackIndex = -1;
            State.hitstunTicks = 0;
            State.hitstopTicks = 0;
            State.jumpsRemaining = (byte)Definition.extraJumps;
            State.intangibleTicks = RespawnIntangibleTicks;
            SetState(FighterStateId.Respawning);
            SyncTransform();
        }

        // ---- motor internals ----------------------------------------------

        void ApplyGravity(FighterDefinition def)
        {
            if (State.grounded) return;
            float maxFall = State.fastFalling ? def.fastFallSpeed : def.maxFallSpeed;
            State.velocity.y = Mathf.Max(State.velocity.y - def.gravity * TickDelta, -maxFall);
        }

        /// <summary>
        /// Swept capsule movement: horizontal then vertical, with ground snap.
        /// Deliberately simple — no slopes/ledge-grab yet (M1 work).
        /// </summary>
        void MoveAndCollide()
        {
            Vector2 delta = State.velocity * TickDelta;

            if (!Mathf.Approximately(delta.x, 0f) &&
                SweepBlocked(new Vector3(Mathf.Sign(delta.x), 0f, 0f), Mathf.Abs(delta.x), out float allowedX))
            {
                delta.x = Mathf.Sign(delta.x) * allowedX;
                State.velocity.x = 0f;
            }
            State.position.x += delta.x;

            bool wasGrounded = State.grounded;
            if (delta.y <= 0f)
            {
                float drop = Mathf.Max(Mathf.Abs(delta.y), wasGrounded ? GroundSnapDistance : 0f);
                if (SweepBlocked(Vector3.down, drop, out float allowedDown))
                {
                    State.position.y -= allowedDown;
                    State.velocity.y = 0f;
                    if (!State.grounded)
                    {
                        State.grounded = true;
                        State.fastFalling = false;
                        State.jumpsRemaining = (byte)Definition.extraJumps;
                    }
                }
                else
                {
                    State.position.y += delta.y;
                    State.grounded = false;
                }
            }
            else
            {
                if (SweepBlocked(Vector3.up, delta.y, out float allowedUp))
                {
                    State.position.y += allowedUp;
                    State.velocity.y = 0f;
                }
                else
                {
                    State.position.y += delta.y;
                }
                State.grounded = false;
            }

            State.position = new Vector2(State.position.x, State.position.y); // z stays 0 via SyncTransform
        }

        bool SweepBlocked(Vector3 direction, float distance, out float allowedDistance)
        {
            GetCapsuleWorld(out Vector3 p1, out Vector3 p2, out float radius);
            if (Physics.CapsuleCast(p1, p2, radius - SkinWidth, direction, out RaycastHit hit,
                                    distance + SkinWidth, groundMask, QueryTriggerInteraction.Ignore))
            {
                allowedDistance = Mathf.Max(0f, hit.distance - SkinWidth);
                return true;
            }
            allowedDistance = distance;
            return false;
        }

        public void GetCapsuleWorld(out Vector3 p1, out Vector3 p2, out float radius)
        {
            var def = Definition;
            radius = def.capsuleRadius;
            Vector3 basePos = new Vector3(State.position.x, State.position.y, 0f);
            p1 = basePos + new Vector3(0f, radius, 0f);
            p2 = basePos + new Vector3(0f, def.capsuleHeight - radius, 0f);
        }

        void SetState(FighterStateId id)
        {
            if (State.stateId == id) return;
            State.stateId = id;
            State.stateTicks = 0;
        }

        void SyncTransform()
        {
            transform.position = new Vector3(State.position.x, State.position.y, 0f);
            transform.rotation = Quaternion.Euler(0f, State.facing >= 0 ? 90f : -90f, 0f);
        }

        /// <summary>Demo mapping: direction + button → attack slot.</summary>
        static AttackInput MapAttackInput(in InputSnapshot input, bool special)
        {
            bool up = input.moveY > 0.5f;
            bool down = input.moveY < -0.5f;
            bool side = Mathf.Abs(input.moveX) > 0.5f;

            if (special)
            {
                if (up) return AttackInput.UpSpecial;
                if (down) return AttackInput.DownSpecial;
                if (side) return AttackInput.SideSpecial;
                return AttackInput.NeutralSpecial;
            }
            if (up) return AttackInput.UpTilt;
            if (down) return AttackInput.DownTilt;
            if (side) return AttackInput.ForwardTilt;
            return AttackInput.Jab;
        }
    }
}
