using System.Collections.Generic;
using System.Linq;
using Inventory.Scripts.Core.Helper;
using Inventory.Scripts.Core.Items.Grids.Helper;
using UnityEngine;

namespace Inventory.Scripts.Core.Items.Grids
{
    [RequireComponent(typeof(RectTransform))]
    public class ContainerGrids : MonoBehaviour
    {
        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            gameObject.AddComponent<LockRotation>();
        }

        private void Start()
        {
            FitChildrenContent();
        }

        /// <summary>
        /// Method that will fit all children grids. This is used to make the ContainerGrids fits inside the display filler without overflowing.
        /// Make sure this method executes after the Grids <see cref="AbstractGrid.ResizeGrid" />. So it must stay on Start and the <see cref="AbstractGrid.ResizeGrid" /> on Awake.
        /// </summary>
        private void FitChildrenContent()
        {
            var sizeFittingAllChildren = RectHelper.GetSizeDeltaFittingChildren(transform);

            if (sizeFittingAllChildren != Vector2.zero)
            {
                _rectTransform.sizeDelta = sizeFittingAllChildren;
            }
        }

        public AbstractGrid[] GetAbstractGrids()
        {
            return GetComponentsInChildren<AbstractGrid>();
        }

        public void SetGridTables(List<GridTable> existingItems)
        {
            var abstractGrids = GetAbstractGrids();

            foreach (var gridTable in existingItems)
            {
                // TODO: Improve this code...
                var abstractGrid = abstractGrids.First(grid =>
                    (grid.Grid == null || grid.Grid.Width == 0 && grid.Grid.Height == 0) &&
                    gridTable.Width == grid.GridWidth && gridTable.Height == grid.GridHeight);

                abstractGrid.Set(gridTable);
            }
        }
    }
}