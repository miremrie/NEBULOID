using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonarAction : ActionObject
{
    public Transform SonarMovingLightPivot;
    public Light SonarMovingLight;
    public Light SonarCircleLight;
    private Timer sonarMoveTimer;
    public float sonarMoveTime;
    private bool sonarStarted;
    public CameraController camController;
    public float newCamSize;
    public float camZoomTime;
    public Repairable repairable;
    public AudioController audioController;

    private void Awake()
    {
        sonarMoveTimer = new Timer(sonarMoveTime);
    }

    private void Update()
    {
        SonarCircleLight.enabled = repairable.IsRepaired();

        if (sonarStarted )
        {
            if (!sonarMoveTimer.IsRunning())
            {
                sonarStarted = false;
                SonarMovingLight.enabled = false;
                camController.RevertToStandardSize(camZoomTime);
                audioController.RevertSonar(camZoomTime);
            }
            sonarMoveTimer.Update(Time.deltaTime);
            SonarMovingLightPivot.transform.Rotate(0,0,2* 360 * Time.deltaTime / sonarMoveTime);
        }
    }

    public override bool IsActionObjectReady()
    {
        return base.IsActionObjectReady() && !sonarStarted;
    }
    public override void DoAction(InputController controller)
    {
        if (repairable.IsRepaired())
        {
            SonarMovingLight.enabled = true;
            sonarStarted = true;
            sonarMoveTimer.Start();
            camController.ChangeSizeOverTime(newCamSize, camZoomTime);
            audioController.ActivateSonar(camZoomTime);
        }
        else
        {
            repairable.StartRepairing();
        }

    }

    public override void OnExitAction()
    {
        base.OnExitAction();
        repairable.StopRepairing();
    }
}
