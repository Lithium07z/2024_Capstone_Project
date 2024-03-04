using System;
using Inventory.Scripts.Core.Enums;
using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.Items.Grids;
using Inventory.Scripts.Core.ScriptableObjects.Items;
using Inventory.Scripts.Core.ScriptableObjects.Items.Dimensions;
using Inventory.Scripts.Core.ScriptableObjects.Options;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Inventory.Tests.EditMode
{
    public class GridTableTests
    {
        private GridTable _gridTable;

        [SetUp]
        public void SetUp()
        {
            _gridTable = new GridTable(5, 5);
        }

        [Test]
        public void Place2X2ItemInTheGrid_ShouldInsert()
        {
            var inventoryItem = GetInventoryItem();

            inventoryItem.Set(CreateItemDataSo());

            var firstInsert = _gridTable.PlaceItem(inventoryItem.ItemTable, 0, 0);

            Assert.AreEqual(GridResponse.Inserted, firstInsert);
            Assert.AreEqual(0, inventoryItem.ItemTable.OnGridPositionX);
            Assert.AreEqual(0, inventoryItem.ItemTable.OnGridPositionY);
        }

        [Test]
        public void PlaceMultipleItems_AllShouldBeInserted()
        {
            var inventoryItem = GetInventoryItem();

            inventoryItem.Set(CreateItemDataSo());

            var firstInsert = _gridTable.PlaceItem(inventoryItem.ItemTable, 0, 0);
            var shouldInsert = _gridTable.PlaceItem(inventoryItem.ItemTable, 2, 0);

            Assert.AreEqual(GridResponse.Inserted, firstInsert);
            Assert.AreEqual(GridResponse.Inserted, shouldInsert);
        }

        [Test]
        public void PlaceItem2X2_ItemShouldBeInsertInTheBottomEdge()
        {
            var inventoryItem = GetInventoryItem();

            inventoryItem.Set(CreateItemDataSo());

            var firstInsert = _gridTable.PlaceItem(inventoryItem.ItemTable, 3, 3);

            Assert.AreEqual(GridResponse.Inserted, firstInsert);
        }

        [Test]
        public void PlaceMultipleItemsAndOneShouldOverlapWhenHoverAnother_Overlapping()
        {
            var inventoryItem = GetInventoryItem();

            inventoryItem.Set(CreateItemDataSo());

            var firstInsert = _gridTable.PlaceItem(inventoryItem.ItemTable, 0, 0);
            var shouldBeOverlapping = _gridTable.PlaceItem(inventoryItem.ItemTable, 1, 1);

            Assert.AreEqual(GridResponse.Inserted, firstInsert);
            Assert.AreEqual(GridResponse.Overlapping, shouldBeOverlapping);
        }

        [Test]
        public void ShouldNotPlaceItemWhenOutOfBounds_OutOfBounds()
        {
            var inventoryItem = GetInventoryItem();

            inventoryItem.Set(CreateItemDataSo());

            var shouldBeOutOfBounds = _gridTable.PlaceItem(inventoryItem.ItemTable, 5, 5);

            Assert.AreEqual(GridResponse.OutOfBounds, shouldBeOutOfBounds);
        }

        [Test]
        public void ShouldPickUpTheItem_PickItemIfIsTheSamePos()
        {
            var inventoryItem = GetInventoryItem();

            inventoryItem.Set(CreateItemDataSo());

            var inserted = _gridTable.PlaceItem(inventoryItem.ItemTable, 1, 1);

            Assert.AreEqual(GridResponse.Inserted, inserted);

            var pickedInventoryItem = _gridTable.PickUpItem(1, 1);

            Assert.NotNull(pickedInventoryItem);
            Assert.AreEqual(1, pickedInventoryItem.OnGridPositionX);
            Assert.AreEqual(1, pickedInventoryItem.OnGridPositionY);
        }

        [Test]
        public void ShouldPickUpTheItem_PickItemIfIsInsideTheTheItemRadius()
        {
            var inventoryItem = GetInventoryItem();

            inventoryItem.Set(CreateItemDataSo());

            var inserted = _gridTable.PlaceItem(inventoryItem.ItemTable, 1, 1);

            Assert.AreEqual(GridResponse.Inserted, inserted);

            var pickedInventoryItem = _gridTable.PickUpItem(2, 2);

            Assert.NotNull(pickedInventoryItem);
            Assert.AreEqual(1, pickedInventoryItem.OnGridPositionX);
            Assert.AreEqual(1, pickedInventoryItem.OnGridPositionY);
        }

        [Test]
        public void ShouldPickUpTheItem_PickItemIfIsInsideTheTheItemRadiusDiffX_And_Y()
        {
            var inventoryItem = GetInventoryItem();

            inventoryItem.Set(CreateItemDataSo());

            var inserted = _gridTable.PlaceItem(inventoryItem.ItemTable, 1, 1);

            Assert.AreEqual(GridResponse.Inserted, inserted);

            var pickedInventoryItem = _gridTable.PickUpItem(1, 2);

            Assert.NotNull(pickedInventoryItem);
            Assert.AreEqual(1, pickedInventoryItem.OnGridPositionX);
            Assert.AreEqual(1, pickedInventoryItem.OnGridPositionY);
        }

        [Test]
        public void ShouldPickUpTheItem_PickItemIfIsInsideTheTheItemRadiusDiffY_And_X()
        {
            var inventoryItem = GetInventoryItem();

            inventoryItem.Set(CreateItemDataSo());

            var inserted = _gridTable.PlaceItem(inventoryItem.ItemTable, 1, 1);

            Assert.AreEqual(GridResponse.Inserted, inserted);

            var pickedInventoryItem = _gridTable.PickUpItem(2, 1);

            Assert.NotNull(pickedInventoryItem);
            Assert.AreEqual(1, pickedInventoryItem.OnGridPositionX);
            Assert.AreEqual(1, pickedInventoryItem.OnGridPositionY);
        }

        [Test]
        public void ShouldNotPickupItem_IfNotFoundWithX_And_Y()
        {
            var inventoryItem = GetInventoryItem();

            inventoryItem.Set(CreateItemDataSo());

            var inserted = _gridTable.PlaceItem(inventoryItem.ItemTable, 1, 1);

            Assert.AreEqual(GridResponse.Inserted, inserted);

            var pickedInventoryItem = _gridTable.PickUpItem(3, 2);

            Assert.IsNull(pickedInventoryItem);
        }

        [Test]
        public void ShouldReturnNullWhenGettingItemOutOfBounds_NullInOutOfBounds()
        {
            var inventoryItem = GetInventoryItem();

            inventoryItem.Set(CreateItemDataSo());

            var inserted = _gridTable.PlaceItem(inventoryItem.ItemTable, 1, 1);

            Assert.AreEqual(GridResponse.Inserted, inserted);

            var pickedInventoryItem = _gridTable.PickUpItem(5, 5);

            Assert.IsNull(pickedInventoryItem);
        }

        private static ItemInventory2D GetInventoryItem()
        {
            var guidsForInventoryItem = AssetDatabase.FindAssets("ItemIcon");

            var prefabInventoryItem =
                (GameObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guidsForInventoryItem[0]),
                    typeof(GameObject));

            var itemGameObject = PrefabUtility.InstantiatePrefab(prefabInventoryItem) as GameObject;

            if (itemGameObject == null)
            {
                return null;
            }

            var inventoryItem = itemGameObject.GetComponent<ItemInventory2D>();

            return inventoryItem;
        }

        private static ItemDataSo CreateItemDataSo()
        {
            var itemDataSo = ScriptableObject.CreateInstance<ItemDataSo>();
            var dimensionsSo = ScriptableObject.CreateInstance<DimensionsSo>();

            dimensionsSo.Width = 2;
            dimensionsSo.Height = 2;

            itemDataSo.DimensionsSo = dimensionsSo;
            itemDataSo.OptionsSo = Array.Empty<OptionSo>();

            return itemDataSo;
        }
    }
}