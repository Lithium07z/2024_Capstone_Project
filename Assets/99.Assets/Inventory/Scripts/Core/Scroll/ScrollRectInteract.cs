using Inventory.Scripts.Core.ScriptableObjects;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Events.Interact;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inventory.Scripts.Core.Scroll
{
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollRectInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Broadcasting on...")] [SerializeField]
        private OnScrollRectInteractEventChannelSo onScrollRectInteractEventChannelSo;

        private ScrollRect _scrollRect;

        private void Awake()
        {
            _scrollRect = GetComponent<ScrollRect>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onScrollRectInteractEventChannelSo.RaiseEvent(_scrollRect);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onScrollRectInteractEventChannelSo.RaiseEvent(null);
        }

#if UNITY_EDITOR

        [Header("Debug Editor")] [SerializeField]
        private bool enableGizmos;
        [SerializeField, Tooltip("Set this property, to debug the padding scroll with gizmos.")] private InventorySettingsAnchorSo inventorySettingsAnchorSo;

        private static readonly Vector2 KShadowOffset = new(1, -1);
        private static readonly Color KShadowColor = new(0, 0, 0, 0.5f);
        private const float KDottedLineSize = 8f;

        private void OnDrawGizmos()
        {
            if (!enableGizmos) return;
            
            if (!inventorySettingsAnchorSo) return;
            
            var gui = GetComponent<RectTransform>();

            var rectInOwnSpace = gui.rect;
            // Rect rectInUserSpace = rectInOwnSpace;
            var rectInParentSpace = rectInOwnSpace;
            var ownSpace = gui.transform;
            // Transform userSpace = ownSpace;
            var parentSpace = ownSpace;
            if (ownSpace.parent != null)
            {
                parentSpace = ownSpace.parent;
                var localPosition = ownSpace.localPosition;
                rectInParentSpace.x += localPosition.x;
                rectInParentSpace.y += localPosition.y;

                parentSpace.GetComponent<RectTransform>();
            }

            // patSilva's post: https://forum.unity.com/threads/ui-image-component-raycast-padding-needs-a-gizmo.1019260/#post-6828020
            // The image.raycastPadding order of the Vector4 is:
            // X = Left
            // Y = Bottom
            // Z = Right
            // W = Top

            var rectBorders = GetRect(rectInParentSpace);
            var rectPadding = GetPaddingRect(rectInParentSpace);

            // uncomment below line to show only when rect tool is active
            // if (Tools.current == Tool.Rect)
            {
                //change the color of the handles as you wish
                Handles.color = Color.green;
                DrawRect(rectBorders, parentSpace, true);
                
                // Paddings gizmos
                Handles.color = Color.red;
                DrawRect(rectPadding, parentSpace, true);
            }
        }

        private Rect GetRect(Rect rectInParentSpace)
        {
            var paddingRect = new Rect(rectInParentSpace);
            
            var image = GetComponent<Image>();

            if (image != null)
            {
                paddingRect.xMin += image.raycastPadding.x;
                paddingRect.xMax -= image.raycastPadding.z;
                paddingRect.yMin += image.raycastPadding.y;
                paddingRect.yMax -= image.raycastPadding.w;   
            }
            
            return paddingRect;
        }

        private Rect GetPaddingRect(Rect rectInParentSpace)
        {
            var paddingRect = new Rect(rectInParentSpace);

            var scrollPadding = inventorySettingsAnchorSo.InventorySettingsSo.HoldScrollPadding;

            paddingRect.xMin += scrollPadding;
            paddingRect.xMax -= scrollPadding;
            paddingRect.yMin += scrollPadding;
            paddingRect.yMax -= scrollPadding;
            
            return paddingRect;
        }

        private static void DrawRect(Rect rect, Transform space, bool dotted)
        {
            var p0 = space.TransformPoint(new Vector2(rect.x, rect.y));
            var p1 = space.TransformPoint(new Vector2(rect.x, rect.yMax));
            var p2 = space.TransformPoint(new Vector2(rect.xMax, rect.yMax));
            var p3 = space.TransformPoint(new Vector2(rect.xMax, rect.y));

            if (!dotted)
            {
                Handles.DrawLine(p0, p1);
                Handles.DrawLine(p1, p2);
                Handles.DrawLine(p2, p3);
                Handles.DrawLine(p3, p0);
            }
            else
            {
                DrawDottedLineWithShadow(KShadowColor, KShadowOffset, p0, p1, KDottedLineSize);
                DrawDottedLineWithShadow(KShadowColor, KShadowOffset, p1, p2, KDottedLineSize);
                DrawDottedLineWithShadow(KShadowColor, KShadowOffset, p2, p3, KDottedLineSize);
                DrawDottedLineWithShadow(KShadowColor, KShadowOffset, p3, p0, KDottedLineSize);
            }
        }

        private static void DrawDottedLineWithShadow(Color shadowColor, Vector2 screenOffset, Vector3 p1, Vector3 p2,
            float screenSpaceSize)
        {
            var cam = Camera.current;
            if (!cam || Event.current.type != EventType.Repaint)
                return;

            var oldColor = Handles.color;

            // shadow
            shadowColor.a *= oldColor.a;
            Handles.color = shadowColor;
            Handles.DrawDottedLine(
                cam.ScreenToWorldPoint(cam.WorldToScreenPoint(p1) + (Vector3)screenOffset),
                cam.ScreenToWorldPoint(cam.WorldToScreenPoint(p2) + (Vector3)screenOffset), screenSpaceSize);

            // line itself
            Handles.color = oldColor;
            Handles.DrawDottedLine(p1, p2, screenSpaceSize);
        }

#endif
    }
}