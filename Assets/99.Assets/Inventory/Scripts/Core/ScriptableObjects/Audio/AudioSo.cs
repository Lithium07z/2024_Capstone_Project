using UnityEngine;

namespace Inventory.Scripts.Core.ScriptableObjects.Audio
{
    [CreateAssetMenu(fileName = "New Audio", menuName = "Inventory/Audio/AudioSo")]
    public class AudioSo : ScriptableObject
    {
        [Range(0, 1f)]
        [SerializeField] private float volume = 1f;
        
        [Range(0, 1f)]
        [SerializeField] private float pitch = 1f;

        [SerializeField] private bool mute;

        [SerializeField] private AudioClip clip;

        public float Volume => volume;

        public float Pitch => pitch;

        public bool Mute => mute;

        public AudioClip Clip => clip;
    }
}
