using Inventory.Scripts.Core.ScriptableObjects;
using UnityEngine;

namespace Inventory.Scripts.Core.Controllers
{
    public class InventoryConfiguration : MonoBehaviour
    {
        [Header("Inventory Settings Anchor Settings")] [SerializeField]
        private InventorySettingsAnchorSo inventorySettingsAnchorSo;

        [SerializeField] private InventorySettingsSo sceneInventorySettingsSo;

        private void Awake()
        {
            inventorySettingsAnchorSo.Set(sceneInventorySettingsSo);
        }

        private void OnEnable()
        {
            inventorySettingsAnchorSo.Set(sceneInventorySettingsSo);
        }
    }
}