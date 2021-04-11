using NBLD.Character;
using NBLD.Ship;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.UseActions
{
    public class RoomControl : UseAction
    {
        public Repairable repairable;
        public Animator roomAnimator;
        private const string activeAnimName = "RoomActionActivated";
        public ShipSystem shipSystem;
        public bool hasAssignedSystem = false;
        private InsideCharBehaviour workingChar;

        public override bool AvailableForCharState(CharState charState)
        {
            return charState == CharState.Inside;
        }


        public void InitializeSystem(ShipSystem shipSystem)
        {
            hasAssignedSystem = shipSystem != null;
            this.shipSystem = shipSystem;
            if (shipSystem != null)
            {
                shipSystem.AssignRoomControl(this);
            }
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

        public override void DoAction(CharController user)
        {
            workingChar = user.insideBehaviour;
            if (hasAssignedSystem && repairable.IsRepaired() && shipSystem.ReadyToUse())
            {
                roomAnimator.SetBool(activeAnimName, true);
                shipSystem.DoAction(workingChar);
            }
            else
            {
                repairable.StartRepairing(workingChar.GetRepairSpeed());
            }
        }

        public override void OnExitAction(CharController user)
        {
            if (hasAssignedSystem && workingChar == user)
            {
                workingChar = null;
                repairable.StopRepairing();
                shipSystem.OnExitAction(user);
            }
        }
    }
}