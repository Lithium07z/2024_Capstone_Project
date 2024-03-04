using UnityEngine;

namespace Inventory.Scripts.Core.ScriptableObjects.Items.IconSize
{
    [CreateAssetMenu(menuName = "Inventory/Items/Icons/New Icon Size")]
    public class IconSizeSo : ScriptableObject
    {
        [Header("Icon Settings")]
        [SerializeField] private int width = 84;
        [SerializeField] private int height = 84;

        [Header("Position Rect Transform")] [SerializeField]
        private float posX = 0;
        
        [SerializeField]
        private float posY = 0;
        
        public int Width => width;

        public int Height => height;

        public float PosX => posX;

        public float PosY => posY;
    }
}