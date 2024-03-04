using Inventory.Scripts.Core.Enums;
using Inventory.Scripts.Core.Helper;
using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.ScriptableObjects;
using Inventory.Scripts.Core.ScriptableObjects.Configuration;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Events;
using Inventory.Scripts.Core.ScriptableObjects.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.Scripts.Core.Holders
{
    public class ItemHolder : MonoBehaviour
    {
        [SerializeField] private bool useDefaultSprite;

        [SerializeField] private Sprite defaultSprite;

        [SerializeField] private Color spriteColor;

        [Header("Item Holder Settings")] [SerializeField]
        private ItemDataTypeSo itemDataTypeSo;

        [SerializeField] [Tooltip("Will display the inventory item icon rotated.")]
        private bool displayRotatedIcon;
        
        [Header("Inventory Settings Anchor Settings")] [SerializeField]
        private InventorySettingsAnchorSo inventorySettingsAnchorSo;

        [Header("Listening on...")] [SerializeField]
        private AbstractItemEventChannelSo onAbstractItemBeingDragEventChannelSo;

        [Header("Broadcasting on...")] [SerializeField]
        private OnEquipItemTableInHolderEventChannelSo onEquipItemTableInHolderEventChannelSo;

        [HideInInspector] public bool isEquipped;

        public bool UseDefaultSprite => useDefaultSprite;

        public Sprite DefaultSprite => defaultSprite;

        public Color SpriteColor => spriteColor;

        public ItemDataTypeSo ItemDataTypeSo => itemDataTypeSo;

        protected Image Image;
        protected Color DefaultColor;

        private Image _childedImage;

        private ItemTable _equippedItemTable;
        private AbstractItem _equippedAbstractItemUi;

        private RectTransform _holderRectTransform;
        private RectTransform _equippedInventoryItemRectTransform;

        private ItemTable _itemTableBeingDrag;
        private bool _shouldEmitEquipEvent;
        private AbstractItem _dragRepresentationInventoryItem;

        private void Awake()
        {
            Image = GetComponent<Image>();
            if (UseDefaultSprite)
            {
                _childedImage = GetChildedImage();
            }

            DefaultColor = Image.color;
            _holderRectTransform = GetComponent<RectTransform>();

            _dragRepresentationInventoryItem = GetOrInstantiateObjectIfNotExists();
        }

        private Image GetChildedImage()
        {
            try
            {
                var imageChilded = GetComponentsInChildren<Image>()[0];

                if (Image == imageChilded)
                {
                    imageChilded = GetComponentsInChildren<Image>()[1];
                }

                return imageChilded;
            }
            catch
            {
                Debug.LogError(
                    "Error getting childed image... Check if the image is created or 'useDefaultSprite' is unchecked."
                        .Settings());
                return null;
            }
        }

        protected virtual void OnEnable()
        {
            onAbstractItemBeingDragEventChannelSo.OnEventRaised += ChangeOnAbstractItemBeingDrag;
        }

        protected virtual void OnDisable()
        {
            onAbstractItemBeingDragEventChannelSo.OnEventRaised -= ChangeOnAbstractItemBeingDrag;
        }

        private void ChangeOnAbstractItemBeingDrag(AbstractItem abstractItem)
        {
            _itemTableBeingDrag = abstractItem != null ? abstractItem.ItemTable : null;
        }

        private void Update()
        {
            if (!IsEnabledHighlightHolder()) return;

            if (_itemTableBeingDrag == null)
            {
                StopHighlight();
                return;
            }

            if (_equippedItemTable != null) return;

            HighlightItemHolder();
        }

        private void HighlightItemHolder()
        {
            if (!IsItemSameType(_itemTableBeingDrag)) return;

            Highlight();
        }

        protected bool IsItemSameType(ItemTable currentItemTable)
        {
            if (currentItemTable == null) return false;

            return itemDataTypeSo == currentItemTable.ItemDataSo.ItemDataTypeSo;
        }

        protected virtual void Highlight()
        {
            Image.color = GetColorHighlightHolder();
        }

        protected virtual void StopHighlight()
        {
            Image.color = DefaultColor;
        }

        protected virtual void OnEquipItem(ItemTable equippedItemTable)
        {
        }

        protected virtual void OnUnEquipItem(ItemTable equippedItemTable)
        {
        }

        public HolderResponse TryEquipItem(ItemTable itemTable,
            bool emitEquipEvent = true)
        {
            if (itemTable == null) return HolderResponse.Error;

            if (_equippedItemTable != null && isEquipped)
            {
                Debug.Log("Already equipped".Info());
                return HolderResponse.AlreadyEquipped;
            }

            if (!IsItemSameType(itemTable))
            {
                return HolderResponse.NotCorrectType;
            }

            _shouldEmitEquipEvent = emitEquipEvent;

            RemoveFromPreviousLocation(itemTable);

            itemTable.SetGridProps(null, default, default, this);

            var itemPrefab = GetItemPrefabSo().ItemPrefab;

            var gameObjectItem = Instantiate(itemPrefab);

            _equippedAbstractItemUi = gameObjectItem.GetComponent<AbstractItem>();

            var transformFromItem = _equippedAbstractItemUi.GetComponent<Transform>();

            transformFromItem.SetParent(transform);

            _equippedAbstractItemUi.RefreshItem(itemTable);

            return Equip(_equippedAbstractItemUi);
        }

        private static void RemoveFromPreviousLocation(ItemTable itemTable)
        {
            itemTable.CurrentGridTable?.RemoveItem(itemTable);
            if (itemTable.CurrentItemHolder != null)
            {
                itemTable.CurrentItemHolder.UnEquip(itemTable);
            }
        }

        private HolderResponse Equip(AbstractItem selectedInventoryItem)
        {
            _equippedItemTable = selectedInventoryItem.ItemTable;
            _equippedAbstractItemUi = selectedInventoryItem;

            if (_equippedItemTable.IsRotated)
            {
                selectedInventoryItem.Rotate();
            }

            _equippedInventoryItemRectTransform = selectedInventoryItem.GetComponent<RectTransform>();

            DisableDefaultImage();
            ResizeImages(selectedInventoryItem, _equippedInventoryItemRectTransform);

            isEquipped = true;

            if (isEquipped && _shouldEmitEquipEvent)
            {
                PushBroadcastEvent(_equippedItemTable, HolderInteraction.Equip);
            }

            OnEquipItem(_equippedItemTable);

            return HolderResponse.Equipped;
        }

        private void ResizeImages(AbstractItem inventoryItem, RectTransform equippedInventoryItemRectTransform)
        {
            equippedInventoryItemRectTransform.SetParent(_holderRectTransform);

            var holderSize = GetSize();
            ResizeItem(equippedInventoryItemRectTransform, holderSize);

            inventoryItem.ResizeIconOnHolder(holderSize);

            if (displayRotatedIcon && !inventoryItem.ItemTable.IsRotated)
            {
                inventoryItem.Rotate();
            }
        }

        private void ResizeItem(RectTransform rectTransform, Vector2 size)
        {
            rectTransform.sizeDelta = size;

            var anchors = new Vector2(0.5f, 0.5f);

            rectTransform.anchorMin = anchors;
            rectTransform.anchorMax = anchors;
            rectTransform.pivot = anchors;

            rectTransform.localPosition = new Vector2(0, 0);
        }

        private Vector2 GetSize()
        {
            var sizeDelta = _holderRectTransform.sizeDelta;

            var size = new Vector2
            {
                x = displayRotatedIcon ? sizeDelta.y - 10 : sizeDelta.x - 10,
                y = displayRotatedIcon ? sizeDelta.x - 10 : sizeDelta.y - 10
            };

            return size;
        }

        public void UnEquip(ItemTable itemTable)
        {
            EnableDefaultImage();

            PushBroadcastEvent(itemTable, HolderInteraction.UnEquip);

            if (itemTable == null) return;

            OnUnEquipItem(itemTable);

            _equippedItemTable = null;
            isEquipped = false;

            Destroy(_equippedAbstractItemUi.gameObject);
        }

        public AbstractItem PlaceDragRepresentation()
        {
            var inventoryItemDragRepresentation = GetOrInstantiateObjectIfNotExists();

            var itemDataSo = _equippedItemTable.ItemDataSo;

            inventoryItemDragRepresentation.SetDragProps(itemDataSo);
            inventoryItemDragRepresentation.ResizeIcon();

            var inventoryItemDragRepresentationRectTransform =
                inventoryItemDragRepresentation.GetComponent<RectTransform>();

            ResizeImages(inventoryItemDragRepresentation, inventoryItemDragRepresentationRectTransform);

            inventoryItemDragRepresentation.SetDragRepresentationDragStyle();

            inventoryItemDragRepresentation.gameObject.SetActive(true);

            isEquipped = false;
            return inventoryItemDragRepresentation;
        }

        private AbstractItem GetOrInstantiateObjectIfNotExists()
        {
            if (_dragRepresentationInventoryItem != null)
            {
                return _dragRepresentationInventoryItem;
            }

            var itemPrefab = GetItemPrefabSo().ItemPrefab;

            var instantiate = Instantiate(itemPrefab);
            instantiate.name += "_BeforeDragItemShowing";
            _dragRepresentationInventoryItem = instantiate.GetComponent<AbstractItem>();

            _dragRepresentationInventoryItem.Set(null, false);

            _dragRepresentationInventoryItem.transform.SetParent(gameObject.transform);

            instantiate.SetActive(false);

            return _dragRepresentationInventoryItem;
        }

        public void RemoveDragRepresentation()
        {
            UnEquip(_dragRepresentationInventoryItem.ItemTable);

            _dragRepresentationInventoryItem.gameObject.SetActive(false);
        }

        public AbstractItem GetItemEquipped()
        {
            if (isEquipped && _equippedItemTable != null)
            {
                return _equippedAbstractItemUi;
            }

            return null;
        }

        private void EnableDefaultImage()
        {
            if (_childedImage == null) return;

            _childedImage.gameObject.SetActive(true);
        }

        private void DisableDefaultImage()
        {
            if (_childedImage == null) return;

            _childedImage.gameObject.SetActive(false);
        }

        private ItemPrefabSo GetItemPrefabSo()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.ItemPrefabSo;
        }

        private bool IsEnabledHighlightHolder()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.EnableHighlightHolder;
        }

        private Color GetColorHighlightHolder()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.ColorHighlightHolder;
        }

        private void PushBroadcastEvent(ItemTable itemTable, HolderInteraction status)
        {
            if (onEquipItemTableInHolderEventChannelSo == null)
            {
                Debug.LogWarning("Not broadcasting event... The event is not configured...".Broadcasting());
                return;
            }

            onEquipItemTableInHolderEventChannelSo.RaiseEvent(itemTable, status);
        }
    }
}