using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderControl : ActionControl
{
    public Transform otherSide;

    public override void DoAction(Char2Controller controller)
    {
        controller.FixedMoveTo(otherSide);
    }
}
