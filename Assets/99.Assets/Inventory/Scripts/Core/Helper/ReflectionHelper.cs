using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Scripts.Core.Helper
{
    public static class ReflectionHelper
    {
        public static T GetValueFromProperty<T>(object objToGetProperty, string propertyOnItemTable)
        {
            if (objToGetProperty == null)
            {
                Debug.LogError("Object argument is null");
                return default;
            }

            if (string.IsNullOrEmpty(propertyOnItemTable))
            {
                Debug.LogError("Property path cannot be null or empty " + nameof(propertyOnItemTable));
                return default;
            }

            var properties = propertyOnItemTable.Split('.');

            var currentObject = objToGetProperty;

            foreach (var property in properties)
            {
                var propertyInfo = currentObject?.GetType().GetProperty(property);

                // if (propertyInfo == null)
                // {
                //     Debug.LogError(
                //         $"Property '{property}' not found in type '{currentObject}'. Check the {propertyOnItemTable} on your filler component");
                // }

                currentObject = propertyInfo?.GetValue(currentObject);
            }

            return currentObject is T o ? o : default;
        }

        public static List<string> GetAllPropertiesByObject(object obj, string parent = "")
        {
            var result = new List<string>();

            if (obj == null)
            {
                return result;
            }

            var type = obj.GetType();
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var propertyPath = string.IsNullOrEmpty(parent) ? propertyName : $"{parent}.{propertyName}";
                result.Add(propertyPath);

                // TODO: Fix this if some get method throw null reference
                var propertyValue = property.GetValue(obj);

                if (propertyValue == null || property.PropertyType.Assembly != type.Assembly) continue;

                var subProperties = GetAllPropertiesByObject(propertyValue, propertyPath);

                result.AddRange(subProperties);
            }

            return result;
        }

        public static List<string> GetAllPropertiesByType(Type type, Type filterType = null)
        {
            return GetAllPropertiesByType(type, new HashSet<Type>(), filterType: filterType);
        }

        private static List<string> GetAllPropertiesByType(Type type, ISet<Type> visitedTypes, string parent = "",
            Type filterType = null)
        {
            var result = new List<string>();

            if (type == null || visitedTypes.Contains(type))
            {
                return result;
            }

            visitedTypes.Add(type);

            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var propertyType = property.PropertyType;

                var propertyPath = string.IsNullOrEmpty(parent) ? propertyName : $"{parent}.{propertyName}";

                if (filterType == null || propertyType == filterType)
                {
                    result.Add(propertyPath);
                }

                if (propertyType.Assembly != type.Assembly || propertyType.IsValueType ||
                    propertyType == typeof(string)) continue;

                var subProperties = GetAllPropertiesByType(propertyType, visitedTypes, propertyPath, filterType);
                result.AddRange(subProperties);
            }

            return result;
        }

        public static Type GetGenericTypeFromReference(object obj)
        {
            var genericArguments = obj.GetType().GetGenericArguments();

            // The first (and in this case, the only) generic argument
            return genericArguments[0];
        }
    }
}