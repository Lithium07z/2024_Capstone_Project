using UnityEngine;

namespace Inventory.Scripts.Core.ScriptableObjects.Configuration.Tile
{
    [CreateAssetMenu(menuName = "Inventory/Configuration/New Tile Spec So")]
    public class TileSpecSo : ScriptableObject
    {
        [SerializeField] private int tileSizeWidth = 84;
        [SerializeField] private int tileSizeHeight = 84;

        public int TileSizeWidth => tileSizeWidth;

        public int TileSizeHeight => tileSizeHeight;
    }
}
