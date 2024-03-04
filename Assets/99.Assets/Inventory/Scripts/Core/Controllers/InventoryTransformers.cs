using Inventory.Scripts.Core.ScriptableObjects.Configuration.Anchors;
using UnityEngine;

namespace Inventory.Scripts.Core.Controllers
{
    public class InventoryTransformers : MonoBehaviour
    {
        [SerializeField] private CanvasAnchorSo canvasAnchorSo;

        [Header("Transforms to set as last siblings"),
         Tooltip(
             "The order is important, so the last element will be the last to be set as sibling. " +
             "So make sure to put in the correct order so it will not bug the UI.")]
        [SerializeField]
        private RectTransformPrefabAnchorSo[] transforms;

        private void Start()
        {
            UpdateTransforms();
        }

        private void OnEnable()
        {
            UpdateTransforms();
        }

        private void UpdateTransforms()
        {
            if (!canvasAnchorSo.isSet) return;
            
            foreach (var transformPrefabAnchorSo in transforms)
            {
                transformPrefabAnchorSo.Value.SetParent(canvasAnchorSo.Value.transform);
                transformPrefabAnchorSo.Value.SetAsLastSibling();
                transformPrefabAnchorSo.NormalizeRectTransform();
            }
        }

        public void SetTransformers(RectTransformPrefabAnchorSo[] newListTransformers)
        {
            transforms = newListTransformers;
        }
    }
}