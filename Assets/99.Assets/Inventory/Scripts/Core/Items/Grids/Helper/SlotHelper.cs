using Inventory.Scripts.Core.ScriptableObjects.Configuration.Tile;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.Scripts.Core.Items.Grids.Helper
{
    public static class SlotHelper
    {
        public static void SetSlotImageNormalizedWithTileSize(Sprite slot, TileSpecSo tileSpecSo, Image slotImage)
        {
            var slotSpriteWidth = slot.rect.size.x;

            slotImage.type = Image.Type.Tiled;
            slotImage.sprite = slot;
            slotImage.pixelsPerUnitMultiplier = slotSpriteWidth / tileSpecSo.TileSizeWidth;
        }
    }
}