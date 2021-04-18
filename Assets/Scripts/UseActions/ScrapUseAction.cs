using System.Collections;
using System.Collections.Generic;
using NBLD.Character;
using UnityEngine;

namespace NBLD.UseActions
{
    public class ScrapUseAction : UseAction
    {
        public CampaignLevel campaignLevel;
        public SystemName systemName;
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
                    user.ship.GetComponent<ShipAssembler>().AddAvailableSystem(systemName);
                    user.RemoveDestroyedAction(this);
                    Destroy(this.gameObject);
                    campaignLevel.EnterGarage();
                }
            }
        }
    }

}
