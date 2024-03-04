using System;
using Inventory.Scripts.Core.Displays.Case;
using Inventory.Scripts.Core.Items.Grids;
using Inventory.Scripts.Core.Items.Grids.Helper;
using UnityEditor;
using UnityEngine;

namespace Inventory.Editor.Grids
{
    [CustomEditor(typeof(AbstractGrid), true)]
    [CanEditMultipleObjects]
    public class AbstractGridEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var objects = Selection.gameObjects;

            GUILayout.Space(10);

            if (GUILayout.Button("Apply Grid"))
            {
                foreach (var obj in objects)
                {
                    var abstractGrid = ParseAbstractGrid(obj);

                    if (abstractGrid == null) continue;

                    ApplyGrid(abstractGrid);
                }
            }
        }

        private static AbstractGrid ParseAbstractGrid(GameObject obj)
        {
            try
            {
                return obj.GetComponent<AbstractGrid>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static void ApplyGrid(AbstractGrid abstractGrid)
        {
            abstractGrid.ResizeGrid();

            HandleContainerGridsSize(abstractGrid);

            ResizeReCaseIfNeeded(abstractGrid.GridWidth, abstractGrid.GridHeight, abstractGrid);

            EditorUtility.SetDirty(abstractGrid);
        }

        private static void ResizeReCaseIfNeeded(int width, int height, AbstractGrid abstractGrid)
        {
            var reCaseDisplayFiller = abstractGrid.GetComponentInParent<ResidentEvilCaseDisplayFiller>(true);

            if (reCaseDisplayFiller == null) return;

            reCaseDisplayFiller.ResizeOnEditor(width, height);
        }

        private static void HandleContainerGridsSize(AbstractGrid abstractGrid)
        {
            var containerGrids = abstractGrid.GetComponentInParent<ContainerGrids>();

            if (containerGrids == null) return;

            var containerGridsRectTransform = containerGrids.GetComponent<RectTransform>();

            var sizeFittingAllChildren = RectHelper.GetSizeDeltaFittingChildren(containerGrids.transform);

            if (sizeFittingAllChildren != Vector2.zero)
            {
                containerGridsRectTransform.sizeDelta = sizeFittingAllChildren;
            }
        }
    }
}