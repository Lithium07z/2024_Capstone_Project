using Inventory.Scripts.Core.Enums;
using Inventory.Scripts.Core.Helper;
using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.Items.Grids;
using Inventory.Scripts.Core.ScriptableObjects;
using Inventory.Scripts.Core.ScriptableObjects.Datastores;
using Inventory.Scripts.Core.ScriptableObjects.Items;
using Photon.Pun;
using System.IO;
using System.Text;
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

    private PhotonView _photonView; // photonView

    private ItemDataSo itemDataSo;  // 아이템 데이터 스크립터블 오브젝트

    private ItemTable[] _allItemsFromGrid;   // 현재 플레이어의 GridTable에 있는 모든 아이템

    private GridTable _selectedGridTable;   // 현재 플레이어의 GridTable

    private (ItemTable, GridResponse) _placeItemResult;  // 그리드에 아이템을 추가한 뒤의 ItemTable과 추가 결과

    private int itemID = -1;    // 아이템 ID
    
    private bool _hasBeenCalled = false;    // Grid를 초기화했는지 판단하는 플래그
    private bool _hasBeenGenerated = false; // 아이템이 생성되었는지 판단하는 플래그

    private int[] _allItemsFromGridInt;          // GridTable의 아이템을 Int형으로 저장하는 배열
    private string[] _allItemsFromGridPosition;  // GridTable의 아이템 위치를 저장하는 배열

    private string[] _itemPosition = new string[2];  // 아이템 위치를 분리해 임시 저장하는 배열

    private void Awake()
    {
        enabled = isEnabled;
    }

    private void Start()
    {
        _photonView = GetComponent<PhotonView>();
    }

    /// <summary>
    /// 현재 플레이어의 Grid를 받아 초기화하는 함수
    /// </summary>
    /// <param name="abstractGrid">현재 플레이어의 Grid</param>
    public void ChangeAbstractGrid(AbstractGrid abstractGrid)
    {
        if (!_hasBeenCalled) // 플레이어 마다 단 한번만 호출 됨
        {
            _selectedGridTable = abstractGrid != null ? abstractGrid.Grid : null;   // 현재 Grid를 초기화해 준 뒤
            GenerateRandomItem();   // 아이템 생성 함수 호출
            _hasBeenCalled = true;  // 플래그 변경으로 재 호출을 방지
        }

        if (_allItemsFromGridInt != null)
        {
            ConvertArrayToGridTable();
        }
    }

    /// <summary>
    /// 아이템 생성 함수, 최초 호출 이후 호출되지 않음 (아이템 재생성 방지)
    /// </summary>
    private void GenerateRandomItem()
    {
        if (_selectedGridTable == null)
        {   // 현재 플레이어의 GridTable이 없다면 중단
            return;
        }

        if (!_hasBeenGenerated)
        {   // 아직 아이템을 생성하지 않은 경우만 진입
            itemID = datastoreItems.GetRandomItemID();          // 랜덤한 아이템ID를 받아옴
            itemDataSo = datastoreItems.GetItemFromID(itemID);  // 아이템ID에 해당하는 아이템을 받아옴 

            PlaceItem(itemDataSo);  // 현재 유저의 GridTable에 해당 아이템을 넣어줌
            SendGenerateFlag(true); // 아이템 생성 플래그 동기화
        }
    }

    /// <summary>
    /// 현재 플레이어의 GridTable을 기반으로 1차원 배열에 아이템 목록과 좌표를 넣어주는 함수
    /// </summary>
    public void ConvertGridTableToArray()
    {
        StringBuilder stringBuilder = new StringBuilder();

        _allItemsFromGrid = _selectedGridTable.GetAllItemsFromGrid();   // 현재 플레이어의 GridTable로부터 모든 아이템을 얻음
        _allItemsFromGridInt = new int [_allItemsFromGrid.Length];
        _allItemsFromGridPosition = new string[_allItemsFromGrid.Length];

        for (int i = 0; i < _allItemsFromGridInt.Length; i++)
        {
            stringBuilder.Clear();
            // 각 아이템의 ItemID를 추출해 Int형 배열에 초기화
            _allItemsFromGridInt[i] = _allItemsFromGrid[i].ItemDataSo.itemID;
            _allItemsFromGridPosition[i] = stringBuilder.Append(_allItemsFromGrid[i].OnGridPositionX).Append(" ").Append(_allItemsFromGrid[i].OnGridPositionY).ToString();
        }

        SendAllItemsInfoFromGrid();  // 현재 컨테이너의 모든 아이템을 Int형 배열로 전달
    }

    /// <summary>
    /// 동기화 된 Int형 1차원 배열을 기반으로 GridTable에 아이템을 넣어주는 함수
    /// </summary>
    private void ConvertArrayToGridTable()
    {
        // 아이템을 채우기 전에 GridTable을 모두 비워줌
        inventorySupplierSo.ClearGridTable(_selectedGridTable);

        for (int i = 0; i < _allItemsFromGridInt.Length; i++)
        {
            itemDataSo = datastoreItems.GetItemFromID(_allItemsFromGridInt[i]); // 동기화 된 배열의 ID를 기반으로 아이템을 얻고
            _itemPosition = _allItemsFromGridPosition[i].Split(' ');            // string형의 좌표를 공백 기준 분리해 임시 저장    
            PlaceItem(itemDataSo, int.Parse(_itemPosition[0]), int.Parse(_itemPosition[1]));    // 현재 유저의 GridTable에 해당 아이템을 넣어줌
        }
    }

    /// <summary>
    /// 컨테이너 아이템 생성 여부 플래그를 받아 동기화하는 함수
    /// </summary>
    /// <param name="_hasBeenGenerated">생성 여부 확인용 플래그</param>
    [PunRPC]
    private void ReceiveGenerateFlag(bool _hasBeenGenerated)
    {
        this._hasBeenGenerated = _hasBeenGenerated;
    }

    /// <summary>
    /// 컨테이너 아이템 생성 여부 플래그를 RPC함수를 호출하며 인자로 전달
    /// </summary>
    private void SendGenerateFlag(bool _hasBeenGenerated)
    {
        _photonView.RPC("ReceiveGenerateFlag", RpcTarget.AllBufferedViaServer, _hasBeenGenerated);
    }

    /// <summary>
    /// RPC 호출로 전달받은 Int형 아이템 배열을 동기화하는 함수
    /// </summary>
    /// <param name="_allItemsFromGridInt">동기화하기 위해 받은 Int형 아이템 배열</param>
    /// <param name="_allItemsFromGridPosition">동기화하기 위해 받은 string형 아이템 위치 배열</param>
    [PunRPC]
    private void ReceiveAllItemsInfoFromGrid(int[] _allItemsFromGridInt, string[] _allItemsFromGridPosition)
    {
        // 현재 Box에 Int형 아이템 배열과 아이템 위치를 동기화
        this._allItemsFromGridInt = _allItemsFromGridInt;
        this._allItemsFromGridPosition = _allItemsFromGridPosition;
    }

    /// <summary>
    /// 컨테이너의 아이템 목록 정수배열을 RPC함수를 호출하며 인자로 전달
    /// </summary>
    private void SendAllItemsInfoFromGrid()
    {
        _photonView.RPC("ReceiveAllItemsInfoFromGrid", RpcTarget.AllBufferedViaServer, _allItemsFromGridInt, _allItemsFromGridPosition);
    }

    /// <summary>
    /// 인자로 받은 현재 플레이어의 GridTable에 아이템을 넣는 함수
    /// </summary>
    /// <param name="_itemDataSo">GridTable에 넣을 아이템</param>
    /// <param name="_posX">GridTable에 넣을 아이템의 x좌표</param>
    /// <param name="_posY">GridTable에 넣을 아이템의 y좌표</param>
    private void PlaceItem(ItemDataSo _itemDataSo, int _posX = -1, int _posY = -1)
    {
        _placeItemResult = inventorySupplierSo.PlaceItem(_itemDataSo, _selectedGridTable, _posX, _posY);

        if (_placeItemResult.Item2.Equals(GridResponse.InventoryFull))
        {   // 인벤토리가 꽉 찼다면 로그 출력
            Debug.Log("Inventory is full...".Info());
        }

        var abstractItem = _placeItemResult.Item1.GetAbstractItem();

        if (!_placeItemResult.Item2.Equals(GridResponse.Inserted) && abstractItem != null)
        {   
            Destroy(abstractItem.gameObject);
        }
    }
}