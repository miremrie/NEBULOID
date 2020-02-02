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
    public float sonarCooldown;
    private Timer cooldownTimer;
    public Animator leverAnimator;
    private const string animName = "LeverPull";

    private void Awake()
    {
        sonarMoveTimer = new Timer(sonarMoveTime);
        cooldownTimer = new Timer(sonarCooldown);
    }

    private void Update()
    {
        //SonarCircleLight.enabled = repairable.IsRepaired();
        cooldownTimer.Update(Time.deltaTime);
        if (sonarStarted )
        {
            if (!sonarMoveTimer.IsRunning())
            {
                sonarStarted = false;
                SonarMovingLightPivot.gameObject.SetActive(false);
                camController.RevertToStandardSize(camZoomTime, () => SonarMovingLightPivot.gameObject.SetActive(false) );
                audioController.RevertSonar(camZoomTime);
            }
           
            sonarMoveTimer.Update(Time.deltaTime);
            SonarMovingLightPivot.transform.Rotate(0,0,2 * 360 * Time.deltaTime / sonarMoveTime);
        }
    }

    public override bool IsActionObjectReady()
    {
        return base.IsActionObjectReady() && !sonarStarted && !cooldownTimer.IsRunning();
    }
    public override void DoAction(InputController controller)
    {
        if (repairable.IsRepaired() && !cooldownTimer.IsRunning())
        {
            SonarMovingLightPivot.gameObject.SetActive(true);
            sonarStarted = true;
            sonarMoveTimer.Start();
            camController.ChangeSizeOverTime(newCamSize, camZoomTime);
            audioController.ActivateSonar(camZoomTime);
            leverAnimator.SetTrigger(animName);
            cooldownTimer.Start();
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
