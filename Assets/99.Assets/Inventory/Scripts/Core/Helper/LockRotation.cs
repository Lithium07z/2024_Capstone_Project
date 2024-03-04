using UnityEngine;

namespace Inventory.Scripts.Core.Helper
{
    public class LockRotation : MonoBehaviour
    {
        private void OnEnable()
        {
            var transformData = transform;
            transformData.rotation = Quaternion.identity;
            transformData.localScale = Vector3.one;
        }
    }
}