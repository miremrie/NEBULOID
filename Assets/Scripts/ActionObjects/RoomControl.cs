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
    private CharController repairingCharacter;

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

    public override void DoAction(CharController controller)
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

    public override void OnExitAction(CharController controller) {
        if (repairingCharacter == controller)
            repairable.StopRepairing();
    }
}