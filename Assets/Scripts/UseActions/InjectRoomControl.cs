using System.Collections;
using System.Collections.Generic;
using NBLD.Character;
using NBLD.Ship;
using UnityEngine;

namespace NBLD.UseActions
{
    public class InjectRoomControl : UseAction
    {
        public ShipEjectSystem ejectSystem;
        public override int DefaultActionPriority => 80;

        public override bool AvailableForChar(CharController charController)
        {
            return charController.GetState() == CharState.Outside;
        }
        public override void DoAction(CharController user)
        {
            if (ejectSystem.ReadyToInject())
            {
                ejectSystem.InjectCharacter(user);
            }
        }
    }

}
