using Inventory.Scripts.Core.Helper;
using Inventory.Scripts.Helper;
using UnityEngine;

namespace Inventory.Scripts.Core.Controllers.Draggable.Processors.Rollbacks
{
    // [CreateAssetMenu(menuName = "Inventory/Configuration/Providers/Processors/Rollback Item to Grid Middleware")]
    public class RollbackItemToGrid : ReleaseProcessor
    {
        protected override void HandleProcess(ReleaseContext ctx, ReleaseState finalState)
        {
            var selectedInventoryItem = ctx.PickupState.Item;

            var itemTable = selectedInventoryItem.ItemTable;

            var itemTableCurrentGridTable = itemTable.CurrentGridTable;

            var inventoryMessages =
                itemTableCurrentGridTable.PlaceItem(itemTable, itemTable.OnGridPositionX, itemTable.OnGridPositionY);

            if (ctx.Debug)
            {
                Debug.Log(("Item rollbacked to grid. Grid: " + itemTableCurrentGridTable + " Status: " +
                           inventoryMessages)
                    .DraggableSystem());
            }
        }

        protected override bool ShouldProcess(ReleaseContext ctx, ReleaseState finalState)
        {
            var selectedInventoryItem = ctx.PickupState.Item;

            var itemTable = selectedInventoryItem.ItemTable;

            return itemTable.CurrentGridTable != null;
        }
    }
}