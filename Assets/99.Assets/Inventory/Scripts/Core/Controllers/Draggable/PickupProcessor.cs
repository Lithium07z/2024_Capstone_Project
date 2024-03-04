using UnityEngine;

namespace Inventory.Scripts.Core.Controllers.Draggable
{
    public abstract class PickupProcessor : ScriptableObject
    {
        public void Process(PickupContext ctx, PickupState finalState)
        {
            if (!ShouldProcess(ctx, finalState)) return;

            HandleProcess(ctx, finalState);
        }

        protected abstract void HandleProcess(PickupContext ctx, PickupState finalState);

        protected virtual bool ShouldProcess(PickupContext ctx, PickupState finalState)
        {
            return true;
        }
    }
}