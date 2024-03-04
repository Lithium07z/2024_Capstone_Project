using Inventory.Scripts.Core.Items;
using UnityEngine;

namespace Inventory.Scripts.Core.Displays.Filler
{
    public abstract class TextReflectionContentFiller : ReflectionContentFiller<string>
    {
        private const string DefaultPropertyReplacer = "${value}";

        [Header("Text Configuration")]
        [SerializeField, Tooltip("This is the text which will be replaced in text.")]
        private string propertyReplacer = DefaultPropertyReplacer;

        [SerializeField,
         Tooltip("You can customize your text with this text template. You can use the " + nameof(propertyReplacer) +
                 " to replace with the property value."),
         TextArea]
        private string textTemplate = DefaultPropertyReplacer;

        public string PropertyReplacer => propertyReplacer;

        public string TextTemplate
        {
            get => textTemplate;
            set => textTemplate = value;
        }

        protected override string GetObjectValue(ItemTable itemTable)
        {
            var propertyValue = ItemProperty.GetPropertyValue(itemTable);

            return string.IsNullOrWhiteSpace(propertyValue)
                ? FallbackObject
                : TextTemplate.Replace(PropertyReplacer, propertyValue);
        }
    }
}