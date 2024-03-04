#if ENABLE_LEGACY_INPUT_MANAGER && !ENABLE_INPUT_SYSTEM
#define USE_LEGACY_INPUT_SYSTEM
using UnityEngine.EventSystems;
#endif
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace Inventory.Scripts.Utilities
{
    public class DetectInputSystemUtility : MonoBehaviour
    {
        private void Awake()
        {
#if ENABLE_INPUT_SYSTEM
            var newInputSystemGameObject = FindObjectOfType<InputSystemUIInputModule>(true);

            newInputSystemGameObject.gameObject.SetActive(true);
#endif

#if USE_LEGACY_INPUT_SYSTEM
            var legacyInputSystemGameObject = FindObjectOfType<StandaloneInputModule>(true);

            legacyInputSystemGameObject.gameObject.SetActive(true);
#endif
        }
    }
}