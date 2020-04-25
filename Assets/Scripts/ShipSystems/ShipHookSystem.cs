using DynamicCamera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.ShipSystems
{
    public class ShipHookSystem : ShipSystem
    {
        public Animator hookAnimator;

        public float fireMaxSpeed, retractSpeed;
        public float accelerationTime;
        public ShipAudioController audioController;
        private Timer fireTimer;
        public AnimationCurve acceleration;
        public float shipPullSpeed;
        public float maxDistance;
        private float minDistance = 0.1f;
        public Transform shipCenterOrigin;
        public Transform hookOrigin;
        public Transform rightHookPivot, leftHookPivot;
        public ShipMovement ship;
        public Collider2D triggerCollider;
        bool wasShot = false;
        bool shotPrepFinished = false;
        bool retracting = false;
        bool hitSomething = false;
        private Quaternion systemRotWhenFired, hookRotWhenFired;
        public float previousHookRotation;
        public GameObject hookLight;
        [Header("Camera")]
        public CameraController camController;
        private CamSet camSet;
        public CamZone camZone;
        private const string cameraSetName = "gameplay";

        private const string hookOpenAnim = "HookOpen";
        private const string hookCloseAnim = "HookClose";
        private const string hookGrabAnim = "HookGrab";
        private const float hookGrabAnimOffset = 0.6667f;
        private GameObject grabbedObject;
        private bool isGrabbedObjectDraggable = false;
        private bool isGrabbing = false;


        public override void Initialize()
        {
            if (!initialized)
            {
                base.Initialize();
                fireTimer = new Timer(accelerationTime);
                camSet = camController.FindSet(cameraSetName);
                OffsetColliderBasedOnCollision();
                previousHookRotation = rightHookPivot.eulerAngles.z;
            }
        }

        protected override void Update()
        {
            base.Update();
            Move();
        }

        private void Move()
        {
            fireTimer.Update(Time.deltaTime);
            if (wasShot && shotPrepFinished)
            {
                float currentSpeed;
                if (!retracting) //Hook flying
                {
                    currentSpeed = GetFireSpeed();
                    transform.position = transform.position + (GetDirFromBase() * currentSpeed * Time.deltaTime);

                    if (Vector3.Distance(transform.position, hookOrigin.position) >= maxDistance)
                    {
                        StartRetracting();
                        hookAnimator.SetTrigger(hookCloseAnim);
                    }
                }
                else if (hitSomething) //Pull ship to hook
                {
                    Vector3 keepPosition = this.transform.position;
                    currentSpeed = shipPullSpeed;
                    ship.MoveShip(GetDirFromBase() * currentSpeed * Time.deltaTime);
                    this.transform.position = keepPosition;
                    UpdateGrabDrag();
                    if (Vector3.Distance(transform.position, hookOrigin.position) <= minDistance)
                    {
                        ResetHook();
                    }
                }
                else //Hook retracting
                {
                    currentSpeed = retractSpeed;
                    transform.position = transform.position + (-GetDirFromBase() * currentSpeed * Time.deltaTime);
                    if (Vector3.Distance(transform.position, hookOrigin.position) <= minDistance)
                    {
                        ResetHook();
                    }
                }
                audioController.UpdateHookSpeedPitch(currentSpeed, fireMaxSpeed);
            }
        }

        private void LateUpdate()
        {
            if (wasShot)
            {
                AdjustRotation();
            }
        }

        private float GetFireSpeed()
        {
            return acceleration.Evaluate(fireTimer.GetCurrentTimePercentClamped()) * fireMaxSpeed;
        }

        private void AdjustRotation()
        {
            //hookOrigin.rotation
            //transform.rotation = hookRotWhenFired;
            shipCenterOrigin.rotation = systemRotWhenFired;
        }
        private void StartRetracting()
        {
            retracting = true;
            audioController.StopCableForward();
            audioController.PlayCableBack();
        }
        private void ResetHook()
        {
            audioController.StopCableBack();
            transform.position = hookOrigin.position;
            wasShot = false;
            retracting = false;
            hitSomething = false;
            isGrabbing = false;
            isGrabbedObjectDraggable = false;
            shotPrepFinished = false;
            hookAnimator.SetTrigger(hookCloseAnim);
            ship.UnlockHook();
            ship.UnlockMovement();
            grabbedObject = null;
            camSet.camZones.Remove(camZone);
            hookLight.SetActive(false);
        }
        private void FinishShotPrep()
        {
            shotPrepFinished = true;
            audioController.PlayCableForward();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (wasShot && !retracting && collision.CompareTag(Tags.OBSTACLE))
            {
                PrepareGrab(collision.gameObject);
            }
        }

        public override void DoAction()
        {
            if (!ship.AreHooksLocked())
            {
                audioController.PlayHookOpen();
                systemRotWhenFired = shipCenterOrigin.rotation;
                hookRotWhenFired = transform.rotation;
                wasShot = true;
                ship.LockHook();
                fireTimer.Start();
                hookAnimator.ResetTrigger(hookCloseAnim);
                hookAnimator.ResetTrigger(hookGrabAnim);
                hookAnimator.SetTrigger(hookOpenAnim);
                camSet.camZones.Add(camZone);
                hookLight.SetActive(true);
            }
        }

        private Vector3 GetDirFromBase()
        {
            return (hookOrigin.transform.position - ship.transform.position).normalized;
        }

        public override bool ReadyToUse()
        {
            return base.ReadyToUse() && !wasShot && !ship.AreHooksLocked();
        }

        private void PrepareGrab(GameObject grabbed)
        {
            audioController.PlayHookBitePrep();
            grabbedObject = grabbed;
            Obstacle obs = grabbedObject.GetComponent<Obstacle>();
            isGrabbedObjectDraggable = obs != null;
            hookAnimator.SetTrigger(hookGrabAnim);
        }

        private void Grab()
        {
            audioController.PlayHookBite();
            previousHookRotation = rightHookPivot.eulerAngles.z;
            hitSomething = true;
            StartRetracting();
            isGrabbing = true;
            ship.LockMovement();
        }

        private void StopGrab()
        {
            isGrabbing = false;
        }

        private void UpdateGrabDrag()
        {
            if (hitSomething && isGrabbing && grabbedObject != null && isGrabbedObjectDraggable)
            {
                float angle = rightHookPivot.eulerAngles.z - previousHookRotation;
                grabbedObject.transform.RotateAround(transform.position, Vector3.forward, angle);
                previousHookRotation = rightHookPivot.eulerAngles.z;
            }
        }

        private void OffsetColliderBasedOnCollision()
        {
            float initalOffsetX = triggerCollider.offset.x;
            float initalOffsetY = triggerCollider.offset.y;
            float animationOffsetX = hookGrabAnimOffset * fireMaxSpeed;
            if (initalOffsetX < 0)
            {
                animationOffsetX = -animationOffsetX;
            }
            triggerCollider.offset = new Vector2(initalOffsetX + animationOffsetX, initalOffsetY);
        }

        //Audio - animation based events
        private void AudioPlayHookOpenAndLocked()
        {
            audioController.PlayHookOpenAndLock();
        }
    }
}
