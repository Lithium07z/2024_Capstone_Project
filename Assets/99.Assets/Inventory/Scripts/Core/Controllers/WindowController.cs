using System.Linq;
using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.ItemsMetadata;
using Inventory.Scripts.Core.ScriptableObjects;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Anchors;
using Inventory.Scripts.Core.Windows;
using Inventory.Scripts.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.Scripts.Core.Controllers
{
    public abstract class WindowController : MonoBehaviour
    {
        [Header("Inventory Settings Anchor Settings")] [SerializeField]
        protected InventorySettingsAnchorSo inventorySettingsAnchorSo;

        [SerializeField] private RectTransformPrefabAnchorSo parentRectTransform;

        private int _currentIndex;
        private Window[] _windows;

        private void Start()
        {
            var maxWindowOpen = GetMaxWindowOpen();

            _windows = new Window[maxWindowOpen];

            for (var i = 0; i < maxWindowOpen; i++)
            {
                var windowInstantiated = Instantiate(GetPrefabWindow(), parentRectTransform.Value);

                _windows[i] = windowInstantiated.GetComponent<Window>();

                windowInstantiated.gameObject.SetActive(false);
            }
        }

        protected virtual Window GetPrefabWindow()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.BasicPrefabWindow;
        }

        protected virtual int GetMaxWindowOpen()
        {
            return 4;
        }

        protected Window[] GetAllWindows()
        {
            return _windows;
        }

        protected Window GetNextWindowInThePool()
        {
            var indexToNewWindow = GetIndexToNewWindow();

            return _windows[indexToNewWindow];
        }

        private int GetIndexToNewWindow()
        {
            int currentIndex;

            for (var i = 0; i < _windows.Length; i++)
            {
                var window = _windows[i];

                if (!window.gameObject.activeSelf) return i;
            }

            if (_currentIndex < GetMaxWindowOpen() - 1)
            {
                currentIndex = _currentIndex;

                _currentIndex++;

                return currentIndex;
            }

            currentIndex = _currentIndex;

            _currentIndex = 0;

            return currentIndex;
        }

        protected static void SetWindowInMiddle(Window window)
        {
            if (window == null) return;

            var rectTransform = window.GetComponent<RectTransform>();

            var rectTransformAnchoredPosition = new Vector2(0.5f, 0.5f);

            rectTransform.anchoredPosition = rectTransformAnchoredPosition;
            rectTransform.anchorMax = rectTransformAnchoredPosition;
            rectTransform.anchorMin = rectTransformAnchoredPosition;
            rectTransform.pivot = rectTransformAnchoredPosition;

            rectTransform.SetAsLastSibling();
        }

        protected virtual void OpenWindow(ItemTable itemTable)
        {
            var nextWindowInThePool = GetNextWindowInThePool();

            var allWindows = GetAllWindows();

            if (allWindows == null) return;

            if (allWindows.Any(window => window.gameObject.activeSelf &&
                                         window.CurrentItemTable == itemTable))
            {
                return;
            }

            ClosePreviousInventory(nextWindowInThePool);
            nextWindowInThePool.SetItemTable(itemTable);
            SetWindowInMiddle(nextWindowInThePool);
            nextWindowInThePool.gameObject.SetActive(true);

            RefreshUI(nextWindowInThePool.RectTransform);
        }

        private static void ClosePreviousInventory(Window nextWindowInThePool)
        {
            var previousItemTable = nextWindowInThePool.CurrentItemTable;

            if (previousItemTable == null) return;

            if (nextWindowInThePool is not ContainerWindow containerWindow) return;
            
            if (!previousItemTable.IsContainer()) return;

            var containerMetadata = (ContainerMetadata)previousItemTable.InventoryMetadata;

            containerMetadata.CloseInventory(containerWindow.GetOpenedGrid());
        }

        protected virtual void CloseWindow(ItemTable itemTable)
        {
            if (itemTable == null) return;

            var allWindows = GetAllWindows();

            foreach (var window in allWindows)
            {
                if (window == null) continue;

                if (window.CurrentItemTable == itemTable)
                {
                    window.CloseWindow();
                }

                RefreshUI(window.RectTransform);
            }
        }

        private void RefreshUI(RectTransform transformRef)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(transformRef);
        }

        public void SetInventorySettingsAnchorSo(InventorySettingsAnchorSo inventorySettingsAnchor)
        {
            inventorySettingsAnchorSo = inventorySettingsAnchor;
        }

        public void SetWindowParent(RectTransformPrefabAnchorSo windowParent)
        {
            parentRectTransform = windowParent;
        }
    }
}