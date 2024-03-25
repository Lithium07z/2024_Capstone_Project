using Inventory.Scripts.Core.Displays.Filler;
using Inventory.Scripts.Core.Enums;
using Inventory.Scripts.Core.Holders;
using Inventory.Scripts.Core.Items.Grids;
using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.ScriptableObjects.Items;
using Inventory.Scripts.Core.ScriptableObjects;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class InteractionController : MonoBehaviour
{
    // photon view
    private PhotonView _photonView;

    // inventory canvas group
    private CanvasGroup _canvasGroup;

    // cinemachine virtual camera
    private GameObject _cinemachineVirtualCamera;

    // displayFiller for ScrollArea_Environment Component
    public DisplayFiller _displayFiller;

    // inventory canvas
    [SerializeField] private GameObject _inventory;         // 인벤토리 캔버스
    [SerializeField] private GameObject _inventoryManager;  // 인벤토리 매니저

    [SerializeField] private InventorySo _playerInventorySo;            // player inventory scriptable object
    [SerializeField] private InventorySupplierSo _inventorySupplierSo;  // inventory supplier scriptable object

    [SerializeField] private ItemContainerDataSo _itemContainerDataSo;  // 플레이어 사망 시 변경할 ItemContainerDataSo

    // player conainer
    [Header("Container Holder")]
    [SerializeField] private ContainerHolder[] _containerHolders = new ContainerHolder[3];
    // [0] : 조끼 인벤토리    [1] : 가방 인벤토리   [2] : 지갑 인벤토리

    [Header("Item Holder")]
    [SerializeField] private ItemHolder[] _itemHolders = new ItemHolder[7];
    /* [0] : 헬멧 인벤토리        [1] : 주무기 인벤토리1    [2] : 주무기 인벤토리2  [3] : 보조무기 인벤토리  *
     * [4] : 근접무기 인벤토리    [5] : 투척무기 인벤토리1  [6] : 투척무기 인벤토리2                        */

    // 컨테이너 아이템 홀더
    private EnvironmentContainerHolder _environmentContainerHolder;
    private EnvironmentContainerHolder _playerContainerHolder;

    // 컨테이너 아이템 생성 및 동기화 컨트롤러
    private EnvironmentContainerCreatorController _environmentContainerCreatorController;
    private EnvironmentContainerCreatorController _playerContainerCreatorController;

    // 아이템 타입 ItemDataTypeSo
    [Header("ItemDataTypeSo")]
    [SerializeField] private ItemDataTypeSo _helmetTypeSo;
    [SerializeField] private ItemDataTypeSo _weaponTypeSo;
    [SerializeField] private ItemDataTypeSo _pistolTypeSo;
    [SerializeField] private ItemDataTypeSo _knifeTypeSo;
    [SerializeField] private ItemDataTypeSo _granadeTypeSo;
    [SerializeField] private ItemDataTypeSo _chestTypeSo;
    [SerializeField] private ItemDataTypeSo _backpackTypeSo;
    [SerializeField] private ItemDataTypeSo _walletTypeSo;

    private enum itemDataTypeSoEnum { helmetTypeSo,  }

    // RaycastHit, Ray
    private RaycastHit _hit;
    private Ray _ray;

    // 인벤토리 플래그
    public bool _isInventoryOpen = false;

    // 커서 On/Off를 위한 델리게이트
    public delegate void ToggleCursorDelegate(bool state);

    //
    public ToggleCursorDelegate _toggleCursorDelegate;

    private void Start()
    {
        _photonView = this.GetComponent<PhotonView>();
        _canvasGroup = _inventory.GetComponent<CanvasGroup>();

        _cinemachineVirtualCamera = GameObject.Find("PlayerFollowCamera");

        _playerContainerHolder = this.GetComponent<EnvironmentContainerHolder>();
        _playerContainerCreatorController = this.GetComponent<EnvironmentContainerCreatorController>();

        if (_photonView.IsMine)
        {
            _inventory.SetActive(true);
            _inventoryManager.SetActive(true);
        }
    }

    private void Update()
    {
        // 시네머신 버츄얼 카메라가 바라보는 위치로 Ray 발사
        _ray = new Ray(_cinemachineVirtualCamera.transform.position, _cinemachineVirtualCamera.transform.forward);

        /*************************************** 
        *              ↓디버깅 코드↓            *                                       
        ***************************************/

        Debug.DrawRay(_cinemachineVirtualCamera.transform.position, _cinemachineVirtualCamera.transform.forward * 6, Color.red);

        if (_playerInventorySo.GetGrids().Count > 0)
        {
            int sum = 0;
            for (int i = 0; i < _playerInventorySo.GetGrids().Count; i++)
            {
                sum += _playerInventorySo.GetGrids()[i].GetAllItemsFromGrid().Length;
            }

            Debug.Log(sum);
        }
    }

    /*************************************** 
    *              ↓인풋 함수↓            *                                       
    ***************************************/

    /// <summary>
    /// 컨테이너 오픈 함수 | Inventory Input Actions와 연동됨 | 키 : E Key
    /// </summary>
    /// <param name="value">NewInput 입력, E Key</param>
    private void OnInteraction(InputValue value)
    {   // 현재 플레이어 레이어를 제외하고 Ray에 충돌한 물체 반환
        if (Physics.Raycast(_ray, out _hit, 6, ~LayerMask.GetMask("Player")))
        {
            string _tag = _hit.transform.tag;

            if (_tag.Equals("Container"))
            {   // 컨테이너인 경우
                OpenContainer();    // 컨테이너 오픈
            }
            else
            {   // FindPlaceForItemInGrids함수의 반환형, 넣으려 시도한 아이템의 ItemTable과 결과를 반환함
                (ItemTable, GridResponse) findPlaceResult = new(null, GridResponse.NoGridTableSelected);
                Item item = _hit.transform.GetComponent<Item>();    // 얻으려는 물체의 아이템 스크립트를 얻어옴

                if (item != null)   // 아이템인 경우
                {   // TryEquipItem함수의 반환형, 입으려 시도한 아이템의 ItemTable과 결과를 반환함
                    (ItemTable, HolderResponse) equipItemResult = new(null, HolderResponse.Error);
                    ItemDataSo itemDataSo = item.GetItemDataSo();               // 실제 아이템 정보를 얻어옴
                    ItemDataTypeSo itemDataTypeSo = itemDataSo.ItemDataTypeSo;  // 아이템의 타입 정보를 얻어옴
                    
                    if (itemDataTypeSo == _helmetTypeSo)        // 헬멧의 경우
                    {
                        equipItemResult = _inventorySupplierSo.TryEquipItem(itemDataSo, _itemHolders[0]);
                    }
                    else if (itemDataTypeSo == _weaponTypeSo)   // 주무기의 경우
                    {
                        equipItemResult = _inventorySupplierSo.TryEquipItem(itemDataSo, _itemHolders[1]);

                        // 아이템을 이미 착용한 경우
                        if (equipItemResult.Item2 == HolderResponse.AlreadyEquipped)
                        {   // 주무기 2번 창에 착용
                            equipItemResult = _inventorySupplierSo.TryEquipItem(itemDataSo, _itemHolders[2]);
                        }
                    }
                    else if (itemDataTypeSo == _pistolTypeSo) // 보조무기 의 경우
                    {
                        equipItemResult = _inventorySupplierSo.TryEquipItem(itemDataSo, _itemHolders[3]);
                    }
                    else if (itemDataTypeSo == _knifeTypeSo) // 근접무기의 경우
                    {
                        equipItemResult = _inventorySupplierSo.TryEquipItem(itemDataSo, _itemHolders[4]);
                    }
                    else if (itemDataTypeSo == _granadeTypeSo) // 투척무기의 경우
                    {
                        equipItemResult = _inventorySupplierSo.TryEquipItem(itemDataSo, _itemHolders[5]);

                        // 아이템을 이미 착용한 경우
                        if (equipItemResult.Item2 == HolderResponse.AlreadyEquipped)
                        {   // 주무기 2번 창에 착용
                            equipItemResult = _inventorySupplierSo.TryEquipItem(itemDataSo, _itemHolders[6]);
                        }
                    }
                    else if (itemDataTypeSo == _chestTypeSo)    // 조끼의 경우
                    {
                        equipItemResult = _inventorySupplierSo.TryEquipItem(itemDataSo, _containerHolders[0]);
                    }
                    else if (itemDataTypeSo == _backpackTypeSo) // 가방의 경우
                    {
                        equipItemResult = _inventorySupplierSo.TryEquipItem(itemDataSo, _containerHolders[1]);
                    }
                    else if (itemDataTypeSo == _walletTypeSo)   // 지갑의 경우
                    {
                        equipItemResult = _inventorySupplierSo.TryEquipItem(itemDataSo, _containerHolders[2]);
                    }
                    else
                    {   // 아이템이지만 장비가 아닌 경우
                        findPlaceResult = _inventorySupplierSo.FindPlaceForItemInGrids(itemDataSo, _playerInventorySo.GetGrids());

                        if (findPlaceResult.Item2 != GridResponse.Inserted)
                        {   // 아이템이 인벤토리에 들어가지 않은 경우
                            return;
                        }
                    }
                    
                    // 아이템을 이미 착용한 경우
                    if (equipItemResult.Item2 == HolderResponse.AlreadyEquipped)
                    {   // 아이템을 인벤토리에 넣음
                        findPlaceResult = _inventorySupplierSo.FindPlaceForItemInGrids(itemDataSo, _playerInventorySo.GetGrids());

                        if (findPlaceResult.Item2 != GridResponse.Inserted)
                        {   // 못넣는 경우
                            return;
                        }
                    }

                    Debug.Log("Take Item");

                    // 아이템 획득이 끝났다면 파괴
                    item.DestroyItem();
                }
            }
        }
    }

    /// <summary>
    /// 인벤토리 오픈 함수 | Inventory Input Actions와 연동됨 | 키 : Tab Key
    /// </summary>
    /// <param name="value">NewInput 입력, TabKey</param>
    private void OnOpenInventory(InputValue value)
    {
        if (_environmentContainerHolder != null)
        {   // 이전에 열었던 컨테이너의 EnvironmentContainerHolder가 아직 열려있다면
            _environmentContainerHolder.CloseContainer();       // 닫아주고
            _isInventoryOpen = false;                           // 플래그 변경
        }
        
        ToggleInventory();  // 인벤토리를 오픈
        _playerContainerHolder.OpenContainer();
    }

    /*************************************** 
    *            ↓상호작용 함수↓            *                                       
    ***************************************/

    /// <summary>
    /// 컨테이너 오픈 함수
    /// </summary>
    private void OpenContainer()
    {   // 바라보는 물체의 EnvironmentContainerHolder를 가져옴
        Transform _environmentContainer = _hit.transform;
        _environmentContainerHolder = _environmentContainer.GetComponent<EnvironmentContainerHolder>();
        _environmentContainerCreatorController = _environmentContainer.GetComponent<EnvironmentContainerCreatorController>();

        if (_canvasGroup.alpha == 0)
        {   // 인벤토리가 닫혀있고
            if (!_environmentContainerHolder._isOpen)
            {   // 다른 플레이어가 컨테이너를 열고있는 상태가 아니라면
                _isInventoryOpen = true;    // 플래그 변경 후
                ToggleInventory();          // 인벤토리를 열고

                _playerContainerHolder.CloseContainer();
                _environmentContainerHolder.OpenContainer();    // 바라보는 물체의 컨테이너를 열음

                // 플레이어로부터 DisplayFiller를 얻고 abstractGrid를 가져와서 인벤토리를 얻음 
                AbstractGrid _abstractGrid = _displayFiller.abstractGrid;

                // 필드 컨테이너에게 Grid를 전달
                _environmentContainerCreatorController.ChangeAbstractGrid(_abstractGrid);
            }
        }
        else
        {   // 인벤토리가 열려 있었다면
            _environmentContainerHolder.CloseContainer();       // 바라보는 물체의 컨테이너를 닫고
            _isInventoryOpen = false;                           // 플래그 변경 후
            ToggleInventory();                                  // 인벤토리 닫음
        }

        Debug.Log("Open the Item box");
    }

    private void ToggleInventory() // << 수정 필요
    {
        if (_canvasGroup.alpha == 0)
        {
            _canvasGroup.alpha = 1;
            ToggleCursor(true);
        }
        else
        {
            _canvasGroup.alpha = 0;
            ToggleCursor(false);
        }
    }

    public void ToggleCursor(bool state)
    {
        _toggleCursorDelegate?.Invoke(state);
    }

    /*************************************** 
    *        ↓플레이어 사망 시 호출↓        *                                       
    ***************************************/

    [PunRPC]
    public void ChangePlayerToEnvironmentContainer()
    {   // 플레이어는 사망 시 더 이상 플레이어가 아닌 필드 컨테이너로 취급
        _playerContainerHolder._isPlayerContainerHolder = false;        // 플레이어 컨테이너 홀더가 아니므로 플래그 변경
        _playerContainerCreatorController._hasBeenGenerated = true;     // 현재 플레이어의 아이템을 Grid에 넣을 것이므로 아이템 생성 플래그 변경

        _playerContainerHolder.itemContainerDataSo = _itemContainerDataSo;  // 플레이어 사망 시 화물 컨테이너에서 플레이어 아이템 컨테이너로 변경
        _playerContainerHolder.InitializeEnvironmentContainer();

        this.gameObject.layer = 0;          // 더 이상 플레이어가 아니므로 레이어 제거
        this.gameObject.tag = "Container";  // 컨테이너 태그 부착
    }

    private void OnDead(InputValue value)
    {
        _photonView.RPC("ChangePlayerToEnvironmentContainer", RpcTarget.AllBufferedViaServer);

        List<ItemTable> _playerEquipment = new List<ItemTable>();

        // 플레이어가 장착하고 있는 컨테이너를 동기화하기 위해 playerEquipment 리스트에 추가
        foreach (ContainerHolder _containerHolder in _containerHolders)
        {
            if (_containerHolder.isEquipped)
            {
                _playerEquipment.Add(_containerHolder.GetItemEquipped().ItemTable);
            }
        }

        // 플레이어가 장착하고 있는 아이템을 동기화하기 위해 playerEquipment 리스트에 추가
        foreach (ItemHolder _itemHolder in _itemHolders)
        {
            if (_itemHolder.isEquipped)
            {
                _playerEquipment.Add(_itemHolder.GetItemEquipped().ItemTable);
            }
        }

        _playerContainerCreatorController.InsertPlayerEquipmentToList(_playerEquipment);            // 현재 플레이어가 장착한 장비를 동기화하기 위해 먼저 추가
        _playerContainerCreatorController.ConvertGridTableToList(_playerInventorySo.GetGrids());    // 현재 플레이어의 Grid를 ECCC에서 동기화하기 위해 List로 변환
    }
}
