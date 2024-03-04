using Inventory.Scripts.Core.Enums;
using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Events;
using Inventory.Scripts.Options;
using UnityEngine;

namespace Inventory.Scripts.Core.ScriptableObjects.Options
{
    [CreateAssetMenu(menuName = "Inventory/Options/New Option")]
    public class OptionSo : ScriptableObject
    {
        [Header("Option Event")] [SerializeField]
        private OnItemExecuteOptionEventChannelSo onItemExecuteOptionEventChannelSo;

        [Header("Option Metadata")] [SerializeField]
        private string displayName;

        [Header("Option Settings")] [SerializeField]
        private OptionsType optionsType;

        [SerializeField] private int order;

        private AbstractItem _currentInventoryItem;

        public string DisplayName
        {
            get => displayName;
            protected set => displayName = value;
        }

        public OptionsType OptionsType => optionsType;

        public AbstractItem CurrentInventoryItem => _currentInventoryItem;

        public int Order => order;

        public OnItemExecuteOptionEventChannelSo OnItemExecuteOptionEventChannelSo
        {
            get => onItemExecuteOptionEventChannelSo;
            protected set => onItemExecuteOptionEventChannelSo = value;
        }

        private void OnEnable()
        {
            if (onItemExecuteOptionEventChannelSo != null)
                onItemExecuteOptionEventChannelSo.OnEventRaised += Execute;

            OnEnableOption();
        }

        private void OnDisable()
        {
            if (onItemExecuteOptionEventChannelSo != null)
                onItemExecuteOptionEventChannelSo.OnEventRaised -= Execute;

            OnDisableOption();
        }

        protected virtual void OnEnableOption()
        {
        }

        protected virtual void OnDisableOption()
        {
        }

        private void Execute(AbstractItem item)
        {
            _currentInventoryItem = item;
            OnExecuteOption();
        }

        protected virtual void OnExecuteOption()
        {
        }
    }
}