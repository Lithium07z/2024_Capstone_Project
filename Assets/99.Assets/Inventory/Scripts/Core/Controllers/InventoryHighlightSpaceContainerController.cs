using System;
using System.Linq;
using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.ItemsMetadata;
using Inventory.Scripts.Core.ScriptableObjects;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Events;
using UnityEngine;
using UnityEngine.Serialization;

namespace Inventory.Scripts.Core.Controllers
{
    public class InventoryHighlightSpaceContainerController : MonoBehaviour
    {
        [Header("Inventory Settings Anchor Settings")] [SerializeField]
        private InventorySettingsAnchorSo inventorySettingsAnchorSo;

        [FormerlySerializedAs("onAbstractItemBeingDragEventChannelSo")] [Header("Listening on...")] [SerializeField]
        private AbstractItemEventChannelSo abstractItemEventChannelSo;

        private void Awake()
        {
            var playerInventorySo = GetPlayerInventorySo();

            playerInventorySo.Inventories.Clear();
        }

        private void OnEnable()
        {
            abstractItemEventChannelSo.OnEventRaised += HighlightContainerWithSpace;
        }

        private void OnDisable()
        {
            abstractItemEventChannelSo.OnEventRaised -= HighlightContainerWithSpace;
        }

        private void HighlightContainerWithSpace(AbstractItem inventoryItem)
        {
            var playerInventorySo = GetPlayerInventorySo();

            if (playerInventorySo.Inventories == null) return;

            if (!IsEnableHighlightContainerWithSpaceInPlayerInventory()) return;

            var allItemsFromPlayerInventory = playerInventorySo
                .GetGrids()
                .SelectMany(grid => grid.GetAllItemsFromGrid());

            foreach (var item in allItemsFromPlayerInventory)
            {
                if (inventoryItem == null)
                {
                    item.UpdateUI(new ItemUIUpdater(
                        Tuple.Create<bool, Color?>(false, null)
                    ));
                    continue;
                }

                if (item.InventoryMetadata is not ContainerMetadata metadata) continue;

                var containsSpaceForItem = metadata.ContainsSpaceForItem(inventoryItem);

                if (inventoryItem.ItemTable.InventoryMetadata is ContainerMetadata itemDraggingMetadata)
                {
                    var isInsertingInsideYourself =
                        itemDraggingMetadata.IsInsertingInsideYourself(item.CurrentGridTable);

                    if (isInsertingInsideYourself)
                    {
                        continue;
                    }
                }

                // TODO: Validate if the inventoryItem is already inside the container that, if so should not update the background.

                item.UpdateUI(new ItemUIUpdater(
                    Tuple.Create<bool, Color?>(containsSpaceForItem,
                        GetColorOnHoverContainerThatCanBeInsertedInPlayerInventory())
                ));
            }
        }

        private bool IsEnableHighlightContainerWithSpaceInPlayerInventory()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo
                .EnableHighlightContainerInPlayerInventoryFitsItemDragged;
        }

        private Color GetColorOnHoverContainerThatCanBeInsertedInPlayerInventory()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.OnContainerInPlayerInventoryFitsItemDragged;
        }

        private InventorySo GetPlayerInventorySo()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.PlayerInventorySo;
        }

        public void SetInventorySettingsAnchorSo(InventorySettingsAnchorSo inventorySettingsAnchor)
        {
            inventorySettingsAnchorSo = inventorySettingsAnchor;
        }
    }
}