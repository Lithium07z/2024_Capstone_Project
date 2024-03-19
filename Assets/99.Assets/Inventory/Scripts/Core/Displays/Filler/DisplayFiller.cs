using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.Items.Grids;
using Inventory.Scripts.Core.Items.Grids.Helper;
using Inventory.Scripts.Core.ItemsMetadata;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.Scripts.Core.Displays.Filler
{
    public class DisplayFiller : MonoBehaviour
    {
        [Header("Grid Parent Configuration")]
        [SerializeField, Tooltip("A rect transform where the grid will be placed in")]
        private RectTransform gridParent;

        public AbstractGrid abstractGrid;

        public ItemTable CurrentInventoryItem { get; private set; }

        public RectTransform GridParent
        {
            get => gridParent;
            set => gridParent = value;
        }

        private RectTransform _rectTransform;
        private AbstractFiller[] _childrenFillers;
        private ContainerGrids _openedContainerGrids;


        protected virtual void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _childrenFillers = GetChildrenFillers();
        }

        public void Set(ItemTable itemTable)
        {
            CurrentInventoryItem = itemTable;

            foreach (var childrenFiller in GetChildrenFillers())
            {
                childrenFiller.OnSet(CurrentInventoryItem);
            }

            RectHelper.SetAnchorsForGrid(_rectTransform);
            transform.SetAsLastSibling();
            OnSetItem();
        }

        public void Reset()
        {
            gameObject.SetActive(false);
            CurrentInventoryItem = null;
            _openedContainerGrids = null;

            foreach (var childrenFiller in GetChildrenFillers())
            {
                childrenFiller.OnReset();
            }

            Sort();
            OnResetItem();
        }

        protected virtual void OnSetItem()
        {
        }

        protected virtual void OnResetItem()
        {
        }

        public void Sort()
        {
            transform.SetAsFirstSibling();
        }

        public void Open(ItemTable container)
        {
            if (container?.InventoryMetadata is not ContainerMetadata containerMetadata)
            {
                Reset();
                return;
            }

            _openedContainerGrids = containerMetadata.OpenInventory(gridParent);

            gameObject.SetActive(true);

            // TODO: 객체 태그로 찾는 방법 나중에 수정해야 할 수 있음
            if (_openedContainerGrids.CompareTag("Container"))
            {
                Transform itemGrid = _openedContainerGrids.transform.GetChild(0);
                abstractGrid = itemGrid.GetComponent<GridInteract2D>().GetAbstractGrid();
            }

            RefreshUI();
        }

        private void RefreshUI()
        {
            foreach (var childrenFiller in GetChildrenFillers())
            {
                childrenFiller.OnRefreshUI();
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);
        }

        public void Close(ItemTable container)
        {
            if (container?.InventoryMetadata is not ContainerMetadata containerMetadata)
            {
                Reset();
                return;
            }

            containerMetadata.CloseInventory(_openedContainerGrids);

            Reset();
        }

        private AbstractFiller[] GetChildrenFillers()
        {
            return _childrenFillers ??= GetComponentsInChildren<AbstractFiller>();
        }
    }
}