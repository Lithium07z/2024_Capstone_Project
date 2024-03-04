using Inventory.Scripts.Core.Helper;
using Inventory.Scripts.Helper;
using UnityEngine;

namespace Inventory.Scripts.Core.Controllers.Draggable.Processors.Pickups
{
    // [CreateAssetMenu(menuName = "Inventory/Configuration/Providers/Processors/Pickup Grab Item In Grid Processor")]
    public class PickupItemInGrid : PickupProcessor
    {
        protected override void HandleProcess(PickupContext ctx, PickupState finalState)
        {
            var tileGridHelperSo = ctx.TileGridHelperSo;
            var selectedAbstractGrid = ctx.AbstractGridFromStartDragging;
            var selectedInventoryItem = ctx.SelectedInventoryItem;

            var tileGridPosition =
                tileGridHelperSo.GetTileGridPosition(selectedAbstractGrid.transform, selectedInventoryItem);

            var pickupItem = selectedAbstractGrid.Grid.PickUpItem(tileGridPosition.x, tileGridPosition.y)
                ?.GetAbstractItem();

            if (pickupItem == null)
            {
                return;
            }

            if (pickupItem != null && pickupItem.ItemTable != null)
            {
                if (ctx.Debug)
                {
                    Debug.Log(("Picking item from grid. Item: " + pickupItem.ItemTable.ItemDataSo.DisplayName +
                               " Grid: " + selectedAbstractGrid.name + " PosX: " + tileGridPosition.x +
                               " PosY: " + tileGridPosition.y).DraggableSystem());
                }

                finalState.DraggableItemInitialRotation = pickupItem.ItemTable.IsRotated;
            }

            pickupItem.SetDragStyle();
            finalState.Item = pickupItem;
        }

        protected override bool ShouldProcess(PickupContext ctx, PickupState finalState)
        {
            return ctx.SelectedInventoryItem == null && ctx.AbstractGridFromStartDragging != null;
        }
    }
}