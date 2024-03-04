using Inventory.Scripts.Core.ScriptableObjects.Audio;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Events;
using UnityEngine;

namespace Inventory.Scripts.Core.ScriptableObjects.Options
{
    [CreateAssetMenu(menuName = "Inventory/Options/New Audio Option")]
    public class AudioOptionSo : OptionSo
    {
        [Header("Audio Settings")]
        [SerializeField] private OnAudioStateEventChannelSo onAudioStateEventChannelSo;

        [SerializeField] private AudioStateSo audioStateSo;

        protected override void OnExecuteOption()
        {
            base.OnExecuteOption();
            
            onAudioStateEventChannelSo.RaiseEvent(audioStateSo);
        }
    }
}
