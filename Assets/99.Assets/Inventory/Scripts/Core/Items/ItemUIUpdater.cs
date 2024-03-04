using System;
using UnityEngine;

namespace Inventory.Scripts.Core.Items
{
    public class ItemUIUpdater
    {
        public Tuple<bool, Color?> Background { get; }

        public ItemUIUpdater(Tuple<bool, Color?> background)
        {
            Background = background;
        }
    }
}