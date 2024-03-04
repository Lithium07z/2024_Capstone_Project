using System;
using Inventory.Scripts.Core.Holders;
using Inventory.Scripts.Core.Items.Grids;
using Inventory.Scripts.Core.ItemsMetadata;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Anchors;
using Inventory.Scripts.Core.ScriptableObjects.Items;
using UnityEngine;

namespace Inventory.Scripts.Core.Items
{
    [Serializable]
    public class ItemTable
    {
        private AbstractGridSelectedAnchorSo _abstractGridSelectedAnchorSo;

        public ItemDataSo ItemDataSo { get; private set; }

        public bool IsRotated { get; private set; }

        public int Width => !IsRotated ? ItemDataSo.DimensionsSo.Width : ItemDataSo.DimensionsSo.Height;

        public int Height => !IsRotated ? ItemDataSo.DimensionsSo.Height : ItemDataSo.DimensionsSo.Width;

        public int OnGridPositionX { get; private set; }
        public int OnGridPositionY { get; private set; }

        [field: SerializeReference] public GridTable CurrentGridTable { get; private set; }

        // TODO: Improve this to not depend on UI component...
        [field: SerializeReference] public ItemHolder CurrentItemHolder { get; private set; }

        [field: SerializeReference] public InventoryMetadata InventoryMetadata { get; private set; }

        // Validate after, how to remove this
        public bool IsDragRepresentation { get; private set; }

        public event Action<ItemUIUpdater> OnUpdateUI;

        public ItemTable(ItemDataSo itemDataSo, AbstractGridSelectedAnchorSo abstractGridSelectedAnchorSo)
        {
            ItemDataSo = itemDataSo;
            _abstractGridSelectedAnchorSo = abstractGridSelectedAnchorSo;
        }

        public void SetGridProps(GridTable gridTable, int posX, int posY, ItemHolder itemHolder)
        {
            CurrentGridTable = gridTable;
            OnGridPositionX = posX;
            OnGridPositionY = posY;
            CurrentItemHolder = itemHolder;
        }

        public void CreateMetadata()
        {
            if (ItemDataSo is ItemContainerDataSo)
            {
                InventoryMetadata = new ContainerMetadata(this);
                return;
            }

            // TODO: Add StackableMetadata for stackable items

            InventoryMetadata = new InventoryMetadata(this);
        }

        public bool IsContainer()
        {
            return InventoryMetadata is ContainerMetadata;
        }

        public void Rotate()
        {
            IsRotated = !IsRotated;
        }

        public void SetDragProps(ItemDataSo itemDataSo)
        {
            ItemDataSo = itemDataSo;
            IsDragRepresentation = true;
        }

        public AbstractItem GetAbstractItem()
        {
            return _abstractGridSelectedAnchorSo.isSet
                ? _abstractGridSelectedAnchorSo.Value.GetAbstractItem(this)
                : null;
        }

        public void UpdateUI(ItemUIUpdater updater)
        {
            OnUpdateUI?.Invoke(updater);
        }
    }
}