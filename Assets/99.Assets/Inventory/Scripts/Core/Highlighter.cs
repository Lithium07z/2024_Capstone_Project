using UnityEngine;
using UnityEngine.UI;

namespace Inventory.Scripts.Core
{
    public class Highlighter : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private Image _image;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _image = GetComponent<Image>();

            transform.localScale = Vector3.one;
        }

        public void Show(bool shouldShow)
        {
            gameObject.SetActive(shouldShow);
        }

        public void SetColor(Color color)
        {
            _image.color = color;
        }

        public void SetParent(RectTransform highlightParent)
        {
            _rectTransform.SetParent(highlightParent);
        }

        public void SetLocalPosition(Vector2 localPositionOnGrid)
        {
            _rectTransform.localPosition = localPositionOnGrid;
        }

        public void SetSizeDelta(Vector2 size)
        {
            _rectTransform.sizeDelta = size;
        }
    }
}