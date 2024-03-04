using Inventory.Scripts.Core.ScriptableObjects.Configuration.Anchors;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Events.Interact;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Inventory.Scripts.Core.Items.Grids
{
    [RequireComponent(typeof(AbstractGrid))]
    public class GridInteract2D : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private OnGridInteractEventChannelSo onGridInteractEventChannelSo;
        [SerializeField] private AbstractGridSelectedAnchorSo abstractGridSelectedAnchorSo;

        private AbstractGrid _abstractGrid;

        private void Awake()
        {
            _abstractGrid = GetComponent<AbstractGrid>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            abstractGridSelectedAnchorSo.Value = _abstractGrid;
            onGridInteractEventChannelSo.RaiseEvent(_abstractGrid);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            abstractGridSelectedAnchorSo.Value = null;
            onGridInteractEventChannelSo.RaiseEvent(null);
        }

        public AbstractGrid GetAbstractGrid()
        {
            return _abstractGrid;
        }
    }
}