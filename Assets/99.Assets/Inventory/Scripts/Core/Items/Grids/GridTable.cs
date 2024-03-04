using System;
using System.Collections.Generic;
using System.Linq;
using Inventory.Scripts.Core.Enums;
using Inventory.Scripts.Core.Helper;
using Inventory.Scripts.Core.Items.Grids.Helper;
using Inventory.Scripts.Core.ItemsMetadata;
using UnityEngine;

namespace Inventory.Scripts.Core.Items.Grids
{
    [Serializable]
    public class GridTable
    {
        public int Width { get; }
        public int Height { get; }
        public ItemTable[,] InventoryItemsSlot { get; private set; }

        // UI Handles
        public event Action<ItemTable> OnInsert;
        public event Action<ItemTable> OnRemove;
        private readonly List<ContainerGrids> _containerGridsInParent = new();

        public GridTable(int newWidth, int newHeight)
        {
            Width = newWidth;
            Height = newHeight;
            InventoryItemsSlot = new ItemTable[Width, Height];
        }

        public GridResponse PlaceItem(ItemTable item, int posX, int posY)
        {
            if (!this.BoundaryCheck(posX, posY, item.Width, item.Height))
            {
                return GridResponse.OutOfBounds;
            }

            if (!OverlapCheck(posX, posY, item.Width, item.Height))
            {
                return GridResponse.Overlapping;
            }

            if (IsInsertingInsideYourself(item))
            {
                return GridResponse.InsertInsideYourself;
            }

            for (var x = 0; x < item.Width; x++)
            {
                for (var y = 0; y < item.Height; y++)
                {
                    InventoryItemsSlot[posX + x, posY + y] = item;
                }
            }

            RemoveFromPreviousLocation(item);

            item.SetGridProps(this, posX, posY, null);

            if (item.InventoryMetadata != null)
            {
                item.InventoryMetadata.OnPlaceItem(item);
            }

            UpdateAllGridsOpened(item);

            return GridResponse.Inserted;
        }

        private static void RemoveFromPreviousLocation(ItemTable item)
        {
            item.CurrentGridTable?.RemoveItemFromPreviousGrids(item);
            if (item.CurrentItemHolder != null)
            {
                item.CurrentItemHolder.UnEquip(item);
            }
        }

        public bool OverlapCheck(int posX, int posY, int itemWidth, int itemHeight)
        {
            for (var x = 0; x < itemWidth; x++)
            {
                for (var y = 0; y < itemHeight; y++)
                {
                    var inventoryItem = GetItem(posX + x, posY + y);

                    if (inventoryItem == null) continue;

                    if (inventoryItem.IsDragRepresentation)
                    {
                        continue;
                    }

                    return false;
                }
            }

            return true;
        }

        public ItemTable GetItem(int x, int y)
        {
            try
            {
                return InventoryItemsSlot[x, y];
            }
            catch (IndexOutOfRangeException)
            {
                return null;
            }
            catch (NullReferenceException)
            {
                Debug.LogError(
                    "InventoryItemsSlot not initialized, maybe you change something in the Editor and the reference turns null. Restart the game, everything should fix.");
                return null;
            }
        }

        public bool IsInsertingInsideYourself(ItemTable item)
        {
            if (item == null)
                return false;

            if (!item.IsContainer())
                return false;

            var containerMetadata = (ContainerMetadata)item.InventoryMetadata;

            var gridTables = containerMetadata.GridsInventory;

            if ((from gridTable in gridTables
                    from inventoryItem in gridTable.GetAllContainersFromGrid()
                    select ((ContainerMetadata)inventoryItem.InventoryMetadata).IsInsertingInsideYourself(this))
                .Any(insertingInside => insertingInside))
            {
                return true;
            }

            // TODO: Improve this to not depend on the ContainerGrids MonoBehavior.
            return _containerGridsInParent.Count != 0 &&
                   _containerGridsInParent.Any(containerGrid =>
                       containerMetadata.GetContainerGrids().Contains(containerGrid));
        }

