using NBLD.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.UseActions
{
    public class LadderControl : UseAction
    {
        public int destinationFloor;
        public Transform destination;
        public override int DefaultActionPriority => 60;

        public override bool AvailableForChar(CharController charController)
        {
            return charController.GetState() == CharState.Inside;
        }
        public override void DoAction(CharController user)
        {
            user.insideBehaviour.TransportToFloor(destinationFloor, destination);
        }
    }
}


