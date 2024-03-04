using Inventory.Scripts.Core.Items.Grids;
using UnityEngine;

namespace Inventory.Scripts.Core.ScriptableObjects.Items
{
    [CreateAssetMenu(menuName = "Inventory/Items/New Item Container")]
    public class ItemContainerDataSo : ItemDataSo
    {
        [Header("Container Settings")] [SerializeField]
        private ContainerGrids containerGrids;

        public ContainerGrids ContainerGrids => containerGrids;
    }
}