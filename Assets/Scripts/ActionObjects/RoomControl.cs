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
    private InputController repairingCharacter;

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
        if (hasAssignedSystem && repairable.IsRepaired() && shipSystem.ReadyToUse())
        {
            roomAnimator.SetBool(activeAnimName, true);
            shipSystem.DoAction();
        }
        else
        {
            repairingCharacter = controller;
            repairable.StartRepairing();
        }
    }

    public override void OnExitAction(InputController controller) {
        if (repairingCharacter == controller)
            repairable.StopRepairing();
    }
}