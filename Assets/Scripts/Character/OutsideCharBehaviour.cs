using NBLD.Cameras;
using NBLD.Ship;
using NBLD.UI;
using NBLD.UseActions;
using NBLD.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.Character
{
    public class OutsideCharBehaviour : CharBehaviour
    {
        [Header("Components")]
        //public ContextualCamera exteriorContextCamera;
        public Transform hoseAttachSpot;
        [Header("Rotation")]
        public Transform rotationCenter;
        public float minRotAngle = 2f;
        public float rotationSpeed = 2f;
        private bool isRotating = false;
        private Quaternion targetRotation;
        [Header("Movement")]
        public float standardMoveSpeed = 1f;
        public float boostMoveSpeed;
        public AnimationCurve moveSpeed;
        [Range(0, 1)]
        public float velocityDropPercentAfterLaunch;
        public float maxVelocityMag = 15f;
        public MaskedSlider moveIntensityUI;
        public GameObject boostVFX;
        private Timer moveTimer;
        private Vector2 moveDirection;
        private Vector2 boostDirection;
        private float overridenSpeedFactor;
        private bool isSpeedOverriden = false;
        private bool isBoosting = false;
        private const float boostSFXPercentStop = 0.9f;
        public float haulingSpeedModifier = 0.5f;
        private bool hauling;
        private HaulUseAction currentlyHauling;
        [Header("Oxygen")]
        public MaskedSlider oxygenSlider;
        public Oxygen oxygen;
        public ShipEjectSystem shipEjectSystem;


        protected override void Start()
        {
            base.Start();
            moveTimer = new Timer(moveSpeed.keys[moveSpeed.length - 1].time, false, true);
            //moveTimer.Start();
            oxygen.SetProvider(shipEjectSystem);
            moveIntensityUI.Initalize(0, standardMoveSpeed);
            oxygenSlider.Initalize(oxygen.min, oxygen.max);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            //moveIntensityUI.gameObject.SetActive(true);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            //moveIntensityUI.gameObject.SetActive(false);
        }
        public override void Activate()
        {
            base.Activate();
            oxygen.ResetToMax();
            oxygenSlider.gameObject.SetActive(true);
            /*exteriorContextCamera.transformToFollow = transform;
            exteriorContextCamera.Activate();*/
        }
        public override void Deactivate()
        {
            base.Deactivate();
            oxygenSlider.gameObject.SetActive(false);
            boostVFX.SetActive(false);
            //exteriorContextCamera.Deactivate();
        }
        protected override void BehaviourUpdate()
        {
            UpdateRotation();
            UpdateOxygen();
        }
        protected override void BehaviourFixedUpdate()
        {
            UpdateMovement();
        }
        public override Vector3 GetLookDirection()
        {
            return -transform.right;
        }
        //Movement
        private Vector2 GetBoostMovement()
        {
            float speed = standardMoveSpeed + moveSpeed.Evaluate(moveTimer.GetCurrentTime()) * boostMoveSpeed;
            Vector2 force = boostDirection * speed;
            return force;
        }
        private Vector2 GetNormalMovement()
        {
            float speed = standardMoveSpeed;
            Vector2 force = moveDirection * speed;
            return force;
        }
        private void UpdateMovement()
        {
            bool moving = (moveDirection.x != 0 || moveDirection.y != 0 || (moveTimer.IsRunning()));
            if (isBoosting && moveTimer.GetCurrentTimePercent() >= boostSFXPercentStop)
            {
                isBoosting = false;
                charAudio.PlayJetpackStop();
            }
            if (moving)
            {
                Vector2 force = Vector2.zero;
                float maxMag = maxVelocityMag;
                if (moveTimer.IsRunning())
                {
                    force = GetBoostMovement();
                    maxMag = (force.magnitude / standardMoveSpeed) * maxVelocityMag;
                }
                else
                {
                    force = GetNormalMovement();
                }
                if (isSpeedOverriden)
                {
                    force = force * overridenSpeedFactor;
                }
                rb2D.AddForce(force);
                rb2D.velocity = Vector2.ClampMagnitude(rb2D.velocity, maxMag);
            }
            boostVFX.SetActive(moveTimer.IsRunning());
        }
        private void UpdateRotation()
        {
            if (isRotating)
            {
                //float angle = Mathf.LerpAngle(transform.rotation.eulerAngles.z, targetRotation.eulerAngles.z, rotationSpeed * Time.deltaTime);
                float totalAngle = Mathf.DeltaAngle(transform.rotation.eulerAngles.z, targetRotation.eulerAngles.z);
                float dAngle = Mathf.LerpAngle(0, totalAngle, rotationSpeed * Time.deltaTime);
                if (totalAngle == 0)
                {
                    return;
                }
                if (Mathf.Abs(totalAngle) > Mathf.Abs(minRotAngle))
                {
                    if (dAngle > 0)
                    {
                        dAngle = Mathf.Clamp(dAngle, minRotAngle, totalAngle);

                    }
                    else
                    {
                        dAngle = Mathf.Clamp(dAngle, totalAngle, -minRotAngle);
                    }
                    transform.RotateAround(rotationCenter.position, Vector3.forward, dAngle);
                }
                else
                {
                    //transform.RotateAround(rotationCenter.position, Vector3.forward, totalAngle);
                }
                //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        //Actions
        public void SetOverridenSpeedFactor(float overridenSpeedFactor)
        {
            isSpeedOverriden = true;
            this.overridenSpeedFactor = overridenSpeedFactor;
        }
        public void ReleaseOverrideSpeed()
        {
            isSpeedOverriden = false;
        }
        public override void ExecuteAction(UseAction action)
        {
            OutsideUseAction iuAction = (OutsideUseAction)action;
            iuAction.DoAction(this);
        }
        public override void DismissAction(UseAction action)
        {
            OutsideUseAction iuAction = (OutsideUseAction)action;
            iuAction.OnExitAction(this);
        }
        //Events
        public override void OnMovement(Vector2 movement)
        {
            base.OnMovement(movement);
            moveDirection = movement;
            if (movement.x != 0 || movement.y != 0)
            {
                boostDirection = moveDirection;
                targetRotation = Quaternion.Euler(0, 0, (Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg) - 180);
                //LaunchMovement(movement);
                isRotating = true;
            }
            else
            {
                isRotating = false;
            }
        }
        public override void OnAction()
        {
            base.OnAction();
            if (TryExecuteAction(UseActionButton.Action))
            {
                //Action succeded
            }
            else
            {
                //Action not available - tool time
                if (charController.IsToolAvailable())
                {
                    charController.ActivateTool();
                }
            }
        }

        public override void OnMoveAssistPerformed()
        {
            base.OnMoveAssistPerformed();
            if (!moveTimer.IsRunning())
            {
                moveTimer.Restart();
                charAudio.PlayJetpackStart();
                isBoosting = true;
            }
            //LaunchMovement();
        }
        public override void OnMoveAssistStarted()
        {
            base.OnMoveAssistStarted();
        }
        #region Oxygen
        public void UpdateOxygen()
        {
            oxygen.UpdateOxygen(Time.deltaTime);
            oxygenSlider.UpdateValue(oxygen.Current);
            if (!oxygen.HasOxygen())
            {
                Die();
            }
        }
        public void GetHit(float oxygenPercentDamage)
        {
            oxygen.ReduceByTotalPercent(oxygenPercentDamage);
            oxygenSlider.UpdateValue(oxygen.Current);
            charAudio.PlayHurt();
        }
        public void Die()
        {
            charController.Die();
        }
        #endregion
        #region Hauling
        public void SetHauling(HaulUseAction haulObject)
        {
            if (IsHauling())
            {
                currentlyHauling.StopHauling();
            }
            currentlyHauling = haulObject;
            hauling = true;
            SetOverridenSpeedFactor(haulingSpeedModifier);
        }
        public bool IsHauling()
        {
            return hauling;
        }
        public void StopHauling()
        {
            hauling = false;
            ReleaseOverrideSpeed();
        }

        #endregion
    }

}
