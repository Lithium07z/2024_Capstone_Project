using Inventory.Scripts.Core.Holders;
using Inventory.Scripts.Core.Items.Grids;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Tile;

namespace Inventory.Scripts.Core.Controllers.Draggable
{
    public class ReleaseContext
    {
        public bool Debug { get; set; }

        public PickupState PickupState { get; }
        public AbstractGrid SelectedAbstractGrid { get; }
        public ItemHolder SelectedItemHolder { get; }
        public TileGridHelperSo TileGridHelperSo { get; }

        public ReleaseContext(PickupState pickupState, AbstractGrid selectedAbstractGrid,
            ItemHolder selectedItemHolder, TileGridHelperSo tileGridHelperSo)
        {
            PickupState = pickupState;
            SelectedAbstractGrid = selectedAbstractGrid;
            SelectedItemHolder = selectedItemHolder;
            TileGridHelperSo = tileGridHelperSo;
        }
    }
}