using System;
using Inventory.Scripts.Core.Helper;
using UnityEngine;

namespace Inventory.Scripts.Core.Reflection
{
    [Serializable]
    public class ReflectionProperty<T>
    {
        [SerializeField] private string propertyPath;

        public string PropertyPath => propertyPath;

        // Editor properties. It shall not be deleted
        [HideInInspector] public bool isUsingRawInput;
        [HideInInspector] public int optionSelected;


        public ReflectionProperty(string propertyPath)
        {
            this.propertyPath = propertyPath;
        }

        public T GetPropertyValue(object obj)
        {
            if (obj == null) return default;

            var value = GetPropertyObjectValue(obj);

            return value is T o ? o : default;
        }

        private object GetPropertyObjectValue(object obj)
        {
            if (obj != null) return ReflectionHelper.GetValueFromProperty<object>(obj, propertyPath);

            Debug.LogError("Cannot get property from object. obj is null");
            return null;
        }
    }
}