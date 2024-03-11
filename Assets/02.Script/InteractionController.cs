using Inventory.Scripts.Core.Displays.Filler;
using Inventory.Scripts.Core.Enums;
using Inventory.Scripts.Core.Holders;
using Inventory.Scripts.Core.Items.Grids;
using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.ScriptableObjects.Items;
using Inventory.Scripts.Core.ScriptableObjects;
using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class InteractionController : MonoBehaviour
{
    // thirdPersonController
    private ThirdPersonController _thirdPersonController;

    // photon
    private PhotonView _photonView;

    // inventory canvas group
    private CanvasGroup _canvasGroup;

    // cinemachine virtual camera
    private GameObject _cinemachineVirtualCamera;

    // displayFiller for ScrollArea_Environment Component
    private Transform displayFiller;

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

    // 컨테이너 아이템 생성 및 동기화 컨트롤러
    private EnvironmentContainerCreatorController _environmentContainerCreatorController;

    // RaycastHit, Ray
    private RaycastHit _hit;
    private Ray _ray;

    // tags
    private string[] itemTags = new string[] { "Item", "Backpack", "Chest", "Wallet" }; // 아이템 태그
    private string[] equipment = new string[] { "Backpack", "Chest", "Wallet" };        // 장비 태그

    private void Start()
    {
        _photonView = this.GetComponent<PhotonView>();
        _canvasGroup = _inventory.GetComponent<CanvasGroup>();
        _thirdPersonController = this.GetComponent<ThirdPersonController>();

        _cinemachineVirtualCamera = GameObject.Find("PlayerFollowCamera");

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
        Debug.DrawRay(_cinemachineVirtualCamera.transform.position, _cinemachineVirtualCamera.transform.forward * 6, Color.red);

        /*************************************** 
        *              ↓디버깅 코드↓            *                                       
        ***************************************/

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
            if (_hit.transform.CompareTag("Container"))
            {   // 컨테이너인 경우
                OpenContainer();    // 컨테이너 오픈
            }
            else if (Array.Exists(itemTags, tag => _hit.transform.CompareTag(tag)))
            {   
                (ItemTable, GridResponse) findPlaceResult = new(null, GridResponse.NoGridTableSelected);
                Item item = _hit.transform.GetComponent<Item>();
                ItemDataSo itemSo = item.GetItemDataSo();

                if (_hit.transform.CompareTag("Item"))
                {
                    findPlaceResult = _inventorySupplierSo.FindPlaceForItemInGrids(itemSo, _playerInventorySo.GetGrids());

                    if (findPlaceResult.Item2 != GridResponse.Inserted)
                    {
                        return;
                    }

                    Debug.Log("Take Item");
                }
                else if (Array.Exists(equipment, tag => _hit.transform.CompareTag(tag)))
                {
                    (ItemTable, HolderResponse) equipItemResult = new(null, HolderResponse.Error);

                    if (_hit.transform.CompareTag("Backpack"))
                    {
                        Debug.Log("Backpack");
                        equipItemResult = _inventorySupplierSo.TryEquipItem(itemSo, _backpackContainerHolder);
                    }
                    else if (_hit.transform.CompareTag("Chest"))
                    {
                        Debug.Log("Chest");
                        equipItemResult = _inventorySupplierSo.TryEquipItem(itemSo, _chestContainerHolder);
                    }
                    else if (_hit.transform.CompareTag("Wallet"))
                    {
                        Debug.Log("Wallet");
                        equipItemResult = _inventorySupplierSo.TryEquipItem(itemSo, _walletContainerHolder);
                    }

                    if (equipItemResult.Item2 == HolderResponse.AlreadyEquipped)
                    {
                        findPlaceResult = _inventorySupplierSo.FindPlaceForItemInGrids(itemSo, _playerInventorySo.GetGrids());

                        if (findPlaceResult.Item2 != GridResponse.Inserted)
                        {
                            return;
                        }
                    }
                }

                item.DestroyItem();
            }
        }
    }

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
                _thirdPersonController._isInventoryOpen = true;
                ToggleInventory();              // 인벤토리를 열고

                _environmentContainerHolder.OpenContainer();    // 바라보는 물체의 컨테이너를 열음

                if (displayFiller == null)
                {
                    displayFiller = this.transform.Find("Canvas/ScrollArea_Enviroment/View/Content/DisplayFiller(Clone)");
                }

                AbstractGrid _abstractGrid = displayFiller.GetComponent<DisplayFiller>().abstractGrid;

                _environmentContainerCreatorController.ChangeAbstractGrid(_abstractGrid);
            }
        }
        else
        {   // 인벤토리가 열려 있었다면
            _environmentContainerHolder.CloseContainer();   // 바라보는 물체의 컨테이너를 닫고
            ToggleInventory();              // 인벤토리도 닫음
            _thirdPersonController._isInventoryOpen = false;
        }

        Debug.Log("Open the Item box");
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
            _thirdPersonController._isInventoryOpen = false;    // 플래그 변경
        }

        ToggleInventory();  // 인벤토리를 오픈
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
}
