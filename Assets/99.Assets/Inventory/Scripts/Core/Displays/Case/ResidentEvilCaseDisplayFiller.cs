using System.Linq;
using Inventory.Scripts.Core.Displays.Filler;
using Inventory.Scripts.Core.ItemsMetadata;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.Scripts.Core.Displays.Case
{
    /// <summary>
    /// Layout to resize paddings to fit the grid inside to be like a case.
    /// You might considering use this if you building a grid inventory like Resident Evil.
    /// </summary>
    [AddComponentMenu("Layout/Resident Evil Case Display Filler", 151)]
    [RequireComponent(typeof(HorizontalOrVerticalLayoutGroup))]
    public class ResidentEvilCaseDisplayFiller : DisplayFiller
    {
        [Header("Initial Paddings for 1x1 Grid")] [SerializeField]
        protected RectOffset initialPaddings = new();

        public RectOffset InitialPaddings
        {
            set => initialPaddings = value;
        }

        private HorizontalOrVerticalLayoutGroup _layoutGroup;

        protected override void Awake()
        {
            base.Awake();

            _layoutGroup = GetComponent<HorizontalOrVerticalLayoutGroup>();
        }

        protected override void OnSetItem()
        {
            base.OnSetItem();

            var (width, height) = GetWidthAndHeightFromGrid();

            ResizeCase(width, height);
        }

        private void ResizeCase(int width, int height)
        {
            var leftPadding = initialPaddings.left * width;
            var rightPadding = initialPaddings.right * width;
            var topPadding = initialPaddings.top * height;
            var bottomPadding = initialPaddings.bottom * height;

            var layoutGroup = GetLayoutGroup();

            layoutGroup.padding = new RectOffset(
                leftPadding,
                rightPadding,
                topPadding,
                bottomPadding
            );
        }

        private (int, int) GetWidthAndHeightFromGrid()
        {
            var container = CurrentInventoryItem;

            if (container?.InventoryMetadata is not ContainerMetadata containerMetadata) return (1, 1);

            // TODO: Add support for multiple grids right here.

            var gridsInventory = containerMetadata.GridsInventory;

            var width = gridsInventory.Sum(grid => grid.Width);
            var height = gridsInventory.Sum(grid => grid.Height);

            return (width, height);
        }

        protected override void OnResetItem()
        {
            base.OnResetItem();

            var layoutGroup = GetLayoutGroup();

            layoutGroup.padding = new RectOffset(
                initialPaddings.left,
                initialPaddings.right,
                initialPaddings.top,
                initialPaddings.bottom
            );
        }

        private HorizontalOrVerticalLayoutGroup GetLayoutGroup()
        {
            if (_layoutGroup == null)
            {
                _layoutGroup = GetComponent<HorizontalOrVerticalLayoutGroup>();
            }

            return _layoutGroup;
        }

#if UNITY_EDITOR
        public void ResizeOnEditor(int width, int height)
        {
            ResizeCase(width, height);
        }
#endif
    }
}