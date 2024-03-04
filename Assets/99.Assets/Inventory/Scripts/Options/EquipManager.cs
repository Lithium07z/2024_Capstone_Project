using System.Collections.Generic;
using Inventory.Scripts.Core.Enums;
using Inventory.Scripts.Core.Helper;
using Inventory.Scripts.Core.Holders;
using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.ScriptableObjects;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Events;
using UnityEngine;

namespace Inventory.Scripts.Options
{
    public class EquipManager : MonoBehaviour
    {
        [Header("Supplier")] [SerializeField] private InventorySupplierSo inventorySupplierSo;

        [Header("Holders")] [SerializeField] private List<ItemHolder> holders;

        [Header("Listening on...")] [SerializeField]
        private OnItemExecuteOptionEventChannelSo onItemExecuteEquipOptionEventChannelSo;

        private void Start()
        {
            if (holders is not { Count: > 0 })
            {
                Debug.Log(("Holders on " + nameof(EquipManager) +
                           " are empty was not configured... Trying to find holders on the scene...").Configuration());

                holders = new List<ItemHolder>();
                holders.AddRange(FindObjectsOfType<ItemHolder>());
            }
        }

        private void OnEnable()
        {
            onItemExecuteEquipOptionEventChannelSo.OnEventRaised += HandleEquipOption;
        }

        private void OnDisable()
        {
            onItemExecuteEquipOptionEventChannelSo.OnEventRaised -= HandleEquipOption;
        }

        private void HandleEquipOption(AbstractItem item)
        {
            if (item == null) return;

            var itemTable = item.ItemTable;
            var itemDataTypeSo = itemTable.ItemDataSo.ItemDataTypeSo;

            var holder = holders.Find(itemHolder =>
                !itemHolder.isEquipped && itemHolder.ItemDataTypeSo == itemDataTypeSo);

            if (holder == null)
            {
                Debug.Log(("Not found any holder free and with the same item type. Type: " + itemDataTypeSo).Info());
                return;
            }

            var equippableMessages = inventorySupplierSo.TryEquipItem(itemTable, holder);

            if (equippableMessages == HolderResponse.Equipped)
            {
                Debug.Log("Equipped item!".Info());
            }
        }
    }
}