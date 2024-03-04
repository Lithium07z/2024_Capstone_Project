using Inventory.Scripts.Core.Controllers.Inputs;
using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.ScriptableObjects;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Events;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Events.Interact;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Tile;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Inventory.Scripts.Core.Controllers
{
    public class InventoryScrollController : MonoBehaviour
    {
        [Header("Inventory Settings Anchor Settings")] [SerializeField]
        private InventorySettingsAnchorSo inventorySettingsAnchorSo;

        [Header("Listening on...")] [SerializeField]
        private InputProviderSo inputProviderSo;

        [FormerlySerializedAs("onAbstractItemBeingDragEventChannelSo")] [SerializeField] private AbstractItemEventChannelSo abstractItemEventChannelSo;

        [SerializeField] private OnScrollRectInteractEventChannelSo onScrollRectInteractEventChannelSo;

        private AbstractItem _currentInventoryItemBeingDrag;
        private ScrollRect _currentScrollRectSelected;

        private void OnEnable()
        {
            abstractItemEventChannelSo.OnEventRaised += ChangeAbstractItem;
            onScrollRectInteractEventChannelSo.OnEventRaised += ChangeScrollRect;
        }

        private void OnDisable()
        {
            abstractItemEventChannelSo.OnEventRaised -= ChangeAbstractItem;
            onScrollRectInteractEventChannelSo.OnEventRaised -= ChangeScrollRect;
        }

        private void ChangeAbstractItem(AbstractItem inventoryItem)
        {
            _currentInventoryItemBeingDrag = inventoryItem;
        }

        private void ChangeScrollRect(ScrollRect scrollRect)
        {
            _currentScrollRectSelected = scrollRect;
        }

        private void Update()
        {
            if (_currentInventoryItemBeingDrag == null) return;

            if (_currentScrollRectSelected == null) return;

            var tileGridHelperSo = GetTileGridHelperSo();

            var cursorPosition = inputProviderSo.GetState().CursorPosition;

            var pointerViewportPosition =
                tileGridHelperSo.GetLocalPosition(_currentScrollRectSelected.viewport, cursorPosition);

            if (pointerViewportPosition.y < _currentScrollRectSelected.viewport.rect.min.y + GetHoldScrollPadding())
            {
                var rect = _currentScrollRectSelected.viewport.rect;
                var scrollValue = _currentScrollRectSelected.verticalNormalizedPosition * rect.height;
                scrollValue -= GetHoldScrollRate() * Time.deltaTime;
                _currentScrollRectSelected.verticalNormalizedPosition = Mathf.Clamp01(scrollValue / rect.height);
            }

            if (pointerViewportPosition.y > _currentScrollRectSelected.viewport.rect.max.y - GetHoldScrollPadding())
            {
                var rect = _currentScrollRectSelected.viewport.rect;
                var scrollValue = _currentScrollRectSelected.verticalNormalizedPosition * rect.height;
                scrollValue += GetHoldScrollRate() * Time.deltaTime;
                _currentScrollRectSelected.verticalNormalizedPosition = Mathf.Clamp01(scrollValue / rect.height);
            }
        }

        private float GetHoldScrollRate()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.HoldScrollRate;
        }

        private float GetHoldScrollPadding()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.HoldScrollPadding;
        }

        private TileGridHelperSo GetTileGridHelperSo()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.TileGridHelperSo;
        }

        public void SetInventorySettingsAnchorSo(InventorySettingsAnchorSo inventorySettingsAnchor)
        {
            inventorySettingsAnchorSo = inventorySettingsAnchor;
        }
    }
}