using UnityEngine;

namespace Inventory.Scripts.Core.ScriptableObjects.Configuration.Anchors
{
    [CreateAssetMenu(menuName = "Inventory/Configuration/Runtime Anchors/Window Parent prefab reference")]
    public class WindowParentRTPrefabAnchorSo : RectTransformPrefabAnchorSo
    {
        public override void NormalizeRectTransform()
        {
            base.NormalizeRectTransform();

            Value.sizeDelta = Vector2.zero;
            Value.anchoredPosition = Vector2.zero;

            Value.anchorMin = new Vector2(0, 0);
            Value.anchorMax = new Vector2(1, 1);
            Value.pivot = new Vector2(0.5f, 0.5f);
        }
    }
}