using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipArmAction : ActionObject
{
    public ShipMovement shipMovement;
    private Repairable repairable;
    public bool isLeftArm;
    public Animator armAnimator;
    private string animName = "LeverPull";
    public static float animTime = 1.4f;
    Timer animTimer;

    void Start()
    {
        repairable = GetComponent<Repairable>();
        animTimer = new Timer(animTime);
    }

    void Update() {
        animTimer.Update(Time.deltaTime);
    }

    public override void DoAction(InputController controller)
    {
        if (repairable.IsRepaired() && !animTimer.IsRunning())
        {
            animTimer.Start();
            shipMovement.Rotate(isLeftArm);
            armAnimator.SetTrigger(animName);
        }
        else {
            repairable.StartRepairing();
        }
    }

    public override void OnExitAction()
    {
        base.OnExitAction();
        repairable.StopRepairing();
    }
}
