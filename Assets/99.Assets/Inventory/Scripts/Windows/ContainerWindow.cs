using Inventory.Scripts.Core.Items.Grids;
using Inventory.Scripts.Core.ItemsMetadata;
using Inventory.Scripts.Core.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.Scripts.Windows
{
    public class ContainerWindow : Window
    {
        [Header("Container Window Settings")] [SerializeField]
        private RectTransform content;

        public RectTransform Content
        {
            get => content;
            set => content = value;
        }

        private ContainerMetadata _containerMetadata;
        private ContainerGrids _containerGrids;

        protected override void OnOpenWindow()
        {
            var inventoryMetadata = CurrentItemTable.InventoryMetadata;

            if (inventoryMetadata is not ContainerMetadata containerMetadata)
            {
                Debug.LogError("Trying to opening a non container item. Item: " +
                               CurrentItemTable.ItemDataSo.DisplayName);
                return;
            }

            _containerMetadata = containerMetadata;

            _containerGrids = _containerMetadata.OpenInventory(content);

            RefreshUI();
        }

        private void RefreshUI()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        }

        protected override void OnCloseWindow()
        {
            base.OnCloseWindow();

            _containerMetadata.CloseInventory(_containerGrids);
        }

        public ContainerGrids GetOpenedGrid()
        {
            return _containerGrids;
        }
    }
}