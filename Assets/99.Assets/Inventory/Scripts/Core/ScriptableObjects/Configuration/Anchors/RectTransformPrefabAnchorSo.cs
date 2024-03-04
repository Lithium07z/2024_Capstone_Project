using UnityEngine;

namespace Inventory.Scripts.Core.ScriptableObjects.Configuration.Anchors
{
    [CreateAssetMenu(menuName = "Inventory/Configuration/Runtime Anchors/RectTransform prefab reference")]
    public class RectTransformPrefabAnchorSo : PrefabAnchorSo<RectTransform>
    {
        public virtual void NormalizeRectTransform()
        {
            if (Value == null) return;

            Value.transform.localScale = new Vector3(1, 1, 1);
        }
    }
}