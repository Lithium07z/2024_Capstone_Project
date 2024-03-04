using Inventory.Scripts.Core.Controllers;
using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Events;
using Inventory.Scripts.Core.Windows;
using UnityEngine;

namespace Inventory.Scripts.Windows
{
    public class InspectWindowManager : WindowController
    {
        [Header("Listening on...")] [SerializeField]
        private OnItemExecuteOptionEventChannelSo onInspectItemEventChannelSo;

        [SerializeField] private WindowContainerChangeStateEventChannelSo windowContainerChangeStateEventChannelSo;

        private void OnEnable()
        {
            onInspectItemEventChannelSo.OnEventRaised += InspectAbstractItem;
            windowContainerChangeStateEventChannelSo.OnEventRaised += HandleChangeStateWindow;
        }

        private void OnDisable()
        {
            onInspectItemEventChannelSo.OnEventRaised -= InspectAbstractItem;
            windowContainerChangeStateEventChannelSo.OnEventRaised -= HandleChangeStateWindow;
        }

        private void InspectAbstractItem(AbstractItem abstractItem)
        {
            OpenWindow(abstractItem.ItemTable);
        }

        private void HandleChangeStateWindow(ItemTable itemTable, WindowUpdateState windowUpdateState)
        {
            if (windowUpdateState == WindowUpdateState.Open)
            {
                OpenWindow(itemTable);
                return;
            }

            if (windowUpdateState == WindowUpdateState.Close)
            {
                CloseWindow(itemTable);
            }
        }


        protected override Window GetPrefabWindow()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.BasicPrefabWindow;
        }

        protected override int GetMaxWindowOpen()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.MaxInspectWindowOpen;
        }
    }
}