using System.Linq;
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
    public class InventoryOptionsController : MonoBehaviour
    {
        [Header("Inventory Settings Anchor Settings")] [SerializeField]
        private InventorySettingsAnchorSo inventorySettingsAnchorSo;

        [Header("Options Parent Settings")] [SerializeField]
        private RectTransformPrefabAnchorSo defaultOptionsParent;

        [SerializeField] private InputProviderSo inputProviderSo;

        [Header("Listening on...")] [SerializeField]
        private OnGridInteractEventChannelSo onGridInteractEventChannelSo;

        [SerializeField] private OnItemHolderInteractEventChannelSo onItemHolderInteractEventChannelSo;

        [SerializeField] private OnOptionInteractEventChannelSo onOptionInteractEventChannelSo;

        [FormerlySerializedAs("onAbstractItemBeingDragEventChannelSo")] [SerializeField]
        private AbstractItemEventChannelSo abstractItemEventChannelSo;

        private AbstractItem _selectedInventoryItem;
        private AbstractGrid _selectedAbstractGrid;

        private ItemHolder _selectedItemHolder;

        private OptionsController _optionsController;
        private RectTransform _optionsControllerRectTransform;

        private OptionsController _selectedOptionsController;

        private void Start()
        {
            var optionsControllerGameObject =
                Instantiate(GetOptionsController(), defaultOptionsParent.Value);

            _optionsController = optionsControllerGameObject.GetComponent<OptionsController>();
            _optionsControllerRectTransform = optionsControllerGameObject.GetComponent<RectTransform>();
            _optionsController.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            inputProviderSo.OnReleaseItem += OnLeftClick;
            inputProviderSo.OnToggleOptions += OnRightClick;

            abstractItemEventChannelSo.OnEventRaised += ResetOpen;

            onOptionInteractEventChannelSo.OnEventRaised += ChangeOptionController;
            onGridInteractEventChannelSo.OnEventRaised += ChangeGrid;
            onItemHolderInteractEventChannelSo.OnEventRaised += ChangeItemHolder;
        }

        private void OnDisable()
        {
            inputProviderSo.OnReleaseItem -= OnLeftClick;
            inputProviderSo.OnToggleOptions -= OnRightClick;

            abstractItemEventChannelSo.OnEventRaised -= ResetOpen;

            onOptionInteractEventChannelSo.OnEventRaised -= ChangeOptionController;
            onGridInteractEventChannelSo.OnEventRaised -= ChangeGrid;
            onItemHolderInteractEventChannelSo.OnEventRaised -= ChangeItemHolder;
        }

        private void OnLeftClick()
        {
            if (!_optionsController.gameObject.activeSelf || _selectedOptionsController != null) return;

            _optionsController.gameObject.SetActive(false);
        }

        private void OnRightClick()
        {
            if (_optionsController.gameObject.activeSelf)
            {
                _optionsController.gameObject.SetActive(false);
                return;
            }

            _selectedInventoryItem = GetInventoryItem();

            if (_selectedInventoryItem == null)
            {
                OnLeftClick();
                return;
            }

            var inventoryMetadata = _selectedInventoryItem.ItemTable.InventoryMetadata;

            if (inventoryMetadata == null) return;

            var orderedEnumerable = inventoryMetadata.OptionsMetadata.OrderBy(so => so.Order).ToList();

            _optionsController.SetOptions(_selectedInventoryItem, orderedEnumerable);
            
            OpenOptionsMenu();
        }

        private void OpenOptionsMenu()
        {
            var inputState = inputProviderSo.GetState();
            var cursorPosition = inputState.CursorPosition;

            _optionsControllerRectTransform.position = new Vector3(cursorPosition.x + 8, cursorPosition.y + -8, 0f);
            _optionsController.gameObject.SetActive(true);
        }

        private AbstractItem GetInventoryItem()
        {
            if (_selectedItemHolder != null)
            {
                var inventoryItem = _selectedItemHolder.GetItemEquipped();

                if (inventoryItem != null)
                    return inventoryItem;
            }

            if (_selectedAbstractGrid == null) return null;

            var tileGridHelperSo = GetTileGridHelperSo();

            var tileGridPosition =
                tileGridHelperSo.GetTileGridPositionByGridTable(_selectedAbstractGrid.transform);

            return _selectedAbstractGrid.Grid.GetItem(tileGridPosition.x, tileGridPosition.y)?.GetAbstractItem();
        }

        private void ChangeGrid(AbstractGrid abstractGrid)
        {
            _selectedAbstractGrid = abstractGrid;
        }

        private void ChangeItemHolder(ItemHolder itemHolder)
        {
            _selectedItemHolder = itemHolder;
        }

        private void ChangeOptionController(OptionsController optionsController)
        {
            _selectedOptionsController = optionsController;
        }

        private void ResetOpen(AbstractItem inventoryItem)
        {
            _optionsController.gameObject.SetActive(false);
        }

        private OptionsController GetOptionsController()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.OptionsController;
        }

        private TileGridHelperSo GetTileGridHelperSo()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.TileGridHelperSo;
        }

        public void SetOptionsParent(RectTransformPrefabAnchorSo rectTransformPrefabAnchorSo)
        {
            defaultOptionsParent = rectTransformPrefabAnchorSo;
        }

        public void SetInventorySettingsAnchorSo(InventorySettingsAnchorSo inventorySettingsAnchor)
        {
            inventorySettingsAnchorSo = inventorySettingsAnchor;
        }
    }
}