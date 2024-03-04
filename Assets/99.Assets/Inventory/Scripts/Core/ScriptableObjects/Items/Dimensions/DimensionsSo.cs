using UnityEngine;

namespace Inventory.Scripts.Core.ScriptableObjects.Items.Dimensions
{
    [CreateAssetMenu(menuName = "Inventory/Items/Dimensions/New Dimensions")]
    public class DimensionsSo : ScriptableObject
    {
        [SerializeField] private int width;
        [SerializeField] private int height;

        public int Width
        {
            get => width;
            set => width = value;
        }

        public int Height
        {
            get => height;
            set => height = value;
        }
    }
}