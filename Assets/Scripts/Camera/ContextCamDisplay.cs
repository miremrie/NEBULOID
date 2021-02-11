using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NBLD.Cameras
{
    public enum YAnchorType { Bottom, Top };
    public enum XAnchorType { Left, Right };
    public class ContextCamDisplay : MonoBehaviour
    {
        private RectTransform rectTransform;
        public RawImage displayImage;
        public Image borderImage;
        public YAnchorType yAnchorType;
        public XAnchorType xAnchorType;
        public float xPercentSize;
        public float xEdgeOffsetPercent;
        public float yEdgeOffsetPercent;
        public float xySizeRatio = 1;
        private float XYScreenRatio => ((float)Screen.width) / Screen.height;


        private void Awake()
        {
            rectTransform = (RectTransform)transform;
        }

        private void Update()
        {
            UpdateSizeAndPosition();
        }

        [ContextMenu("Update Position")]
        private void UpdateSizeAndPosition()
        {
            rectTransform = (RectTransform)transform;
            float yPercentSize = xPercentSize * xySizeRatio * XYScreenRatio;
            Vector2 minAnchor = GetMinAnchorPosition(yPercentSize);
            Vector2 maxAnchor = minAnchor + new Vector2(xPercentSize, yPercentSize);
            rectTransform.anchorMin = minAnchor;
            rectTransform.anchorMax = maxAnchor;
            rectTransform.offsetMin = rectTransform.offsetMax = Vector2.zero;
        }

        private Vector2 GetMinAnchorPosition(float yPercentSize)
        {
            float xPos = (xAnchorType == XAnchorType.Left) ? xEdgeOffsetPercent : 1 - xPercentSize - xEdgeOffsetPercent;
            float yPos = (yAnchorType == YAnchorType.Bottom) ? yEdgeOffsetPercent : 1 - yPercentSize - yEdgeOffsetPercent;
            return new Vector2(xPos, yPos);
        }
    }
}

