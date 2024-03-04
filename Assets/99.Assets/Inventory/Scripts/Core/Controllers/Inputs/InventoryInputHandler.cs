using UnityEngine;

namespace Inventory.Scripts.Core.Controllers.Inputs
{
    public class InventoryInputHandler : MonoBehaviour
    {
        [Header("Input Provider")]
        [SerializeField]
        private InputProviderSo inputProviderSo;

        private void Update()
        {
            inputProviderSo.Process();
        }
    }
}