using Inventory.Editor.Helper;
using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.Items.Grids;
using UnityEditor;
using UnityEngine;

namespace Inventory.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(GridTable))]
    public class GridTablePropertyDrawer : PropertyDrawer
    {
        private static readonly float LineSpace = EditorGUIUtility.singleLineHeight;
        private const float MarginTopField = 2f;

        private Vector2 _scrollPos;

        private Rect _oldPosition;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var gridTable = GetValue(property);

            if (gridTable == null)
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

            var width = gridTable.Width;
            var height = gridTable.Height;

            EditorGUI.indentLevel++;

            var widthRect = new Rect(totalPosition.x, totalPosition.y + LineSpace, totalPosition.width, LineSpace);
            EditorGUI.LabelField(widthRect, nameof(GridTable.Width), width.ToString());

            var heightRect = new Rect(widthRect);
            EditorGUI.LabelField(
                new Rect(totalPosition.x, heightRect.y + LineSpace + MarginTopField, totalPosition.width, LineSpace),
                nameof(GridTable.Height),
                height.ToString());

            BuildTableInventory(new Rect(totalPosition.x, heightRect.y + LineSpace, totalPosition.width, LineSpace),
                width,
                height,
                gridTable.InventoryItemsSlot
            );

            EditorGUI.indentLevel--;

            EditorGUI.EndProperty();
        }

        private void BuildTableInventory(Rect position, int width, int height, ItemTable[,] inventoryItemsSlot)
        {
            position.y += LineSpace + MarginTopField + 12;

            EditorGUI.LabelField(new Rect(position.x, position.y, position.width, LineSpace),
                nameof(GridTable.InventoryItemsSlot));

            position.x += 15f;
            position.width -= 15f;
            position.y += LineSpace * 1.25f;

            if (inventoryItemsSlot == null)
            {
                const int fakeWidth = 2;
                const int fakeHeight = 2;
                _scrollPos = TableDrawerHelper.DrawScrollableTable(_scrollPos, position, fakeWidth, fakeHeight,
                    new ItemTable[fakeWidth, fakeHeight]);
                return;
            }

            _scrollPos = TableDrawerHelper.DrawScrollableTable(_scrollPos, position, width, height,
                inventoryItemsSlot);

            position.y += LineSpace * 1.5f;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
            {
                return base.GetPropertyHeight(property, label);
            }

            var gridTable = GetValue(property);

            if (gridTable == null)
            {
                return base.GetPropertyHeight(property, label);
            }

            const float marginBottom = 20f;
            const float extraPaddings = 4f;

            const float tableLineSpace = TableDrawerHelper.TableLineSpace;
            const float scrollViewArea = TableDrawerHelper.ScrollViewArea;

            if (gridTable.InventoryItemsSlot == null)
            {
                return 2 * (LineSpace + MarginTopField) + (tableLineSpace * 3) + extraPaddings;
            }

            var resolvedHeight = gridTable.Height == 0 ? 1 : gridTable.Height;

            var backgroundInventory = resolvedHeight * tableLineSpace + LineSpace + 12f + 4f;

            return 2 * (LineSpace + MarginTopField) +
                   (backgroundInventory < scrollViewArea ? backgroundInventory : scrollViewArea + 4f + tableLineSpace) +
                   marginBottom +
                   extraPaddings;
        }

        private GridTable GetValue(SerializedProperty property)
        {
            var gridTable = EditorPropertyHelper.GetValue<GridTable>(property, fieldInfo);

            return gridTable;
        }
    }
}