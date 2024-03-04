using Inventory.Editor.Helper;
using Inventory.Scripts.Core.Items;
using UnityEditor;
using UnityEngine;

namespace Inventory.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ItemTable))]
    public class ItemTablePropertyDrawer : PropertyDrawer
    {
        private const float LineSpace = 18f;
        private const float MarginTopField = 2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var itemTable = EditorPropertyHelper.GetValue<ItemTable>(property, fieldInfo);

            if (itemTable == null)
            {
                EditorGUI.LabelField(position, property.displayName, "null");
                EditorGUI.EndProperty();
                return;
            }

            property.isExpanded = EditorGUI.Foldout(
                new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                property.isExpanded,
                property.displayName
            );

            if (!property.isExpanded) return;

            var totalPosition = new Rect(position);

            EditorGUI.indentLevel++;

            var rect = CreateItemDataSoProperties(totalPosition, itemTable);

            rect.y += 1.5f * LineSpace;
            EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, LineSpace),
                "Grid Props", EditorStyles.boldLabel);
            var gridPosXRect = BuildTextField(totalPosition, nameof(ItemTable.OnGridPositionX),
                itemTable.OnGridPositionX.ToString(),
                rect);
            var gridPosYRect = BuildTextField(totalPosition, nameof(ItemTable.OnGridPositionY),
                itemTable.OnGridPositionY.ToString(),
                gridPosXRect);

            gridPosYRect.y += LineSpace + MarginTopField;

            var currentGridTableProperty =
                EditorPropertyHelper.FindRelativeAutoProperty(property, nameof(ItemTable.CurrentGridTable));

            EditorGUI.PropertyField(new Rect(gridPosYRect.x, gridPosYRect.y, gridPosYRect.width, LineSpace),
                currentGridTableProperty,
                true);

            var gridPropertyHeight = EditorGUI.GetPropertyHeight(currentGridTableProperty, true);

            gridPosYRect.y += gridPropertyHeight > 50 ? gridPropertyHeight + LineSpace : LineSpace + MarginTopField;

            EditorGUI.PropertyField(new Rect(gridPosYRect.x, gridPosYRect.y, gridPosYRect.width, LineSpace),
                EditorPropertyHelper.FindRelativeAutoProperty(property, nameof(ItemTable.CurrentItemHolder)),
                true);

            gridPosYRect.y += LineSpace + MarginTopField;

            EditorGUI.PropertyField(new Rect(gridPosYRect.x, gridPosYRect.y, gridPosYRect.width, LineSpace),
                EditorPropertyHelper.FindRelativeAutoProperty(property, nameof(ItemTable.InventoryMetadata)),
                true);

            EditorGUI.indentLevel--;

            EditorGUI.EndProperty();
        }

        private static Rect CreateItemDataSoProperties(Rect totalPosition, ItemTable itemTable)
        {
            if (itemTable.ItemDataSo == null)
            {
                return totalPosition;
            }

            var widthRect = BuildTextField(totalPosition, "Display name", itemTable.ItemDataSo.DisplayName,
                new Rect(totalPosition.x, totalPosition.y + LineSpace, totalPosition.width, LineSpace), true);
            var widthProperty = BuildTextField(totalPosition, "Width", itemTable.Width.ToString(), widthRect);
            var heightProperty = BuildTextField(totalPosition, "Height", itemTable.Height.ToString(), widthProperty);

            return heightProperty;
        }

        private static Rect BuildTextField(Rect position, string text, string value, Rect verticalRect,
            bool isInitialProp = false)
        {
            var rect = new Rect(verticalRect);
            if (!isInitialProp)
            {
                rect.y += LineSpace + MarginTopField;
            }

            EditorGUI.LabelField(
                new Rect(position.x, rect.y, position.width, LineSpace), text, value);
            return rect;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
            {
                return base.GetPropertyHeight(property, label);
            }

            var propertiesHeight = 0f;

            var grid = property.FindPropertyRelative(
                EditorPropertyHelper.ParseAutoPropertyName(nameof(ItemTable.CurrentGridTable)));
            if (grid != null)
            {
                propertiesHeight += EditorGUI.GetPropertyHeight(grid, true);
            }

            var inventoryMetadataProperty =
                property.FindPropertyRelative(
                    EditorPropertyHelper.ParseAutoPropertyName(nameof(ItemTable.InventoryMetadata)));

            if (inventoryMetadataProperty != null)
            {
                propertiesHeight += EditorGUI.GetPropertyHeight(inventoryMetadataProperty, true);
            }


            const float marginBottom = 45f;
            const float extraPaddings = 4f;

            return 8 * (LineSpace + MarginTopField) + propertiesHeight + marginBottom +
                   extraPaddings;
        }
    }
}