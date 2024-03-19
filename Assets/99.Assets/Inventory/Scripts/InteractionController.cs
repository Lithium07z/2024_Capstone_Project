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
using System;

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

    [SerializeField] private InventorySo _playerInventorySo;             // player inventory scriptable object
    [SerializeField] private InventorySupplierSo _inventorySupplierSo;   // inventory supplier scriptable object

    // player conainer
    [SerializeField] private ContainerHolder _chestContainerHolder;      // 가슴 인벤토리
    [SerializeField] private ContainerHolder _backpackContainerHolder;   // 가방 인벤토리
    [SerializeField] private ContainerHolder _walletContainerHolder;     // 지갑 인벤토리

    // 컨테이너 아이템 홀더
    private EnvironmentContainerHolder _environmentContainerHolder;
    private EnvironmentContainerHolder _playerContainerHolder;

    // 컨테이너 아이템 생성 및 동기화 컨트롤러
    private EnvironmentContainerCreatorController _environmentContainerCreatorController;
    private EnvironmentContainerCreatorController _playerContainerCreatorController;

    // RaycastHit, Ray
    private RaycastHit _hit;
    private Ray _ray;

    // 인벤토리 오픈 플래그
    public bool _isInventoryOpen = false;

    private string[] itemTags = { "Item", "Backpack", "Chest", "Wallet"};

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
            else if (Array.Exists(itemTags, itemTag => _hit.transform.CompareTag(itemTag)))
            {   // 아이템인 경우
                // FindPlaceForItemInGrids함수의 반환형, 넣으려 시도한 아이템의 ItemTable과 결과를 반환함
                (ItemTable, GridResponse) findPlaceResult = new(null, GridResponse.NoGridTableSelected);
                Item item = _hit.transform.GetComponent<Item>();    // 아이템 스크립트를 얻어옴
                ItemDataSo itemDataSo = item.GetItemDataSo();       // 실제 아이템 정보를 얻어옴

                if (itemDataSo is ItemContainerDataSo)
                {   // 아이템 중에서도 장착물인 경우 (가방, 조끼, 지갑)
                    (ItemTable, HolderResponse) equipItemResult = new(null, HolderResponse.Error);
                    // TryEquipItem함수의 반환형, 입으려 시도한 아이템의 ItemTable과 결과를 반환함

                    if (_hit.transform.CompareTag("Backpack"))  // 가방의 경우
                    {
                        Debug.Log("Backpack");
                        equipItemResult = _inventorySupplierSo.TryEquipItem(itemDataSo, _backpackContainerHolder);
                    }
                    else if (_hit.transform.CompareTag("Chest"))    // 조끼의 경우
                    {
                        Debug.Log("Chest");
                        equipItemResult = _inventorySupplierSo.TryEquipItem(itemDataSo, _chestContainerHolder);
                    }
                    else if (_hit.transform.CompareTag("Wallet"))   // 지갑의 경우
                    {
                        Debug.Log("Wallet");
                        equipItemResult = _inventorySupplierSo.TryEquipItem(itemDataSo, _walletContainerHolder);
                    }

                    // 아이템을 이미 착용한 경우 (가방, 조끼, 지갑 등)
                    if (equipItemResult.Item2 == HolderResponse.AlreadyEquipped)
                    {   // 아이템을 인벤토리에 넣음
                        findPlaceResult = _inventorySupplierSo.FindPlaceForItemInGrids(itemDataSo, _playerInventorySo.GetGrids());

                        if (findPlaceResult.Item2 != GridResponse.Inserted)
                        {   // 못넣는 경우
                            return;
                        }
                    }
                }
                else
                {   // 아이템 중에서도 장착물이 아닌 경우
                    findPlaceResult = _inventorySupplierSo.FindPlaceForItemInGrids(itemDataSo, _playerInventorySo.GetGrids());

                    if (findPlaceResult.Item2 != GridResponse.Inserted)
                    {   // 아이템이 인벤토리에 들어가지 않은 경우
                        return;
                    }

                    Debug.Log("Take Item");
                }

                // 아이템 획득이 끝났다면 파괴
                item.DestroyItem();
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

                _environmentContainerHolder.OpenContainer();    // 바라보는 물체의 컨테이너를 열음

                // 플레이어로부터 DisplayFiller를 얻고 abstractGrid를 가져와서 인벤토리를 얻음 
                AbstractGrid _abstractGrid = _displayFiller.abstractGrid;
                Debug.Log(_abstractGrid + "!@#!@#@!#@!#!@#@!#!#@!#@!#@!#!#@!");
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
        if (_canvasGroup.alpha == 1)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        _canvasGroup.alpha = _canvasGroup.alpha == 0 ? 1 : 0;
    }

    /*************************************** 
    *        ↓플레이어 사망 시 호출↓        *                                       
    ***************************************/

    [PunRPC]
    public void ChangePlayerToEnvironmentContainer()
    {   // 플레이어는 사망 시 더 이상 플레이어가 아닌 필드 컨테이너로 취급
        _playerContainerHolder.enabled = true;              // 컨테이너 이므로 ECH 활성화
        _playerContainerCreatorController.enabled = true;   // 컨테이너 이므로 ECCC 활성화

        _playerContainerCreatorController._hasBeenGenerated = true;    // 현재 플레이어의 아이템을 Grid에 넣을 것이므로 아이템 생성 플래그 변경

        this.gameObject.layer = 0;          // 더 이상 플레이어가 아니므로 레이어 제거
        this.gameObject.tag = "Container";  // 컨테이너 태그 부착
    }

    private void OnDead(InputValue value)
    {
        _photonView.RPC("ChangePlayerToEnvironmentContainer", RpcTarget.AllBufferedViaServer);
        _playerContainerCreatorController.ConvertGridTableToList(_playerInventorySo.GetGrids());    // 현재 플레이어의 Grid를 ECCC에서 동기화하기 위해 List로 변환
    }
}
