using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Inventory.Scripts.Helper
{
    [RequireComponent(typeof(TMP_Text))]
    public class OpenHyperlink : MonoBehaviour, IPointerClickHandler
    {
        private TMP_Text _mTextMeshPro;

        private void Start()
        {
            _mTextMeshPro = GetComponent<TMP_Text>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var linkIndex = TMP_TextUtilities.FindIntersectingLink(_mTextMeshPro, Input.mousePosition, null);

            if (linkIndex == -1) return;

            var linkInfo = _mTextMeshPro.textInfo.linkInfo[linkIndex];
            Application.OpenURL(linkInfo.GetLinkID());
        }
    }
}