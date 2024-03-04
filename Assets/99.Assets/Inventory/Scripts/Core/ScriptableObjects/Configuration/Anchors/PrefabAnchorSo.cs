using UnityEngine;

namespace Inventory.Scripts.Core.ScriptableObjects.Configuration.Anchors
{
    public class PrefabAnchorSo<T> : ScriptableObject where T : Object
    {
        [SerializeField] private T prefab;

        private T _value;

        public T Value
        {
            get
            {
                if (_value == null)
                {
                    _value = Instantiate(prefab);
                }

                return _value;
            }
        }

        private void OnDisable()
        {
            // Sets to null when stops the game in the editor
            _value = null;
        }
    }
}