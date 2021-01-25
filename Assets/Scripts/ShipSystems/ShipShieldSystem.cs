using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.ShipSystems
{
    public class ShipShieldSystem : ShipSystem
    {
        [Header("Components")]
        public Animator animator;
        public GameObject collidersRoot;
        [Header("Values")]
        public float duration;
        private const string shieldOpenAnim = "Open";
        private const string shieldCloseAnim = "Close";
        private Timer durationTimer;
        private bool isShieldOpen;

        public override void Initialize()
        {
            if (!initialized)
            {
                durationTimer = new Timer(duration);
                DeactivateShield();
            }
            base.Initialize();

        }
        protected override void Update()
        {
            base.Update();
            if (isShieldOpen)
            {
                durationTimer.Update(Time.deltaTime);
                if (!durationTimer.IsRunning())
                {
                    DeactivateShield();
                }
            }
        }
        public override void DoAction()
        {
            base.DoAction();
            ActivateShield();
        }

        private void ActivateShield()
        {
            durationTimer.Start();
            isShieldOpen = true;
            animator.SetTrigger(shieldOpenAnim);
            collidersRoot.SetActive(true);
        }
        private void DeactivateShield()
        {
            isShieldOpen = false;
            animator.SetTrigger(shieldCloseAnim);

            collidersRoot.SetActive(false);
        }
    }
}

