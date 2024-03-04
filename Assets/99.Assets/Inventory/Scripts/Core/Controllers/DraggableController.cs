using Inventory.Scripts.Core.Controllers.Draggable;
using Inventory.Scripts.Core.Controllers.Inputs;
using Inventory.Scripts.Core.Holders;
using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.Items.Grids;
using Inventory.Scripts.Core.ScriptableObjects;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Anchors;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Events;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Events.Interact;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Tile;
using UnityEngine;
using UnityEngine.Serialization;

namespace Inventory.Scripts.Core.Controllers
{
    public class DraggableController : MonoBehaviour
    {
        [Header("Inventory Settings Anchor Settings")] [SerializeField]
        private InventorySettingsAnchorSo inventorySettingsAnchorSo;

        [Header("Provider Configuration")] [SerializeField]
        private DraggableProviderSo draggableProviderSo;

        [Header("Item Configuration")] [SerializeField] [Tooltip("Used to item being drag not appear behind the grid.")]
        private RectTransformPrefabAnchorSo defaultItemParentWhenDrag;

        [Tooltip("How fast the item position will move in the lerp")] [SerializeField]
        private float itemDragMovementSpeed = 128f;

        [FormerlySerializedAs("onAbstractItemBeingDragEventChannelSo")] [Header("Highlight Reference")] [SerializeField]
        private AbstractItemEventChannelSo abstractItemEventChannelSo;

        [Header("Listening on...")] [SerializeField]
        private OnGridInteractEventChannelSo onGridInteractEventChannelSo;

        [SerializeField] private OnItemHolderInteractEventChannelSo onItemHolderInteractEventChannelSo;

        [Header("Inputs")] [SerializeField] private InputProviderSo inputProvider;

        [Header("Broadcasting on...")] [SerializeField]
        private OnAudioStateEventChannelSo onAudioStateEventChannelSo;

        [SerializeField] private UpdateHighlightEventChannelSo updateHighlightEventChannelSo;

        private AbstractGrid _selectedAbstractGrid;
        private ItemHolder _selectedItemHolder;
        private AbstractItem _selectedInventoryItem;

        private AbstractItem _inventoryItemDragRepresentation;

        private PickupContext _pickupContext;
        private PickupState _pickupState;

        private bool _isDragging;

        private void OnEnable()
        {
            inputProvider.OnRotateItem += RotateItem;
            inputProvider.OnPickupItem += OnPickupItem;
            inputProvider.OnReleaseItem += OnReleaseItem;

            onGridInteractEventChannelSo.OnEventRaised += ChangeAbstractGrid;
            onItemHolderInteractEventChannelSo.OnEventRaised += ChangeItemHolderHolder;
        }

        private void OnDisable()
        {
            inputProvider.OnRotateItem -= RotateItem;
            inputProvider.OnPickupItem -= OnPickupItem;
            inputProvider.OnReleaseItem -= OnReleaseItem;

            onGridInteractEventChannelSo.OnEventRaised -= ChangeAbstractGrid;
            onItemHolderInteractEventChannelSo.OnEventRaised -= ChangeItemHolderHolder;
        }

        private void ChangeAbstractGrid(AbstractGrid abstractGrid)
        {
            _selectedAbstractGrid = abstractGrid;
        }

        private void ChangeItemHolderHolder(ItemHolder itemHolder)
        {
            _selectedItemHolder = itemHolder;
        }

        private void Update()
        {
            UpdateItemIconDragging();
        }

        private void UpdateItemIconDragging()
        {
            if (_selectedInventoryItem == null) return;

            var selectedItemTransform = _selectedInventoryItem.transform;

            var inputState = inputProvider.GetState();

            var cursorPosition = inputState.CursorPosition;

            selectedItemTransform.position =
                Vector3.Lerp(selectedItemTransform.position, cursorPosition, Time.deltaTime * itemDragMovementSpeed);
            selectedItemTransform.SetAsLastSibling();
        }

        private void RotateItem()
        {
            if (_selectedInventoryItem == null)
                return;

            _selectedInventoryItem.Rotate();
        }

