using System;
using UnityEngine;

namespace Inventory.Scripts.Core.Controllers.Inputs.Middleware
{
    [CreateAssetMenu(menuName = "Inventory/Configuration/Inputs/Application Out Of Focus Middleware")]
    public class ApplicationOutOfFocusMiddleware : InputMiddleware
    {
        public override event Action OnReleaseItem;

        public override void Process(InputState inputState)
        {
            if (!Application.isFocused)
            {
                OnReleaseItem?.Invoke();
            }
        }
    }
}