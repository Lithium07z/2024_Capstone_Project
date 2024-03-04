using Inventory.Scripts.Core.Displays;
using Inventory.Scripts.Core.Enums;
using Inventory.Scripts.Core.Items;
using UnityEngine;

namespace Inventory.Scripts.Core.ScriptableObjects.Configuration.Anchors
{
    [CreateAssetMenu(menuName = "Inventory/Container Display Anchor So")]
    public class ContainerDisplayAnchorSo : AnchorSo<AbstractContainerDisplay>
    {
        public void OpenContainer(ItemTable container)
        {
            if (!isSet) return;

            Value.DisplayInteract(container, DisplayInteraction.Open);
        }

        public void CloseContainer(ItemTable container)
        {
            if (!isSet) return;

            Value.DisplayInteract(container, DisplayInteraction.Close);
        }
    }
}