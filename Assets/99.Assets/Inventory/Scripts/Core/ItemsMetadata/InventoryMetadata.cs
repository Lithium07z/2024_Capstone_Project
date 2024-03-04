using System;
using System.Collections.Generic;
using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.ScriptableObjects.Options;

namespace Inventory.Scripts.Core.ItemsMetadata
{
    [Serializable]
    public class InventoryMetadata
    {
        public List<OptionSo> OptionsMetadata { get; } = new();

        public ItemTable ItemTable { get; }

        public InventoryMetadata(ItemTable itemTable)
        {
            ItemTable = itemTable;
            InitializeOptions();
        }

        public virtual void OnPlaceItem(ItemTable item)
        {
        }

        private void InitializeOptions()
        {
            if (ItemTable == null) return;

            foreach (var optionSo in ItemTable.ItemDataSo.GetOptionsOrdered())
            {
                OptionsMetadata.Add(optionSo);
            }
        }
    }
}