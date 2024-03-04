using Inventory.Scripts.Core.Items;
using TMPro;
using UnityEngine;

namespace Inventory.Scripts.Core.Displays.Filler
{
    [RequireComponent(typeof(TMP_Text))]
    public class TMPTextReflectionFiller : TextReflectionContentFiller
    {
        private TMP_Text _text;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        public override void OnSet(ItemTable itemTable)
        {
            var valueText = GetObjectValue(itemTable);

            _text.SetText(valueText);
        }

        public override void OnReset()
        {
            _text.SetText("");
        }
    }
}