using Inventory.Scripts.Core.Helper;
using UnityEngine;

namespace Inventory.Scripts.Core.Items.Grids.Helper
{
    public static class RectHelper
    {
        public static Vector2 GetSizeDeltaFittingChildren(Transform transform)
        {
            var sizeFittingAllChildren = Vector2.zero;

            // Iterate through each child of the parent
            foreach (Transform child in transform)
            {
                var childRectTransform = child.GetComponent<RectTransform>();

                if (childRectTransform == null) continue;

                sizeFittingAllChildren.x = Mathf.Max(sizeFittingAllChildren.x,
                    childRectTransform.anchoredPosition.x + childRectTransform.sizeDelta.x);
                sizeFittingAllChildren.y = Mathf.Max(sizeFittingAllChildren.y,
                    -childRectTransform.anchoredPosition.y + childRectTransform.sizeDelta.y);
            }

            return sizeFittingAllChildren;
        }

        public static void SetLeftTopParentEdge(RectTransform rectTransform)
        {
            SetLeftTopParentEdge(rectTransform, rectTransform);
        }

        public static void SetLeftTopParentEdge(RectTransform rectTransform, RectTransform parentRectTransform)
        {
            rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, parentRectTransform.rect.width);
            rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, parentRectTransform.rect.height);
        }

        public static void SetAnchorsForGrid(RectTransform rectTransform)
        {
            if (rectTransform == null)
            {
                Debug.LogError("rectTransform is null... Could not update the anchors".Configuration());
                return;
            }

            var imageAnchoredPosition = new Vector2(0, 1);

            rectTransform.anchoredPosition = imageAnchoredPosition;
            rectTransform.anchorMin = imageAnchoredPosition;
            rectTransform.anchorMax = imageAnchoredPosition;
            rectTransform.pivot = imageAnchoredPosition;
        }

        public static Color ChangeAlphaColor(Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        public static void SetDisplayFillerProperties(RectTransform rectTransform)
        {
            if (rectTransform == null)
            {
                Debug.LogError("rectTransform is null... Could not set display filler properties".Configuration());
                return;
            }

            var zero = new Vector2(0, 0);

            rectTransform.anchoredPosition = zero;
            rectTransform.anchorMin = zero;
            rectTransform.anchorMax = zero;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
        }

        public static void SetOptionsControllerProperties(RectTransform rectTransform)
        {
            if (rectTransform == null)
            {
                Debug.LogError("rectTransform is null... Could not set display filler properties".Configuration());
                return;
            }

            var anchors = new Vector2(0, 1);

            rectTransform.anchoredPosition = anchors;
            rectTransform.anchorMin = anchors;
            rectTransform.anchorMax = anchors;
            rectTransform.pivot = anchors;
        }

        public static void SetOptionButtonProperties(RectTransform rectTransform)
        {
            if (rectTransform == null)
            {
                Debug.LogError("rectTransform is null... Could not set display filler properties".Configuration());
                return;
            }

            var zero = new Vector2(0, 0);

            rectTransform.anchoredPosition = zero;
            rectTransform.anchorMin = zero;
            rectTransform.anchorMax = zero;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
        }

        public static void SetOptionButtonTextProperties(RectTransform rectTransform)
        {
            if (rectTransform == null)
            {
                Debug.LogError("rectTransform is null... Could not set display filler properties".Configuration());
                return;
            }


            rectTransform.anchoredPosition = new Vector2(0f, 0f);
            rectTransform.anchorMin = new Vector2(0f, 0f);
            rectTransform.anchorMax = new Vector2(1f, 1f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(0f, 0f);
        }
    }
}