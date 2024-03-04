using Inventory.Scripts.Core.Items.Grids.Helper;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Tile;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.Scripts.Core.Items.Grids
{
    public class ItemGrid2D : AbstractGrid
    {
        private const string BorderGameObjectName = "Border_Grid";
        private const string HighlightGameObjectName = "Highlight_Area";

        [Header("Border Image Config")] [SerializeField]
        private Image.Type imageType = Image.Type.Sliced;

        [SerializeField] private bool fillCenter = true;
        [SerializeField] private float pixelsPerUnitMultiplier = 1.0f;

        private RectTransform RectTransform { get; set; }

        private bool _isInitiated;

        private AbstractItem _dragRepresentationInventoryItem;
        private RectTransform _highlightArea;

        private GameObject _borderGameObject;
        private RectTransform _borderRectTransform;

        private Image _tileGridImage;


        protected override void Awake()
        {
            ResizeSlotImage();
            RectTransform = GetComponent<RectTransform>();

            base.Awake();

            InitComponents();
        }

        private void ResizeSlotImage()
        {
            SetSlotImage();

            var inventorySlotSprite = GetInventorySlotSprite();
            var tileSpecSo = GetTileSpecSo();
            
            SlotHelper.SetSlotImageNormalizedWithTileSize(inventorySlotSprite, tileSpecSo, _tileGridImage);
        }

        private void SetSlotImage()
        {
            if (_tileGridImage == null)
            {
                _tileGridImage = GetComponent<Image>();
            }
        }

        private void InitComponents()
        {
            if (_isInitiated) return;

            _dragRepresentationInventoryItem = GetOrInstantiateObjectIfNotExists();
            GetOrInstantiateHighlightArea();

            _isInitiated = true;
        }

        private void OnEnable()
        {
            if (Grid == null) return;

            var allItemsFromGrid = Grid.GetAllItemsFromGrid();

            foreach (var itemTable in allItemsFromGrid)
            {
                UpdateRectTransform(itemTable, itemTable.OnGridPositionX, itemTable.OnGridPositionY);
            }
        }

        public override void ResizeGrid()
        {
            ResizeSlotImage();

            if (GetBorderGridSprite() != null)
            {
                InstantiateBorder();
            }

            var size = GetGridSize();

            if (_borderRectTransform != null)
            {
                var sizeParent = new Vector2(size.x, size.y);

                _borderRectTransform.sizeDelta = sizeParent;
            }

            if (RectTransform == null)
            {
                RectTransform = GetComponent<RectTransform>();
            }

            RectTransform.sizeDelta = size;
        }

        private void InstantiateBorder()
        {
            _borderGameObject = GetBorderGameObject();

            var borderimage = GetImageFromBorder();

            borderimage.sprite = GetBorderGridSprite();
            borderimage.raycastTarget = false;
            borderimage.type = imageType;
            borderimage.fillCenter = fillCenter;
            borderimage.pixelsPerUnitMultiplier = pixelsPerUnitMultiplier;

            if (RectTransform == null)
            {
                RectTransform = GetComponent<RectTransform>();
            }

            _borderRectTransform = GetBorderRectTransform();
            _borderRectTransform.SetParent(RectTransform);

            _borderRectTransform.anchoredPosition = Vector2.zero;

            var topLeftAnchor = new Vector2(0, 1);
            _borderRectTransform.anchorMin = topLeftAnchor;
            _borderRectTransform.anchorMax = topLeftAnchor;
            _borderRectTransform.pivot = topLeftAnchor;
        }

        private GameObject GetBorderGameObject()
        {
            var foundGameObject = transform.Find(BorderGameObjectName);

            if (foundGameObject != null)
            {
                _borderGameObject = foundGameObject.gameObject;

                return _borderGameObject;
            }

            _borderGameObject = new GameObject(BorderGameObjectName, typeof(RectTransform))
            {
                layer = LayerMask.NameToLayer("UI")
            };

            _borderRectTransform = GetBorderRectTransform();
            _borderRectTransform.SetParent(RectTransform);

            return _borderGameObject;
        }

        private RectTransform GetBorderRectTransform()
        {
            return _borderRectTransform != null
                ? _borderRectTransform
                : _borderGameObject.GetComponent<RectTransform>();
        }

        public override Transform GetOrInstantiateHighlightArea()
        {
            var foundHighlightArea = transform.Find(HighlightGameObjectName);

            if (foundHighlightArea != null)
            {
                _highlightArea = foundHighlightArea.gameObject.GetComponent<RectTransform>();

                return _highlightArea;
            }

            var highlightGameObject = new GameObject(HighlightGameObjectName, typeof(RectTransform))
            {
                layer = LayerMask.NameToLayer("UI")
            };

            _highlightArea = highlightGameObject.GetComponent<RectTransform>();
            _highlightArea.SetParent(RectTransform);
            _highlightArea.sizeDelta = RectTransform.sizeDelta;
            _highlightArea.pivot = RectTransform.pivot;
            _highlightArea.anchorMin = new Vector2(0, 1);
            _highlightArea.anchorMax = new Vector2(0, 1);
            _highlightArea.anchoredPosition = new Vector2(0, 0);
            _highlightArea.localScale = new Vector3(1, 1, 1);

            _highlightArea.gameObject.AddComponent<RectMask2D>();

            return _highlightArea;
        }

        private Image GetImageFromBorder()
        {
            var image = _borderGameObject.GetComponent<Image>();

            return image != null ? image : _borderGameObject.AddComponent<Image>();
        }

        protected override void HandleUpdateItem(ItemTable item, int posX, int posY)
        {
            UpdateRectTransform(item, posX, posY);
        }

        private void UpdateRectTransform(ItemTable itemTable, int posX, int posY)
        {
            var abstractItem = GetAbstractItem(itemTable);

            if (abstractItem == null) return;

            var tileGridHelperSo = GetTileGridHelperSo();

            var position = tileGridHelperSo.CalculatePositionOnGrid(abstractItem,
                posX,
                posY
            );

            abstractItem.transform.SetParent(gameObject.transform);
            abstractItem.ResizeIcon();

            abstractItem.transform.localPosition = position;
        }

        public override AbstractItem PlaceDragRepresentation(AbstractItem selectedInventoryItem)
        {
            var inventoryItemDragRepresentation = GetOrInstantiateObjectIfNotExists();

            var itemDataSo = selectedInventoryItem.ItemTable.ItemDataSo;

            inventoryItemDragRepresentation.SetDragProps(itemDataSo);
            inventoryItemDragRepresentation.ResizeIcon();

            if (CheckIfShouldRotate(selectedInventoryItem, inventoryItemDragRepresentation))
            {
                inventoryItemDragRepresentation.Rotate();
            }

            Grid.PlaceItem(inventoryItemDragRepresentation.ItemTable, selectedInventoryItem.ItemTable.OnGridPositionX,
                selectedInventoryItem.ItemTable.OnGridPositionY);

            inventoryItemDragRepresentation.SetDragRepresentationDragStyle();
            var tileGridHelperSo = GetTileGridHelperSo();

            var position = tileGridHelperSo.CalculatePositionOnGrid(inventoryItemDragRepresentation,
                inventoryItemDragRepresentation.ItemTable.OnGridPositionX,
                inventoryItemDragRepresentation.ItemTable.OnGridPositionY);

            inventoryItemDragRepresentation.transform.localPosition = position;

            inventoryItemDragRepresentation.gameObject.SetActive(true);

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

        private static bool CheckIfShouldRotate(AbstractItem selectedInventoryItem,
            AbstractItem inventoryItemDragRepresentation)
        {
            return selectedInventoryItem.ItemTable.IsRotated && !inventoryItemDragRepresentation.ItemTable.IsRotated ||
                   !selectedInventoryItem.ItemTable.IsRotated && inventoryItemDragRepresentation.ItemTable.IsRotated;
        }

        public override void RemoveDragRepresentation(AbstractItem inventoryItemDrag)
        {
            Grid.PickUpItem(inventoryItemDrag.ItemTable.OnGridPositionX, inventoryItemDrag.ItemTable.OnGridPositionY);

            _dragRepresentationInventoryItem.gameObject.SetActive(false);
        }

        private Sprite GetBorderGridSprite()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.BorderGridSprite;
        }

        private Sprite GetInventorySlotSprite()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.SlotSprite;
        }

        private TileSpecSo GetTileSpecSo()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.TileSpecSo;
        }

        private TileGridHelperSo GetTileGridHelperSo()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.TileGridHelperSo;
        }
    }
}