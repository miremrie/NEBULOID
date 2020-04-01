using NBLD.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.UseActions
{
    public class RoomControl : InsideUseAction
    {
        private bool actionObjectReady = true;
        public Repairable repairable;
        public Animator roomAnimator;
        private const string activeAnimName = "RoomActionActivated";
        public ShipSystem shipSystem;
        public bool hasAssignedSystem = false;
        private InsideCharController repairingCharacter;

        public virtual bool IsActionObjectReady()
        {
            return actionObjectReady;
        }

        private void Update()
        {
            if (hasAssignedSystem)
            {
                UpdateRoomAnimationState();
            }

        }

        private void UpdateRoomAnimationState()
        {
            if (shipSystem.ReadyToUse())
            {
                roomAnimator.SetBool(activeAnimName, false);
            }
        }

        public override void DoAction(InsideCharController controller)
        {
            if (hasAssignedSystem && repairable.IsRepaired() && shipSystem.ReadyToUse())
            {
                roomAnimator.SetBool(activeAnimName, true);
                shipSystem.DoAction();
            }
            else
            {
                repairingCharacter = controller;
                repairable.StartRepairing();
            }
        }

        public override void OnExitAction(InsideCharController controller)
        {
            if (repairingCharacter == controller)
                repairable.StopRepairing();
        }
    }
}