using System.Collections.Generic;
using Inventory.Scripts.Core.Enums;
using Inventory.Scripts.Core.Holders;
using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.Items.Grids;
using Inventory.Scripts.Core.ScriptableObjects.Configuration;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Anchors;
using Inventory.Scripts.Core.ScriptableObjects.Items;
using UnityEngine;

namespace Inventory.Scripts.Core.ScriptableObjects
{
    // [CreateAssetMenu(menuName = "Inventory/Supplier/InventorySupplierSo")]
    public class InventorySupplierSo : ScriptableObject
    {
        [Header("Inventory Settings Anchor So")] [SerializeField]
        private InventorySettingsAnchorSo inventorySettingsAnchorSo;

        [Header("Abstract Grid Selected AnchorSo")] [SerializeField]
        private AbstractGridSelectedAnchorSo abstractGridSelectedAnchorSo;

        /// <summary>
        /// Will place the item inside the grid, and also remove from the previous place (if was on a holder, going to unequipped and then place inside the grid)
        /// </summary>
        /// <param name="itemDataSo">The item data so which will be placed (Will create an ItemTable)</param>
        /// <param name="gridTable">The grid which will be placed</param>
        /// <returns>ItemTable created and also the Grid Response to see if was inserted or not</returns>
        public (ItemTable, GridResponse) PlaceItem(ItemDataSo itemDataSo, GridTable gridTable)
        {
            var itemTable = new ItemTable(itemDataSo, abstractGridSelectedAnchorSo);

            var inserted = PlaceItem(itemTable, gridTable);

            return (itemTable, inserted);
        }

        /// <summary>
        /// Will place the item inside the grid, and also remove from the previous place (if was on a holder, going to unequipped and then place inside the grid)
        /// </summary>
        /// <param name="itemTable">Item Table that is the item serialization</param>
        /// <param name="gridTable">The grid which will be placed</param>
        /// <returns>Grid Response with the result if was inserted or not</returns>
        public GridResponse PlaceItem(ItemTable itemTable, GridTable gridTable)
        {
            if (gridTable == null)
            {
                return GridResponse.NoGridTableSelected;
            }

            var posOnGrid = gridTable.FindSpaceForObjectAnyDirection(itemTable);

            if (posOnGrid == null)
            {
                return GridResponse.InventoryFull;
            }

            var inventoryMessage = gridTable.PlaceItem(itemTable, posOnGrid.Value.x, posOnGrid.Value.y);

            return inventoryMessage;
        }

        /// <summary>
        /// Will find a space for the item inside the list of grids.
        /// </summary>
        /// <param name="itemDataSo">Item will be inserted</param>
        /// <param name="grids">Grids to find a place for the item</param>
        /// <returns>GridResponse if the item was inserted or not</returns>
        public (ItemTable item, GridResponse gridResponse) FindPlaceForItemInGrids(ItemDataSo itemDataSo,
            List<GridTable> grids)
        {
            foreach (var gridTable in grids)
            {
                var (item, gridResponse) = PlaceItem(itemDataSo, gridTable);

                if (gridResponse == GridResponse.Inserted)
                {
                    return (item, gridResponse);
                }
            }

            return (null, GridResponse.InventoryFull);
        }

        /// <summary>
        /// Remove the item from grid or holder.
        /// </summary>
        /// <param name="itemTable">The item table will be removed/unequipped</param>
        public void RemoveItem(ItemTable itemTable)
        {
            var currentGridTable = itemTable.CurrentGridTable;

            if (currentGridTable != null)
            {
                currentGridTable.RemoveItem(itemTable);
                return;
            }

            var currentItemHolder = itemTable.CurrentItemHolder;

            if (currentItemHolder != null)
            {
                currentItemHolder.UnEquip(itemTable);
            }
        }

        /// <summary>
        /// Will try to equip on a Holder.
        /// </summary>
        /// <param name="itemDataSo">The item data so which will be created the ItemTable and then equip.</param>
        /// <param name="itemHolder">The holder which will be equipped</param>
        /// <returns>ItemTable created and the HolderResponse with the result if was equipped or not</returns>
        public (ItemTable, HolderResponse) TryEquipItem(ItemDataSo itemDataSo, ItemHolder itemHolder)
        {
            var itemTable = new ItemTable(itemDataSo, abstractGridSelectedAnchorSo);

            if (itemHolder == null)
            {
                return (null, HolderResponse.NoItemHolderSelected);
            }

            var holderResponse = TryEquipItem(itemTable, itemHolder);

            return (itemTable, holderResponse);
        }

        /// <summary>
        /// Will try to equip on a Holder.
        /// </summary>
        /// <param name="itemTable">The item which will be equipped</param>
        /// <param name="itemHolder">The holder which to equip</param>
        /// <returns>HolderResponse with the result if was equipped or not</returns>
        public HolderResponse TryEquipItem(ItemTable itemTable, ItemHolder itemHolder)
        {
            return itemHolder == null ? HolderResponse.NoItemHolderSelected : itemHolder.TryEquipItem(itemTable);
        }

        /// <summary>
        /// Initialize the Grid for Environment Containers
        /// </summary>
        /// <param name="itemContainerDataSo">Container Item which will be initialized</param>
        /// <param name="environmentGoTransform">The environment container transform for parent</param>
        /// <returns>The Item Table created</returns>
        public ItemTable InitializeEnvironmentContainer(ItemContainerDataSo itemContainerDataSo,
            Transform environmentGoTransform)
        {
            var itemPrefab = GetItemPrefabSo().ItemPrefab;

            var instantiate = Instantiate(itemPrefab, environmentGoTransform);

            var abstractItem = instantiate.GetComponent<AbstractItem>();

            abstractItem.Set(itemContainerDataSo);

            abstractItem.gameObject.SetActive(false);

            return abstractItem.ItemTable;
        }

        private ItemPrefabSo GetItemPrefabSo()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.ItemPrefabSo;
        }

        /// <summary>
        /// Will clear all items inside a grid.
        /// </summary>
        /// <param name="grids">Grids which will be cleared</param>
        public void ClearGridTables(List<GridTable> grids)
        {
            foreach (var gridTable in grids)
            {
                ClearGridTable(gridTable);
            }
        }

        /// <summary>
        /// Clear the entire grid. Remove all items
        /// </summary>
        /// <param name="gridTable">Grid which will be cleared</param>
        public void ClearGridTable(GridTable gridTable)
        {
            gridTable.RemoveAllItems();
        }
    }
}