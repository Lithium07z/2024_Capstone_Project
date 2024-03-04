using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Scripts.Core.Controllers.Draggable
{
    [CreateAssetMenu(menuName = "Inventory/Configuration/Providers/Draggable Provider")]
    public class DraggableProviderSo : ScriptableObject
    {
        [Header("Configuration")]
        [SerializeField,
         Tooltip(
             "If set to true, will show debug logs. Used for debug the draggable system or also to validate if your routines are working properly")]
        private bool debug;

        [Header("Processors")] [SerializeField]
        private List<PickupProcessor> pickupProcessors;

        [SerializeField] private List<ReleaseProcessor> placeMiddlewares;

        [SerializeField] private List<ReleaseProcessor> rollbackMiddlewares;

        public PickupState ProcessPickup(PickupContext pickupContext)
        {
            pickupContext.Debug = debug;

            var pickupState = new PickupState();

            ProcessPickupsProcessors(pickupContext, pickupState, pickupProcessors);

            return pickupState;
        }

        private static void ProcessPickupsProcessors(PickupContext pickupContext, PickupState pickupState,
            IEnumerable<PickupProcessor> pickupProcessors)
        {
            foreach (var pickupProcessor in pickupProcessors)
            {
                pickupProcessor.Process(pickupContext, pickupState);
            }
        }

        public void ProcessRelease(ReleaseContext releaseContext)
        {
            releaseContext.Debug = debug;

            var releaseState = new ReleaseState();

            ProcessMiddlewares(releaseContext, releaseState, placeMiddlewares);

            if (releaseState.Placed) return;

            ResolveInitialRotation(releaseContext);
            ProcessMiddlewares(releaseContext, releaseState, rollbackMiddlewares);
        }

        private static void ProcessMiddlewares(ReleaseContext releaseContext, ReleaseState releaseState,
            IEnumerable<ReleaseProcessor> releaseMiddlewares)
        {
            foreach (var releaseMiddleware in releaseMiddlewares)
            {
                releaseMiddleware.Process(releaseContext, releaseState);
            }
        }

        private static void ResolveInitialRotation(ReleaseContext releaseContext)
        {
            var contextPickupState = releaseContext.PickupState;
            var abstractItem = contextPickupState.Item;

            var initialItemRotation = contextPickupState.DraggableItemInitialRotation;

            if (!initialItemRotation.HasValue) return;

            if (initialItemRotation.Value != abstractItem.ItemTable.IsRotated)
            {
                abstractItem.ItemTable.Rotate();
            }
        }
    }
}