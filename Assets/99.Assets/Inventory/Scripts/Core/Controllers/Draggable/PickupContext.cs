using Inventory.Scripts.Core.Holders;
using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.Items.Grids;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Tile;

namespace Inventory.Scripts.Core.Controllers.Draggable
{
    public class PickupContext
    {
        public bool Debug { get; set; }

        public AbstractItem SelectedInventoryItem { get; }
        public AbstractGrid AbstractGridFromStartDragging { get; }
        public ItemHolder ItemHolderFromStartDragging { get; }
        public TileGridHelperSo TileGridHelperSo { get; }

        public PickupContext(AbstractItem selectedInventoryItem, AbstractGrid abstractGridFromStartDragging,
            ItemHolder itemHolderFromStartDragging, TileGridHelperSo tileGridHelperSo)
        {
            SelectedInventoryItem = selectedInventoryItem;
            AbstractGridFromStartDragging = abstractGridFromStartDragging;
            ItemHolderFromStartDragging = itemHolderFromStartDragging;
            TileGridHelperSo = tileGridHelperSo;
        }
    }
}