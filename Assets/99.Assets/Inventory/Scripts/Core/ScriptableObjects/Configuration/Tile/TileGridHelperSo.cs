using Inventory.Scripts.Core.Controllers.Inputs;
using Inventory.Scripts.Core.Items;
using UnityEngine;

namespace Inventory.Scripts.Core.ScriptableObjects.Configuration.Tile
{
    // You can create an implementation from this TileGridHelperSo to be for 3D inventories.
    // [CreateAssetMenu(menuName = "Inventory/Tile/New Tile Get Grid Position 2D")]
    public sealed class TileGridHelperSo : ScriptableObject
    {
        [SerializeField] private InventorySettingsAnchorSo inventorySettingsAnchorSo;

        [SerializeField] private InputProviderSo inputProviderSo;

        private Vector2Int _tileGridPosition;

        public Vector2Int GetTileGridPosition(Transform gridTransform, AbstractItem inventoryItem)
        {
            var position = inputProviderSo.GetState().CursorPosition;

            var itemGridTransform = (RectTransform)gridTransform;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(itemGridTransform, position, null,
                out var positionInScreen);

            if (inventoryItem == null) return GetTileGridPositionByPositionInScreen(positionInScreen);

            var tileSpecSo = GetTileSpecSo();

            positionInScreen.x -= (inventoryItem.ItemTable.Width - 1.0f) * tileSpecSo.TileSizeWidth / 2;
            positionInScreen.y += (inventoryItem.ItemTable.Height - 1.0f) * tileSpecSo.TileSizeHeight / 2;

            return GetTileGridPositionByPositionInScreen(positionInScreen);
        }

        public Vector2Int GetTileGridPositionByGridTable(Transform gridTransform)
        {
            var position = inputProviderSo.GetState().CursorPosition;

            var itemGridTransform = (RectTransform)gridTransform;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(itemGridTransform, position, null,
                out var positionInScreen);

            return GetTileGridPositionByPositionInScreen(positionInScreen);
        }

        private Vector2Int GetTileGridPositionByPositionInScreen(Vector2 positionInScreen)
        {
            var tileSpecSo = GetTileSpecSo();

            _tileGridPosition.x = (int)(positionInScreen.x / tileSpecSo.TileSizeWidth);
            _tileGridPosition.y = (int)-(positionInScreen.y / tileSpecSo.TileSizeHeight);

            return _tileGridPosition;
        }

        public Vector2 GetLocalPosition(RectTransform parent, Vector2 position, Camera camera = null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, position, camera, out var localPosition);
            return localPosition;
        }

        public Vector2 CalculatePositionOnGrid(AbstractItem item, int posX,
            int posY)
        {
            var tileSpecSo = GetTileSpecSo();

            var position = new Vector2
            {
                x = posX * tileSpecSo.TileSizeWidth +
                    tileSpecSo.TileSizeWidth * item.ItemTable.Width / 2,
                y = -(posY * tileSpecSo.TileSizeHeight +
                      tileSpecSo.TileSizeHeight * item.ItemTable.Height / 2)
            };

            return position;
        }

        public Vector2 GetGridSize(int gridWidth, int gridHeight)
        {
            var tileSpecSo = GetTileSpecSo();

            return new Vector2(gridWidth * tileSpecSo.TileSizeWidth,
                gridHeight * tileSpecSo.TileSizeHeight);
        }

        public Vector2 GetItemSize(int itemWidth, int itemHeight)
        {
            var tileSpecSo = GetTileSpecSo();

            var size = new Vector2
            {
                x = itemWidth * tileSpecSo.TileSizeWidth,
                y = itemHeight * tileSpecSo.TileSizeHeight
            };

            return size;
        }


        private TileSpecSo GetTileSpecSo()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.TileSpecSo;
        }
    }
}