using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.UI
{
    public class MaskedSlider : MonoBehaviour
    {
        public RectTransform mask;
        public RectTransform fill;
        private float minVal, maxVal;
        public RectTransform.Edge relativeEdge;
        private float size;

        public void Initalize(float minVal, float maxVal)
        {
            this.minVal = minVal;
            this.maxVal = maxVal;
            if(relativeEdge == RectTransform.Edge.Top || relativeEdge == RectTransform.Edge.Bottom)
            {
                size = mask.rect.size.y;
            } else
            {
                size = mask.rect.size.x;
            }
        }

        public void UpdateValue(float value)
        {
            float distance = (value - minVal) / maxVal;
            distance = Mathf.Clamp01(distance);
            distance = 1 - distance;

            mask.SetInsetAndSizeFromParentEdge(relativeEdge, -distance * size, size);
            fill.SetInsetAndSizeFromParentEdge(relativeEdge, distance * size, size);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}

