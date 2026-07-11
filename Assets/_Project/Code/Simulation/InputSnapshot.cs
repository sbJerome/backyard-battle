using System;

namespace BB.Simulation
{
    [Flags]
    public enum FighterButtons : byte
    {
        None    = 0,
        Jump    = 1 << 0,
        Attack  = 1 << 1,
        Special = 1 << 2,
        Shield  = 1 << 3,
        Grab    = 1 << 4,
        Taunt   = 1 << 5,
    }

    /// <summary>
    /// One tick of player intent. This is the ONLY thing that may drive the
    /// simulation — never read devices or wall-clock time inside gameplay code.
    /// Plain struct on purpose: it is what gets replicated at M4, and what a
    /// future rollback sim would consume.
    /// </summary>
    [Serializable]
    public struct InputSnapshot : IEquatable<InputSnapshot>
    {
        /// <summary>Horizontal stick, -1..1.</summary>
        public float moveX;
        /// <summary>Vertical stick, -1..1 (down is negative).</summary>
        public float moveY;
        /// <summary>Buttons held this tick. Edges are derived by comparing to the previous tick.</summary>
        public FighterButtons held;

        public bool IsHeld(FighterButtons b) => (held & b) != 0;

        /// <summary>True if the button is held now but was not held in <paramref name="previous"/>.</summary>
        public bool WasPressed(FighterButtons b, in InputSnapshot previous)
            => (held & b) != 0 && (previous.held & b) == 0;

        public bool Equals(InputSnapshot other)
            => moveX == other.moveX && moveY == other.moveY && held == other.held;

        public override bool Equals(object obj) => obj is InputSnapshot s && Equals(s);
        public override int GetHashCode() => HashCode.Combine(moveX, moveY, (byte)held);
    }

    /// <summary>
    /// Where a fighter's inputs come from. Local device, bot brain, or (at M4)
    /// the network. Fighters never know which.
    /// </summary>
    public interface IInputSource
    {
        InputSnapshot ReadTick();
    }

    /// <summary>Placeholder brain: stands still. Useful as a training dummy from day one.</summary>
    public sealed class BotInputSource : IInputSource
    {
        public InputSnapshot ReadTick() => default;
    }
}
