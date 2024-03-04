using Inventory.Scripts.Core.Items;
using UnityEngine;

namespace Inventory.Scripts.Core.Displays.Filler
{
    public abstract class AbstractFiller : MonoBehaviour
    {
        /// <summary>
        /// Execute when a Display filler or Window is opened. This is used to set the texts, sprites and actions once is opened.
        /// </summary>
        /// <param name="itemTable">Item Table that was opened</param>
        public abstract void OnSet(ItemTable itemTable);

        /// <summary>
        /// Will execute once the Window or the Display filler is closed or removed.
        /// Normally the method implementation will be to set the values to empty or null again. 
        /// </summary>
        public abstract void OnReset();

        /// <summary>
        /// This method executes after the SetActive is called, normally will be used to refresh some UI to auto resize.
        /// </summary>
        public virtual void OnRefreshUI()
        {
        }
    }
}