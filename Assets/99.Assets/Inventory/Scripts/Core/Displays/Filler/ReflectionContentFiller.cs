using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.Reflection;
using UnityEngine;

namespace Inventory.Scripts.Core.Displays.Filler
{
    public abstract class ReflectionContentFiller<T> : AbstractFiller
    {
        [Header("Property Configuration")] [SerializeField]
        private ReflectionProperty<T> itemProperty;

        [Header("Fallback")]
        [SerializeField, Tooltip("This object will be used when the " + nameof(itemProperty) + " is null or empty.")]
        private T fallbackObject;

        protected ReflectionProperty<T> ItemProperty => itemProperty;

        public T FallbackObject
        {
            get => fallbackObject;
            set => fallbackObject = value;
        }

        protected virtual T GetObjectValue(ItemTable itemTable)
        {
            var propertyValue = ItemProperty.GetPropertyValue(itemTable);

            return !(propertyValue is null || Equals(propertyValue, default))
                ? propertyValue
                : FallbackObject;
        }
    }
}