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
        public override int DefaultActionPriority => 100;
        public override bool AvailableForChar(CharController charController)
        {
            return haulUseAction.IsInsideShip && charController.GetState() == CharState.Inside && haulUseAction.IsBeingHauled() && haulUseAction.GetHauler() == charController;
        }

        public override void DoAction(CharController user)
        {
            if (haulUseAction.IsBeingHauled())
            {
                if (user == haulUseAction.GetHauler())
                {

                    user.ship.GetComponentInParent<ShipStatus>().FuelCollected();
                    user.RemoveDestroyedAction(this);
                    Destroy(this.gameObject);
                }
            }
        }
    }
}

