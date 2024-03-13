using Inventory.Scripts.Core.Enums;
using Inventory.Scripts.Core.Helper;
using Inventory.Scripts.Core.Items.Grids;
using Inventory.Scripts.Core.ItemsMetadata;
using Photon.Pun;
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

            GridResponse inventoryMessages = GridResponse.NoGridTableSelected;

            // TODO: 객체 태그로 Grid 특정, 추후 수정 필요할 수 있음
            // 아이템을 넣으려는 Grid가 필드 컨테이너이고 넣으려는 아이템이 컨테이너(가방, 조끼, 지갑) 종류라면
            if (itemTable.InventoryMetadata is ContainerMetadata containerMetadata)
            {
                var gridsInventory = containerMetadata.GridsInventory;  // 아이템의 Grid를 모두 가져온 뒤

                if (selectedAbstractGrid.transform.parent.CompareTag("Container"))
                {
                    // 컨테이너 내부가 비어있지 않은 경우
                    foreach (GridTable inventoryGridTable in gridsInventory)    // 반복문으로 확인
                    {
                        if (inventoryGridTable.GetAllItemsFromGrid().Length > 0)    // 내부가 비어있지 않다면
                        {   // 아이템 이동 불가능
                            finalState.Placed = false;
                            return;
                        }
                    }

                    // 컨테이너 내부가 모두 비어있는 경우
                    foreach (GridTable inventoryGridTable in gridsInventory)
                    {
                        inventoryGridTable._isLocked = true;    // 내부 Grid를 전부 잠궈버리고
                    }

                    inventoryMessages = gridTable.PlaceItem(itemTable, posX, posY); // 아이템 배치
                }
                else
                {   // 필드 컨테이너가 아닌 곳에 넣는 경우
                    foreach (GridTable inventoryGridTable in gridsInventory)
                    {   // 내부 Grid 잠금 해제
                        inventoryGridTable._isLocked = false;
                    }
                }
            }

            if (!gridTable._isLocked)
            {   // Grid가 잠겨있지 않은 경우만
                inventoryMessages = gridTable.PlaceItem(itemTable, posX, posY);
            }

            //var inventoryMessages = gridTable.PlaceItem(itemTable, posX, posY);

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