using Inventory.Scripts.Core.ScriptableObjects.Configuration.Anchors;
using UnityEngine;

namespace Inventory.Scripts.Core
{
    public class CanvasInitializer : MonoBehaviour
    {
        [SerializeField] private CanvasAnchorSo canvasAnchorSo;

        [SerializeField] private Canvas canvas;

        private void Awake()
        {
            if (canvas == null)
            {
                canvas = GetComponent<Canvas>();
            }

            canvasAnchorSo.Value = canvas;
        }

        public void SetAnchors(CanvasAnchorSo canvasAnchor)
        {
            canvas = GetComponent<Canvas>();
            canvasAnchorSo = canvasAnchor;
        }
    }
}