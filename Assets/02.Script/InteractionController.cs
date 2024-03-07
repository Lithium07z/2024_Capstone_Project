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
    //
    private ThirdPersonController _thirdPersonController;

    // photon
    private PhotonView _photonView;
    private PhotonView _environmentContainerPhotonView;

    // inventory canvas group
    private CanvasGroup _canvasGroup;

    // cinemachine virtual camera
    private GameObject _cinemachineVirtualCamera;

    // displayFiller for ScrollArea_Environment Component
    private Transform displayFiller;

    // inventory canvas
    [SerializeField] private GameObject _inventory;
    [SerializeField] private GameObject _inventoryManager;

    [SerializeField] private InventorySo _playerInventorySo;             // player inventory scriptable object
    [SerializeField] private InventorySupplierSo _inventorySupplierSo;   // inventory supplier scriptable object

    // player conainer
    [SerializeField] private ContainerHolder _chestContainerHolder;      // ���� �κ��丮
    [SerializeField] private ContainerHolder _backpackContainerHolder;   // ���� �κ��丮
    [SerializeField] private ContainerHolder _walletContainerHolder;     // ���� �κ��丮

    // 
    private EnvironmentContainerHolder _environmentContainerHolder;

    //
    private EnvironmentContainerCreatorController _environmentContainerCreatorController;

    // RaycastHit, Ray
    private RaycastHit _hit;
    private Ray _ray;

    private string[] itemTags = new string[] { "Item", "Backpack", "Chest", "Wallet" };
    private string[] equipment = new string[] { "Backpack", "Chest", "Wallet" };

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
        _ray = new Ray(_cinemachineVirtualCamera.transform.position, _cinemachineVirtualCamera.transform.forward);
        Debug.DrawRay(_cinemachineVirtualCamera.transform.position, _cinemachineVirtualCamera.transform.forward * 6, Color.red);

        /*************************************** 
        *              ������ �ڵ��            *                                       
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

    private void OnInteraction(InputValue value)
    {
        if (Physics.Raycast(_ray, out _hit, 6, ~LayerMask.GetMask("Player")))
        {

            if (_hit.transform.CompareTag("Box"))
            {
                OpenBox();
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

    private void OpenBox()
    {   // �ٶ󺸴� ��ü�� EnvironmentContainerHolder�� ������
        Transform _environmentContainer = _hit.transform;
        _environmentContainerPhotonView = _environmentContainer.GetComponent<PhotonView>();
        _environmentContainerHolder = _environmentContainer.GetComponent<EnvironmentContainerHolder>();
        _environmentContainerCreatorController = _environmentContainer.GetComponent<EnvironmentContainerCreatorController>();

        if (_canvasGroup.alpha == 0)
        {   // �κ��丮�� ���� �־��ٸ�
            _thirdPersonController._isInventoryOpen = true;
            ToggleInventory();              // �κ��丮�� ����

            _environmentContainerHolder.OpenContainer();    // �ٶ󺸴� ��ü�� �����̳ʸ� ����

            if (displayFiller == null)
            {
                displayFiller = this.transform.Find("Canvas/ScrollArea_Enviroment/View/Content/DisplayFiller(Clone)");
            }

            AbstractGrid _abstractGrid = displayFiller.GetComponent<DisplayFiller>().abstractGrid;

            _environmentContainerCreatorController.ChangeAbstractGrid(_abstractGrid);
        }
        else
        {   // �κ��丮�� ���� �־��ٸ�
            _environmentContainerHolder.CloseContainer();   // �ٶ󺸴� ��ü�� �����̳ʸ� �ݰ�
            ToggleInventory();              // �κ��丮�� ����
            _thirdPersonController._isInventoryOpen = false;
        }

        Debug.Log("Open the Item box");
    }

    private void OnOpenInventory(InputValue value)
    {
        if (_environmentContainerHolder != null)
        {
            _environmentContainerHolder.CloseContainer();
            _thirdPersonController._isInventoryOpen = false;
        }

        ToggleInventory();        // �κ��丮�� ����
    }

    private void ToggleInventory() // << �̰� ������ ����
    {
        if (_canvasGroup.alpha == 1)
        {
            //_starterAssetsInputs.cursorLocked = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            //_starterAssetsInputs.cursorLocked = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        _canvasGroup.alpha = _canvasGroup.alpha == 0 ? 1 : 0;
    }
}
