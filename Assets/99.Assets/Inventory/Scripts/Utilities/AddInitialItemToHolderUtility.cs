using Inventory.Scripts.Core.Enums;
using Inventory.Scripts.Core.Helper;
using Inventory.Scripts.Core.Holders;
using Inventory.Scripts.Core.ScriptableObjects;
using Inventory.Scripts.Core.ScriptableObjects.Items;
using UnityEngine;

namespace Inventory.Scripts.Utilities
{
    [DefaultExecutionOrder(100)]
    public class AddInitialItemToHolderUtility : MonoBehaviour
    {
        [Header("Configs")] [SerializeField] private ItemHolder itemHolder;

        [SerializeField] private ItemDataSo initialItemDataSo;

        [Header("Supplier")] [SerializeField] private InventorySupplierSo inventorySupplierSo;

        private void Start()
        {
            var (_, holderResponse) = inventorySupplierSo.TryEquipItem(initialItemDataSo, itemHolder);

            if (holderResponse == HolderResponse.Equipped)
            {
                Debug.Log("Equipped initial item!".Info());
            }
        }
    }
}