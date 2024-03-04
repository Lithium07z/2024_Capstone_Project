using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace Inventory.Editor.Helper
{
    public static class EditorPropertyHelper
    {
        public static T GetValue<T>(SerializedProperty property, FieldInfo fieldInfo)
        {
            try
            {
                if (fieldInfo.GetValue(property.serializedObject.targetObject) is T value)
                {
                    return value;
                }

                if (fieldInfo.GetValue(property.serializedObject.targetObject) is List<T> list)
                {
                    return list.FirstOrDefault(item => item.Equals(property.managedReferenceValue));
                }
            }
            catch
            {
                var itemTableProperty = GetTargetObjectOfProperty<T>(property);

                return itemTableProperty;
            }

            return default;
        }

        public static T GetTargetObjectOfProperty<T>(SerializedProperty prop)
        {
            if (prop == null) return default;

            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    var elementName = element[..element.IndexOf("[", StringComparison.Ordinal)];
                    var index = Convert.ToInt32(element[element.IndexOf("[", StringComparison.Ordinal)..]
                        .Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }

            return obj is T o ? o : default;
        }

        private static object GetValue_Imp(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();

            while (type != null)
            {
                var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                    return f.GetValue(source);

                var p = type.GetProperty(name,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null)
                    return p.GetValue(source, null);

                type = type.BaseType;
            }

            return null;
        }

        private static object GetValue_Imp(object source, string name, int index)
        {
            if (GetValue_Imp(source, name) is not IEnumerable enumerable) return null;

            var enm = enumerable.GetEnumerator();

            for (var i = 0; i <= index; i++)
            {
                if (!enm.MoveNext()) return null;
            }

            return enm.Current;
        }

        public static SerializedProperty FindRelativeAutoProperty(SerializedProperty property, string fieldName)
        {
            return property.FindPropertyRelative(ParseAutoPropertyName(fieldName));
        }

        public static string ParseAutoPropertyName(string propertyName)
        {
            return $"<{propertyName}>k__BackingField";
        }

        public static object GetObjectPropertyFromProperty(SerializedProperty property)
        {
            var path = property.propertyPath.Replace(".Array.data[", "[");
            var elements = path.Split('.');

            return elements.Select(element => GetValue_Imp(property.serializedObject.targetObject, element))
                .FirstOrDefault();
        }
    }
}