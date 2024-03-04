using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.ScriptableObjects;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Events;
using UnityEngine;

namespace Inventory.Scripts.Options
{
    public class DiscardManager : MonoBehaviour
    {
        [Header("Supplier")] [SerializeField] private InventorySupplierSo inventorySupplierSo;

        [Header("Listening on...")] [SerializeField]
        private OnItemExecuteOptionEventChannelSo onItemExecuteOptionEventChannelSo;

        [Header("Broadcasting on...")] [SerializeField]
        private WindowContainerChangeStateEventChannelSo windowContainerChangeStateEventChannelSo;

        private void OnEnable()
        {
            onItemExecuteOptionEventChannelSo.OnEventRaised += HandleDiscardOption;
        }

        private void OnDisable()
        {
            onItemExecuteOptionEventChannelSo.OnEventRaised -= HandleDiscardOption;
        }

        private void HandleDiscardOption(AbstractItem item)
        {
            var itemTable = item.ItemTable;

            inventorySupplierSo.RemoveItem(itemTable);

            windowContainerChangeStateEventChannelSo.RaiseEvent(itemTable, WindowUpdateState.Close);

            Destroy(item.gameObject);
        }
    }
}