using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipArmAction : ActionObject
{
    public ShipMovement shipMovement;
    private Repairable repairable;
    public bool isLeftArm;

    void Start()
    {
        repairable = GetComponent<Repairable>();
    }

    public override void DoAction(InputController controller)
    {
        if (repairable.IsRepaired())
        {
            shipMovement.Rotate(isLeftArm);
        }
        else {
            repairable.StartRepairing();
        }
    }
}
