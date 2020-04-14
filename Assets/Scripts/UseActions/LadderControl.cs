using NBLD.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.UseActions
{
    public class LadderControl : InsideUseAction
    {
        public int destinationFloor;
        public Transform destination;

        public override void DoAction(InsideCharBehaviour controller)
        {
            controller.TransportToFloor(destinationFloor, destination);
        }
    }
}


