using Inventory.Scripts.Core.Enums;
using Inventory.Scripts.Core.Helper;
using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.Items.Grids;
using Inventory.Scripts.Core.ScriptableObjects;
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

    private (ItemTable, GridResponse) placeItemResult;

    private bool isOpen = false;
    private bool hasBeenCalled = false;
    private bool hasBeenGenerated = false;

    private int itemID = -1;

    private void Awake()
    {
        enabled = isEnabled;
    }

    private void Start()
    {
        _photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (placeItemResult.Item1 != null && placeItemResult.Item1.CurrentGridTable != null)
        {
            Debug.Log(placeItemResult.Item1.CurrentGridTable.ToString() + "!!!!!");

            /*
            ItemTable[] itemtable = placeItemResult.Item1.CurrentGridTable.GetAllItemsFromGrid();
            Debug.Log("GetAllItemsFromGrid is : " + itemtable.Length);
            foreach (var item in itemtable)
            {
                Debug.Log(item.ItemDataSo.name + "!");
            }
            */
            string temp = "";

            ItemTable[,] itemsSlot = placeItemResult.Item1.CurrentGridTable.InventoryItemsSlot;

            for (int i = 0; i < itemsSlot.GetLength(0); i++)
            {
                for (int j = 0; j < itemsSlot.GetLength(1); j++)
                {
                    temp += itemsSlot[i, j] == null ? "null " : itemsSlot[i, j].ItemDataSo.name + " ";
                }
                temp += "\n";
            }

            Debug.Log(temp);
        }
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
            _photonView.RPC("ReceiveRandomValue", RpcTarget.AllBufferedViaServer, itemID, true);
            hasBeenGenerated = true;
        }

        placeItemResult = inventorySupplierSo.PlaceItem(itemDataSo, _selectedGridTable);

        if (placeItemResult.Item2.Equals(GridResponse.InventoryFull))
        {
            Debug.Log("Inventory is full...".Info());
        }

        var abstractItem = placeItemResult.Item1.GetAbstractItem();

        if (!placeItemResult.Item2.Equals(GridResponse.Inserted) && abstractItem != null)
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