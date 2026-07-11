using BB.Data;
using BB.Simulation;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BB.Core
{
    /// <summary>
    /// One seat in a match: who is playing, as what, controlled by which input
    /// source. Fighters never own devices — couch pads, bots, and (at M4)
    /// network peers all arrive here as an IInputSource. This is the seam that
    /// makes couch, online, bots, and replays interchangeable.
    /// </summary>
    public class PlayerSlot
    {
        public int Index { get; }
        public string DisplayName { get; set; }
        public FighterDefinition Fighter { get; set; }
        public int SkinIndex { get; set; }
        public int Team { get; set; } = -1; // -1 = free-for-all
        public IInputSource InputSource { get; set; }
        public bool IsBot => InputSource is BotInputSource;

        public PlayerSlot(int index)
        {
            Index = index;
            DisplayName = $"Player {index + 1}";
        }
    }

    /// <summary>
    /// IInputSource over the Unity Input System: samples a PlayerInput's
    /// actions into a plain InputSnapshot once per tick. This is the only
    /// place in the game allowed to touch devices.
    /// </summary>
    public sealed class DeviceInputSource : IInputSource
    {
        readonly InputAction _move;
        readonly InputAction _jump;
        readonly InputAction _attack;
        readonly InputAction _special;
        readonly InputAction _shield;
        readonly InputAction _grab;
        readonly InputAction _taunt;

        public DeviceInputSource(InputActionMap fighterMap)
        {
            _move = fighterMap.FindAction("Move", throwIfNotFound: true);
            _jump = fighterMap.FindAction("Jump", throwIfNotFound: true);
            _attack = fighterMap.FindAction("Attack", throwIfNotFound: true);
            _special = fighterMap.FindAction("Special", throwIfNotFound: true);
            _shield = fighterMap.FindAction("Shield", throwIfNotFound: false);
            _grab = fighterMap.FindAction("Grab", throwIfNotFound: false);
            _taunt = fighterMap.FindAction("Taunt", throwIfNotFound: false);
        }

        public InputSnapshot ReadTick()
        {
            Vector2 move = _move.ReadValue<Vector2>();
            FighterButtons held = FighterButtons.None;
            if (_jump.IsPressed()) held |= FighterButtons.Jump;
            if (_attack.IsPressed()) held |= FighterButtons.Attack;
            if (_special.IsPressed()) held |= FighterButtons.Special;
            if (_shield != null && _shield.IsPressed()) held |= FighterButtons.Shield;
            if (_grab != null && _grab.IsPressed()) held |= FighterButtons.Grab;
            if (_taunt != null && _taunt.IsPressed()) held |= FighterButtons.Taunt;

            return new InputSnapshot { moveX = move.x, moveY = move.y, held = held };
        }
    }
}
