using System.Linq;
using Inventory.Editor.Helper;
using Inventory.Scripts.Core.Helper;
using Inventory.Scripts.Core.Items.Grids;
using Inventory.Scripts.Core.ItemsMetadata;
using Inventory.Scripts.Core.ScriptableObjects.Items;
using UnityEditor;
using UnityEngine;

namespace Inventory.Editor.Drawers.Metadata
{
    [CustomPropertyDrawer(typeof(ContainerMetadata))]
    public class ContainerMetadataPropertyDrawer : PropertyDrawer
    {
        private const float ScrollViewArea = 484f;
        private const float PaddingContainerGrids = 30f;

        private bool _useDefaultDrawer;
        private Vector2 _scrollPos;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var metadata = EditorPropertyHelper.GetValue<ContainerMetadata>(property, fieldInfo);

            property.isExpanded = EditorGUI.Foldout(
                new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                property.isExpanded,
                property.displayName
            );

            if (!property.isExpanded)
            {
                EditorGUI.EndProperty();
                return;
            }

            EditorGUI.indentLevel++;

            position = position.NextLine();

            EditorGUI.LabelField(position, "Type", metadata.GetType().Name);

            position = position.NextLine();

            _useDefaultDrawer = EditorGUI.Toggle(position, "Use Default Drawer", _useDefaultDrawer);

            position = position.NextLine();

            if (_useDefaultDrawer)
            {
                UseDefaultDrawerProperty(position, property);
            }
            else
            {
                BuildContainerDrawerGUI(position, metadata, property);
            }

            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
        }

        private void BuildContainerDrawerGUI(Rect position, ContainerMetadata metadata, SerializedProperty property)
        {
            position = position.NextLine();

            var containerGrids = GetContainerGridsIfExists(metadata);

            if (containerGrids == null)
            {
                UseDefaultDrawerProperty(position, property);
                return;
            }

            var gridsInventory = metadata.GridsInventory;
            var abstractGrids = containerGrids.GetAbstractGrids();

            if (abstractGrids.Length == 0 || gridsInventory.Count != abstractGrids.Length)
            {
                UseDefaultDrawerProperty(position, property);
                return;
            }

            var abstractGridForTile = FindAbstractGridWhereTileSpecIsNotNull(abstractGrids);

            if (abstractGridForTile == null)
            {
                Debug.LogWarning(
                    "AbstractGrid have something null... inventorySettingsAnchorSo or inventorySettingsSo or tileSpecSo"
                        .Editor());
                UseDefaultDrawerProperty(position, property);
                return;
            }

            var inventorySettingsAnchorSo = abstractGridForTile.InventorySettingsAnchorSo;
            var inventorySettingsSo = inventorySettingsAnchorSo.InventorySettingsSo;
            var tileSpecSo = inventorySettingsSo.TileSpecSo;

            EditorGUI.LabelField(position, "Container Drawer", "Experiment");

            position = position.NextLine();

            EditorGUI.DrawRect(
                new Rect(position.x + 30f, position.y + 8f, position.width - 13f, ScrollViewArea),
                new Color(0, 0, 0, 0.1f));

            var rectContainerGrids = GetContainerGridsWidthAndHeight(containerGrids);
            var widthAndHeightForEditorSize =
                TableDrawerHelper.ParseWidthAndHeightForEditorSize(rectContainerGrids, tileSpecSo);

            var scrollArea = new Rect(position.x + 30f, position.y + 8f, position.width - 28f, ScrollViewArea);

            _scrollPos = GUI.BeginScrollView(
                scrollArea,
                _scrollPos,
                new Rect(position.x + 20f, position.y - 8f, widthAndHeightForEditorSize.x + PaddingContainerGrids,
                    widthAndHeightForEditorSize.y + PaddingContainerGrids)
            );

            for (var i = 0; i < gridsInventory.Count; i++)
            {
                var abstractGrid = abstractGrids[i];
                var gridTable = gridsInventory[i];

                var abstractGridTransform = (RectTransform)abstractGrid.transform;

                var pos = GetWorldRect(abstractGridTransform);

                pos.x += position.x + 40f;
                pos.y -= position.y + 15f * EditorGUIUtility.singleLineHeight;

                pos = TableDrawerHelper.ResolvePosToEditorTable(pos, tileSpecSo);

                pos.y = Mathf.Abs(pos.y) - TableDrawerHelper.TableLineSpace * abstractGrid.GridHeight;
                pos.width = position.width;
                pos.height = position.height;

                TableDrawerHelper.DrawTable(pos, gridTable.Width, gridTable.Height, gridTable.InventoryItemsSlot);
            }

            GUI.EndScrollView();
        }

        private static void UseDefaultDrawerProperty(Rect position, SerializedProperty property)
        {
            var propertyGridsInventory =
                EditorPropertyHelper.FindRelativeAutoProperty(property, nameof(ContainerMetadata.GridsInventory));

            if (propertyGridsInventory != null)
            {
                EditorGUI.PropertyField(
                    new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                    propertyGridsInventory,
                    true
                );
            }
        }

        private static Vector2 GetContainerGridsWidthAndHeight(ContainerGrids containerGrids)
        {
            var rectTransform = (RectTransform)containerGrids.transform;
            var rectTransformRect = rectTransform.rect;

            var rectWidth = rectTransformRect.width;
            var rectHeight = rectTransformRect.height;

            return new Vector2(rectWidth, rectHeight);
        }

        private static Rect GetWorldRect(RectTransform rectTransform)
        {
            var corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            return new Rect(corners[0], corners[2] - corners[0]);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
            {
                return base.GetPropertyHeight(property, label);
            }

            var metadata = EditorPropertyHelper.GetValue<ContainerMetadata>(property, fieldInfo);

            var gridsHeight = 0f;

            var currentGridTableProperty =
                EditorPropertyHelper.FindRelativeAutoProperty(property, nameof(ContainerMetadata.GridsInventory));

            if (currentGridTableProperty != null && _useDefaultDrawer)
            {
                gridsHeight = EditorGUI.GetPropertyHeight(currentGridTableProperty, true);
            }

            var containerGridsIfExists = GetContainerGridsIfExists(metadata);

            if (!_useDefaultDrawer && containerGridsIfExists != null)
            {
                var abstractGrids = containerGridsIfExists.GetAbstractGrids();
                var abstractGrid = FindAbstractGridWhereTileSpecIsNotNull(abstractGrids);

                if (abstractGrid == null)
                {
                    gridsHeight += EditorGUI.GetPropertyHeight(currentGridTableProperty, true);
                }

                gridsHeight += ScrollViewArea + 2f * TableDrawerHelper.TableLineSpace;
            }

            return gridsHeight;
        }

        private ContainerGrids GetContainerGridsIfExists(ContainerMetadata containerMetadata)
        {
            var itemTable = containerMetadata.ItemTable;

            if (itemTable?.ItemDataSo is ItemContainerDataSo containerDataSo)
            {
                return containerDataSo.ContainerGrids;
            }

            return null;
        }

        private static AbstractGrid FindAbstractGridWhereTileSpecIsNotNull(AbstractGrid[] abstractGrids)
        {
            var abstractGridForTile = abstractGrids.FirstOrDefault(abstractGrid =>
                abstractGrid.InventorySettingsAnchorSo != null &&
                abstractGrid.InventorySettingsAnchorSo.InventorySettingsSo != null &&
                abstractGrid.InventorySettingsAnchorSo.InventorySettingsSo.TileSpecSo !=
                null);

            return abstractGridForTile;
        }
    }
}