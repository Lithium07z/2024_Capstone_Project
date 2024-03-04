using UnityEngine;

namespace Inventory.Scripts.Core.ScriptableObjects.Configuration.Anchors
{
    public abstract class AnchorSo<T> : ScriptableObject
    {
        [HideInInspector] public bool
            isSet = false; // Any script can check if the transform is null before using it, by just checking this bool

        [SerializeField]
        private T value;

        public T Value
        {
            get => value;
            set
            {
                this.value = value;
                isSet = this.value != null;
            }
        }

        public void OnDisable()
        {
            value = default;
            isSet = false;
        }
    }
}