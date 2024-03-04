using System;
using System.Collections.Generic;
using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.ScriptableObjects;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Events.Interact;
using Inventory.Scripts.Core.ScriptableObjects.Options;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inventory.Scripts.Core.Controllers
{
    [RequireComponent(typeof(RectTransform))]
    public class OptionsController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Configuration")] [SerializeField]
        private InventorySettingsAnchorSo inventorySettingsAnchorSo;

        [SerializeField,
         Tooltip("How much buttons will be once the game starts, so will not need to create at runtime.")]
        private int quantityPreCreated = 4;

        [Header("Broadcasting on...")] [SerializeField]
        private OnOptionInteractEventChannelSo onOptionInteractEventChannelSo;

        private readonly List<Button> _buttons = new();

        private void Awake()
        {
            InitButtons();
        }

        private void InitButtons()
        {
            for (var i = 0; i < quantityPreCreated; i++)
            {
                CreateNewButton();
            }
        }

        private Button CreateNewButton()
        {
            var buttonPrefab = GetButtonPrefab();

            var buttonInstantiate = Instantiate(buttonPrefab, transform);

            var buttonComponent = buttonInstantiate.GetComponent<Button>();

            _buttons.Add(buttonComponent);

            buttonComponent.onClick.RemoveAllListeners();
            buttonInstantiate.gameObject.SetActive(false);

            return buttonComponent;
        }

        public void SetOptions(AbstractItem inventoryItem, List<OptionSo> options)
        {
            if (options == null) return;

            DisableButtons();

            for (var i = 0; i < options.Count; i++)
            {
                var option = options[i];

                if (option == null) continue;

                var button = FindButtonByIndex(i);

                SetOptionProperties(option, button);
                AddListenerToButton(inventoryItem, button, option);
            }
        }

        private void AddListenerToButton(AbstractItem abstractItem, Button button, OptionSo option)
        {
            button.onClick.AddListener(() =>
            {
                option.OnItemExecuteOptionEventChannelSo.RaiseEvent(abstractItem);
                gameObject.SetActive(false);
            });

            button.gameObject.SetActive(true);
        }

        private void DisableButtons()
        {
            foreach (var button in _buttons)
            {
                button.onClick.RemoveAllListeners();
                button.gameObject.SetActive(false);
            }
        }

        private Button FindButtonByIndex(int index)
        {
            try
            {
                var button = _buttons[index];

                return button == null ? CreateNewButton() : button;
            }
            catch (ArgumentOutOfRangeException)
            {
                return CreateNewButton();
            }
        }

        private void SetOptionProperties(OptionSo option, Button button)
        {
            // TODO: Maybe use AbstractFiller to fill this properties.
            var tmpText = button.GetComponentInChildren<TMP_Text>();

            tmpText.text = option.DisplayName;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onOptionInteractEventChannelSo.RaiseEvent(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onOptionInteractEventChannelSo.RaiseEvent(null);
        }

        private Button GetButtonPrefab()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.ButtonPrefab;
        }
    }
}