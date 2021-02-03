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
        public CameraController mainCameraController;
        public Camera cam;
        public RawImage displayImage;
        public bool captureEnabled;
        public float mainCamTriggerSize = 20f;
        public float smoothTime;
        private bool MainCamSizeConditionMet => mainCameraController.cam.orthographicSize >= mainCamTriggerSize;
        private bool capturing;
        private Timer smoothColorTimer;

        private void Awake()
        {
            smoothColorTimer = new Timer(smoothTime);
        }
        private void OnEnable()
        {
            mainCameraController.onCameraMoved += OnMainCameraMoved;
        }
        private void OnDisable()
        {
            mainCameraController.onCameraMoved -= OnMainCameraMoved;
        }

        public void Activate()
        {
            captureEnabled = true;

        }
        public void Deactivate()
        {
            captureEnabled = false;
        }

        public void OnMainCameraMoved()
        {
            if (capturing)
            {

            }
        }
    }
}
