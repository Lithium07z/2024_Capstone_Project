using System;
using System.Collections.Generic;
using System.Linq;
using Inventory.Scripts.Core.Enums;
using Inventory.Scripts.Core.Helper;
using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.Items.Grids;
using Inventory.Scripts.Core.Items.Grids.Renderer;
using Inventory.Scripts.Core.ScriptableObjects.Items;
using Inventory.Scripts.Core.ScriptableObjects.Options;
using UnityEngine;

namespace Inventory.Scripts.Core.ItemsMetadata
{
    [Serializable]
    public class ContainerMetadata : InventoryMetadata
    {
        private List<ContainerGrids> UIGridsOpened { get; } = new();

        private RendererGrids _rendererGrids;

        [field: SerializeReference] public List<GridTable> GridsInventory { get; private set; }

        public ContainerMetadata(ItemTable itemTable) : base(itemTable)
        {
            SetProps();
        }

        private void SetProps()
        {
            GridsInventory = new List<GridTable>();

            var containerGridsPrefab = GetPrefabGrids();
            var abstractGrids = containerGridsPrefab.GetAbstractGrids();

            foreach (var abstractGrid in abstractGrids)
            {
                GridsInventory.Add(new GridTable(abstractGrid.GridWidth, abstractGrid.GridHeight));
            }

            _rendererGrids = new RendererGrids2D();
        }

        public override void OnPlaceItem(ItemTable item)
        {
            base.OnPlaceItem(item);

            AddOption(OptionsType.Open);
            RemoveOption(OptionsType.Unequip);
            AddOption(OptionsType.Equip);
        }

        public void OnEquipContainerHolder()
        {
            RemoveOption(OptionsType.Equip);
            AddOption(OptionsType.Unequip);
        }

        public ContainerGrids OpenInventory(Transform parentTransform)
        {
            if (parentTransform == null)
            {
                Debug.LogError("Opening Inventory. ParentTransform cannot be null...".Error());
                return null;
            }

            var containerGridsPrefab = GetPrefabGrids();

            var containerGrids =
                _rendererGrids.Hydrate(parentTransform, containerGridsPrefab, GridsInventory);

            UIGridsOpened.Add(containerGrids);

            return containerGrids;
        }

        public void CloseInventory(ContainerGrids containerGrids)
        {
            if (UIGridsOpened.Count == 0) return;

            if (containerGrids == null) return;

            _rendererGrids.Dehydrate(containerGrids, GridsInventory);

            UIGridsOpened.Remove(containerGrids);
        }

        public bool IsInsertingInsideYourself(GridTable grid)
        {
            var gridTablesList = GridsInventory;

            if (gridTablesList.Count == 0) return false;

            if (gridTablesList.Contains(grid))
            {
                return true;
            }

            foreach (var gridTable in gridTablesList)
            {
                foreach (var containerItem in gridTable.GetAllContainersFromGrid())
                {
                    var containerItemInventoryMetadata = (ContainerMetadata)containerItem.InventoryMetadata;

                    var containsGrid = containerItemInventoryMetadata.GridsInventory
                        .Contains(grid);

                    if (containsGrid)
                    {
                        return true;
                    }

                    if (containerItemInventoryMetadata.IsInsertingInsideYourself(grid))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public GridResponse PlaceItemInInventory(ItemTable selectedInventoryItem)
        {
            var gridTables = GridsInventory;

            GridResponse? status = null;

            var firstRotation = selectedInventoryItem.IsRotated;

            foreach (var gridTable in gridTables)
            {
                var posInGrid = gridTable.FindSpaceForObjectAnyDirection(selectedInventoryItem);

                if (posInGrid == null) continue;

                if (gridTable == selectedInventoryItem.CurrentGridTable)
                {
                    status = GridResponse.AlreadyInserted;
                    break;
                }

                status = gridTable.PlaceItem(selectedInventoryItem, posInGrid.Value.x, posInGrid.Value.y);

                if (status != GridResponse.Inserted) continue;

                var abstractItem = selectedInventoryItem.GetAbstractItem();

                if (abstractItem != null)
                {
                    GameObject.Destroy(abstractItem.gameObject);
                }

                break;
            }

            if (status != GridResponse.Inserted && selectedInventoryItem.IsRotated != firstRotation)
            {
                selectedInventoryItem.Rotate();
            }

            return status ?? GridResponse.InventoryFull;
        }

        public bool ContainsSpaceForItem(AbstractItem selectedInventoryItem)
        {
            var lastRotateState = selectedInventoryItem.ItemTable.IsRotated;

            var gridTables = GridsInventory;

            var availablePosOnGrid = gridTables
                .Select(gridTable => gridTable.FindSpaceForObjectAnyDirection(selectedInventoryItem.ItemTable))
                .FirstOrDefault(posInGrid => posInGrid != null);

            if (lastRotateState != selectedInventoryItem.ItemTable.IsRotated)
            {
                selectedInventoryItem.Rotate();
            }

            return availablePosOnGrid.HasValue;
        }

        public List<ContainerGrids> GetContainerGrids()
        {
            return UIGridsOpened;
        }

        private void AddOption(OptionsType optionsType)
        {
            var optionSo = FindOption(optionsType);

            if (optionSo == null) return;

            AddToOptions(optionSo);
        }

        private void AddToOptions(OptionSo optionSo)
        {
            if (ItemTable.InventoryMetadata.OptionsMetadata.Contains(optionSo)) return;

            ItemTable.InventoryMetadata.OptionsMetadata.Add(optionSo);
        }

        private void RemoveOption(OptionsType optionsType)
        {
            var optionSo = FindOption(optionsType);

            if (optionSo == null) return;

            ItemTable.InventoryMetadata.OptionsMetadata.Remove(optionSo);
        }

        private OptionSo FindOption(OptionsType optionsType)
        {
            var itemDataSo = ItemTable.ItemDataSo;

            var options = itemDataSo.GetOptionsOrdered();

            var optionSo = Array.Find(options, so => so.OptionsType == optionsType);

            return optionSo;
        }

        private ContainerGrids GetPrefabGrids()
        {
            var itemContainerDataSo = (ItemContainerDataSo)ItemTable.ItemDataSo;

            return itemContainerDataSo.ContainerGrids;
        }
    }
}