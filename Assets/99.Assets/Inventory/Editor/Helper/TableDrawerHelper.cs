using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Tile;
using UnityEditor;
using UnityEngine;

namespace Inventory.Editor.Helper
{
    public static class TableDrawerHelper
    {
        public const float TableLineSpace = 48f;
        private const float TableColumnSpace = 48f;
        public const float ScrollViewArea = 384f;


        public static Vector2 DrawScrollableTable(Vector2 scrollPos, Rect position, float width,
            float height,
            ItemTable[,] inventoryItemsSlot)
        {
            var backgroundInventory = height * TableLineSpace + 4f;

            EditorGUI.DrawRect(
                new Rect(position.x + 15f, position.y, position.width - 13f,
                    backgroundInventory < ScrollViewArea ? backgroundInventory : ScrollViewArea + 4f),
                new Color(0, 0, 0, 0.1f));

            var scrollArea = new Rect(position.x + 17f, position.y + 2f, position.width - 15f, ScrollViewArea);
            var newScrollPos = GUI.BeginScrollView(
                scrollArea,
                scrollPos,
                new Rect(position.x + 15f, position.y, width * TableColumnSpace + 17f, height * TableLineSpace + 17f)
            );

            DrawTable(position, width, height, inventoryItemsSlot);

            GUI.EndScrollView();

            return newScrollPos;
        }

        public static void DrawTable(Rect position, float width, float height, ItemTable[,] inventoryItemsSlot)
        {
            var initialPosition = new Rect(position);
            initialPosition.y -= TableLineSpace;

            for (var i = 0; i < height; i++)
            {
                initialPosition.y += TableLineSpace;
                initialPosition.x = position.x - TableColumnSpace;

                for (var j = 0; j < width; j++)
                {
                    initialPosition.x += TableColumnSpace;

                    var itemTable = inventoryItemsSlot[j, i];

                    if (itemTable != null)
                    {
                        var imageRect = new Rect(initialPosition.x + 15f, initialPosition.y, TableColumnSpace,
                            TableLineSpace);

                        const float padding = 2f;
                        const float resolvedPadding = padding / 2;
                        var paddingRect = new Rect(imageRect.x + resolvedPadding, imageRect.y + resolvedPadding,
                            imageRect.width - padding,
                            imageRect.height - padding);

                        EditorGUI.DrawRect(imageRect, new Color(0, 0, 0, 0.2f));
                        EditorGUI.DrawRect(
                            paddingRect,
                            new Color(0, 0, 0, 0.4f));

                        var guiColor = GUI.color;
                        GUI.color = Color.clear;

                        var paddingRectWidth = paddingRect.width;
                        var paddingRectHeight = paddingRect.height;

                        const float imagePadding = 6f;
                        const float resolvedImagePadding = imagePadding / 2;
                        var imagePaddingRect = new Rect(paddingRect.x + resolvedImagePadding,
                            paddingRect.y + resolvedImagePadding,
                            paddingRectWidth - imagePadding,
                            paddingRectHeight - imagePadding);

                        EditorGUI.DrawTextureTransparent(imagePaddingRect, itemTable.ItemDataSo.Icon.texture);
                        GUI.color = guiColor;
                    }
                    else
                    {
                        var imageRect = new Rect(initialPosition.x + 15f, initialPosition.y, TableColumnSpace,
                            TableLineSpace);

                        const float padding = 2f;
                        const float resolvedPadding = padding / 2;
                        var paddingRect = new Rect(imageRect.x + resolvedPadding, imageRect.y + resolvedPadding,
                            imageRect.width - padding,
                            imageRect.height - padding);

                        EditorGUI.DrawRect(imageRect, new Color(0, 0, 0, 0.125f));
                        EditorGUI.DrawRect(
                            paddingRect,
                            new Color(0, 0, 0, 0.245f));

                        // var guiStyle = new GUIStyle(EditorStyles.textField);
                        //
                        // guiStyle.alignment = TextAnchor.MiddleLeft;
                        //
                        // EditorGUI.SelectableLabel(
                        //     new Rect(initialPosition.x, initialPosition.y, TableColumnSpace + 15f, TableLineSpace),
                        //     "",
                        //     guiStyle);
                    }
                }
            }
            // position.x += 39f;
            // for (var i = 0; i < height; i++)
            // {
            //     DrawTableRectNumber(position);
            //     position.y += LineSpace * 1.22f;
            //     DrawTableRectNumber(position, true);
            //     for (var j = 0; j < width; j++)
            //     {
            //         Debug.Log("Lines");
            //     }
            // }
        }

        public static Rect ResolvePosToEditorTable(Rect pos, TileSpecSo tileSpecSo)
        {
            var tableColumnSpace = TableColumnSpace * pos.x;
            pos.x = tableColumnSpace / tileSpecSo.TileSizeHeight;

            var tableLineSpace = TableLineSpace * pos.y;
            pos.y = tableLineSpace / tileSpecSo.TileSizeWidth;

            return pos;
        }

        public static Vector2 ParseWidthAndHeightForEditorSize(Vector2 rectContainerGrids, TileSpecSo tileSpecSo)
        {
            var tableColumnSpace = TableColumnSpace * rectContainerGrids.x;
            var width = tableColumnSpace / tileSpecSo.TileSizeHeight;

            var tableLineSpace = TableLineSpace * rectContainerGrids.y;
            var height = tableLineSpace / tileSpecSo.TileSizeWidth;

            return new Vector2(width, height);
        }

        private static void DrawTableRectNumber(Rect position, bool isVertical = false)
        {
            var guiStyle = new GUIStyle(EditorStyles.textArea);

            guiStyle.padding = new RectOffset(8, 8, 4, 4);
            guiStyle.alignment = TextAnchor.MiddleCenter;

            var rect = isVertical ? new Rect(position.x, position.y, 39, 60) : new Rect(position.x, position.y, 74, 22);

            EditorGUI.SelectableLabel(rect, "1", guiStyle);
        }
    }
}