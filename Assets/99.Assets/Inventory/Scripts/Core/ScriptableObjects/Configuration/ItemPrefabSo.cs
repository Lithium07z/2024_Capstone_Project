using UnityEngine;

namespace Inventory.Scripts.Core.ScriptableObjects.Configuration
{
    [CreateAssetMenu(menuName = "Inventory/Configuration/Prefabs/New Item Prefab Config")]
    public class ItemPrefabSo : ScriptableObject
    {
        [SerializeField] private GameObject itemPrefab;

        public GameObject ItemPrefab => itemPrefab;
    }
}