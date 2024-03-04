using UnityEngine;

namespace Inventory.Scripts.Core.ScriptableObjects.Audio
{
    [CreateAssetMenu(fileName = "New AudioState", menuName = "Inventory/Audio/Audio State")]
    public class AudioStateSo : ScriptableObject
    {
        private int _currentIndex;

        [SerializeField] private bool random;
        [SerializeField] private AudioSo[] audioSos;

        public AudioSo GetAudioSo()
        {
            _currentIndex = GenerateCurrentIndex();

            return (AudioSo) audioSos.GetValue(_currentIndex);
        }

        private int GenerateCurrentIndex()
        {
            if (random)
            {
                return Random.Range(0, audioSos.Length);
            }
            
            return _currentIndex > audioSos.Length ? 0 : _currentIndex++;
        }
    }
}