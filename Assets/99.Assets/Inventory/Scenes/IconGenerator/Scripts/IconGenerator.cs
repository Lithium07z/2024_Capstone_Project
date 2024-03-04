using System.Collections;
using System.Collections.Generic;
using System.IO;
using Inventory.Scripts.Core.Helper;
using UnityEditor;
using UnityEngine;

namespace Inventory.Scenes.IconGenerator.Scripts
{
    public class IconGenerator : MonoBehaviour
    {
        // TODO: Add a IconGeneratorEditor in order to have buttons in the inspector to facilitate the process of screenshots.
        private static int _index;

        private static int _pixels = 1024;

        [Header("Icon Generator Configuration")]
        [SerializeField]
        [Tooltip("Quality of the picture, if minus than 0 it will be lower than 1024 pixels.")]
        [Range(-4, 9)]
        private int qualityPng = 1;

        [Header("Folder Configuration")] [SerializeField]
        private string pathFolder = "Inventory/UI/Icons/IconGenerator";

        [Header("Objects Configuration")]
        [SerializeField]
        [Tooltip("Objects placed in the scene in order to have a screenshot.")]
        private List<GameObject> sceneObjects;

        private Camera _camera;

        public List<GameObject> SceneObjects => sceneObjects;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void Init()
        {
            if (_camera == null)
            {
                _camera = GetComponent<Camera>();
            }
        }

        [ContextMenu("Screenshot")]
        public void ProcessScreenshot()
        {
            StartCoroutine(Screenshot());
        }

        private IEnumerator Screenshot()
        {
            DisableAllObjects();

            foreach (var obj in GetSceneObjects())
            {
                obj.gameObject.SetActive(true);

                yield return null;

                TakeScreenshot($"{Application.dataPath}/{GetPathFolder()}/{RandomName()}_Icon.png");

                yield return null;

                obj.gameObject.SetActive(false);
            }
        }

        private static string RandomName()
        {
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                return GUID.Generate().ToString();
            }
#endif

            return "random_name_" + _index++;
        }

        private string GetPathFolder()
        {
            return pathFolder == null ? "Icons" : pathFolder.Trim();
        }

        private void DisableAllObjects()
        {
            foreach (var sceneObject in sceneObjects)
            {
                sceneObject.gameObject.SetActive(false);
            }
        }

        private void TakeScreenshot(string fullPath)
        {
            Init();

            var pixels = GetPixels();

            var messageDebug = "Pictures taking with " + pixels + " pixels...";
            Debug.Log(messageDebug.Editor());

            var renderTexture = new RenderTexture(pixels, pixels, 24);
            _camera.targetTexture = renderTexture;
            var screenShot = new Texture2D(pixels, pixels, TextureFormat.RGBA32, false);
            _camera.Render();
            RenderTexture.active = renderTexture;
            screenShot.ReadPixels(new Rect(0, 0, pixels, pixels), 0, 0);
            _camera.targetTexture = null;
            RenderTexture.active = null;

            DestroyRenderTexture(renderTexture);

            var imageInBytes = screenShot.EncodeToPNG();
            File.WriteAllBytes(fullPath, imageInBytes);

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        private static void DestroyRenderTexture(RenderTexture renderTexture)
        {
            if (Application.isEditor)
            {
                DestroyImmediate(renderTexture);
            }
            else
            {
                Destroy(renderTexture);
            }
        }

        public List<GameObject> GetSceneObjects()
        {
            return sceneObjects.FindAll(o => o != null);
        }

        public int GetPixels()
        {
            return _pixels * qualityPng;
        }
    }
}