        public void RemoveItem(ItemTable itemTable)
        {
            if (itemTable == null)
            {
                Debug.Log("Cannot remove null item...".Error());
                return;
            }

            PickUpItem(itemTable.OnGridPositionX, itemTable.OnGridPositionY);
            RemoveItemFromPreviousGrids(itemTable);
        }

        public void RemoveAllItems()
        {
            var allItemsFromGrid = GetAllItemsFromGrid();

            foreach (var itemTable in allItemsFromGrid)
            {
                RemoveItem(itemTable);
            }
        }

        public ItemTable PickUpItem(int x, int y)
        {
            var abstractItem = GetItem(x, y);

            if (abstractItem == null) return null;

            for (var ix = 0; ix < abstractItem.Width; ix++)
            {
                for (var iy = 0; iy < abstractItem.Height; iy++)
                {
                    InventoryItemsSlot[abstractItem.OnGridPositionX + ix, abstractItem.OnGridPositionY + iy] = null;
                }
            }

            return abstractItem;
        }

        private void UpdateAllGridsOpened(ItemTable itemTable)
        {
            if (itemTable.IsDragRepresentation) return;

            OnInsert?.Invoke(itemTable);
        }

        public void RemoveItemFromPreviousGrids(ItemTable itemTable)
        {
            if (itemTable.IsDragRepresentation) return;

            OnRemove?.Invoke(itemTable);
        }

        public Vector2Int? FindSpaceForObjectAnyDirection(ItemTable inventoryItemToInsert)
        {
            var posOnGrid = FindSpaceForObject(inventoryItemToInsert);

            if (posOnGrid != null) return posOnGrid;

            var lastRotateState = inventoryItemToInsert.IsRotated;

            inventoryItemToInsert.Rotate();
            posOnGrid = FindSpaceForObject(inventoryItemToInsert);

            if (posOnGrid == null && lastRotateState != inventoryItemToInsert.IsRotated)
            {
                inventoryItemToInsert.Rotate();
            }

            return posOnGrid;
        }

        private Vector2Int? FindSpaceForObject(ItemTable itemToInsert)
        {
            var normalizedHeight = Height - itemToInsert.Height + 1;
            var normalizedWidth = Width - itemToInsert.Width + 1;

            for (var y = 0; y < normalizedHeight; y++)
            {
                for (var x = 0; x < normalizedWidth; x++)
                {
                    if (OverlapCheck(x, y, itemToInsert.Width,
                            itemToInsert.Height))
                    {
                        return new Vector2Int(x, y);
                    }
                }
            }

            return null;
        }

        public ItemTable[] GetAllContainersFromGrid()
        {
            var containerItems = new HashSet<ItemTable>();

            if (InventoryItemsSlot == null)
            {
                return containerItems.ToArray();
            }

            foreach (var inventoryItem in InventoryItemsSlot)
            {
                if (inventoryItem?.InventoryMetadata is ContainerMetadata)
                {
                    containerItems.Add(inventoryItem);
                }
            }

            return containerItems.ToArray();
        }

        public ItemTable[] GetAllItemsFromGrid()
        {
            var inventoryItems = new HashSet<ItemTable>();

            if (InventoryItemsSlot == null)
            {
                return inventoryItems.ToArray();
            }

            foreach (var inventoryItem in InventoryItemsSlot)
            {
                if (inventoryItem != null)
                {
                    inventoryItems.Add(inventoryItem);
                }
            }

            return inventoryItems.ToArray();
        }

        public void AddGrid(AbstractGrid abstractGrid)
        {
            var containerGrids = abstractGrid.GetComponentInParent<ContainerGrids>(true);

            if (containerGrids != null)
            {
                _containerGridsInParent.Add(containerGrids);
            }
        }

        public override string ToString()
        {
            return "Grid (" + Width + ", " + Height + ") ContainerGrids: " + _containerGridsInParent + ", Matrix: " +
                   InventoryItemsSlot;
        }
    }
}