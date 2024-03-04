using System.Collections.Generic;
using System.Linq;
using Inventory.Scripts.Core.Helper;
using Inventory.Scripts.Core.ScriptableObjects.Datastores;
using Inventory.Scripts.Core.ScriptableObjects.Items;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Inventory.Editor.Datastores
{
    public class DatastoreImporterEditor : EditorWindow
    {
        [SerializeField] private DatastoreItems datastore;

        [SerializeField] private bool clearBeforeImport = true;

        [SerializeField] private string pathToImportItems = "Inventory/Items/";

        [MenuItem("Ultimate Grid Inventory/Items Datastore Importer")]
        private static void Init()
        {
            // Get existing open window or if none, make a new one:
            var window = GetWindow<DatastoreImporterEditor>("Datastore Importer Settings");
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            GUILayout.Space(8);
            GUILayout.Label("Data Source Importer Settings", EditorStyles.boldLabel);


            GUILayout.Space(16);
            GUILayout.Label("Importer Settings", EditorStyles.boldLabel);

            datastore = EditorGUILayout.ObjectField("Datastore Items", datastore,
                typeof(DatastoreItems),
                true) as DatastoreItems;

            clearBeforeImport =
                EditorGUILayout.Toggle("Clear datastore before import", clearBeforeImport);

            pathToImportItems = EditorGUILayout.TextField("Path to import items", pathToImportItems);

            GUILayout.Space(24);

            GUILayout.BeginHorizontal();

            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Import Items"))
            {
                ImportItems();
                SaveScene();
            }

            GUILayout.EndHorizontal();
        }

        private void ImportItems()
        {
            Debug.Log($"Import items from path {pathToImportItems}".Editor());

            var allItemsInPath = GetAllItemsInPath(pathToImportItems);

            if (datastore == null)
            {
                Debug.Log("Datasource is null... Cannot import items...".Editor());
                return;
            }

            Debug.Log($"Importing {allItemsInPath.Count} items...".Editor());
            datastore.Import(allItemsInPath, !clearBeforeImport);
            EditorUtility.SetDirty(datastore);
        }

        private List<ItemDataSo> GetAllItemsInPath(string path)
        {
            if (!Application.isEditor) return new List<ItemDataSo>();

#if UNITY_EDITOR
            var guids = AssetDatabase.FindAssets("t:ItemDataSo", new[] { ResolvePath(path) });

            return guids.Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<ItemDataSo>).ToList();
#endif
        }

        private static string ResolvePath(string path)
        {
            if (path == null)
            {
                Debug.LogError($"Path cannot be null... Please provide some value".Error());
                return null;
            }

            return path.StartsWith("Assets/") ? path : $"Assets/{path}";
        }

        private static void SaveScene()
        {
#if UNITY_EDITOR
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
#endif
        }
    }
}