using Inventory.Scripts.Core.Enums;
using Inventory.Scripts.Core.Items;
using UnityEngine;

namespace Inventory.Scripts.Core.ScriptableObjects.Configuration.Events
{
    [CreateAssetMenu(menuName = "Inventory/Configuration/Events/On Equip Item Table Event ChannelSo")]
    public class OnEquipItemTableInHolderEventChannelSo : AbstractT2EventChannelSo<ItemTable, HolderInteraction>
    {
    }
}