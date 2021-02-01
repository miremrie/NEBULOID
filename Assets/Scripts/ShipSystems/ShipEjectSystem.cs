using System.Collections;
using System.Collections.Generic;
using NBLD.Character;
using NBLD.Graphics.Path;
using UnityEngine;

namespace NBLD.ShipSystems
{
    public class ShipEjectSystem : ShipSystem, IOxygenProvider
    {
        public Transform outsidePipeEnd, insidePipeEnd, outsideFloorPos, insideFloorPos;
        public PathAnimator hoseAnimator;
        public float maxDistance;
        public float pullTowardSpeedMultiplier;
        public float maxPullTowardsDistance = 1f;
        public AnimationCurve pullTowardSpeedCurve;
        private CharController outsideChar;
        private bool occupied = false;
        private bool pullTowardsStarted;
        private Timer pullTowardTimer;
        [Header("Oxygen")]

        public float workingOxygenPerSecond;
        public float brokenOxygenPerSecond;

        public override void Initialize()
        {
            if (!initialized)
            {
                pullTowardTimer = new Timer(pullTowardSpeedCurve.keys[pullTowardSpeedCurve.keys.Length - 1].time);
                pullTowardTimer.Start();
            }
            base.Initialize();
        }
        protected override void Update()
        {
            base.Update();
            UpdateDistance();
            UpdatePullTowards();
        }

        public bool ReadyToInject()
        {
            return occupied;
        }
        public override void DoAction(CharBehaviour charBehaviour)
        {
            base.DoAction(charBehaviour);
            if (!occupied)
            {
                EjectCharacter(charBehaviour);
            }
            else
            {
                StartPullTowards(charBehaviour);
            }
        }

        private void EjectCharacter(CharBehaviour charBehaviour)
        {
            occupied = true;
            outsideChar = charBehaviour.charController;
            outsideChar.PerformTransition(insideFloorPos, outsideFloorPos, CharacterState.Outside, true);
            hoseAnimator.gameObject.SetActive(true);
            hoseAnimator.endFollow = outsideChar.outsideBehaviour.hoseAttachSpot;
        }

        public void InjectCharacter(CharBehaviour charBehaviour)
        {
            if (ReadyToInject())
            {
                if (charBehaviour.charController == outsideChar)
                {
                    occupied = false;
                    pullTowardsStarted = false;
                    outsideChar.PerformTransition(outsideFloorPos, insideFloorPos, CharacterState.Inside, true);
                    hoseAnimator.gameObject.SetActive(false);
                }
            }
        }

        private void StartPullTowards(CharBehaviour puller)
        {
            if (outsideChar.state == CharacterState.Outside || outsideChar.state == CharacterState.Dead)
            {
                pullTowardsStarted = true;
                pullTowardTimer.Start();
            }
        }

        public override void OnExitAction(CharBehaviour charBehaviour)
        {
            base.OnExitAction(charBehaviour);
            StopPullTowards();
        }
        private void StopPullTowards()
        {
            outsideChar.ApplyForceMovement(Vector2.zero, true);
            pullTowardsStarted = false;
        }
        private void UpdateDistance()
        {
            if (occupied)
            {
                float distance = Vector3.Distance(outsideChar.transform.position, outsidePipeEnd.position);
                if (distance > maxDistance)
                {
                    Vector3 newPoint = (outsideChar.transform.position - outsidePipeEnd.position).normalized * maxDistance + outsidePipeEnd.position;
                    outsideChar.transform.position = newPoint;
                }
            }
        }

        private void UpdatePullTowards()
        {
            if (pullTowardsStarted)
            {
                if (maxPullTowardsDistance > Vector2.Distance(outsidePipeEnd.position, outsideChar.transform.position) || (outsideChar.state != CharacterState.Outside && outsideChar.state != CharacterState.Dead))
                {
                    StopPullTowards();
                }
                else
                {
                    if (!pullTowardTimer.IsRunning())
                    {
                        //pullTowardTimer.Start();
                    }
                    else
                    {
                        pullTowardTimer.Update(Time.deltaTime);
                    }
                    Vector2 direction = outsidePipeEnd.position - outsideChar.transform.position;
                    Vector2 force = direction.normalized * GetCurrentPullTowardSpeed();
                    outsideChar.ApplyForceMovement(force, true);
                }
            }
        }
        private float GetCurrentPullTowardSpeed()
        {
            return pullTowardSpeedCurve.Evaluate(pullTowardTimer.GetCurrentTime()) * pullTowardSpeedMultiplier;
        }

        public float GetOxygenPerSecond()
        {
            if (roomControl.repairable.IsRepaired())
            {
                return workingOxygenPerSecond;
            }
            else
            {
                return brokenOxygenPerSecond;
            }
        }
    }
}
