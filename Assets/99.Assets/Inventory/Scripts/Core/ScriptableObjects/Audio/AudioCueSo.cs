using UnityEngine;

namespace Inventory.Scripts.Core.ScriptableObjects.Audio
{
    [CreateAssetMenu(fileName = "New AudioCue", menuName = "Inventory/Audio/Audio Cue")]
    public class AudioCueSo : ScriptableObject
    {
        [SerializeField] private AudioStateSo onBeingPlace;
        [SerializeField] private AudioStateSo onBeingPicked;

        public AudioStateSo OnBeingPlace => onBeingPlace;

        public AudioStateSo OnBeingPicked => onBeingPicked;
    }
}
