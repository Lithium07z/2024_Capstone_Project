using Inventory.Scripts.Core;
using Inventory.Scripts.Core.Controllers;
using Inventory.Scripts.Core.ScriptableObjects;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Anchors;
using Inventory.Scripts.Windows;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Inventory.Editor
{
    public class InitializrInventoryEditor : EditorWindow
    {
        [SerializeField] private GameObject prefabInventoryGrid;

        [SerializeField] private CanvasAnchorSo canvasAnchorSo;

        [SerializeField] private InventorySettingsAnchorSo inventorySettingsAnchorSo;

        // GameObjects Dependencies
        [SerializeField] private RectTransformPrefabAnchorSo itemParentWhenDragPrefabAnchorSo;
        [SerializeField] private RectTransformPrefabAnchorSo optionsParentPrefabAnchorSo;
        [SerializeField] private RectTransformPrefabAnchorSo windowParentPrefabAnchorSo;

        [FormerlySerializedAs("highlighterAnchorSo")] [SerializeField]
        private HighlighterPrefabAnchorSo highlighterPrefabAnchorSo;

        private Canvas _canvas;

        [MenuItem("Ultimate Grid Inventory/Initializr Inventory")]
        private static void Init()
        {
            // Get existing open window or if none, make a new one:
            var window = GetWindow<InitializrInventoryEditor>(nameof(InitializrInventoryEditor));
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);
            GUILayout.Space(8);
            GUILayout.Label("Ultimate Grid Inventory Settings", EditorStyles.boldLabel);


            GUILayout.Space(16);
            GUILayout.Label("Initialization Settings", EditorStyles.boldLabel);

            prefabInventoryGrid = EditorGUILayout.ObjectField("Inventory Manager Prefab", prefabInventoryGrid,
                typeof(GameObject),
                true) as GameObject;

            inventorySettingsAnchorSo = EditorGUILayout.ObjectField("Inventory Settings Anchor So",
                inventorySettingsAnchorSo,
                typeof(InventorySettingsAnchorSo),
                true) as InventorySettingsAnchorSo;

            GUILayout.Space(8);
            GUILayout.Label("Inventory Manager Dependencies", EditorStyles.boldLabel);

            itemParentWhenDragPrefabAnchorSo = EditorGUILayout.ObjectField("Default Item Parent When Drag",
                itemParentWhenDragPrefabAnchorSo,
                typeof(RectTransformPrefabAnchorSo),
                true) as RectTransformPrefabAnchorSo;

            optionsParentPrefabAnchorSo = EditorGUILayout.ObjectField("Options Parent Settings",
                optionsParentPrefabAnchorSo,
                typeof(RectTransformPrefabAnchorSo),
                true) as RectTransformPrefabAnchorSo;

            windowParentPrefabAnchorSo = EditorGUILayout.ObjectField("Window Parent", windowParentPrefabAnchorSo,
                typeof(RectTransformPrefabAnchorSo),
                true) as RectTransformPrefabAnchorSo;

            highlighterPrefabAnchorSo = EditorGUILayout.ObjectField("HighlighterAnchorSo", highlighterPrefabAnchorSo,
                typeof(HighlighterPrefabAnchorSo),
                true) as HighlighterPrefabAnchorSo;

            GUILayout.Space(24);

            GUILayout.BeginHorizontal();

            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Initializr or Update"))
            {
                ApplyCanvasSettings();
                InitializrInventory();
                SaveScene();
            }

            GUILayout.EndHorizontal();
        }

        private void ApplyCanvasSettings()
        {
            _canvas = FindObjectOfType<Canvas>();

            if (_canvas == null)
            {
                Debug.LogError(
                    "Please create a canvas in the scene in order to complete the Ultimate Grid Inventory configuration.");
                return;
            }

            var canvasInitializer = GetCanvasInitializer(_canvas);

            canvasInitializer.SetAnchors(canvasAnchorSo);
        }

        private void InitializrInventory()
        {
            var foundInventoryManagerInScene = FindObjectOfType<DraggableController>();

            if (foundInventoryManagerInScene != null)
            {
                Debug.LogWarning("Found Inventory Manager in Scene, altering properties...");
                AlterProperties(foundInventoryManagerInScene.gameObject);
                return;
            }

            Debug.Log("Instantiating Inventory Manager Prefab. Prefab Name: " + prefabInventoryGrid.name);
            var inventoryManager = PrefabUtility.InstantiatePrefab(prefabInventoryGrid) as GameObject;

            if (inventoryManager != null)
            {
                inventoryManager.transform.SetAsFirstSibling();
            }

            AlterProperties(inventoryManager);
        }

        private static CanvasInitializer GetCanvasInitializer(Canvas canvas)
        {
            var canvasInitializer = canvas.gameObject.GetComponent<CanvasInitializer>();

            return canvasInitializer != null ? canvasInitializer : canvas.gameObject.AddComponent<CanvasInitializer>();
        }

        private void AlterProperties(GameObject inventoryManager)
        {
            var draggableController = inventoryManager.GetComponent<DraggableController>();

            if (draggableController != null)
            {
                AlterDraggableController(draggableController);
            }

            var inventoryTransformers = inventoryManager.GetComponent<InventoryTransformers>();

            if (inventoryTransformers != null)
            {
                AlterInventoryTransformers(inventoryTransformers);
            }

            var inventoryHighlight = inventoryManager.GetComponent<InventoryHighlightController>();

            if (inventoryHighlight != null)
            {
                AlterInventoryHighlight(inventoryHighlight);
            }

            var inventoryScrollController = inventoryManager.GetComponent<InventoryScrollController>();

            if (inventoryScrollController != null)
            {
                AlterScrollController(inventoryScrollController);
            }

            var optionsController = inventoryManager.GetComponent<InventoryOptionsController>();

            if (optionsController != null)
            {
                AlterOptionsController(optionsController);
            }

            var inventoryHighlightSpaceContainer =
                inventoryManager.GetComponent<InventoryHighlightSpaceContainerController>();

            if (inventoryHighlightSpaceContainer != null)
            {
                AlterInventoryHighlightSpaceContainer(inventoryHighlightSpaceContainer);
            }

            AlterComponentInChildren(inventoryManager);
        }

        private void AlterDraggableController(DraggableController draggableController)
        {
            EditorUtility.SetDirty(draggableController);
            draggableController.SetItemParent(itemParentWhenDragPrefabAnchorSo);
            draggableController.SetInventorySettingsAnchorSo(inventorySettingsAnchorSo);
        }

        private void AlterInventoryTransformers(InventoryTransformers inventoryTransformers)
        {
            EditorUtility.SetDirty(inventoryTransformers);
            var transforms = new[]
            {
                windowParentPrefabAnchorSo,
                itemParentWhenDragPrefabAnchorSo,
                optionsParentPrefabAnchorSo
            };
            inventoryTransformers.SetTransformers(transforms);
        }

        private void AlterInventoryHighlight(InventoryHighlightController inventoryHighlightController)
        {
            EditorUtility.SetDirty(inventoryHighlightController);
            inventoryHighlightController.SetHighlighterAnchorSo(highlighterPrefabAnchorSo);
            inventoryHighlightController.SetInventorySettingsAnchorSo(inventorySettingsAnchorSo);
        }

        private void AlterScrollController(InventoryScrollController inventoryScrollController)
        {
            EditorUtility.SetDirty(inventoryScrollController);
            inventoryScrollController.SetInventorySettingsAnchorSo(inventorySettingsAnchorSo);
        }

        private void AlterOptionsController(InventoryOptionsController optionsController)
        {
            EditorUtility.SetDirty(optionsController);
            optionsController.SetOptionsParent(optionsParentPrefabAnchorSo);
            optionsController.SetInventorySettingsAnchorSo(inventorySettingsAnchorSo);
        }

        private void AlterInventoryHighlightSpaceContainer(
            InventoryHighlightSpaceContainerController inventoryHighlightSpaceContainerController)
        {
            EditorUtility.SetDirty(inventoryHighlightSpaceContainerController);
            inventoryHighlightSpaceContainerController.SetInventorySettingsAnchorSo(inventorySettingsAnchorSo);
        }


        // ========== Altering Children Components ==========


        private void AlterComponentInChildren(GameObject inventoryManager)
        {
            AlterWindowManager(inventoryManager);
        }

        private void AlterWindowManager(GameObject inventoryManager)
        {
            var inspectWindowManager = inventoryManager.GetComponentInChildren<InspectWindowManager>();

            if (inspectWindowManager != null)
            {
                AlterWindowManager(inspectWindowManager);
            }

            var containerWindowManager = inventoryManager.GetComponentInChildren<ContainerWindowManager>();

            if (containerWindowManager != null)
            {
                AlterWindowManager(containerWindowManager);
            }
        }

        private void AlterWindowManager(WindowController windowController)
        {
            EditorUtility.SetDirty(windowController);
            windowController.SetInventorySettingsAnchorSo(inventorySettingsAnchorSo);
            windowController.SetWindowParent(windowParentPrefabAnchorSo);
        }

        private static void SaveScene()
        {
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }
    }
}