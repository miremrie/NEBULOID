using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderControl : ShipActionControl
{
    public Transform otherSide;

    public override void DoAction(InputController controller)
    {
        controller.MoveUpTo(otherSide);
    }
}
