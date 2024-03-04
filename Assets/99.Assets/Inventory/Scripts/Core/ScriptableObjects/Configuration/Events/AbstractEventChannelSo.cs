using UnityEngine.Events;

namespace Inventory.Scripts.Core.ScriptableObjects.Configuration.Events
{
    public class AbstractEventChannelSo<T> : EventChannelSo
    {
        public event UnityAction<T> OnEventRaised;

        public void RaiseEvent(T value)
        {
            OnEventRaised?.Invoke(value);
        }
    }
}