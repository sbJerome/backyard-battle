using System;
using UnityEngine;

namespace BB.Simulation
{
    public enum FighterStateId : byte
    {
        Idle,
        Run,
        Airborne,
        Attack,
        Launched,   // in hitstun, motor integrates launch velocity
        Tumble,     // hitstun over, still flying
        Respawning, // on respawn platform, intangible
        Dead,       // out of stocks
    }

    /// <summary>
    /// The COMPLETE simulation state of a fighter, as one plain struct.
    /// If it isn't in here, it doesn't affect gameplay. This is the struct
    /// Fish-Net reconciles at M4 and a rollback sim would snapshot.
    /// </summary>
    [Serializable]
    public struct FighterState
    {
        public Vector2 position;
        public Vector2 velocity;
        /// <summary>+1 facing right, -1 facing left.</summary>
        public sbyte facing;
        public bool grounded;
        public bool fastFalling;
        public byte jumpsRemaining;

        public FighterStateId stateId;
        /// <summary>Ticks spent in the current state.</summary>
        public int stateTicks;

        /// <summary>Index into FighterDefinition.attacks while stateId == Attack; -1 otherwise.</summary>
        public short attackIndex;
        /// <summary>Increments on every attack start; HitResolver keys hit-dedupe on it.</summary>
        public ushort attackActivationId;

        public float percent;
        public byte stocks;

        public int hitstunTicks;
        public int hitstopTicks;
        public int intangibleTicks;

        /// <summary>Buffered attack input pressed during an attack: 0 none, 1 attack, 2 special.</summary>
        public byte bufferedAttack;
        public int bufferTicks;

        /// <summary>Previous tick's input, kept for edge detection.</summary>
        public InputSnapshot previousInput;

        public static FighterState Spawn(Vector2 at, byte stocks)
        {
            return new FighterState
            {
                position = at,
                facing = 1,
                stocks = stocks,
                attackIndex = -1,
                stateId = FighterStateId.Airborne,
            };
        }
    }
}
