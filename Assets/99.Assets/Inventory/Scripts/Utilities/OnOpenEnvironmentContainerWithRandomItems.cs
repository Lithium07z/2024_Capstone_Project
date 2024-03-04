using System.Collections.Generic;
using Inventory.Scripts.Core.Holders;
using Inventory.Scripts.Core.ItemsMetadata;
using Inventory.Scripts.Core.ScriptableObjects;
using Inventory.Scripts.Core.ScriptableObjects.Datastores;
using Inventory.Scripts.Core.ScriptableObjects.Items;
using UnityEngine;

namespace Inventory.Scripts.Utilities
{
    [RequireComponent(typeof(EnvironmentContainerHolder))]
    public class OnOpenEnvironmentContainerWithRandomItems : MonoBehaviour
    {
        [Header("Configuration")] [SerializeField]
        private DatastoreItems datastoreItems;

        [SerializeField,
         Tooltip(
             "The probability of generating an item. If x value is 0 will possibly not generate any item, but also could generate 2/3 items if you set 3/4 on y value.")]
        private Vector2Int minMaxItemsGeneration = new(1, 4);

        [SerializeField] private bool regenerateOnReopen;

        [Header("Reference")] [SerializeField] private InventorySupplierSo inventorySupplierSo;

        private EnvironmentContainerHolder _environmentContainerHolder;

        private bool _alreadyGeneratedItems;

        private void Awake()
        {
            _environmentContainerHolder = GetComponent<EnvironmentContainerHolder>();
            _alreadyGeneratedItems = false;
        }

        private void OnEnable()
        {
            _environmentContainerHolder.OnChangeOpenState += OnEnvironmentContainerChangeState;
        }

        private void OnDisable()
        {
            _environmentContainerHolder.OnChangeOpenState -= OnEnvironmentContainerChangeState;
        }

        private void OnEnvironmentContainerChangeState(bool isOpen)
        {
            if (isOpen)
            {
                GenerateRandomItems();
            }
        }

        private void GenerateRandomItems()
        {
            if (_alreadyGeneratedItems && !regenerateOnReopen) return;

            var itemDataSos = GetRandomItems();

            var itemTable = _environmentContainerHolder.GetItemTable();

            if (itemTable.InventoryMetadata is not ContainerMetadata containerMetadata) return;

            var grids = containerMetadata.GridsInventory;

            if (regenerateOnReopen)
            {
                inventorySupplierSo.ClearGridTables(grids);
            }

            foreach (var itemDataSo in itemDataSos)
            {
                inventorySupplierSo.FindPlaceForItemInGrids(itemDataSo, grids);
            }

            _alreadyGeneratedItems = true;
        }

        private List<ItemDataSo> GetRandomItems()
        {
            var itemsToGet = Random.Range(minMaxItemsGeneration.x, minMaxItemsGeneration.y);

            return datastoreItems.GetRandomItems(itemsToGet);
        }
    }
}