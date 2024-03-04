using Inventory.Scripts.Core.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.Scripts.Core.Displays.Filler
{
    [RequireComponent(typeof(Image))]
    public class SpriteFiller : ReflectionContentFiller<Sprite>
    {
        private Image _image;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        public override void OnSet(ItemTable itemTable)
        {
            var propertyValue = GetObjectValue(itemTable);

            _image.sprite = propertyValue;
        }

        public override void OnReset()
        {
            _image.sprite = default;
        }
    }
}