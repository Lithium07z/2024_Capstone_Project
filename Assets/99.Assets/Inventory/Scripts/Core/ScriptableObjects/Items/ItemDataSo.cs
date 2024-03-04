using System.Linq;
using Inventory.Scripts.Core.ScriptableObjects.Audio;
using Inventory.Scripts.Core.ScriptableObjects.Items.Dimensions;
using Inventory.Scripts.Core.ScriptableObjects.Items.IconSize;
using Inventory.Scripts.Core.ScriptableObjects.Options;
using UnityEngine;

namespace Inventory.Scripts.Core.ScriptableObjects.Items
{
    [CreateAssetMenu(menuName = "Inventory/Items/New Item")]
    public class ItemDataSo : ScriptableObject
    {
        [Header("Item Data Settings")] [SerializeField]
        private string displayName;

        [SerializeField, TextArea] private string description;

        [SerializeField] private DimensionsSo dimensionsSo;

        [SerializeField] private Sprite icon;

        [SerializeField] private ItemDataTypeSo itemDataTypeSo;

        [SerializeField] private AudioCueSo audioCueSo;

        [Header("Images Configuration")] [SerializeField] [Tooltip("The icon size in the grid")]
        private IconSizeSo iconSizeSo;

        [SerializeField] [Tooltip("The icon size when the item is in the ItemHolder")]
        private IconSizeSo inHolderIconSizeSo;

        [Header("Group Options Settings")] [SerializeField]
        private OptionSo[] optionsSo;

        public string DisplayName => displayName;

        public string Description => description;

        public DimensionsSo DimensionsSo
        {
            get => dimensionsSo;
            set => dimensionsSo = value;
        }

        public Sprite Icon => icon;

        public ItemDataTypeSo ItemDataTypeSo => itemDataTypeSo;

        public IconSizeSo IconSizeSo => iconSizeSo;

        public IconSizeSo InHolderIconSizeSo => inHolderIconSizeSo;

        public AudioCueSo AudioCueSo => audioCueSo;

        public OptionSo[] OptionsSo
        {
            get => optionsSo;
            set => optionsSo = value;
        }

        public OptionSo[] GetOptionsOrdered()
        {
            return OptionsSo.OrderBy(so => so.Order).ToArray();
        }
    }
}