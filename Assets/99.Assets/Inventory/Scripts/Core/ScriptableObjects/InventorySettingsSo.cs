using Inventory.Scripts.Core.Controllers;
using Inventory.Scripts.Core.Displays.Filler;
using Inventory.Scripts.Core.ScriptableObjects.Configuration;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Tile;
using Inventory.Scripts.Windows;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Inventory.Scripts.Core.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Inventory/New Inventory Settings So")]
    public class InventorySettingsSo : ScriptableObject
    {
        [Header("Tile Settings")] [SerializeField] [Tooltip("The tile specification used calculate the size of tile.")]
        private TileSpecSo tileSpecSo;

        [Tooltip(
            "This TileGetGridPositionSo we use to keep the logic to calculate the grid position on the player click. You can implement your own, but inherit from it.")]
        [SerializeField]
        private TileGridHelperSo tileGridHelperSo;

        [Header("Grid Settings")] [SerializeField] [Tooltip("Will be the border of the grid.")]
        private Sprite borderGridSprite;

        [SerializeField] [Tooltip("Will be the slot icon from the grid tiles")]
        private Sprite slotSprite;

        [Header("Inventory Settings")]
        [SerializeField]
        [Tooltip("If false, doesn't need to hold mouse button in order to drag the item.")]
        private bool holdToDrag = true;

        [SerializeField] [Tooltip("If false, will not appear an item representation on last position when dragging...")]
        private bool showDragRepresentation = true;

        [SerializeField, Tooltip("Inventory So that will hold all player grids.")]
        private InventorySo playerInventorySo;

        [SerializeField, Tooltip("The prefab used to instantiate the item in the grid or in item holder.")]
        private ItemPrefabSo itemPrefabSo;

        [Header("Auto Scroll Settings")]
        [SerializeField]
        [Tooltip("How close the item should be from corners of the ScrollArea in order to auto scroll in a direction.")]
        [Range(45f, 200f)]
        private float holdScrollPadding = 85;

        [SerializeField] [Tooltip("The speed of the scroll when is auto scrolling in a direction.")] [Range(1f, 18f)]
        private float holdScrollRate = 4.25f;

        [Header("Highlight Colors")] [SerializeField] [Tooltip("Show when the cursor is on top of a item in the grid.")]
        private Color onHoverItem;

        [SerializeField] [Tooltip("Show when a item is over another item in the grid. Cannot be inserted.")]
        private Color onHoverItemOverlapError;

        [SerializeField] [Tooltip("Show when the item can be inserted in the place.")]
        private Color onHoverEmptyCell;

        [SerializeField] [Tooltip("Show when a item is over a free space container.")]
        private Color onHoverContainerThatCanBeInserted;

        [Header("Highlight Item Holder")]
        [SerializeField]
        [Tooltip("Item Holder will be highlighted when a correct item holder is being drag.")]
        private bool enableHighlightHolder = true;

        [SerializeField] [Tooltip("Color will be the borders of the Item Holder")]
        private Color colorHighlightHolder;

        [Header("Highlight Player Settings")]
        [SerializeField]
        [Tooltip("Will enable the highlight the containers with free space in player's inventory")]
        private bool enableHighlightContainerInPlayerInventoryFitsItemDragged = true;

        [SerializeField]
        [Tooltip(
            "Will be the background color of all containers with free space in the player inventory (Player inventory is controlled by InventorySo)")]
        private Color onContainerInPlayerInventoryFitsItemDragged;

        [Header("Prefabs Configurations")]
        [SerializeField]
        [Tooltip("Will be the prefab to be displayed when equip or open an inventory on the scene.")]
        private DisplayFiller displayFiller;

        [Header("Options Configuration Settings")] [SerializeField]
        private OptionsController optionsController;

        [SerializeField] private Button buttonPrefab;

        [Header("Window Settings")]
        [Header("Inspect Window Settings")]
        [SerializeField]
        [Tooltip("How many windows of inspect can be opened.")]
        private int maxInspectWindowOpen = 4;

        [FormerlySerializedAs("inspectPrefabWindow")]
        [SerializeField]
        [Tooltip("The prefab used when open a inspect window")]
        private BasicWindow basicPrefabWindow;

        [Header("Container Window Settings")] [SerializeField] [Tooltip("How many windows of container can be opened.")]
        private int maxContainerWindowOpen = 4;

        [SerializeField] [Tooltip("The prefab used when open a container window")]
        private ContainerWindow containerPrefabWindow;

        public TileSpecSo TileSpecSo => tileSpecSo;

        public TileGridHelperSo TileGridHelperSo => tileGridHelperSo;

        public Sprite BorderGridSprite => borderGridSprite;

        public Sprite SlotSprite => slotSprite;

        public bool HoldToDrag => holdToDrag;

        public bool ShowDragRepresentation => showDragRepresentation;

        public InventorySo PlayerInventorySo => playerInventorySo;

        public ItemPrefabSo ItemPrefabSo => itemPrefabSo;

        public DisplayFiller DisplayFiller => displayFiller;

        public float HoldScrollPadding => holdScrollPadding;

        public float HoldScrollRate => holdScrollRate * 100;

        public OptionsController OptionsController => optionsController;

        public Button ButtonPrefab => buttonPrefab;

        public Color OnHoverItem => onHoverItem;

        public Color OnHoverItemOverlapError => onHoverItemOverlapError;

        public Color OnHoverEmptyCell => onHoverEmptyCell;

        public Color OnHoverContainerThatCanBeInserted => onHoverContainerThatCanBeInserted;

        public bool EnableHighlightHolder => enableHighlightHolder;

        public Color ColorHighlightHolder => colorHighlightHolder;

        public bool EnableHighlightContainerInPlayerInventoryFitsItemDragged =>
            enableHighlightContainerInPlayerInventoryFitsItemDragged;

        public Color OnContainerInPlayerInventoryFitsItemDragged =>
            onContainerInPlayerInventoryFitsItemDragged;

        public int MaxInspectWindowOpen => maxInspectWindowOpen;

        public BasicWindow BasicPrefabWindow => basicPrefabWindow;

        public int MaxContainerWindowOpen => maxContainerWindowOpen;

        public ContainerWindow ContainerPrefabWindow => containerPrefabWindow;
    }
}