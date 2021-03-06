﻿using NBLD.Character;
using NBLD.Ship;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.UseActions
{
    public class RoomControl : InsideUseAction
    {
        public Repairable repairable;
        public Animator roomAnimator;
        private const string activeAnimName = "RoomActionActivated";
        public ShipSystem shipSystem;
        public bool hasAssignedSystem = false;
        private InsideCharBehaviour workingChar;


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

        public override void DoAction(InsideCharBehaviour behaviour)
        {
            workingChar = behaviour;
            if (hasAssignedSystem && repairable.IsRepaired() && shipSystem.ReadyToUse())
            {
                roomAnimator.SetBool(activeAnimName, true);
                shipSystem.DoAction(behaviour);
            }
            else
            {
                repairable.StartRepairing();
            }
        }

        public override void OnExitAction(InsideCharBehaviour behaviour)
        {
            if (hasAssignedSystem && workingChar == behaviour)
            {
                workingChar = null;
                repairable.StopRepairing();
                shipSystem.OnExitAction(behaviour);
            }
        }
    }
}