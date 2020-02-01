using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderAction : ActionObject
{
    public Transform otherSide;

    public override void DoAction(InputController controller)
    {
        controller.MoveUpTo(otherSide);
    }
}
