using Inventory.Scripts.Core.Displays.Filler;
using Inventory.Scripts.Core.Items;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Inventory.Scripts.Core.Windows
{
    public abstract class Window : MonoBehaviour, IPointerDownHandler
    {
        public RectTransform RectTransform { get; private set; }

        public ItemTable CurrentItemTable { get; private set; }

        private AbstractFiller[] _childrenFillers;

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            _childrenFillers = GetComponentsInChildren<AbstractFiller>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            RectTransform.SetAsLastSibling();
        }

        public void SetItemTable(ItemTable itemTable)
        {
            CurrentItemTable = itemTable;

            foreach (var childrenFiller in _childrenFillers)
            {
                childrenFiller.OnSet(CurrentItemTable);
            }

            OnOpenWindow();

            foreach (var childrenFiller in _childrenFillers)
            {
                childrenFiller.OnRefreshUI();
            }
        }

        public void CloseWindow()
        {
            gameObject.SetActive(false);

            foreach (var childrenFiller in _childrenFillers)
            {
                childrenFiller.OnReset();
            }

            OnCloseWindow();
            CurrentItemTable = null;
        }

        protected abstract void OnOpenWindow();

        protected virtual void OnCloseWindow()
        {
        }
    }
}