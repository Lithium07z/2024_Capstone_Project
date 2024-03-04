using UnityEngine;

namespace Inventory.Scripts.Core.ScriptableObjects
{
    // Should be created only one in the project. This will holds the 'InventorySettingsSo' in order to remove the dependencies from code.
    // [CreateAssetMenu(menuName = "Inventory/Runtime Anchors/New Inventory Settings Anchor So")]
    public class InventorySettingsAnchorSo : ScriptableObject
    {
        [Header("Inventory Settings So")] [SerializeField]
        private InventorySettingsSo inventorySettingsSo;

        public InventorySettingsSo InventorySettingsSo
        {
            get => inventorySettingsSo;
            private set => inventorySettingsSo = value;
        }

        public bool IsSet()
        {
            return InventorySettingsSo != null;
        }

        public void Set(InventorySettingsSo newInventorySettingsSo)
        {
            if (newInventorySettingsSo == null) return;

            InventorySettingsSo = newInventorySettingsSo;
        }
    }
}