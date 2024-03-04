using UnityEngine.Events;

namespace Inventory.Scripts.Core.ScriptableObjects.Configuration.Events
{
    public class VoidEventChannelSo : EventChannelSo
    {
        public event UnityAction OnEventRaised;

        public void RaiseEvent()
        {
            OnEventRaised?.Invoke();
        }
    }
}