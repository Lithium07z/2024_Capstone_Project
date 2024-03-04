using Inventory.Scripts.Core.Enums;
using Inventory.Scripts.Core.Helper;
using Inventory.Scripts.Helper;
using UnityEngine;

namespace Inventory.Scripts.Core.Controllers.Draggable.Processors.Places
{
    // [CreateAssetMenu(menuName = "Inventory/Configuration/Providers/Processors/Place Item in Holder Middleware")]
    public class PlaceItemInHolder : ReleaseProcessor
    {
        protected override void HandleProcess(ReleaseContext ctx, ReleaseState finalState)
        {
            var selectedItemHolder = ctx.SelectedItemHolder;
            var selectedInventoryItem = ctx.PickupState.Item;
            var itemTable = selectedInventoryItem.ItemTable;

            var equippableMessages = selectedItemHolder.TryEquipItem(itemTable);

            if (ctx.Debug)
            {
                Debug.Log(("Placing item in holder. Holder: " + selectedItemHolder + " Status: " + equippableMessages)
                    .DraggableSystem());
            }

            finalState.Placed = equippableMessages == HolderResponse.Equipped;
        }

        protected override bool ShouldProcess(ReleaseContext ctx, ReleaseState finalState)
        {
            var selectedItemHolder = ctx.SelectedItemHolder;

            return selectedItemHolder != null;
        }
    }
}