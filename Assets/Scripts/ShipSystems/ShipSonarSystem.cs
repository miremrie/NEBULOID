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
        public CameraController camController;
        public float newCamSize;
        public float camZoomTime;
        public ShipAudioController audioController;

        public override void Initialize()
        {
            if (!initialized)
            {
                base.Initialize();
                sonarMoveTimer = new Timer(sonarMoveTime);
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
                if (!sonarMoveTimer.IsRunning())
                {
                    sonarStarted = false;
                    SonarMovingLightPivot.gameObject.SetActive(false);
                    camController.RevertToStandardSize(camZoomTime, () => SonarMovingLightPivot.gameObject.SetActive(false));
                    audioController.StopSonar();
                }

                sonarMoveTimer.Update(Time.deltaTime);
                SonarMovingLightPivot.transform.Rotate(0, 0, 2 * 360 * Time.deltaTime / sonarMoveTime);
            }
        }
        public override void DoAction()
        {
            base.DoAction();
            SonarMovingLightPivot.gameObject.SetActive(true);
            sonarStarted = true;
            sonarMoveTimer.Start();
            camController.ChangeSizeOverTime(newCamSize, camZoomTime);
            audioController.ActivateSonar(camZoomTime);
        }

    }
}