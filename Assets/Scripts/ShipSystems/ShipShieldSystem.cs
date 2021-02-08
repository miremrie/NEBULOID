using System.Collections;
using System.Collections.Generic;
using NBLD.Utils;
using UnityEngine;

namespace NBLD.Ship
{
    public class ShipShieldSystem : ShipSystem
    {
        [Header("Components")]
        public Animator animator;
        public GameObject collidersRoot;
        public ShipAudioController shipAudioController;
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
                if (durationTimer.IsTimerDone())
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
            durationTimer.Restart();
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

        #region AnimationEvents
        public void OnAnimOpenExtrude()
        {
            shipAudioController.PlayShieldOpenExtrude();
        }
        public void OnAnimOpenIntrudeFanOut()
        {
            shipAudioController.PlayShieldOpenIntrudeFanOut();
        }
        public void OnAnimCloseExtrudeFanIn()
        {
            shipAudioController.PlayShieldCloseExtrudeFanIn();
        }
        public void OnAnimCloseIntrude()
        {
            shipAudioController.PlayShieldCloseIntrude();
        }
        #endregion
    }
}

