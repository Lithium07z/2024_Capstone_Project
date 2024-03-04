using UnityEditor;
using UnityEngine;

namespace Inventory.Editor.Helper
{
    public static class GUIRushHelper
    {
        public static Rect NextLine(this Rect position)
        {
            position.y += EditorGUIUtility.singleLineHeight;

            return position;
        }
    }
}