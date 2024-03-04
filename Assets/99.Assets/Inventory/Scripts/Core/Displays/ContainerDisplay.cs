using Inventory.Scripts.Core.Items;
using System.Diagnostics;

namespace Inventory.Scripts.Core.Displays
{
    public class ContainerDisplay : AbstractContainerDisplay
    {
        protected override void OnAddDisplayContainer(ItemTable container)
        {
            base.OnAddDisplayContainer(container);

            var displayFiller = GetFreeDisplayFiller(container);

            if (displayFiller == null) return;

            displayFiller.Open(container);
        }

        protected override void OnRemoveDisplayContainer(ItemTable container)
        {
            base.OnRemoveDisplayContainer(container);

            var displayFiller = FindDisplayFillerBy(container);

            if (displayFiller == null) return;

            displayFiller.Close(container);
        }
    }
}