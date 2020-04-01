using NBLD.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.UseActions
{
    public class LadderControl : InsideUseAction
    {
        public Transform otherSide;

        public override void DoAction(InsideCharController controller)
        {
            controller.TransportTo(otherSide);
        }
    }
}


