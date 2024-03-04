using Inventory.Scripts.Core.Controllers.Inputs;
using Inventory.Scripts.Core.Enums;
using Inventory.Scripts.Core.Helper;
using Inventory.Scripts.Core.Items.Grids;
using Inventory.Scripts.Core.ScriptableObjects;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Events.Interact;
using Inventory.Scripts.Core.ScriptableObjects.Datastores;
using Inventory.Scripts.Core.ScriptableObjects.Items;
using Photon.Pun;
using UnityEngine;

public class EnvironmentContainerCreatorController : MonoBehaviour
{
    [Header("Creator Configuration")]
    [SerializeField]
    private bool isEnabled;

    [Header("Datasource Items")]
    [SerializeField]
    private DatastoreItems datastoreItems;

    [Header("Inventory Supplier")]
    [SerializeField]
    private InventorySupplierSo inventorySupplierSo;

    private PhotonView _photonView;

    private GridTable _selectedGridTable;

    private ItemDataSo itemDataSo;

    private bool hasBeenCalled = false;
    private bool hasBeenGenerated = false;

    private int itemID;

    private void Awake()
    {
        enabled = isEnabled;
    }

    public void ChangeAbstractGrid(AbstractGrid abstractGrid)
    {
        if (!hasBeenCalled)
        {
            _selectedGridTable = abstractGrid != null ? abstractGrid.Grid : null;
            InsertRandomItem();
            hasBeenCalled = true;
        }
    }

    private void InsertRandomItem()
    {
        if (_selectedGridTable == null) return;

        //itemDataSo = datastoreItems.GetRandomItem();

        if (!hasBeenGenerated)
        {
            itemID = datastoreItems.GetRandomItemID();
            itemDataSo = datastoreItems.GetItemFromID(itemID);
            _photonView = GetComponent<PhotonView>();
            _photonView.RPC("ReceiveRandomValue", RpcTarget.AllBufferedViaServer, itemID, true);
            hasBeenGenerated = true;
        }

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

    [PunRPC]
    void ReceiveRandomValue(int itemID, bool hasBeenGenerated)
    {
        this.itemID = itemID;
        this.hasBeenGenerated = hasBeenGenerated;
        itemDataSo = datastoreItems.GetItemFromID(itemID);
        Debug.Log("Received Random Value: " + itemID);
    }
}