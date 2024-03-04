using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Scripts.Core.Items.Grids.Renderer
{
    public abstract class RendererGrids
    {
        public ContainerGrids Hydrate(Transform parentTransform, ContainerGrids containerGrids,
            List<GridTable> existingItems)
        {
            return HydrateGrid(parentTransform, containerGrids, existingItems);
        }

        protected abstract ContainerGrids HydrateGrid(Transform parentTransform, ContainerGrids containerGrids,
            List<GridTable> existingItems);

        public abstract void Dehydrate(ContainerGrids containerGrids, List<GridTable> gridTables);
    }
}