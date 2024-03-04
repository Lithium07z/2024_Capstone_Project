#pragma warning disable 0067
using System;
using UnityEngine;

namespace Inventory.Scripts.Core.Controllers.Inputs.Middleware
{
    public abstract class InputMiddleware : ScriptableObject
    {
        public virtual event Action OnGenerateItem;
        public virtual event Action OnPickupItem;
        public virtual event Action OnReleaseItem;
        public virtual event Action OnToggleOptions;
        public virtual event Action OnRotateItem;

        public abstract void Process(InputState inputState);
    }
}