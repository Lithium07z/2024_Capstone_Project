using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.ScriptableObjects.Items.Dimensions;
using TMPro;
using UnityEngine;

namespace Inventory.Scripts.Core.Displays.Filler
{
    [RequireComponent(typeof(TMP_Text))]
    public class DimensionTextReflectionFiller : ReflectionContentFiller<DimensionsSo>
    {
        private TMP_Text _text;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        public override void OnSet(ItemTable itemTable)
        {
            var dimensionsSo = GetObjectValue(itemTable);

            var valueText = $"{dimensionsSo.Width}x{dimensionsSo.Height}";

            _text.SetText(valueText);
        }

        public override void OnReset()
        {
            _text.SetText("");
        }
    }
}