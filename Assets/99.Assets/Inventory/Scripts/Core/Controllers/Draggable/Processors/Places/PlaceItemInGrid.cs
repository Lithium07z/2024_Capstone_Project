using Inventory.Scripts.Core.Enums;
using Inventory.Scripts.Core.Helper;
using Inventory.Scripts.Core.Items.Grids;
using Inventory.Scripts.Core.ItemsMetadata;
using Inventory.Scripts.Core.ScriptableObjects.Items;
using Inventory.Scripts.Helper;
using System;
using UnityEngine;

namespace Inventory.Scripts.Core.Controllers.Draggable.Processors.Places
{
    // [CreateAssetMenu(menuName = "Inventory/Configuration/Providers/Processors/Place Item in Grid Middleware")]
    public class PlaceItemInGrid : ReleaseProcessor
    {
        protected override void HandleProcess(ReleaseContext ctx, ReleaseState finalState)
        {
            var selectedAbstractGrid = ctx.SelectedAbstractGrid;
            var selectedInventoryItem = ctx.PickupState.Item;
            var itemTable = selectedInventoryItem.ItemTable;

            var gridTable = selectedAbstractGrid.Grid;

            var ctxTileGridPosition = GetTileGridPosition(ctx);

            if (!ctxTileGridPosition.HasValue)
            {
                finalState.Placed = false;
                return;
            }
 
            var posX = ctxTileGridPosition.Value.x;
            var posY = ctxTileGridPosition.Value.y;

            // TODO: 객체 이름으로 Grid 특정하는 방법 추후 수정해야 함
            // 아이템을 넣으려는 Grid가 필드 컨테이너이고 넣으려는 아이템이 컨테이너(가방, 조끼, 지갑) 종류라면
            if (selectedAbstractGrid.transform.parent.name.Equals("Box_Container(Clone)") && itemTable.InventoryMetadata is ContainerMetadata containerMetadata)
            {
                var gridsInventory = containerMetadata.GridsInventory;  // 아이템의 Grid를 모두 가져온 뒤

                foreach (GridTable inventoryGridTable in gridsInventory)    // 반복문으로 확인
                {
                    if (inventoryGridTable.GetAllItemsFromGrid().Length > 0)    // 내부가 비어있지 않다면
                    {   // 아이템 이동 불가능
                        finalState.Placed = false;
                        return;
                    }
                }
            }

            var inventoryMessages = gridTable.PlaceItem(itemTable, posX, posY);

            if (ctx.Debug)
            {
                Debug.Log(("Placing item in grid. Grid: " + selectedAbstractGrid + " Status: " + inventoryMessages)
                    .DraggableSystem());
            }

            finalState.Placed = inventoryMessages == GridResponse.Inserted;
        }

        private Vector2Int? GetTileGridPosition(ReleaseContext ctx)
        {
            var tileGridHelperSo = ctx.TileGridHelperSo;

            var selectedAbstractGrid = ctx.SelectedAbstractGrid;
            var selectedInventoryItem = ctx.PickupState.Item;

            if (selectedAbstractGrid == null) return null;

            return tileGridHelperSo.GetTileGridPosition(selectedAbstractGrid.transform, selectedInventoryItem);
        }

        protected override bool ShouldProcess(ReleaseContext ctx, ReleaseState finalState)
        {
            var selectedAbstractGrid = ctx.SelectedAbstractGrid;

            return selectedAbstractGrid != null;
        }
    }
}