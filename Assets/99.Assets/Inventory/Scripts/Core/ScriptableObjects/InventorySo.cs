using System.Collections.Generic;
using System.Linq;
using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.Items.Grids;
using Inventory.Scripts.Core.ItemsMetadata;
using UnityEngine;

namespace Inventory.Scripts.Core.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Inventory/New Inventory Group")]
    public class InventorySo : ScriptableObject
    {
        [SerializeReference] private List<ItemTable> inventories;

        public List<ItemTable> Inventories => inventories;

        public void AddInventory(ItemTable itemTable)
        {
            if (itemTable?.InventoryMetadata is not ContainerMetadata) return;

            inventories.Add(itemTable);
        }

        public void RemoveInventory(ItemTable inventoryItem)
        {
            inventories.Remove(inventoryItem);
        }

        public List<GridTable> GetGrids()
        {
            return inventories.Where(item => item.IsContainer())
                .Select(item => (ContainerMetadata)item.InventoryMetadata)
                .SelectMany(metadata => metadata.GridsInventory)
                .ToList();
        }
    }
}