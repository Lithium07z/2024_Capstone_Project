using System.Collections.Generic;
using Inventory.Scripts.Core.Helper;
using Inventory.Scripts.Core.ScriptableObjects;
using Inventory.Scripts.Core.ScriptableObjects.Configuration;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Tile;
using UnityEngine;

namespace Inventory.Scripts.Core.Items.Grids
{
    public abstract class AbstractGrid : MonoBehaviour
    {
        [Header("Inventory Settings Anchor Settings")] [SerializeField]
        protected InventorySettingsAnchorSo inventorySettingsAnchorSo;

        [Header("Grid Configuration")] [SerializeField]
        protected int gridWidth = 1;

        [SerializeField] protected int gridHeight = 1;

        public int GridWidth => gridWidth;
        public int GridHeight => gridHeight;

        [field: SerializeReference] public GridTable Grid { get; private set; }

        public InventorySettingsAnchorSo InventorySettingsAnchorSo => inventorySettingsAnchorSo;

        private bool _isInitialConfigSet;
        private bool _isGridTableInitialed;

        private readonly List<AbstractItem> _abstractItems = new();

        protected virtual void Awake()
        {
            Init();
        }

        private void Init()
        {
            if (_isInitialConfigSet) return;

            InitializeIfParentNotItemContainer();
            ResizeGrid();

            _isInitialConfigSet = true;
        }

        private void InitializeIfParentNotItemContainer()
        {
            var containerGrids = GetComponentInParent<ContainerGrids>();

            if (containerGrids != null) return;

            Set(new GridTable(gridWidth, gridHeight));
        }

        public void Set(GridTable gridTable)
        {
            ClearPreviousGrid();

            Grid = gridTable;
            Grid.AddGrid(this);

            Grid.OnInsert += HandleInsertItem;
            Grid.OnRemove += HandleRemoveItem;

            RefreshGrid();
        }

        private void ClearPreviousGrid()
        {
            var previousGrid = Grid;

            if (previousGrid == null)
            {
                var items = GetComponentsInChildren<AbstractItem>(true);
                foreach (var abstractItem in items)
                {
                    if (abstractItem == null) continue;
                    Destroy(abstractItem.gameObject);
                }

                return;
            }

            var allItemsFromGrid = previousGrid.GetAllItemsFromGrid();

            foreach (var itemTable in allItemsFromGrid)
            {
                HandleRemoveItem(itemTable);
            }
        }

        private void HandleInsertItem(ItemTable itemTable)
        {
            AddItemUi(itemTable);
            HandleUpdateGrid(itemTable);
        }

        private void HandleRemoveItem(ItemTable itemTable)
        {
            RemoveItemUi(itemTable);
            HandleUpdateGrid(itemTable);
        }

        private void HandleUpdateGrid(ItemTable item)
        {
            if (Grid != item.CurrentGridTable) return;

            HandleUpdateItem(item, item.OnGridPositionX, item.OnGridPositionY);
        }

        public AbstractItem GetAbstractItem(ItemTable itemTable)
        {
            return _abstractItems.Find(itemUi => itemUi.ItemTable == itemTable);
        }

        protected Vector2 GetGridSize()
        {
            var tileGetGridPositionSo = GetTileGetGridPositionSo();

            return tileGetGridPositionSo.GetGridSize(gridWidth, gridHeight);
        }

        private TileGridHelperSo GetTileGetGridPositionSo()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.TileGridHelperSo;
        }

        protected ItemPrefabSo GetItemPrefabSo()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.ItemPrefabSo;
        }

        protected abstract void HandleUpdateItem(ItemTable item, int itemOnGridPositionX, int itemOnGridPositionY);

        public abstract void ResizeGrid();

        public abstract Transform GetOrInstantiateHighlightArea();

        public abstract AbstractItem PlaceDragRepresentation(AbstractItem selectedInventoryItem);

        public abstract void RemoveDragRepresentation(AbstractItem inventoryItemDrag);

        private void OnDestroy()
        {
            if (Grid == null) return;

            Grid.OnInsert -= HandleInsertItem;
            Grid.OnRemove -= HandleRemoveItem;
        }

        private void RefreshGrid()
        {
            if (Grid == null)
            {
                Debug.LogWarning("Could not perform refresh on grid because it was null...".Info());
                return;
            }

            foreach (var item in Grid.GetAllItemsFromGrid())
            {
                AddItemUi(item);
                HandleUpdateGrid(item);
            }
        }

        private bool ContainsItemUi(ItemTable itemTable)
        {
            return _abstractItems.Exists(itemUi => itemUi.ItemTable == itemTable);
        }

        private void AddItemUi(ItemTable itemTable)
        {
            if (itemTable.IsDragRepresentation) return;

            if (ContainsItemUi(itemTable)) return;

            var itemPrefab = GetItemPrefabSo().ItemPrefab;

            var gameObjectItem = Instantiate(itemPrefab);

            var abstractItem = gameObjectItem.GetComponent<AbstractItem>();

            var transformFromItem = abstractItem.GetComponent<Transform>();

            transformFromItem.SetParent(transform);

            abstractItem.RefreshItem(itemTable);

            _abstractItems.Add(abstractItem);
        }

        private void RemoveItemUi(ItemTable itemTable)
        {
            if (itemTable.IsDragRepresentation) return;

            var abstractItem = _abstractItems.Find(item => item.ItemTable == itemTable);

            if (abstractItem == null) return;

            _abstractItems.Remove(abstractItem);

            Destroy(abstractItem.gameObject);
        }
    }
}