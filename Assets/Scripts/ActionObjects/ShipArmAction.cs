using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipArmAction : ActionObject
{
    public ShipMovement shipMovement;
    public bool isLeftArm;

    public override void DoAction(InputController controller)
    {
        shipMovement.Rotate(isLeftArm);
    }
}
