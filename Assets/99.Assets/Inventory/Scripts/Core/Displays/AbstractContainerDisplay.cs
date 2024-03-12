using System;
using System.Collections.Generic;
using Inventory.Scripts.Core.Displays.Filler;
using Inventory.Scripts.Core.Enums;
using Inventory.Scripts.Core.Helper;
using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.ScriptableObjects;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Anchors;
using Inventory.Scripts.Core.ScriptableObjects.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.Scripts.Core.Displays
{
    [RequireComponent(typeof(RectTransform))]
    public class AbstractContainerDisplay : MonoBehaviour
    {
        [Header("Display Anchor")] [SerializeField]
        private ContainerDisplayAnchorSo displayAnchorSo;

        [SerializeField]
        [Tooltip(
            "The prefab display filler which will be displayed inside the 'displayAnchorSo'. " +
            "If null, will use the one from InventorySettingsSo. " +
            "The priority is to use this property instead of the InventorySettingsSo")]
        private DisplayFiller customPrefabDisplayFiller;

        [Header("Display Sort (If multiple)")]
        [SerializeField]
        [Tooltip("This will be used to Sort the Containers in Player Inventory. Ps: This will be reverse in runtime.")]
        private ItemDataTypeSo[] itemDataTypeSos;

        [Header("Inventory Settings Anchor")] [SerializeField]
        private InventorySettingsAnchorSo inventorySettingsAnchorSo;

        private readonly List<ItemTable> _displayedContainers = new();

        private readonly List<DisplayFiller> _displayFillers = new();

        protected InventorySettingsAnchorSo InventorySettingsAnchorSo => inventorySettingsAnchorSo;

        protected RectTransform ContainerParent { get; private set; }

        private bool _initialRefreshUi;

        private void Awake()
        {
            SetAnchor();

            ContainerParent = GetComponent<RectTransform>();

            Array.Reverse(itemDataTypeSos);
        }

        private void SetAnchor()
        {
            if (displayAnchorSo == null)
            {
                UnityEngine.Debug.LogError(
                    "[Display] ContainerDisplay not configured correctly... Missing the anchor to the Holder."
                        .Configuration());
                return;
            }

            displayAnchorSo.Value = this;
        }

        public void DisplayInteract(ItemTable container, DisplayInteraction displayInteraction)
        {
            if (_displayedContainers.Contains(container) &&
                displayInteraction == DisplayInteraction.Close)
            {
                _displayedContainers.Remove(container);
                OnRemoveDisplayContainer(container);
                ResortContainers();
                return;
            }

            if (_displayedContainers.Contains(container) ||
                displayInteraction != DisplayInteraction.Open) return;

            _displayedContainers.Add(container);
            OnAddDisplayContainer(container);
            ResortContainers();

            if (_initialRefreshUi) return;

            RefreshUI();
            _initialRefreshUi = true;
        }

        private void RefreshUI()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(ContainerParent);
        }

        protected DisplayFiller GetFreeDisplayFiller(ItemTable itemTable)
        {
            if (itemTable == null) return null;

            var foundDisplayFiller = FindDisplayFillerBy(null);

            if (foundDisplayFiller != null)
            {
                foundDisplayFiller.Set(itemTable);

                return foundDisplayFiller;
            }

            var newDisplayFiller = AddNewDisplayFiller();

            newDisplayFiller.Set(itemTable);

            return newDisplayFiller;
        }

        protected DisplayFiller FindDisplayFillerBy(ItemTable itemTable)
        {
            var found = _displayFillers.Find(filler => filler.CurrentInventoryItem == itemTable);

            return found ? found : null;
        }

        private DisplayFiller AddNewDisplayFiller()
        {
            var displayFillerPrefab = GetDisplayFillerPrefab();

            var newDisplayFiller = Instantiate(displayFillerPrefab, ContainerParent);

            InteractionController interactionController = this.transform.root.GetComponent<InteractionController>();
            interactionController._displayFiller = newDisplayFiller;

            newDisplayFiller.gameObject.SetActive(false);

            _displayFillers.Add(newDisplayFiller);

            return newDisplayFiller;
        }

        protected List<ItemTable> GetDisplayedContainers()
        {
            return _displayedContainers;
        }

        protected virtual void OnAddDisplayContainer(ItemTable container)
        {
        }

        protected virtual void OnRemoveDisplayContainer(ItemTable container)
        {
        }

        private void ResortContainers()
        {
            var displayedContainers = GetDisplayedContainers();

            foreach (var itemDataTypeSo in itemDataTypeSos)
            {
                var inventoryItem = displayedContainers
                    .Find(item => item.ItemDataSo.ItemDataTypeSo == itemDataTypeSo);

                if (inventoryItem == null) continue;

                var foundDisplayFiller = FindDisplayFillerBy(inventoryItem);

                if (foundDisplayFiller != null)
                {
                    foundDisplayFiller.Sort();
                }
            }
        }

        private DisplayFiller GetDisplayFillerPrefab()
        {
            if (customPrefabDisplayFiller != null)
            {
                return customPrefabDisplayFiller;
            }

            if (inventorySettingsAnchorSo != null)
            {
                var displayFiller = inventorySettingsAnchorSo.InventorySettingsSo.DisplayFiller;

                if (displayFiller != null) return displayFiller;

                UnityEngine.Debug.LogError("Display filler prefab not configured in InventorySettingsSo...".Settings());
                return null;
            }

            UnityEngine.Debug.LogError("ContainerDisplay not configured correctly...".Configuration());
            return null;
        }
    }
}