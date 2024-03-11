using System.Collections.Generic;
using Inventory.Scripts.Core.ScriptableObjects.Items;
using Photon.Pun;
using UnityEngine;

namespace Inventory.Scripts.Core.ScriptableObjects.Datastores
{
    [CreateAssetMenu(menuName = "Inventory/Datastore/New Items Datastore")]
    public class DatastoreItems : ScriptableObject
    {
        [Header("Items DataSource")] [SerializeField]
        public List<ItemDataSo> items = new();

        private void OnEnable()
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].itemID = i;
            }
        }

        public ItemDataSo GetRandomItem()
        {
            return items[GetRandomItemID()];
        }

        public List<ItemDataSo> GetRandomItems(int quantityToGet)
        {
            var randomItems = new List<ItemDataSo>();

            for (var i = 0; i < quantityToGet; i++)
            {
                randomItems.Add(GetRandomItem());
            }

            return randomItems;
        }

        public void Import(IEnumerable<ItemDataSo> itemDataSos, bool clearBeforeImport = true)
        {
            if (clearBeforeImport)
            {
                items.Clear();
                items.AddRange(itemDataSos);
                return;
            }

            items.AddRange(itemDataSos);
        }

        public int GetRandomItemID()
        {
            var selectedItemID = Random.Range(0, items.Count);

            return selectedItemID;
        }

        public ItemDataSo GetItemFromID(int selectedItemID)
        {
            return items[selectedItemID];
        }
    }
}