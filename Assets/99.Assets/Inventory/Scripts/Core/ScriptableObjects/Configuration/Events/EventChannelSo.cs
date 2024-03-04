using UnityEngine;

namespace Inventory.Scripts.Core.ScriptableObjects.Configuration.Events
{
    public abstract class EventChannelSo : ScriptableObject
    {
        [SerializeField, TextArea] private string description;
    }
}