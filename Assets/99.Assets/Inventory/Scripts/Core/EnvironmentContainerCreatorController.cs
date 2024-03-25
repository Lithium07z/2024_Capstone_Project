using Inventory.Scripts.Core.Enums;
using Inventory.Scripts.Core.Helper;
using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.Items.Grids;
using Inventory.Scripts.Core.ItemsMetadata;
using Inventory.Scripts.Core.ScriptableObjects;
using Inventory.Scripts.Core.ScriptableObjects.Datastores;
using Inventory.Scripts.Core.ScriptableObjects.Items;
using Photon.Pun;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class EnvironmentContainerCreatorController : MonoBehaviour
{
    [Header("Datasource Items")]
    [SerializeField]
    private DatastoreItems datastoreItems;

    [Header("Inventory Supplier")]
    [SerializeField]
    private InventorySupplierSo inventorySupplierSo;

    private PhotonView _photonView; // photonView

    private ItemDataSo itemDataSo;  // ������ ������ ��ũ���ͺ� ������Ʈ

    private ItemTable[] _allItemsFromGrid;   // ���� �÷��̾��� GridTable�� �ִ� ��� ������

    private GridTable _selectedGridTable;   // ���� �÷��̾��� GridTable

    private (ItemTable, GridResponse) _placeItemResult;  // �׸��忡 �������� �߰��� ���� ItemTable�� �߰� ���

    private int itemID = -1;    // ������ ID
    
    private bool _hasBeenCalled = false;    // Grid�� �ʱ�ȭ�ߴ��� �Ǵ��ϴ� �÷���
    public bool _hasBeenGenerated = false;  // �������� �����Ǿ����� �Ǵ��ϴ� �÷���

    private List<int> _allItemsFromGridInt;         // GridTable�� �������� Int������ �����ϴ� �迭
    private string[] _allItemsFromGridPosition;     // GridTable�� ������ ��ġ�� �����ϴ� �迭
    private bool[] _allItemsFormGridRotation;         // GridTable�� ������ ȸ�� ���θ� �����ϴ� �迭

    private string[] _itemPosition = new string[2];  // ������ ��ġ�� �и��� �ӽ� �����ϴ� �迭

    private void Start()
    {
        _photonView = GetComponent<PhotonView>();
    }

    /// <summary>
    /// ���� �÷��̾��� Grid�� �޾� �ʱ�ȭ�ϴ� �Լ�
    /// </summary>
    /// <param name="abstractGrid">���� �÷��̾��� Grid</param>
    public void ChangeAbstractGrid(AbstractGrid abstractGrid)
    {
        if (!_hasBeenCalled) // �÷��̾� ���� �� �ѹ��� ȣ�� ��
        {
            _selectedGridTable = abstractGrid != null ? abstractGrid.Grid : null;   // ���� Grid�� �ʱ�ȭ�� �� ��
            GenerateRandomItem();   // ������ ���� �Լ� ȣ��
            _hasBeenCalled = true;  // �÷��� �������� �� ȣ���� ����
        }

        if (_allItemsFromGridInt != null)
        {
            ConvertArrayToGridTable();
        }
    }

    /*************************************** 
    *          �� ������ ���� �Լ� ��         *                                       
    ***************************************/

    /// <summary>
    /// ������ ���� �Լ�, ���� ȣ�� ���� ȣ����� ���� (������ ����� ����)
    /// </summary>
    private void GenerateRandomItem()
    {
        if (_selectedGridTable == null)
        {   // ���� �÷��̾��� GridTable�� ���ٸ� �ߴ�
            return;
        }

        if (!_hasBeenGenerated)
        {   // ���� �������� �������� ���� ��츸 ����
            itemID = datastoreItems.GetRandomItemID();          // ������ ������ID�� �޾ƿ�
            itemDataSo = datastoreItems.GetItemFromID(itemID);  // ������ID�� �ش��ϴ� �������� �޾ƿ� 

            PlaceItem(itemDataSo);  // ���� ������ GridTable�� �ش� �������� �־���
            SendGenerateFlag(true); // ������ ���� �÷��� ����ȭ
        }
    }

    /*************************************** 
    *          �� ������ ���� �Լ� ��          *                                       
    ***************************************/

    /// <summary>
    /// GridTable�� ������ ���� �Լ�
    /// </summary>
    /// <param name="_itemDataSo">GridTable�� ���� �������� ItemDataSo</param>
    /// <param name="_posX">GridTable�� ���� �������� x��ǥ</param>
    /// <param name="_posY">GridTable�� ���� �������� y��ǥ</param>
    private void PlaceItem(ItemDataSo _itemDataSo, int _posX = -1, int _posY = -1, bool isRotated = false)
    {
        _placeItemResult = inventorySupplierSo.PlaceItem(_itemDataSo, _selectedGridTable, _posX, _posY, isRotated);

        if (_placeItemResult.Item2.Equals(GridResponse.InventoryFull))
        {   // �κ��丮�� ���� �� ���
            Debug.Log("Inventory is full...".Info());
        }

        var abstractItem = _placeItemResult.Item1.GetAbstractItem();

        if (!_placeItemResult.Item2.Equals(GridResponse.Inserted) && abstractItem != null)
        {
            Destroy(abstractItem.gameObject);
        }
    }

    public void InsertPlayerEquipmentToList(List<ItemTable> _playerEquipment)
    {
        _allItemsFromGridInt = new List<int>();

        foreach (ItemTable equipment in _playerEquipment)
        {
            _allItemsFromGridInt.Add(equipment.ItemDataSo.itemID);
        }
    }

    public void InsertItemToList(ItemTable[] _allItemsFromGrid)
    {
        foreach (ItemTable _itemTable in _allItemsFromGrid) //
        {
            if (_itemTable.InventoryMetadata is ContainerMetadata _containerMetadata)   //
            {
                foreach (GridTable _gridTable in _containerMetadata.GridsInventory)     //
                {
                    ItemTable[] _allItemsFromNextGrid = _gridTable.GetAllItemsFromGrid();   //

                    if (_allItemsFromNextGrid.Length > 0)   //
                    {
                        InsertItemToList(_gridTable.GetAllItemsFromGrid()); //
                    }
                }
            }

            //
            _allItemsFromGridInt.Add(_itemTable.ItemDataSo.itemID);
        }

        SendAllItemsInfoFromGrid();
    }

    /*************************************** 
    *    �� GridTable, Array�� ��ȯ �Լ� ��   *                                       
    ***************************************/

    /// <summary>
    /// ���� �÷��̾��� GridTable�� ������� 1���� �迭�� ������ ��ϰ� ��ǥ�� �־��ִ� �Լ�
    /// </summary>
    public void ConvertGridTableToArray()
    {
        StringBuilder stringBuilder = new StringBuilder();

        _allItemsFromGrid = _selectedGridTable.GetAllItemsFromGrid();   // ���� �÷��̾��� GridTable�κ��� ��� �������� ����
        _allItemsFromGridInt = new List<int>();
        _allItemsFromGridPosition = new string[_allItemsFromGrid.Length];
        _allItemsFormGridRotation = new bool[_allItemsFromGrid.Length];

        for (int i = 0; i < _allItemsFromGrid.Length; i++)
        {
            stringBuilder.Clear();
            // �� �������� ItemID�� ������ Int�� �迭�� �ʱ�ȭ
            _allItemsFromGridInt.Add(_allItemsFromGrid[i].ItemDataSo.itemID);
            _allItemsFromGridPosition[i] = stringBuilder.Append(_allItemsFromGrid[i].OnGridPositionX).Append(" ").Append(_allItemsFromGrid[i].OnGridPositionY).ToString();
            _allItemsFormGridRotation[i] = _allItemsFromGrid[i].IsRotated;
        }

        SendAllItemsInfoFromGrid();  // ���� �����̳��� ��� �������� Int�� �迭�� ����
    }

    /// <summary>
    /// ����ȭ �� Int�� 1���� �迭�� ������� GridTable�� �������� �־��ִ� �Լ�
    /// </summary>
    private void ConvertArrayToGridTable()
    {
        // �������� ä��� ���� GridTable�� ��� �����
        inventorySupplierSo.ClearGridTable(_selectedGridTable);

        for (int i = 0; i < _allItemsFromGridInt.Count; i++)
        {
            itemDataSo = datastoreItems.GetItemFromID(_allItemsFromGridInt[i]); // ����ȭ �� �迭�� ID�� ������� �������� ���

            if (_allItemsFromGridPosition != null)
            {
                _itemPosition = _allItemsFromGridPosition[i].Split(' ');                            // string���� ��ǥ�� ���� ���� �и��� �ӽ� ����    
                PlaceItem(itemDataSo, int.Parse(_itemPosition[0]), int.Parse(_itemPosition[1]), _allItemsFormGridRotation[i]);    // ���� ������ GridTable�� �ش� �������� �־���
            }
            else
            {
                PlaceItem(itemDataSo);
            }
        }
    }

    /*************************************** 
    *      �� �÷��̾� ������ ���� �Լ� ��     *                                       
    ***************************************/

    public void ConvertGridTableToList(List<GridTable> _gridTables)
    {
        foreach (GridTable _gridTable in _gridTables)   // ��ü GridTable�� �˻�
        {
            ItemTable[] _allItemsFromGrid = _gridTable.GetAllItemsFromGrid();   // �� GridTable�� ��� �������� ����

            if (_allItemsFromGrid.Length > 0)   // GridTable�� �������� �����ϴ� ���
            {
                InsertItemToList(_allItemsFromGrid);    // ��� �߰��ϱ� ���� InsertItemToList �Լ� ȣ��
            }
        }
    }

    /*************************************** 
    *            �� ����ȭ �Լ� ��            *                                       
    ***************************************/

    /// <summary>
    /// �����̳� ������ ���� ���� �÷��׸� �޾� ����ȭ�ϴ� �Լ�
    /// </summary>
    /// <param name="_hasBeenGenerated">���� ���� Ȯ�ο� �÷���</param>
    [PunRPC]
    private void ReceiveGenerateFlag(bool _hasBeenGenerated)
    {
        this._hasBeenGenerated = _hasBeenGenerated;
    }

    /// <summary>
    /// �����̳� ������ ���� ���� �÷��׸� RPC�Լ��� ȣ���ϸ� ���ڷ� ����
    /// </summary>
    private void SendGenerateFlag(bool _hasBeenGenerated)
    {
        _photonView.RPC("ReceiveGenerateFlag", RpcTarget.AllBufferedViaServer, _hasBeenGenerated);
    }

    /// <summary>
    /// RPC ȣ��� ���޹��� Int�� ������ �迭�� ����ȭ�ϴ� �Լ�
    /// </summary>
    /// <param name="_allItemsFromGridInt">����ȭ�ϱ� ���� ���� Int�� ������ �迭</param>
    /// <param name="_allItemsFromGridPosition">����ȭ�ϱ� ���� ���� string�� ������ ��ġ �迭</param>
    /// <param name="_allItemsFormGridRotation">����ȭ�ϱ� ���� ���� bool�� ������ ȸ�� ���� �迭</param>
    [PunRPC]
    private void ReceiveAllItemsInfoFromGrid(int[] _allItemsFromGridInt, string[] _allItemsFromGridPosition, bool[] _allItemsFormGridRotation)
    {
        // ���� Box�� Int�� ������ �迭�� ������ ��ġ�� ����ȭ
        this._allItemsFromGridInt = new List<int>(_allItemsFromGridInt);
        this._allItemsFromGridPosition = _allItemsFromGridPosition;
        this._allItemsFormGridRotation = _allItemsFormGridRotation;
    }

    /// <summary>
    /// �����̳��� ������ ��� �����迭�� RPC�Լ��� ȣ���ϸ� ���ڷ� ����
    /// </summary>
    private void SendAllItemsInfoFromGrid()
    {
        _photonView.RPC("ReceiveAllItemsInfoFromGrid", RpcTarget.AllBufferedViaServer, _allItemsFromGridInt.ToArray(), _allItemsFromGridPosition, _allItemsFormGridRotation);
    }
}