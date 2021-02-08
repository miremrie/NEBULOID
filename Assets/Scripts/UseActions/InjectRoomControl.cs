using System.Collections;
using System.Collections.Generic;
using NBLD.Character;
using NBLD.Ship;
using UnityEngine;

namespace NBLD.UseActions
{
    public class InjectRoomControl : OutsideUseAction
    {
        public ShipEjectSystem ejectSystem;

        public override void DoAction(OutsideCharBehaviour behaviour)
        {
            base.DoAction(behaviour);
            if (ejectSystem.ReadyToInject())
            {
                ejectSystem.InjectCharacter(behaviour);
            }
        }
    }

}
