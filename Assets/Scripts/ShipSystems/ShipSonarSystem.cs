using DynamicCamera;
using NBLD.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.ShipSystems
{
    public class ShipSonarSystem : ShipSystem
    {
        public Transform SonarMovingLightPivot;
        private Timer sonarMoveTimer;
        public float sonarMoveTime;
        private bool sonarStarted;
        public ShipAudioController audioController;
        public GameObject audioEmitter;
        [Header("Camera")]
        public CameraController camController;
        public CamZone sonarCamZone;
        public float camZoomTime;
        public float revertTime;
        public float zoomRadius;
        public AnimationCurve radiusOverTimeCurve;
        public AnimationCurve revertRadiusCurve;
        private const string cameraSetName = "gameplay";
        private float sonarCamStartRadius;
        private Timer revertTimer;
        private bool revertStarted = false;
        private CamSet camSet;

        public override void Initialize()
        {
            if (!initialized)
            {
                base.Initialize();
                sonarCamStartRadius = sonarCamZone.radius;
                camSet = camController.FindSet(cameraSetName);
                sonarMoveTimer = new Timer(sonarMoveTime);
                revertTimer = new Timer(revertTime);
            }
        }

        protected override void Update()
        {
            base.Update();
            UpdateSonar();
        }

        private void UpdateSonar()
        {
            //SonarCircleLight.enabled = repairable.IsRepaired();
            if (sonarStarted)
            {
                if (sonarMoveTimer.IsTimerDone())
                {
                    sonarStarted = false;
                    SonarMovingLightPivot.gameObject.SetActive(false);
                    revertTimer.Restart();
                    revertStarted = true;
                    audioController.StopSonar(audioEmitter);
                }
                SonarMovingLightPivot.transform.Rotate(0, 0, 2 * 360 * Time.deltaTime / sonarMoveTime);
                AnimateCamZoneRadius();
            }
            if (revertStarted)
            {
                AnimateCamZoneRadius();
                if (revertTimer.IsTimerDone())
                {
                    ResetCamZone();
                    revertStarted = false;

                }
            }
        }
        private void ResetCamZone()
        {
            camSet.camZones.Remove(sonarCamZone);
            sonarCamZone.radius = sonarCamStartRadius;
        }
        private void AnimateCamZoneRadius()
        {
            if (revertTimer.IsRunning())
            {
                float timePercent = revertTimer.GetCurrentTimePercentClamped();
                sonarCamZone.radius = Mathf.Lerp(sonarCamStartRadius, zoomRadius, revertRadiusCurve.Evaluate(timePercent));
            }
            else
            {
                float time = sonarMoveTimer.GetCurrentTime();
                if (time <= camZoomTime)
                {
                    sonarCamZone.radius = Mathf.Lerp(sonarCamStartRadius, zoomRadius, radiusOverTimeCurve.Evaluate(time / camZoomTime));
                }
            }
        }
        public override void DoAction()
        {
            base.DoAction();
            SonarMovingLightPivot.gameObject.SetActive(true);
            sonarStarted = true;
            sonarMoveTimer.Restart();
            camSet.camZones.Add(sonarCamZone);
            //camController.ChangeSizeOverTime(newCamSize, camZoomTime);
            audioController.ActivateSonar(camZoomTime, audioEmitter);
        }

    }
}