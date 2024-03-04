using UnityEngine;

namespace Inventory.Scripts.Core.Controllers.Draggable
{
    public abstract class ReleaseProcessor : ScriptableObject
    {
        public void Process(ReleaseContext ctx, ReleaseState finalState)
        {
            if (!ShouldProcess(ctx, finalState)) return;

            HandleProcess(ctx, finalState);
        }

        protected abstract void HandleProcess(ReleaseContext ctx, ReleaseState finalState);

        protected virtual bool ShouldProcess(ReleaseContext ctx, ReleaseState finalState)
        {
            return true;
        }
    }
}