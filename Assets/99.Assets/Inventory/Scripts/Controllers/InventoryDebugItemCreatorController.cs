using Inventory.Scripts.Core.Controllers.Inputs;
using Inventory.Scripts.Core.Enums;
using Inventory.Scripts.Core.Helper;
using Inventory.Scripts.Core.Items.Grids;
using Inventory.Scripts.Core.ScriptableObjects;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Events.Interact;
using Inventory.Scripts.Core.ScriptableObjects.Datastores;
using UnityEngine;

namespace Inventory.Scripts.Controllers
{
    public class InventoryDebugItemCreatorController : MonoBehaviour
    {
        [Header("Creator Configuration")] [SerializeField]
        private bool isEnabled;

        [Header("Datasource Items")] [SerializeField]
        private DatastoreItems datastoreItems;

        [Header("Inventory Supplier")] [SerializeField]
        private InventorySupplierSo inventorySupplierSo;

        [Header("Listening on...")] [SerializeField]
        private InputProviderSo inputProviderSo;

        [SerializeField] private OnGridInteractEventChannelSo onGridInteractEventChannelSo;

        private GridTable _selectedGridTable;

        private void Awake()
        {
            enabled = isEnabled;
            InsertRandomItem();
        }

        private void OnEnable()
        {
            inputProviderSo.OnGenerateItem += InsertRandomItem;
            onGridInteractEventChannelSo.OnEventRaised += ChangeAbstractGrid;
        }

        private void OnDisable()
        {
            inputProviderSo.OnGenerateItem -= InsertRandomItem;
            onGridInteractEventChannelSo.OnEventRaised -= ChangeAbstractGrid;
        }

        private void ChangeAbstractGrid(AbstractGrid abstractGrid)
        {
            _selectedGridTable = abstractGrid != null ? abstractGrid.Grid : null;
        }

        private void InsertRandomItem()
        {
            if (_selectedGridTable == null) return;

            var itemDataSo = datastoreItems.GetRandomItem();

            var (itemTable, inserted) = inventorySupplierSo.PlaceItem(itemDataSo, _selectedGridTable);

            if (inserted.Equals(GridResponse.InventoryFull))
            {
                Debug.Log("Inventory is full...".Info());
            }

            var abstractItem = itemTable.GetAbstractItem();

            if (!inserted.Equals(GridResponse.Inserted) && abstractItem != null)
            {
                Destroy(abstractItem.gameObject);
            }
        }
    }
}