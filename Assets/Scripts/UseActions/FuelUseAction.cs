using System.Collections;
using System.Collections.Generic;
using NBLD.Character;
using NBLD.Ship;
using UnityEngine;

namespace NBLD.UseActions
{
    public class FuelUseAction : UseAction
    {
        public HaulUseAction haulUseAction;
        public override bool AvailableForCharState(CharState charState)
        {
            return haulUseAction.IsInsideShip && charState == CharState.Inside;
        }

        public override void DoAction(CharController user)
        {
            if (haulUseAction.IsBeingHauled())
            {
                if (user == haulUseAction.GetHauler())
                {

                    user.ship.GetComponent<ShipStatus>().FuelCollected();
                    haulUseAction.StopHauling();
                    Destroy(this.gameObject);
                }
            }
        }
    }
}