        private void OnPickupItem()
        {
            _pickupContext = new PickupContext(
                _selectedInventoryItem,
                _selectedAbstractGrid,
                _selectedItemHolder,
                GetTileGridHelperSo()
            );

            _pickupState = draggableProviderSo.ProcessPickup(_pickupContext);

            _selectedInventoryItem = _pickupState.Item;

            if (_selectedInventoryItem == null) return;

            PlayAudioPickup();
            _selectedInventoryItem.SetDragStyle();
            _selectedInventoryItem.transform.SetParent(defaultItemParentWhenDrag.Value);
            _inventoryItemDragRepresentation = StartDragRepresentation(_pickupContext.AbstractGridFromStartDragging,
                _pickupContext.ItemHolderFromStartDragging,
                _selectedInventoryItem
            );
            abstractItemEventChannelSo.RaiseEvent(_selectedInventoryItem);
        }

        private void OnReleaseItem()
        {
            if (_pickupContext == null || _pickupState == null) return;

            if (_selectedInventoryItem == null) return;

            var releaseContext = new ReleaseContext(
                _pickupState,
                _selectedAbstractGrid,
                _selectedItemHolder,
                _pickupContext.TileGridHelperSo
            );

            StopDragRepresentation();

            draggableProviderSo.ProcessRelease(releaseContext);

            PlayAudioPlace();
            StopSelectInventoryItem();
        }

        private AbstractItem StartDragRepresentation(AbstractGrid abstractGrid, ItemHolder itemHolderFromPickup,
            AbstractItem selectedInventoryItem)
        {
            if (selectedInventoryItem != null)
            {
                selectedInventoryItem.ResizeIcon();
            }

            if (GetShowDragRepresentation() && abstractGrid != null)
            {
                return abstractGrid.PlaceDragRepresentation(selectedInventoryItem);
            }

            if (itemHolderFromPickup != null)
            {
                return itemHolderFromPickup.PlaceDragRepresentation();
            }

            return null;
        }

        private void StopDragRepresentation()
        {
            var gridFromStartDragging = _pickupContext.AbstractGridFromStartDragging;

            if (GetShowDragRepresentation() && gridFromStartDragging != null)
            {
                if (_inventoryItemDragRepresentation == null) return;

                gridFromStartDragging.RemoveDragRepresentation(_inventoryItemDragRepresentation);
            }

            var itemHolderFromStartDragging = _pickupContext.ItemHolderFromStartDragging;

            if (itemHolderFromStartDragging != null)
            {
                itemHolderFromStartDragging.RemoveDragRepresentation();
            }

            _inventoryItemDragRepresentation = null;
        }

        private void StopSelectInventoryItem()
        {
            var pickupStateItem = _pickupState.Item;

            pickupStateItem.UnsetDragStyle();

            _pickupState = null;
            _selectedInventoryItem = null;
            abstractItemEventChannelSo.RaiseEvent(null);
            updateHighlightEventChannelSo.RaiseEvent();
        }

        private void PlayAudioPlace()
        {
            var audioCueSo = _selectedInventoryItem.ItemTable.ItemDataSo.AudioCueSo;
            if (audioCueSo)
            {
                onAudioStateEventChannelSo.RaiseEvent(audioCueSo.OnBeingPlace);
            }
        }

        private void PlayAudioPickup()
        {
            var audioCueSo = _selectedInventoryItem.ItemTable.ItemDataSo.AudioCueSo;
            if (audioCueSo)
            {
                onAudioStateEventChannelSo.RaiseEvent(audioCueSo.OnBeingPicked);
            }
        }

        private TileGridHelperSo GetTileGridHelperSo()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.TileGridHelperSo;
        }

        private bool GetShowDragRepresentation()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.ShowDragRepresentation;
        }

        public void SetItemParent(RectTransformPrefabAnchorSo parentTransform)
        {
            defaultItemParentWhenDrag = parentTransform;
        }

        public void SetInventorySettingsAnchorSo(InventorySettingsAnchorSo inventorySettingsAnchor)
        {
            inventorySettingsAnchorSo = inventorySettingsAnchor;
        }
    }
}