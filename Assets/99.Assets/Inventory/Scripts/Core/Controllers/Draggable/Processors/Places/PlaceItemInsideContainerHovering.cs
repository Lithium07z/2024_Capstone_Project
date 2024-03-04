using Inventory.Scripts.Core.Enums;
using Inventory.Scripts.Core.Helper;
using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.ItemsMetadata;
using Inventory.Scripts.Helper;
using UnityEngine;

namespace Inventory.Scripts.Core.Controllers.Draggable.Processors.Places
{
    // [CreateAssetMenu(menuName = "Inventory/Configuration/Providers/Processors/Place Item Inside Container Hovering Middleware")]
    public class PlaceItemInsideContainerHovering : ReleaseProcessor
    {
        protected override void HandleProcess(ReleaseContext ctx, ReleaseState finalState)
        {
            var abstractItemHovering = GetItemHover(ctx);

            if (abstractItemHovering.ItemTable.InventoryMetadata is not ContainerMetadata
                containerMetadataFromHoveringItem)
            {
                finalState.Placed = false;
                return;
            }

            var selectedInventoryItem = ctx.PickupState.Item;

            var inventoryMessages =
                containerMetadataFromHoveringItem.PlaceItemInInventory(selectedInventoryItem.ItemTable);

            if (ctx.Debug)
            {
                Debug.Log(("Placing item inside container. Container: " + containerMetadataFromHoveringItem +
                           " Status: " +
                           inventoryMessages).DraggableSystem());
            }

            finalState.Placed = inventoryMessages == GridResponse.Inserted;
        }

        private static AbstractItem GetItemHover(ReleaseContext ctx)
        {
            var tileGridHelperSo = ctx.TileGridHelperSo;

            var abstractGrid = ctx.SelectedAbstractGrid;

            var tileGridPositionByGrid = tileGridHelperSo.GetTileGridPositionByGridTable(abstractGrid.transform);

            return abstractGrid.Grid.GetItem(tileGridPositionByGrid.x, tileGridPositionByGrid.y)?.GetAbstractItem();
        }

        protected override bool ShouldProcess(ReleaseContext ctx, ReleaseState finalState)
        {
            if (ctx.SelectedAbstractGrid == null) return false;

            var abstractItemHovering = GetItemHover(ctx);

            if (abstractItemHovering == null) return false;

            return abstractItemHovering.ItemTable?.InventoryMetadata is ContainerMetadata &&
                   abstractItemHovering.ItemTable != ctx.PickupState.Item.ItemTable;
        }
    }
}