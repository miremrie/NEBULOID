using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomControl : ActionControl
{
    private bool actionObjectReady = true;
    public Repairable repairable;
    public Animator roomAnimator;
    private const string activeAnimName = "RoomActionActivated";
    public ShipSystem shipSystem;
    public bool hasAssignedSystem = false;

    public virtual bool IsActionObjectReady()
    {
        return actionObjectReady;
    }

    private void Update()
    {
        if (hasAssignedSystem)
        {
            UpdateRoomAnimationState();
        }

    }

    private void UpdateRoomAnimationState()
    {
        if (shipSystem.ReadyToUse())
        {
            roomAnimator.SetBool(activeAnimName, false);
        }
    }

    public override void DoAction(InputController controller)
    {
        if (repairable.IsRepaired() && shipSystem.ReadyToUse())
        {
            roomAnimator.SetBool(activeAnimName, true);
            shipSystem.DoAction();
        }
        else
        {
            repairable.StartRepairing();
        }
    }

    public override void OnExitAction() {
        repairable.StopRepairing();
    }
}