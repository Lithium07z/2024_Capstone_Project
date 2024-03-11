using System;
using Inventory.Scripts.Core.Helper;
using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.ScriptableObjects;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Anchors;
using Inventory.Scripts.Core.ScriptableObjects.Items;
using Photon.Pun;
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

        [SerializeField] private EnvironmentContainerCreatorController environmentContainerCreatorController;

        public Action<bool> OnChangeOpenState;

        private ItemTable _containerInventoryItem;

        private PhotonView _photonView;

        public bool _isOpen;

        private void Start()
        {
            if (!containerDisplayAnchorSo)
            {
                Debug.LogError(
                    "Error configuring Environment Container Holder... Property ContainerDisplayAnchorSo has not being set."
                        .Configuration());
            }

            _photonView = this.GetComponent<PhotonView>();

            InitializeEnvironmentContainer();
        }

        private void Update()
        {
            if (_containerInventoryItem.CurrentGridTable != null)
            {
                Debug.Log(_containerInventoryItem.CurrentGridTable.ToString());
            }
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
            SendIsOpen(true);
            OnChangeOpenState?.Invoke(_isOpen);
        }

        /// <summary>
        /// After you done interacting with this Environment Container Holder you must close in order to remove the Grid Inventory from UI.
        /// </summary>
        public void CloseContainer()
        {
            environmentContainerCreatorController.ConvertGridTableToArray();
            containerDisplayAnchorSo.CloseContainer(_containerInventoryItem);
            SendIsOpen(false);
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

        [PunRPC]
        private void ReceiveIsOpen(bool _isOpen)
        {
            this._isOpen = _isOpen;
        }

        private void SendIsOpen(bool _isOpen)
        {
            _photonView.RPC("ReceiveIsOpen", RpcTarget.AllBuffered, _isOpen);
        }
    }
}