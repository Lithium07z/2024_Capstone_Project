using Inventory.Editor.Helper;
using Inventory.Scripts.Core.ItemsMetadata;
using UnityEditor;
using UnityEngine;

namespace Inventory.Editor.Drawers.Metadata
{
    [CustomPropertyDrawer(typeof(InventoryMetadata))]
    public class InventoryMetadataPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // TODO: Implement this metadata
            EditorGUI.BeginProperty(position, label, property);

            var metadata = EditorPropertyHelper.GetValue<InventoryMetadata>(property, fieldInfo);

            EditorGUI.LabelField(position, nameof(InventoryMetadata), "The basic implementation");

            EditorGUI.EndProperty();
        }
    }
}