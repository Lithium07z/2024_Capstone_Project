using Inventory.Scripts.Core.Items;
using UnityEngine;

namespace Inventory.Scripts.Core.ScriptableObjects.Configuration.Events
{
    public enum WindowUpdateState
    {
        Open,
        Close
    }
    
    /**
     * This will trigger for both windows, Inspect and Container... If you want to for specific window,
     * you can add new parameter in the UnityAction. And validate in the InspectWindowManager or ContainerWindowManager.
     */
    [CreateAssetMenu(menuName = "Inventory/Configuration/Events/Window Container Change State Event ChannelSo")]
    public class WindowContainerChangeStateEventChannelSo : AbstractT2EventChannelSo<ItemTable, WindowUpdateState>
    {
    }
}
