using System.Linq;
using Inventory.Scripts.Core.Holders;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Inventory.Editor.Holders
{
    [CustomEditor(typeof(ItemHolder), true)]
    public class ItemHolderEditor : UnityEditor.Editor
    {
        private const string ScriptPropertyName = "m_Script";
        private const string UseDefaultSpritePropertyName = "useDefaultSprite";
        private const string DefaultSpritePropertyName = "defaultSprite";
        private const string SpriteColorPropertyName = "spriteColor";

        private const string GameObjectName = "Image_Holder";

        private ItemHolder _itemHolder;

        private bool _useDefaultSprite;

        private SerializedProperty _useDefaultSpriteProperty;
        private SerializedProperty _defaultSprite;
        private SerializedProperty _spriteColor;

        private GameObject _itemHolderImage;

        private void OnEnable()
        {
            _useDefaultSpriteProperty = serializedObject.FindProperty(UseDefaultSpritePropertyName);
            _defaultSprite = serializedObject.FindProperty(DefaultSpritePropertyName);
            _spriteColor = serializedObject.FindProperty(SpriteColorPropertyName);
        }

        public override void OnInspectorGUI()
        {
            _itemHolder = (ItemHolder)target;

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(ScriptPropertyName));
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(8);
            GUILayout.Label("Image Settings", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(_useDefaultSpriteProperty);

            EditorGUI.BeginDisabledGroup(!_useDefaultSpriteProperty.boolValue);

            EditorGUILayout.PropertyField(_defaultSprite);
            EditorGUILayout.PropertyField(_spriteColor);

            if (!_itemHolder.UseDefaultSprite)
            {
                DeleteObjectInHierarchy();
            }
            
            GUILayout.Space(4);
            if (GUILayout.Button("Create/Update Image in Children"))
            {
                if (_itemHolder.UseDefaultSprite)
                {
                    CreateSpriteInChildren();
                }
            }

            EditorGUI.EndDisabledGroup();

            DrawPropertiesExcluding(serializedObject, ScriptPropertyName, UseDefaultSpritePropertyName,
                DefaultSpritePropertyName, SpriteColorPropertyName);

            serializedObject.ApplyModifiedProperties();
        }

        private void CreateSpriteInChildren()
        {
            if (_itemHolder == null) return;

            _itemHolderImage = GetItemHolderImage();

            _itemHolderImage.transform.SetParent(_itemHolder.transform, false);
            _itemHolderImage.transform.SetAsFirstSibling();

            _itemHolderImage.layer = LayerMask.NameToLayer("UI");

            var image = GetImage(_itemHolderImage);

            image.sprite = _itemHolder.DefaultSprite;
            image.color = _itemHolder.SpriteColor;

            SaveScene();
        }

        private void DeleteObjectInHierarchy()
        {
            var gameObject = FindImageHolderInChildren();

            if (gameObject == null) return;
            
            DestroyImmediate(gameObject.gameObject);
            SaveScene();
        }

        private GameObject GetItemHolderImage()
        {
            var gameObject = FindImageHolderInChildren();

            return gameObject ? gameObject.gameObject : new GameObject(GameObjectName, typeof(RectTransform));
        }

        private Image FindImageHolderInChildren()
        {
            var componentsInChildren = _itemHolder.GetComponentsInChildren<Image>();

            return (from componentsInChild in componentsInChildren
                where componentsInChild != null && componentsInChild.name.Equals(GameObjectName)
                select componentsInChild.GetComponent<Image>()).FirstOrDefault();
        }

        private static Image GetImage(GameObject gameObject)
        {
            var image = gameObject.GetComponent<Image>();

            if (image == null)
            {
                image = gameObject.AddComponent<Image>();
            }

            return image;
        }

        private static void SaveScene()
        {
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }
    }
}