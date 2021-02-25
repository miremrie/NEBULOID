using System.Collections;
using System.Collections.Generic;
using NBLD.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace NBLD.Cameras
{
    public class ContextualCamera : MonoBehaviour
    {
        public Transform transformToFollow;
        private Vector3 initialOffset;
        public CameraController mainCameraController;
        public ContextCamDisplay camDisplay;
        public Camera cam;
        public RawImage displayImage;
        public Image borderImage;
        public Color borderImageMinColor, borderImageMaxColor;
        public bool captureEnabled;
        public float mainCamTriggerSize = 20f;
        public float smoothTime;
        public AnimationCurve smoothCurve;
        private bool MainCamSizeConditionMet => mainCameraController.cam.orthographicSize >= mainCamTriggerSize;
        public bool capturing;
        private Timer smoothColorTimer;

        private void Awake()
        {
            smoothColorTimer = new Timer(smoothTime, false, true);
            initialOffset = transform.position;
        }
        private void OnEnable()
        {
            mainCameraController.onCameraMoved += OnMainCameraMoved;
        }
        private void OnDisable()
        {
            mainCameraController.onCameraMoved -= OnMainCameraMoved;
        }

        private void Update()
        {
            UpdateCapturingState();
            UpdateDisplayColor();
        }
        private void LateUpdate()
        {
            FollowTransform();
        }
        public void Activate()
        {
            captureEnabled = true;
            camDisplay.gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            captureEnabled = false;
            camDisplay.gameObject.SetActive(false);
        }
        private void FollowTransform()
        {
            if (transformToFollow != null)
            {
                this.transform.position = transformToFollow.position + initialOffset;
            }
        }
        private void UpdateCapturingState()
        {
            if (capturing)
            {
                if (!captureEnabled || !MainCamSizeConditionMet)
                {
                    capturing = false;
                    smoothColorTimer.StartBackward();
                }
            }
            else
            {
                if (captureEnabled && MainCamSizeConditionMet)
                {
                    capturing = true;
                    smoothColorTimer.StartForward();
                }
            }
        }
        private void UpdateDisplayColor()
        {
            float t = smoothCurve.Evaluate(smoothColorTimer.GetCurrentTimePercentClamped());
            displayImage.color = new Color(1, 1, 1, t);
            borderImage.color = Color.Lerp(borderImageMinColor, borderImageMaxColor, t);
        }
        public void OnMainCameraMoved()
        {
        }

        public void SetFollowTarget(Transform target)
        {
            this.transformToFollow = target;
        }
    }
}
