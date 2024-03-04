using Inventory.Scripts.Core.Helper;
using Inventory.Scripts.Helper;
using UnityEngine;

namespace Inventory.Scripts.Core.Controllers.Draggable.Processors.Pickups
{
    // [CreateAssetMenu(menuName = "Inventory/Configuration/Providers/Processors/Pickup Item In Holder Processor")]
    public class PickupItemInHolder : PickupProcessor
    {
        protected override void HandleProcess(PickupContext ctx, PickupState finalState)
        {
            var itemHolderFromPickup = ctx.ItemHolderFromStartDragging;

            var pickupItem = itemHolderFromPickup.GetItemEquipped();

            if (pickupItem == null) return;

            finalState.Item = pickupItem;

            if (ctx.Debug)
            {
                Debug.Log(("Picking item from holder. Item: " + pickupItem.ItemTable.ItemDataSo.DisplayName + " Holder: " +
                           itemHolderFromPickup.name)
                    .DraggableSystem());
            }
        }

        protected override bool ShouldProcess(PickupContext ctx, PickupState finalState)
        {
            var selectedInventoryItem = ctx.SelectedInventoryItem;
            var itemHolderFromPickup = ctx.ItemHolderFromStartDragging;

            return selectedInventoryItem == null && itemHolderFromPickup != null && itemHolderFromPickup.isEquipped;
        }
    }
}