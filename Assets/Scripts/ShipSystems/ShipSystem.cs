using NBLD.Character;
using NBLD.UseActions;
using NBLD.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.Ship
{

    public class ShipSystem : MonoBehaviour
    {
        public float cooldownTime = 1f;
        private Timer cooldownTimer;
        protected bool initialized = false;
        protected RoomControl roomControl;

        /*protected virtual void Start()
        {
            Initialize();
        }*/
        public void Reinitialize()
        {
            initialized = false;
            Initialize();
        }
        public virtual void Initialize()
        {
            if (!initialized)
            {
                cooldownTimer = new Timer(cooldownTime);
                cooldownTimer.Finish();
                initialized = true;
            }
        }
        public void AssignRoomControl(RoomControl roomControl)
        {
            this.roomControl = roomControl;
        }

        protected virtual void Update()
        {
        }
        public virtual void DoAction(CharBehaviour charBehaviour)
        {
            DoAction();
        }
        public virtual void DoAction()
        {
            cooldownTimer.Restart();
        }

        public virtual bool ReadyToUse()
        {
            return cooldownTimer.IsTimerDone();
        }
        public virtual void OnExitAction(CharBehaviour charBehaviour)
        {

        }
    }
}