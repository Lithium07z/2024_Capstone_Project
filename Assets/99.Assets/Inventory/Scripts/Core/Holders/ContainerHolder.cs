using Inventory.Scripts.Core.Helper;
using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.ItemsMetadata;
using Inventory.Scripts.Core.ScriptableObjects;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Anchors;
using UnityEngine;

namespace Inventory.Scripts.Core.Holders
{
    public class ContainerHolder : ItemHolder
    {
        [Space(16)] [Header("Container Holder Settings")] [SerializeField]
        private ContainerDisplayAnchorSo containerDisplayAnchorSo;

        [Header("Inventory Settings")] [SerializeField]
        private InventorySo inventorySo;

        protected override void OnEquipItem(ItemTable equippedItemTable)
        {
            if (equippedItemTable.InventoryMetadata is not ContainerMetadata containerMetadata) return;

            containerMetadata.OnEquipContainerHolder();

            if (inventorySo != null)
            {
                inventorySo.AddInventory(equippedItemTable);
            }

            if (containerDisplayAnchorSo == null)
            {
                Debug.LogError(
                    "[containerDisplayAnchorSo] Display Anchor not configured... Please make sure to create the scriptable object, pointing to a Container Display and this Container Holder."
                        .Configuration());
                return;
            }

            containerDisplayAnchorSo.OpenContainer(equippedItemTable);
        }

        protected override void OnUnEquipItem(ItemTable equippedItemTable)
        {
            if (equippedItemTable.InventoryMetadata is not ContainerMetadata) return;

            if (inventorySo != null)
            {
                inventorySo.RemoveInventory(equippedItemTable);
            }

            if (containerDisplayAnchorSo == null)
            {
                Debug.LogError(
                    "[containerDisplayAnchorSo] Display Anchor not configured... Please make sure to create the scriptable object, pointing to a Container Display and this Container Holder."
                        .Configuration());
                return;
            }

            containerDisplayAnchorSo.CloseContainer(equippedItemTable);
        }
    }
}