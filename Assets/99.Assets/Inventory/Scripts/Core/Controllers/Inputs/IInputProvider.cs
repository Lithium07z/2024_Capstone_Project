using System;

namespace Inventory.Scripts.Core.Controllers.Inputs
{
    public interface IInputProvider
    {
        public event Action OnPickupItem;
        public event Action OnReleaseItem;
        public event Action OnToggleOptions;
        public event Action OnRotateItem;

        public InputState GetState();

// #if ENABLE_INPUT_SYSTEM
//         // New input system backends are enabled.
// #endif
//
// #if ENABLE_LEGACY_INPUT_MANAGER
//         // Old input backends are enabled.
// #endif
//
//         // NOTE: Both can be true at the same time as it is possible to select "Both"
//         //       under "Active Input Handling".
    }
}