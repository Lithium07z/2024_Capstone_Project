using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.Items.Grids.Helper;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.Scripts.Core.Displays.Filler
{
    [RequireComponent(typeof(RectTransform))]
    public class RefreshLayoutFiller : AbstractFiller
    {
        [SerializeField] private bool refreshOnSet;
        [SerializeField] private bool refreshOnReset;
        [SerializeField] private bool refreshOnRefresh = true;

        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public override void OnSet(ItemTable itemTable)
        {
            RectHelper.SetAnchorsForGrid(_rectTransform);

            RefreshIfNeeded(refreshOnSet);
        }

        public override void OnReset()
        {
            RefreshIfNeeded(refreshOnReset);
        }

        public override void OnRefreshUI()
        {
            base.OnRefreshUI();

            RefreshIfNeeded(refreshOnRefresh);
        }

        private void RefreshIfNeeded(bool shouldRefresh)
        {
            if (shouldRefresh)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);
            }
        }
    }
}