using System;
using Inventory.Scripts.Core.Helper;
using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.ScriptableObjects;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Anchors;
using Inventory.Scripts.Core.ScriptableObjects.Items;
using UnityEngine;

namespace Inventory.Scripts.Core.Holders
{
    public class EnvironmentContainerHolder : MonoBehaviour
    {
        [Header("Inventory Supplier So")] [SerializeField]
        private InventorySupplierSo inventorySupplierSo;

        [Header("Environment Container Holder Settings")] [SerializeField]
        private ItemContainerDataSo itemContainerDataSo;

        [Header("Displaying on...")] [SerializeField]
        private ContainerDisplayAnchorSo containerDisplayAnchorSo;

        public Action<bool> OnChangeOpenState;

        private bool _isOpen;
        private ItemTable _containerInventoryItem;

        private void Start()
        {
            if (!containerDisplayAnchorSo)
            {
                Debug.LogError(
                    "Error configuring Environment Container Holder... Property ContainerDisplayAnchorSo has not being set."
                        .Configuration());
            }

            InitializeEnvironmentContainer();
        }

        private void InitializeEnvironmentContainer()
        {
            _containerInventoryItem =
                inventorySupplierSo.InitializeEnvironmentContainer(itemContainerDataSo, transform);
        }

        /// <summary>
        /// Once you interact with this Environment Container Holder you must call this method to open the Grid Inventory on the UI.
        /// </summary>
        public void OpenContainer()
        {
            containerDisplayAnchorSo.OpenContainer(_containerInventoryItem);
            _isOpen = true;
            OnChangeOpenState?.Invoke(_isOpen);
        }

        /// <summary>
        /// After you done interacting with this Environment Container Holder you must close in order to remove the Grid Inventory from UI.
        /// </summary>
        public void CloseContainer()
        {
            containerDisplayAnchorSo.CloseContainer(_containerInventoryItem);
            _isOpen = false;
            OnChangeOpenState?.Invoke(_isOpen);
        }

        /// <summary>
        /// Will toggle the inventory, if is closed will open the inventory on the UI, if opened will close the Inventory.
        /// </summary>
        public void ToggleEnvironmentContainer()
        {
            if (_isOpen)
            {
                CloseContainer();
                return;
            }

            OpenContainer();
        }

        public ItemTable GetItemTable()
        {
            return _containerInventoryItem;
        }
    }
}