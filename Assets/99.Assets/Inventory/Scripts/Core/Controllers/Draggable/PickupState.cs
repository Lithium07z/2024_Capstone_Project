using Inventory.Scripts.Core.Items;

namespace Inventory.Scripts.Core.Controllers.Draggable
{
    public class PickupState
    {
        public AbstractItem Item { get; set; }
        
        public bool? DraggableItemInitialRotation { get; set; }
    }
}