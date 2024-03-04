using Inventory.Scripts.Core.Controllers;
using Inventory.Scripts.Core.Enums;
using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Events;
using Inventory.Scripts.Core.Windows;
using UnityEngine;

namespace Inventory.Scripts.Windows
{
    public class ContainerWindowManager : WindowController
    {
        [Header("Listening on...")] [SerializeField]
        private OnItemExecuteOptionEventChannelSo onOpenItemEventChannelSo;

        [SerializeField] private WindowContainerChangeStateEventChannelSo windowContainerChangeStateEventChannelSo;

        [SerializeField] private OnEquipItemTableInHolderEventChannelSo onEquipItemContainerInHolderEventChannelSo;

        private void OnEnable()
        {
            onOpenItemEventChannelSo.OnEventRaised += OpenContainerAbstractItem;
            windowContainerChangeStateEventChannelSo.OnEventRaised += ChangeStateFromWindow;
            onEquipItemContainerInHolderEventChannelSo.OnEventRaised += CloseContainerOnEquip;
        }

        private void OnDisable()
        {
            onOpenItemEventChannelSo.OnEventRaised -= OpenContainerAbstractItem;
            windowContainerChangeStateEventChannelSo.OnEventRaised -= ChangeStateFromWindow;
            onEquipItemContainerInHolderEventChannelSo.OnEventRaised -= CloseContainerOnEquip;
        }

        private void OpenContainerAbstractItem(AbstractItem abstractItem)
        {
            OpenWindow(abstractItem.ItemTable);
        }

        private void ChangeStateFromWindow(ItemTable itemTable, WindowUpdateState windowUpdateState)
        {
            if (WindowUpdateState.Open == windowUpdateState)
            {
                OpenWindow(itemTable);
                return;
            }

            if (WindowUpdateState.Close == windowUpdateState)
            {
                CloseWindow(itemTable);
            }
        }

        private void CloseContainerOnEquip(ItemTable itemTable, HolderInteraction holderInteraction)
        {
            if (HolderInteraction.UnEquip == holderInteraction) return;

            CloseWindow(itemTable);
        }

        protected override void CloseWindow(ItemTable itemTable)
        {
            if (itemTable == null) return;

            if (!itemTable.IsContainer()) return;

            base.CloseWindow(itemTable);
        }

        protected override Window GetPrefabWindow()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.ContainerPrefabWindow;
        }

        protected override int GetMaxWindowOpen()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.MaxContainerWindowOpen;
        }
    }
}