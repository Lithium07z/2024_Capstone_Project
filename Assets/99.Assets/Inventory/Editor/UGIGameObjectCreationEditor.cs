using System;
using System.Collections.Generic;
using Inventory.Editor.Helper;
using Inventory.Scripts.Core.Controllers;
using Inventory.Scripts.Core.Displays.Case;
using Inventory.Scripts.Core.Displays.Filler;
using Inventory.Scripts.Core.Holders;
using Inventory.Scripts.Core.Items.Grids;
using Inventory.Scripts.Core.Items.Grids.Helper;
using Inventory.Scripts.Core.Scroll;
using Inventory.Scripts.Core.Windows;
using Inventory.Scripts.Windows;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Inventory.Editor
{
    public class UGIGameObjectCreationEditor : EditorWindow
    {
        private const string PrefixMenu = "GameObject/";
        private const string UltimateGridInventoryPrefixMenu = PrefixMenu + "Ultimate Grid Inventory/";

        private static readonly Vector2 ItemGridElementSize = new(100f, 100f);
        private static readonly Vector2 HolderElementSize = new(200f, 200f);
        private static readonly Vector2 ScrollAreaElementSize = new(200f, 200f);
        private static readonly Vector2 OptionButtonElementSize = new(145f, 32f);
        private static readonly Vector2 BasicWindowElementSize = new(618f, 296f);
        private const float AlphaOne = 0.003921f; // Will be 1f of 255f on alpha color.
        private const float AlphaOneHundred = AlphaOne * 100f; // Will be 100f of 255f on alpha color.
        private const float AlphaMax = AlphaOne * 255f; // Will be 255f of 255f on alpha color.
        private const float DefaultScrollSensitivity = 35f;
        private const float WindowMinWidth = 290f;

        private const string BackgroundSpritePath = "UI/Skin/Background.psd";
        private const string StandardSpritePath = "UI/Skin/UISprite.psd";
        private const string MaskPath = "UI/Skin/UIMask.psd";

        private const string ContainerWindowGuid = "bff3c1c28879b4c1d81d6b2353c2a614";
        private const string SearchIconGuid = "70344c4e8451043208bd42cef87fb39c";
        private const string CloseButtonGuid = "6679a06cbc7004aad9ae199edc3c8f35";
        private const string FontGuid = "85c118c46a4e041f0a8b0d20d088a8c3";
        private const string ResidentEvilBoxGuid = "a03e5c1c83765fa4b8ad3e0a077e26b9";

        private enum PriorityOrder
        {
            Ui = 1000,
            Prefabs = 1020,
        }

        [MenuItem(UltimateGridInventoryPrefixMenu + "Item Grid", false, (int)PriorityOrder.Ui)]
        private static void AddItemGrid(MenuCommand menuCommand)
        {
            GameObject go;
            using (new UGIEditorHelper.FactorySwapToEditor())
                go = CreateItemGrid();

            UGIEditorHelper.PlaceUIElementRoot(go, menuCommand);
        }

        private static GameObject CreateItemGrid()
        {
            var go = UGIEditorHelper.CreateUIElementRoot("Item Grid", ItemGridElementSize, typeof(Image),
                typeof(ItemGrid2D),
                typeof(GridInteract2D));

            var itemGrid = go.GetComponent<ItemGrid2D>();
            var image = go.GetComponent<Image>();

            RectHelper.SetAnchorsForGrid(image.rectTransform);

            if (itemGrid.InventorySettingsAnchorSo != null)
            {
                itemGrid.ResizeGrid();
            }

            return go;
        }

        [MenuItem(UltimateGridInventoryPrefixMenu + "Container Grids", false, (int)PriorityOrder.Prefabs)]
        private static void AddContainerItemGrid(MenuCommand menuCommand)
        {
            GameObject go;
            using (new UGIEditorHelper.FactorySwapToEditor())
                go = CreateContainerItemGrid();

            UGIEditorHelper.PlaceUIElementRoot(go, menuCommand);

            var rectTransform = go.GetComponent<RectTransform>();

            RectHelper.SetLeftTopParentEdge(rectTransform);
        }

        private static GameObject CreateContainerItemGrid()
        {
            var go = UGIEditorHelper.CreateUIElementRoot("ContainerGrids_Container", ItemGridElementSize,
                typeof(RectTransform),
                typeof(ContainerGrids));

            var itemGrid = CreateItemGrid();

            var containerGridsRectTransform = go.GetComponent<RectTransform>();
            var rectTransform = itemGrid.GetComponent<RectTransform>();

            rectTransform.SetParent(containerGridsRectTransform);

            RectHelper.SetLeftTopParentEdge(containerGridsRectTransform, rectTransform);
            RectHelper.SetLeftTopParentEdge(rectTransform);
            RectHelper.SetAnchorsForGrid(containerGridsRectTransform);

            return go;
        }

        [MenuItem(UltimateGridInventoryPrefixMenu + "Item Holder", false, (int)PriorityOrder.Ui)]
        private static void AddItemHolder(MenuCommand menuCommand)
        {
            GameObject go;
            using (new UGIEditorHelper.FactorySwapToEditor())
                go = CreateHolder("ItemHolder", typeof(ItemHolder));

            UGIEditorHelper.PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem(UltimateGridInventoryPrefixMenu + "Container Holder", false, (int)PriorityOrder.Ui)]
        private static void AddContainerHolder(MenuCommand menuCommand)
        {
            GameObject go;
            using (new UGIEditorHelper.FactorySwapToEditor())
                go = CreateHolder("ContainerHolder", typeof(ContainerHolder));

            UGIEditorHelper.PlaceUIElementRoot(go, menuCommand);
        }

        private static GameObject CreateHolder(string rootName, Type holderType)
        {
            var root = UGIEditorHelper.CreateUIElementRoot(rootName, HolderElementSize,
                typeof(Image),
                holderType,
                typeof(ItemHolderInteract),
                typeof(RectMask2D)
            );

            var rootImage = root.GetComponent<Image>();
            rootImage.sprite = GetAssetByGuid<Sprite>("67e69267f9f8c4f2196bd5d85c3ccbab");

            return root;
        }

        [MenuItem(UltimateGridInventoryPrefixMenu + "Scroll Area", false, (int)PriorityOrder.Ui)]
        private static void AddInventoryScrollArea(MenuCommand menuCommand)
        {
            GameObject go;
            using (new UGIEditorHelper.FactorySwapToEditor())
                go = CreateScrollArea();

            UGIEditorHelper.PlaceUIElementRoot(go, menuCommand);
        }

        private static GameObject CreateScrollArea()
        {
            var root = UGIEditorHelper.CreateUIElementRoot("Ugi_ScrollArea", ScrollAreaElementSize,
                typeof(Image),
                typeof(ScrollRectInventory),
                typeof(ScrollRectInteract)
            );

            var viewport = UGIEditorHelper.CreateUIObject("Viewport", root,
                typeof(Image),
                typeof(Mask)
            );

            var content = UGIEditorHelper.CreateUIObject("Content", viewport,
                typeof(RectTransform)
            );

            // Sub controls.

            var hScrollbar = CreateScrollbar();
            hScrollbar.name = "Scrollbar Horizontal";
            UGIEditorHelper.SetParentAndAlign(hScrollbar, root);
            var hScrollbarRT = hScrollbar.GetComponent<RectTransform>();
            hScrollbarRT.anchorMin = Vector2.zero;
            hScrollbarRT.anchorMax = Vector2.right;
            hScrollbarRT.pivot = Vector2.zero;
            hScrollbarRT.sizeDelta = new Vector2(0, hScrollbarRT.sizeDelta.y);

            var vScrollbar = CreateScrollbar();
            vScrollbar.name = "Scrollbar Vertical";
            UGIEditorHelper.SetParentAndAlign(vScrollbar, root);
            vScrollbar.GetComponent<Scrollbar>().SetDirection(Scrollbar.Direction.BottomToTop, true);
            var vScrollbarRT = vScrollbar.GetComponent<RectTransform>();
            vScrollbarRT.anchorMin = Vector2.right;
            vScrollbarRT.anchorMax = Vector2.one;
            vScrollbarRT.pivot = Vector2.one;
            vScrollbarRT.sizeDelta = new Vector2(vScrollbarRT.sizeDelta.x, 0);

            // Setup RectTransforms.

            // Make viewport fill entire scroll view.
            var viewportRT = viewport.GetComponent<RectTransform>();
            viewportRT.anchorMin = Vector2.zero;
            viewportRT.anchorMax = Vector2.one;
            viewportRT.sizeDelta = Vector2.zero;
            viewportRT.pivot = Vector2.up;

            // Make context match viewpoprt width and be somewhat taller.
            // This will show the vertical scrollbar and not the horizontal one.
            var contentRT = content.GetComponent<RectTransform>();
            contentRT.anchorMin = Vector2.up;
            contentRT.anchorMax = Vector2.one;
            contentRT.sizeDelta = new Vector2(0, 300);
            contentRT.pivot = Vector2.up;

            // Setup UI components.

            var scrollRect = root.GetComponent<ScrollRectInventory>();
            scrollRect.content = contentRT;
            scrollRect.viewport = viewportRT;
            scrollRect.horizontalScrollbar = hScrollbar.GetComponent<Scrollbar>();
            scrollRect.verticalScrollbar = vScrollbar.GetComponent<Scrollbar>();
            scrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRect.horizontalScrollbarSpacing = -3;
            scrollRect.verticalScrollbarSpacing = -3;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.inertia = false;
            scrollRect.scrollSensitivity = DefaultScrollSensitivity;

            var rootImage = root.GetComponent<Image>();
            rootImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(BackgroundSpritePath);
            rootImage.type = Image.Type.Sliced;
            rootImage.color = new Color(1f, 1f, 1f, 0.392f);

            var viewportMask = viewport.GetComponent<Mask>();
            viewportMask.showMaskGraphic = false;

            var viewportImage = viewport.GetComponent<Image>();
            viewportImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(MaskPath);
            viewportImage.type = Image.Type.Sliced;

            return root;
        }

        /// <summary>
        /// Create the basic UI Scrollbar.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     Scrollbar
        ///         - Sliding Area
        ///             - Handle
        /// </remarks>
        /// <returns>The root GameObject of the created element.</returns>
        private static GameObject CreateScrollbar()
        {
            // Create GOs Hierarchy
            var scrollbarRoot = UGIEditorHelper.CreateUIElementRoot("Scrollbar", new Vector2(160f, 20f), typeof(Image),
                typeof(Scrollbar));

            var sliderArea = UGIEditorHelper.CreateUIObject("Sliding Area", scrollbarRoot, typeof(RectTransform));
            var handle = UGIEditorHelper.CreateUIObject("Handle", sliderArea, typeof(Image));

            var handleImageColor = new Color(1f, 1f, 1f, 1f);

            var bgImage = scrollbarRoot.GetComponent<Image>();
            bgImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(BackgroundSpritePath);
            bgImage.type = Image.Type.Sliced;
            bgImage.color = handleImageColor;

            var handleImage = handle.GetComponent<Image>();
            handleImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(StandardSpritePath);
            handleImage.type = Image.Type.Sliced;
            handleImage.color = handleImageColor;

            var sliderAreaRect = sliderArea.GetComponent<RectTransform>();
            sliderAreaRect.sizeDelta = new Vector2(-20, -20);
            sliderAreaRect.anchorMin = Vector2.zero;
            sliderAreaRect.anchorMax = Vector2.one;

            var handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, 20);

            var scrollbar = scrollbarRoot.GetComponent<Scrollbar>();
            scrollbar.handleRect = handleRect;
            scrollbar.targetGraphic = handleImage;
            SetDefaultColorTransitionValues(scrollbar);

            return scrollbarRoot;
        }

        private static void SetDefaultColorTransitionValues(Selectable slider)
        {
            var colors = slider.colors;
            colors.highlightedColor = new Color(0.882f, 0.882f, 0.882f);
            colors.pressedColor = new Color(0.698f, 0.698f, 0.698f);
            colors.disabledColor = new Color(0.521f, 0.521f, 0.521f);
        }

        [MenuItem(UltimateGridInventoryPrefixMenu + "Display Filler", false, (int)PriorityOrder.Prefabs)]
        private static void AddDisplayFiller(MenuCommand menuCommand)
        {
            GameObject go;
            using (new UGIEditorHelper.FactorySwapToEditor())
                go = CreateDisplayFiller();

            UGIEditorHelper.PlaceUIElementRoot(go, menuCommand);
        }

        private static GameObject CreateDisplayFiller()
        {
            var root = UGIEditorHelper.CreateUIElementRoot("DisplayFiller", ScrollAreaElementSize,
                typeof(Image),
                typeof(DisplayFiller),
                typeof(VerticalLayoutGroup),
                typeof(ContentSizeFitter)
            );

            var gridParent = UGIEditorHelper.CreateUIObject("GridParent", root,
                typeof(RectTransform),
                typeof(HorizontalLayoutGroup)
            );

            var gridParentHlg = gridParent.GetComponent<HorizontalLayoutGroup>();
            gridParentHlg.childAlignment = TextAnchor.UpperLeft;
            gridParentHlg.childForceExpandWidth = true;
            gridParentHlg.childForceExpandHeight = true;

            var rootRT = root.GetComponent<RectTransform>();
            RectHelper.SetDisplayFillerProperties(rootRT);

            var rootImage = root.GetComponent<Image>();
            rootImage.color = RectHelper.ChangeAlphaColor(rootImage.color, 0f);

            var gridParentRT = gridParent.GetComponent<RectTransform>();
            RectHelper.SetAnchorsForGrid(gridParentRT);

            var displayFiller = root.GetComponent<DisplayFiller>();
            displayFiller.GridParent = gridParentRT;

            var rootVlg = root.GetComponent<VerticalLayoutGroup>();
            rootVlg.childAlignment = TextAnchor.UpperLeft;
            rootVlg.childControlWidth = true;
            rootVlg.childControlHeight = true;
            rootVlg.childForceExpandWidth = true;
            rootVlg.childForceExpandHeight = true;

            var rootCsf = root.GetComponent<ContentSizeFitter>();
            rootCsf.horizontalFit = ContentSizeFitter.FitMode.MinSize;
            rootCsf.verticalFit = ContentSizeFitter.FitMode.MinSize;

            return root;
        }

        [MenuItem(UltimateGridInventoryPrefixMenu + "Options Controller", false, (int)PriorityOrder.Prefabs)]
        private static void AddOptionsController(MenuCommand menuCommand)
        {
            GameObject go;
            using (new UGIEditorHelper.FactorySwapToEditor())
                go = CreateOptionsController();

            UGIEditorHelper.PlaceUIElementRoot(go, menuCommand);
        }

        private static GameObject CreateOptionsController()
        {
            var root = UGIEditorHelper.CreateUIElementRoot("OptionsController", ScrollAreaElementSize,
                typeof(Image),
                typeof(OptionsController),
                typeof(VerticalLayoutGroup),
                typeof(ContentSizeFitter)
            );

            var rootRT = root.GetComponent<RectTransform>();
            RectHelper.SetOptionsControllerProperties(rootRT);

            var rootImage = root.GetComponent<Image>();
            rootImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(BackgroundSpritePath);
            rootImage.type = Image.Type.Sliced;

            var rootVlg = root.GetComponent<VerticalLayoutGroup>();
            rootVlg.childAlignment = TextAnchor.UpperLeft;
            rootVlg.childControlWidth = true;
            rootVlg.childForceExpandWidth = true;
            rootVlg.spacing = 6f;
            rootVlg.padding = new RectOffset(4, 4, 4, 4);

            var rootCsf = root.GetComponent<ContentSizeFitter>();
            rootCsf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            return root;
        }

        [MenuItem(UltimateGridInventoryPrefixMenu + "Option Button", false, (int)PriorityOrder.Prefabs)]
        private static void AddOptionButton(MenuCommand menuCommand)
        {
            GameObject go;
            using (new UGIEditorHelper.FactorySwapToEditor())
                go = CreateOptionButton();

            UGIEditorHelper.PlaceUIElementRoot(go, menuCommand);
        }

        private static GameObject CreateOptionButton()
        {
            var root = UGIEditorHelper.CreateUIElementRoot("OptionButton", OptionButtonElementSize,
                typeof(Image),
                typeof(Button)
            );

            var rootRT = root.GetComponent<RectTransform>();
            RectHelper.SetOptionButtonProperties(rootRT);

            var rootImage = root.GetComponent<Image>();
            rootImage.color = new Color(191f, 191f, 191f, AlphaMax);

            var text = UGIEditorHelper.CreateUIObject("Text (TMP)", root,
                typeof(TextMeshProUGUI)
            );

            var textRT = text.GetComponent<RectTransform>();
            RectHelper.SetOptionButtonTextProperties(textRT);

            var tmpText = text.GetComponent<TMP_Text>();
            tmpText.SetText("Inspect");
            tmpText.fontSize = 18f;
            tmpText.fontStyle = FontStyles.Bold + (int)FontStyles.UpperCase;
            tmpText.color = new Color(0, 0, 0, AlphaMax);
            tmpText.alignment = TextAlignmentOptions.MidlineLeft;
            tmpText.margin = new Vector4(12f, 0f, 0f);

            return root;
        }

        [MenuItem(UltimateGridInventoryPrefixMenu + "Basic Window", false, (int)PriorityOrder.Prefabs)]
        private static void AddBasicWindow(MenuCommand menuCommand)
        {
            GameObject go;
            using (new UGIEditorHelper.FactorySwapToEditor())
                go = CreateBasicWindow("BasicWindow", typeof(BasicWindow));

            UGIEditorHelper.PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem(UltimateGridInventoryPrefixMenu + "Container Window", false, (int)PriorityOrder.Prefabs)]
        private static void AddContainerWindow(MenuCommand menuCommand)
        {
            GameObject go;
            using (new UGIEditorHelper.FactorySwapToEditor())
                go = CreateBasicWindow("ContainerWindow", typeof(ContainerWindow));

            UGIEditorHelper.PlaceUIElementRoot(go, menuCommand);
        }

        private static GameObject CreateBasicWindow(string windowName, Type windowType)
        {
            var root = UGIEditorHelper.CreateUIElementRoot(windowName, BasicWindowElementSize,
                typeof(Image),
                windowType,
                typeof(VerticalLayoutGroup),
                typeof(ContentSizeFitter),
                typeof(Shadow),
                typeof(Shadow)
            );

            var rootImage = root.GetComponent<Image>();
            rootImage.sprite = GetAssetByGuid<Sprite>(ContainerWindowGuid);
            rootImage.type = Image.Type.Sliced;

            var rootVlg = root.GetComponent<VerticalLayoutGroup>();
            rootVlg.childAlignment = TextAnchor.UpperLeft;
            rootVlg.childControlWidth = true;
            rootVlg.childForceExpandWidth = true;
            rootVlg.childForceExpandHeight = true;

            var rootCsf = root.GetComponent<ContentSizeFitter>();
            rootCsf.horizontalFit = ContentSizeFitter.FitMode.MinSize;
            rootCsf.verticalFit = ContentSizeFitter.FitMode.MinSize;

            var shadowComponents = root.GetComponents<Shadow>();

            ConfigureShadow(shadowComponents);

            var window = root.GetComponent<Window>();

            ConfigureHeaderWindow(root, window);

            var line = UGIEditorHelper.CreateUIObject("Line", root,
                typeof(RectTransform)
            );

            var lineRT = line.GetComponent<RectTransform>();
            lineRT.anchoredPosition = new Vector2(0f, 1f);
            lineRT.anchorMin = new Vector2(0f, 1f);
            lineRT.anchorMax = new Vector2(0f, 1f);
            lineRT.pivot = new Vector2(0.5f, 0.5f);
            lineRT.sizeDelta = new Vector2(WindowMinWidth, 1f);

            var content = UGIEditorHelper.CreateUIObject("Content", root,
                typeof(RectTransform),
                typeof(VerticalLayoutGroup),
                typeof(ContentSizeFitter)
            );

            var contentRT = content.GetComponent<RectTransform>();
            contentRT.anchoredPosition = new Vector2(0f, 1f);
            contentRT.anchorMin = new Vector2(0f, 1f);
            contentRT.anchorMax = new Vector2(0f, 1f);
            contentRT.pivot = new Vector2(0f, 1f);
            contentRT.sizeDelta = new Vector2(WindowMinWidth, 148f);

            var contentVlg = content.GetComponent<VerticalLayoutGroup>();
            contentVlg.childAlignment = TextAnchor.UpperLeft;
            contentVlg.padding = new RectOffset(19, 19, 19, 19);
            contentVlg.spacing = 4f;
            contentVlg.childForceExpandWidth = true;
            contentVlg.childForceExpandHeight = true;

            var contentCsf = content.GetComponent<ContentSizeFitter>();
            contentCsf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            switch (window)
            {
                case ContainerWindow containerWindow:
                    CreateContainerProperties(containerWindow, root, contentRT);
                    break;
                case BasicWindow basicWindow:
                    CreateBasicProperties(basicWindow, root, content);
                    break;
            }

            return root;
        }

        private static void ConfigureShadow(IReadOnlyList<Shadow> shadowComponents)
        {
            if (shadowComponents.Count < 2) return;

            var shadowComponent = shadowComponents[0];
            shadowComponent.effectDistance = new Vector2(3, -3);
            var shadowComponent2 = shadowComponents[1];
            shadowComponent2.effectDistance = new Vector2(-3, 3);
        }

        private static void ConfigureHeaderWindow(GameObject root, Window window)
        {
            var header = UGIEditorHelper.CreateUIObject("Header", root,
                typeof(Image),
                typeof(WindowMover),
                typeof(LayoutElement)
            );

            var icon = UGIEditorHelper.CreateUIObject("Icon", header,
                typeof(Image)
            );

            var textArea = UGIEditorHelper.CreateUIObject("TextArea", header,
                typeof(RectTransform),
                typeof(RectMask2D)
            );

            var title = UGIEditorHelper.CreateUIObject("Title", textArea,
                typeof(TextMeshProUGUI),
                typeof(TMPTextReflectionFiller)
            );

            var tmpTextFiller = title.GetComponent<TMPTextReflectionFiller>();
            ComponentUtility.MoveComponentUp(tmpTextFiller);

            var closeButton = UGIEditorHelper.CreateUIObject("CloseButton", header,
                typeof(Image),
                typeof(Button)
            );

            var closeText = UGIEditorHelper.CreateUIObject("Text (TMP)", closeButton,
                typeof(TextMeshProUGUI)
            );

            var headerRT = header.GetComponent<RectTransform>();
            headerRT.anchoredPosition = new Vector2(0f, 1f);
            headerRT.anchorMin = new Vector2(0f, 1f);
            headerRT.anchorMax = new Vector2(0f, 1f);
            headerRT.pivot = new Vector2(0.5f, 1f);
            headerRT.sizeDelta = new Vector2(WindowMinWidth, 52f);

            var headerImage = header.GetComponent<Image>();
            headerImage.color = RectHelper.ChangeAlphaColor(headerImage.color, 0f);

            var headerLe = header.GetComponent<LayoutElement>();
            headerLe.minWidth = WindowMinWidth;
            headerLe.minHeight = 52f;

            var iconRT = icon.GetComponent<RectTransform>();
            iconRT.anchoredPosition = new Vector2(15f, 0.5f);
            iconRT.anchorMin = new Vector2(0, 0.5f);
            iconRT.anchorMax = new Vector2(0, 0.5f);
            iconRT.pivot = new Vector2(0, 0.5f);
            iconRT.sizeDelta = new Vector2(32f, 32f);

            var iconImage = icon.GetComponent<Image>();
            iconImage.sprite = GetAssetByGuid<Sprite>(SearchIconGuid);

            var textAreaRT = textArea.GetComponent<RectTransform>();
            textAreaRT.anchorMin = new Vector2(0f, 0.5f);
            textAreaRT.anchorMax = new Vector2(1f, 0.5f);
            textAreaRT.pivot = new Vector2(0.5f, 0.5f);
            textAreaRT.anchoredPosition = new Vector2(0f, 0f);
            textAreaRT.sizeDelta = new Vector2(-128f, 48f);

            var titleRT = title.GetComponent<RectTransform>();
            titleRT.anchoredPosition = new Vector2(0f, 0f);
            titleRT.anchorMin = new Vector2(0f, 0f);
            titleRT.anchorMax = new Vector2(1f, 1f);
            titleRT.pivot = new Vector2(0.5f, 0.5f);
            titleRT.sizeDelta = new Vector2(0f, 0f);

            var tmpText = title.GetComponent<TMP_Text>();
            tmpText.SetText(window is ContainerWindow ? "Container" : "Inspect");
            tmpText.fontSize = 28f;
            tmpText.fontStyle = FontStyles.Bold;
            tmpText.font = GetAssetByGuid<TMP_FontAsset>(FontGuid);
            tmpText.color = new Color(191f, 191f, 191f, AlphaMax);
            tmpText.alignment = TextAlignmentOptions.Midline;
            tmpText.alignment = TextAlignmentOptions.Left;
            tmpText.overflowMode = TextOverflowModes.Ellipsis;

            ChangeFillersByWindow(window, title, tmpTextFiller);

            var closeButtonRT = closeButton.GetComponent<RectTransform>();
            closeButtonRT.anchoredPosition = new Vector2(-15f, 0.5f);
            closeButtonRT.anchorMin = new Vector2(1f, 0.5f);
            closeButtonRT.anchorMax = new Vector2(1f, 0.5f);
            closeButtonRT.pivot = new Vector2(1f, 0.5f);
            closeButtonRT.sizeDelta = new Vector2(42f, 32f);

            var closeButtonImage = closeButton.GetComponent<Image>();
            closeButtonImage.sprite = GetAssetByGuid<Sprite>(CloseButtonGuid);

            var closeButtonComponent = closeButton.GetComponent<Button>();
            UnityEventTools.AddPersistentListener(closeButtonComponent.onClick, window.CloseWindow);

            var closeTextRT = closeText.GetComponent<RectTransform>();
            closeTextRT.anchoredPosition = new Vector2(0f, 0f);
            closeTextRT.anchorMin = new Vector2(0f, 0f);
            closeTextRT.anchorMax = new Vector2(1f, 1f);
            closeTextRT.pivot = new Vector2(0.5f, 0.5f);
            closeTextRT.sizeDelta = new Vector2(0f, 0f);

            var txtClose = closeText.GetComponent<TMP_Text>();
            txtClose.SetText("X");
            txtClose.fontSize = 24f;
            txtClose.fontStyle = FontStyles.Bold;
            txtClose.font = GetAssetByGuid<TMP_FontAsset>(FontGuid);
            txtClose.color = new Color(255f, 255f, 255f, AlphaMax);
            txtClose.alignment = TextAlignmentOptions.Center;
            txtClose.alignment = TextAlignmentOptions.Midline;
        }

        private static void ChangeFillersByWindow(Window window, GameObject title,
            TextReflectionContentFiller tmpTextReflectionFiller)
        {
            switch (window)
            {
                case ContainerWindow:
                    tmpTextReflectionFiller.TextTemplate = $"Container {tmpTextReflectionFiller.PropertyReplacer}";
                    tmpTextReflectionFiller.FallbackObject = "Container without name...";
                    break;
                case BasicWindow:
                    tmpTextReflectionFiller.TextTemplate = $"Inspecting {tmpTextReflectionFiller.PropertyReplacer}";
                    tmpTextReflectionFiller.FallbackObject = "Item without name...";
                    break;
            }
        }

        private static void CreateContainerProperties(ContainerWindow containerWindow, GameObject root,
            RectTransform contentRT)
        {
            containerWindow.Content = contentRT;
        }

        private static void CreateBasicProperties(BasicWindow basicWindow, GameObject root, GameObject content)
        {
            for (var i = 0; i < 3; i++)
            {
                CreateSimpleRow($"SimpleRow - ({i + 1})", content);
            }
        }

        private static void CreateSimpleRow(string rowName, GameObject content)
        {
            var simpleRow = UGIEditorHelper.CreateUIObject(rowName, content,
                typeof(RectTransform),
                typeof(HorizontalLayoutGroup),
                typeof(ContentSizeFitter)
            );

            var simpleRowHlg = simpleRow.GetComponent<HorizontalLayoutGroup>();
            simpleRowHlg.childAlignment = TextAnchor.UpperLeft;
            simpleRowHlg.childForceExpandWidth = true;
            simpleRowHlg.childForceExpandHeight = true;

            var simpleRowCsf = simpleRow.GetComponent<ContentSizeFitter>();
            simpleRowCsf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            simpleRowCsf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        [MenuItem(UltimateGridInventoryPrefixMenu + "Resident Evil Display Filler", false, (int)PriorityOrder.Prefabs)]
        private static void AddResidentEvilDisplayFillerMenu(MenuCommand menuCommand)
        {
            GameObject go;
            using (new UGIEditorHelper.FactorySwapToEditor())
                go = CreateResidentEvilDisplayFiller();

            UGIEditorHelper.PlaceUIElementRoot(go, menuCommand);
        }

        private static GameObject CreateResidentEvilDisplayFiller()
        {
            var root = UGIEditorHelper.CreateUIElementRoot("RECase_DisplayFiller", BasicWindowElementSize,
                typeof(Image),
                typeof(VerticalLayoutGroup),
                typeof(ResidentEvilCaseDisplayFiller),
                typeof(ContentSizeFitter)
            );

            var rootRT = root.GetComponent<RectTransform>();

            var rootImage = root.GetComponent<Image>();
            rootImage.sprite = GetAssetByGuid<Sprite>(ResidentEvilBoxGuid);
            rootImage.type = Image.Type.Sliced;

            var boxPaddings = new RectOffset(
                18,
                18,
                21,
                18
            );

            var rootVlg = root.GetComponent<VerticalLayoutGroup>();
            rootVlg.padding = boxPaddings;
            rootVlg.childScaleWidth = true;
            rootVlg.childScaleHeight = true;
            rootVlg.childForceExpandWidth = false;
            rootVlg.childForceExpandHeight = false;
            rootVlg.childAlignment = TextAnchor.UpperLeft;

            var rootRecdf = root.GetComponent<ResidentEvilCaseDisplayFiller>();
            rootRecdf.GridParent = rootRT;
            rootRecdf.InitialPaddings = boxPaddings;

            var rootCsf = root.GetComponent<ContentSizeFitter>();
            rootCsf.horizontalFit = ContentSizeFitter.FitMode.MinSize;
            rootCsf.verticalFit = ContentSizeFitter.FitMode.MinSize;

            return root;
        }

        private static T GetAssetByGuid<T>(string guid) where T : Object
        {
            var guidToAssetPath = AssetDatabase.GUIDToAssetPath(guid);

            return AssetDatabase.LoadAssetAtPath<T>(guidToAssetPath);
        }
    }
}