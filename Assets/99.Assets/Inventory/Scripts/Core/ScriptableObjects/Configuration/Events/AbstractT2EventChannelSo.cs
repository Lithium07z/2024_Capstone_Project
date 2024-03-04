using UnityEngine.Events;

namespace Inventory.Scripts.Core.ScriptableObjects.Configuration.Events
{
    public class AbstractT2EventChannelSo<T, K> : EventChannelSo
    {
        public event UnityAction<T, K> OnEventRaised;

        public void RaiseEvent(T firstValue, K secondValue)
        {
            OnEventRaised?.Invoke(firstValue, secondValue);
        }
    }
}