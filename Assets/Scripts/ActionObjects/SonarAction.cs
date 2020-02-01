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

    private void Awake()
    {
        sonarMoveTimer = new Timer(sonarMoveTime);
    }

    private void Update()
    {
        if (sonarStarted )
        {
            if (!sonarMoveTimer.IsRunning())
            {
                sonarStarted = false;
                SonarMovingLight.enabled = false;
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
        SonarMovingLight.enabled = true;
        sonarStarted = true;
        sonarMoveTimer.Start();
    }


}
