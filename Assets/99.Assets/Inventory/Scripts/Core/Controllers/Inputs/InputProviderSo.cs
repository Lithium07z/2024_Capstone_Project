using System;
using System.Collections.Generic;
using Inventory.Scripts.Core.Controllers.Inputs.Middleware;
using UnityEngine;

namespace Inventory.Scripts.Core.Controllers.Inputs
{
    [CreateAssetMenu(menuName = "Inventory/Configuration/Providers/Input Provider")]
    public class InputProviderSo : ScriptableObject, IInputProvider
    {
        [SerializeField] private List<InputMiddleware> middlewares;

        public event Action OnGenerateItem;
        public event Action OnPickupItem;
        public event Action OnReleaseItem;
        public event Action OnToggleOptions;
        public event Action OnRotateItem;

        private InputState _inputState;

        public InputState GetState()
        {
            return _inputState;
        }

        public void Process()
        {
            var inputState = new InputState();

            foreach (var inputMiddleware in middlewares)
            {
                inputMiddleware.Process(inputState);
            }

            _inputState = inputState;
        }

        private void OnEnable()
        {
            middlewares.ForEach(AddListeners);
        }

        private void OnDisable()
        {
            middlewares.ForEach(RemoveListeners);
        }

        private void AddListeners(InputMiddleware inputMiddleware)
        {
            inputMiddleware.OnGenerateItem += HandleGenerateItem;
            inputMiddleware.OnPickupItem += HandleOnPickupItemMiddleware;
            inputMiddleware.OnReleaseItem += HandleOnReleaseItemMiddleware;
            inputMiddleware.OnToggleOptions += HandleOnToggleOptionsMiddleware;
            inputMiddleware.OnRotateItem += HandleOnRotateItemMiddleware;
        }

        private void RemoveListeners(InputMiddleware inputMiddleware)
        {
            inputMiddleware.OnGenerateItem -= HandleGenerateItem;
            inputMiddleware.OnPickupItem -= HandleOnPickupItemMiddleware;
            inputMiddleware.OnReleaseItem -= HandleOnReleaseItemMiddleware;
            inputMiddleware.OnToggleOptions -= HandleOnToggleOptionsMiddleware;
            inputMiddleware.OnRotateItem -= HandleOnRotateItemMiddleware;
        }

        private void HandleGenerateItem()
        {
            OnGenerateItem?.Invoke();
        }

        private void HandleOnPickupItemMiddleware()
        {
            OnPickupItem?.Invoke();
        }

        private void HandleOnReleaseItemMiddleware()
        {
            OnReleaseItem?.Invoke();
        }

        private void HandleOnToggleOptionsMiddleware()
        {
            OnToggleOptions?.Invoke();
        }

        private void HandleOnRotateItemMiddleware()
        {
            OnRotateItem?.Invoke();
        }
    }
}