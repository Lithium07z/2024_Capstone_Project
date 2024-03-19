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
    [SerializeField] private GameObject _inventory;         // �κ��丮 ĵ����
    [SerializeField] private GameObject _inventoryManager;  // �κ��丮 �Ŵ���

    [SerializeField] private InventorySo _playerInventorySo;             // player inventory scriptable object
    [SerializeField] private InventorySupplierSo _inventorySupplierSo;   // inventory supplier scriptable object

    // player conainer
    [SerializeField] private ContainerHolder _chestContainerHolder;      // ���� �κ��丮
    [SerializeField] private ContainerHolder _backpackContainerHolder;   // ���� �κ��丮
    [SerializeField] private ContainerHolder _walletContainerHolder;     // ���� �κ��丮

    // �����̳� ������ Ȧ��
    private EnvironmentContainerHolder _environmentContainerHolder;
    private EnvironmentContainerHolder _playerContainerHolder;

    // �����̳� ������ ���� �� ����ȭ ��Ʈ�ѷ�
    private EnvironmentContainerCreatorController _environmentContainerCreatorController;
    private EnvironmentContainerCreatorController _playerContainerCreatorController;

    // RaycastHit, Ray
    private RaycastHit _hit;
    private Ray _ray;

    // �κ��丮 ���� �÷���
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
        // �ó׸ӽ� ����� ī�޶� �ٶ󺸴� ��ġ�� Ray �߻�
        _ray = new Ray(_cinemachineVirtualCamera.transform.position, _cinemachineVirtualCamera.transform.forward);

        /*************************************** 
        *              ������ �ڵ��            *                                       
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
    *              ����ǲ �Լ���            *                                       
    ***************************************/

    /// <summary>
    /// �����̳� ���� �Լ� | Inventory Input Actions�� ������ | Ű : E Key
    /// </summary>
    /// <param name="value">NewInput �Է�, E Key</param>
    private void OnInteraction(InputValue value)
    {   // ���� �÷��̾� ���̾ �����ϰ� Ray�� �浹�� ��ü ��ȯ
        if (Physics.Raycast(_ray, out _hit, 6, ~LayerMask.GetMask("Player")))
        {
            string _tag = _hit.transform.tag;

            if (_tag.Equals("Container"))
            {   // �����̳��� ���
                OpenContainer();    // �����̳� ����
            }
            else if (Array.Exists(itemTags, itemTag => _hit.transform.CompareTag(itemTag)))
            {   // �������� ���
                // FindPlaceForItemInGrids�Լ��� ��ȯ��, ������ �õ��� �������� ItemTable�� ����� ��ȯ��
                (ItemTable, GridResponse) findPlaceResult = new(null, GridResponse.NoGridTableSelected);
                Item item = _hit.transform.GetComponent<Item>();    // ������ ��ũ��Ʈ�� ����
                ItemDataSo itemDataSo = item.GetItemDataSo();       // ���� ������ ������ ����

                if (itemDataSo is ItemContainerDataSo)
                {   // ������ �߿����� �������� ��� (����, ����, ����)
                    (ItemTable, HolderResponse) equipItemResult = new(null, HolderResponse.Error);
                    // TryEquipItem�Լ��� ��ȯ��, ������ �õ��� �������� ItemTable�� ����� ��ȯ��

                    if (_hit.transform.CompareTag("Backpack"))  // ������ ���
                    {
                        Debug.Log("Backpack");
                        equipItemResult = _inventorySupplierSo.TryEquipItem(itemDataSo, _backpackContainerHolder);
                    }
                    else if (_hit.transform.CompareTag("Chest"))    // ������ ���
                    {
                        Debug.Log("Chest");
                        equipItemResult = _inventorySupplierSo.TryEquipItem(itemDataSo, _chestContainerHolder);
                    }
                    else if (_hit.transform.CompareTag("Wallet"))   // ������ ���
                    {
                        Debug.Log("Wallet");
                        equipItemResult = _inventorySupplierSo.TryEquipItem(itemDataSo, _walletContainerHolder);
                    }

                    // �������� �̹� ������ ��� (����, ����, ���� ��)
                    if (equipItemResult.Item2 == HolderResponse.AlreadyEquipped)
                    {   // �������� �κ��丮�� ����
                        findPlaceResult = _inventorySupplierSo.FindPlaceForItemInGrids(itemDataSo, _playerInventorySo.GetGrids());

                        if (findPlaceResult.Item2 != GridResponse.Inserted)
                        {   // ���ִ� ���
                            return;
                        }
                    }
                }
                else
                {   // ������ �߿����� �������� �ƴ� ���
                    findPlaceResult = _inventorySupplierSo.FindPlaceForItemInGrids(itemDataSo, _playerInventorySo.GetGrids());

                    if (findPlaceResult.Item2 != GridResponse.Inserted)
                    {   // �������� �κ��丮�� ���� ���� ���
                        return;
                    }

                    Debug.Log("Take Item");
                }

                // ������ ȹ���� �����ٸ� �ı�
                item.DestroyItem();
            }
        }
    }

    /// <summary>
    /// �κ��丮 ���� �Լ� | Inventory Input Actions�� ������ | Ű : Tab Key
    /// </summary>
    /// <param name="value">NewInput �Է�, TabKey</param>
    private void OnOpenInventory(InputValue value)
    {
        if (_environmentContainerHolder != null)
        {   // ������ ������ �����̳��� EnvironmentContainerHolder�� ���� �����ִٸ�
            _environmentContainerHolder.CloseContainer();       // �ݾ��ְ�
            _isInventoryOpen = false;                           // �÷��� ����
        }

        ToggleInventory();  // �κ��丮�� ����
    }

    /*************************************** 
    *            ���ȣ�ۿ� �Լ���            *                                       
    ***************************************/

    /// <summary>
    /// �����̳� ���� �Լ�
    /// </summary>
    private void OpenContainer()
    {   // �ٶ󺸴� ��ü�� EnvironmentContainerHolder�� ������
        Transform _environmentContainer = _hit.transform;
        _environmentContainerHolder = _environmentContainer.GetComponent<EnvironmentContainerHolder>();
        _environmentContainerCreatorController = _environmentContainer.GetComponent<EnvironmentContainerCreatorController>();

        if (_canvasGroup.alpha == 0)
        {   // �κ��丮�� �����ְ�
            if (!_environmentContainerHolder._isOpen)
            {   // �ٸ� �÷��̾ �����̳ʸ� �����ִ� ���°� �ƴ϶��
                _isInventoryOpen = true;    // �÷��� ���� ��
                ToggleInventory();          // �κ��丮�� ����

                _environmentContainerHolder.OpenContainer();    // �ٶ󺸴� ��ü�� �����̳ʸ� ����

                // �÷��̾�κ��� DisplayFiller�� ��� abstractGrid�� �����ͼ� �κ��丮�� ���� 
                AbstractGrid _abstractGrid = _displayFiller.abstractGrid;
                Debug.Log(_abstractGrid + "!@#!@#@!#@!#!@#@!#!#@!#@!#@!#!#@!");
                // �ʵ� �����̳ʿ��� Grid�� ����
                _environmentContainerCreatorController.ChangeAbstractGrid(_abstractGrid);
            }
        }
        else
        {   // �κ��丮�� ���� �־��ٸ�
            _environmentContainerHolder.CloseContainer();       // �ٶ󺸴� ��ü�� �����̳ʸ� �ݰ�
            _isInventoryOpen = false;                           // �÷��� ���� ��
            ToggleInventory();                                  // �κ��丮 ����
        }

        Debug.Log("Open the Item box");
    }

    private void ToggleInventory() // << ���� �ʿ�
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
    *        ���÷��̾� ��� �� ȣ���        *                                       
    ***************************************/

    [PunRPC]
    public void ChangePlayerToEnvironmentContainer()
    {   // �÷��̾�� ��� �� �� �̻� �÷��̾ �ƴ� �ʵ� �����̳ʷ� ���
        _playerContainerHolder.enabled = true;              // �����̳� �̹Ƿ� ECH Ȱ��ȭ
        _playerContainerCreatorController.enabled = true;   // �����̳� �̹Ƿ� ECCC Ȱ��ȭ

        _playerContainerCreatorController._hasBeenGenerated = true;    // ���� �÷��̾��� �������� Grid�� ���� ���̹Ƿ� ������ ���� �÷��� ����

        this.gameObject.layer = 0;          // �� �̻� �÷��̾ �ƴϹǷ� ���̾� ����
        this.gameObject.tag = "Container";  // �����̳� �±� ����
    }

    private void OnDead(InputValue value)
    {
        _photonView.RPC("ChangePlayerToEnvironmentContainer", RpcTarget.AllBufferedViaServer);
        _playerContainerCreatorController.ConvertGridTableToList(_playerInventorySo.GetGrids());    // ���� �÷��̾��� Grid�� ECCC���� ����ȭ�ϱ� ���� List�� ��ȯ
    }
}
