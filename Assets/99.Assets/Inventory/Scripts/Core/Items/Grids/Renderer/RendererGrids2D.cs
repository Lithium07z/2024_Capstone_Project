using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Scripts.Core.Items.Grids.Renderer
{
    public class RendererGrids2D : RendererGrids
    {
        protected override ContainerGrids HydrateGrid(Transform parentTransform, ContainerGrids prefabContainerGrids,
            List<GridTable> existingItems)
        {
            var instantiatedContainerGrid = GameObject.Instantiate(prefabContainerGrids, parentTransform);

            instantiatedContainerGrid.gameObject.SetActive(false);

            instantiatedContainerGrid.SetGridTables(existingItems);

            HandleRectTransform(parentTransform, instantiatedContainerGrid);

            instantiatedContainerGrid.gameObject.SetActive(true);

            return instantiatedContainerGrid;
        }

        private static void HandleRectTransform(Transform parentTransform, ContainerGrids containerGridsInstantiated)
        {
            var containerGridsRectTransform = containerGridsInstantiated.GetComponent<RectTransform>();

            containerGridsRectTransform.SetParent(parentTransform);
            containerGridsRectTransform.localPosition = new Vector3(0, 0, 0);

            containerGridsRectTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            containerGridsRectTransform.localScale = new Vector3(1, 1, 1);
        }

        public override void Dehydrate(ContainerGrids containerGrids, List<GridTable> gridTables)
        {
            GameObject.Destroy(containerGrids.gameObject);
        }
    }
}