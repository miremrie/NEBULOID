using NBLD.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.ShipSystems
{

    public class ShipSystem : MonoBehaviour
    {
        public float cooldownTime = 1f;
        private Timer cooldownTimer;
        protected bool initialized = false;

        protected virtual void Start()
        {
            Initialize();
        }
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
                initialized = true;
            }
        }

        protected virtual void Update()
        {
            UpdateCooldownTimer();
        }

        private void UpdateCooldownTimer()
        {
            cooldownTimer.Update(Time.deltaTime);
        }
        public virtual void DoAction(CharBehaviour charBehaviour)
        {
            DoAction();
        }
        public virtual void DoAction()
        {
            cooldownTimer.Start();
        }

        public virtual bool ReadyToUse()
        {

            return !cooldownTimer.IsRunning();
        }
    }
}