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

    void Start()
    {
        repairable = GetComponent<Repairable>();
    }

    public override void DoAction(InputController controller)
    {
        if (repairable.IsRepaired())
        {
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
