using Inventory.Scripts.Core.Items.Enums;
using Inventory.Scripts.Core.Items.Helper;
using Inventory.Scripts.Core.ScriptableObjects.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.Scripts.Core.Items
{
    public class ItemInventory2D : AbstractItem
    {
        [Header("Item Props Settings")] [SerializeField]
        private Image image;

        [SerializeField] private TMP_Text text;

        [SerializeField] private Image background;

        private RectTransform _rectTransform;
        private RectTransform _rectTransformIcon;
        private RectTransform _displayNameRectTransform;
        private Color _unsetColorFromDrag;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _rectTransformIcon = image.GetComponent<RectTransform>();
            _displayNameRectTransform = text.GetComponent<RectTransform>();

            if (background != null)
            {
                background.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            _rectTransform.localScale = new Vector3(1, 1, 1);
        }

        protected override void OnSetProperties(ItemDataSo item)
        {
            if (item == null) return;

            text.text = item.DisplayName;
            ResizeIcon();
        }

        public override void ResizeIcon()
        {
            image.sprite = ItemTable.ItemDataSo.Icon;

            var tileSpecSo = GetTileSpecSo();

            var size = new Vector2
            {
                x = ItemTable.ItemDataSo.DimensionsSo.Width * tileSpecSo.TileSizeWidth,
                y = ItemTable.ItemDataSo.DimensionsSo.Height * tileSpecSo.TileSizeHeight
            };

            GetRectTransform().sizeDelta = size;

            var rectTransformIcon = GetRectTransformIcon();

            rectTransformIcon.sizeDelta = GetIconSize();
            rectTransformIcon.localPosition = GetIconPosition();

            var rectTransformFromDisplayName = GetRectTransformFromDisplayName();
            rectTransformFromDisplayName.sizeDelta = GetSizeDeltaForItemText(ItemTable.IsRotated, _rectTransform);
        }

        private static Vector2 GetSizeDeltaForItemText(bool itemRotated, RectTransform rectTransform)
        {
            var sizeDelta = rectTransform.sizeDelta;

            return itemRotated ? new Vector2(sizeDelta.y, sizeDelta.x) : sizeDelta;
        }

        private RectTransform GetRectTransform()
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }

            return _rectTransform;
        }

        private RectTransform GetRectTransformIcon()
        {
            if (_rectTransformIcon == null)
            {
                _rectTransformIcon = GetComponent<RectTransform>();
            }

            return _rectTransformIcon;
        }

        private Vector2 GetIconSize()
        {
            var rectTransformSizeDelta = GetRectTransform().sizeDelta;

            var iconSizeSo = ItemTable.ItemDataSo.IconSizeSo;
            if (iconSizeSo != null)
            {
                return new Vector2
                {
                    x = iconSizeSo.Width,
                    y = iconSizeSo.Height
                };
            }

            return new Vector2
            {
                x = rectTransformSizeDelta.x,
                y = rectTransformSizeDelta.y,
            };
        }

        private Vector3 GetIconPosition()
        {
            var iconSizeSo = ItemTable.ItemDataSo.IconSizeSo;

            if (iconSizeSo != null)
            {
                return new Vector3
                {
                    x = iconSizeSo.PosX,
                    y = iconSizeSo.PosY,
                    z = 0
                };
            }

            return Vector3.zero;
        }

        private RectTransform GetRectTransformFromDisplayName()
        {
            if (_displayNameRectTransform == null)
            {
                _displayNameRectTransform = GetComponent<RectTransform>();
            }

            return _displayNameRectTransform;
        }

        public override void ResizeIconOnHolder(Vector2 holderSize)
        {
            var inHolderIconSizeSo = ItemTable.ItemDataSo.InHolderIconSizeSo;

            if (inHolderIconSizeSo == null)
            {
                ResizeItem(_rectTransformIcon, holderSize);
                return;
            }

            var size = new Vector2
            {
                x = inHolderIconSizeSo.Width,
                y = inHolderIconSizeSo.Height
            };

            _rectTransformIcon.sizeDelta = size;
            _rectTransformIcon.localPosition = new Vector3
            {
                x = inHolderIconSizeSo.PosX,
                y = inHolderIconSizeSo.PosY,
                z = 0
            };
        }

        private void ResizeItem(RectTransform rectTransform, Vector2 holderSize)
        {
            rectTransform.sizeDelta = holderSize;

            var anchors = new Vector2(0.5f, 0.5f);

            rectTransform.anchorMin = anchors;
            rectTransform.anchorMax = anchors;
            rectTransform.pivot = anchors;

            rectTransform.localPosition = new Vector2(0, 0);
        }

        protected override void RotateUI()
        {
            var rectTransform = _rectTransform;
            rectTransform.rotation =
                Quaternion.Euler(0, 0, ItemTable.IsRotated ? RotationHelper.GetRotationByType(rotationType) : 0f);

            if (rotationType == Rotation.MinusNinety)
            {
                _displayNameRectTransform.rotation = Quaternion.Euler(0, 0, ItemTable.IsRotated ? -0f : 0f);
                RotateRectTransform(_displayNameRectTransform, ItemTable.IsRotated ? 0 : 1, 1);
                return;
            }

            _displayNameRectTransform.rotation = Quaternion.Euler(0, 0, ItemTable.IsRotated ? -0f : 0f);
            RotateRectTransform(_displayNameRectTransform, 1, ItemTable.IsRotated ? 0 : 1);

            _displayNameRectTransform.pivot = new Vector2(1, ItemTable.IsRotated ? 0 : 1);

            var sizeDelta = _displayNameRectTransform.sizeDelta;

            var localPositionHeight = ItemTable.IsRotated ? sizeDelta.y : 0;
            _displayNameRectTransform.anchoredPosition = new Vector2(-localPositionHeight, 0);
        }

        private void RotateRectTransform(RectTransform displayNameRectTransform, float x = 0f, float y = 0f)
        {
            var anchoredPosition = new Vector2(x, y);
            displayNameRectTransform.anchoredPosition = anchoredPosition;
            displayNameRectTransform.anchorMax = anchoredPosition;
            displayNameRectTransform.anchorMin = anchoredPosition;
        }

        public override void SetDragRepresentationDragStyle()
        {
            image.color = new Color32(255, 255, 225, 165);
            text.gameObject.SetActive(false);
        }

        public override void SetDragStyle()
        {
            _unsetColorFromDrag = image.color;
            image.color = new Color32(255, 255, 225, 235);
            text.gameObject.SetActive(false);
        }

        public override void UnsetDragStyle()
        {
            image.color = _unsetColorFromDrag;
            text.gameObject.SetActive(true);
        }

        public override void SetBackground(bool setBackground, Color? color = null)
        {
            if (background == null) return;

            if (color.HasValue)
            {
                background.color = color.Value;
            }

            if (setBackground)
            {
                background.gameObject.SetActive(true);
                return;
            }

            background.gameObject.SetActive(false);
        }
    }
}