using System;
using Inventory.Editor.Helper;
using Inventory.Scripts.Core.Helper;
using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.Reflection;
using UnityEditor;
using UnityEngine;

namespace Inventory.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ReflectionProperty<>), true)]
    public class ReflectionPropertyPropertyDrawer : PropertyDrawer
    {
        private const string PropertyPathFieldName = "propertyPath";
        private const string UsingRawInputFieldName = nameof(ReflectionProperty<object>.isUsingRawInput);
        private const string OptionSelectedFieldName = nameof(ReflectionProperty<object>.optionSelected);

        private const float ToggleWidth = 20f;

        private bool _useRawInputText;
        private string[] _options;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var inputOptionRect = new Rect(position.x, position.y, position.width - ToggleWidth, position.height);
            var toggleRect = new Rect(position.x + 5f + inputOptionRect.width, position.y, ToggleWidth,
                position.height);

            var rawPropertyPath = property.FindPropertyRelative(PropertyPathFieldName);
            var usingRawInputProperty = property.FindPropertyRelative(UsingRawInputFieldName);
            var optionSelectedProperty = property.FindPropertyRelative(OptionSelectedFieldName);

            var reflectionPropertyGenericType = GetReflectionPropertyGenericType(property);
            
            // TODO: Create a label field to show the type of reflection property
            
            EditorGUI.BeginChangeCheck();

            var usingRawInput = usingRawInputProperty.boolValue;

            _useRawInputText = CreateToggleRawInput(usingRawInput, toggleRect);

            if (usingRawInput)
            {
                EditorGUI.PropertyField(inputOptionRect, rawPropertyPath, GUIContent.none);
            }
            else
            {
                optionSelectedProperty.intValue = DrawPropertiesSelectInput(
                    reflectionPropertyGenericType,
                    optionSelectedProperty.intValue,
                    inputOptionRect
                );

                rawPropertyPath.stringValue = _options[optionSelectedProperty.intValue];
            }

            if (EditorGUI.EndChangeCheck())
            {
                usingRawInputProperty.boolValue = _useRawInputText;
            }

            EditorGUI.EndProperty();
        }

        private static bool CreateToggleRawInput(bool usingRawInput, Rect toggleRect)
        {
            var guiContent = new GUIContent("", "Toggle the raw input property path.");
            EditorGUI.LabelField(toggleRect, guiContent);

            return EditorGUI.Toggle(
                toggleRect,
                guiContent,
                usingRawInput
            );
        }

        private int DrawPropertiesSelectInput(Type reflectionPropertyGenericType, int optionSelectedIndex,
            Rect inputOptionRect)
        {
            _options = ReflectionHelper.GetAllPropertiesByType(typeof(ItemTable), reflectionPropertyGenericType)
                .ToArray();

            // Draw the select option menu
            return EditorGUI.Popup(inputOptionRect, optionSelectedIndex, _options);
        }

        private static Type GetReflectionPropertyGenericType(SerializedProperty property)
        {
            try
            {
                var objectProperty = EditorPropertyHelper.GetObjectPropertyFromProperty(property);

                return ReflectionHelper.GetGenericTypeFromReference(objectProperty);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}