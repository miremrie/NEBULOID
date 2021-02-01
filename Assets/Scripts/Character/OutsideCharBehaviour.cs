﻿using NBLD.ShipSystems;
using NBLD.UI;
using NBLD.UseActions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.Character
{
    public class OutsideCharBehaviour : CharBehaviour
    {
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
        [Header("Oxygen")]
        public MaskedSlider oxygenSlider;
        public Oxygen oxygen;
        public ShipEjectSystem shipEjectSystem;

        protected override void Start()
        {
            base.Start();
            moveTimer = new Timer(moveSpeed.keys[moveSpeed.length - 1].time);
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
            oxygen.current = oxygen.max;
            oxygenSlider.gameObject.SetActive(true);
        }
        public override void Deactivate()
        {
            base.Deactivate();
            oxygenSlider.gameObject.SetActive(false);
            boostVFX.SetActive(false);
        }
        private void Update()
        {
            UpdateRotation();
            UpdateMoveTimer();
            UpdateOxygen();
        }
        private void FixedUpdate()
        {
            UpdateMovement();
        }
        //Movement
        private void UpdateMoveTimer()
        {
            /*if (!moveTimer.IsRunning())
            {
                moveTimer.Start();
            }
            moveIntensityUI.UpdateValue(GetCurrentSpeed());
            moveTimer.Update(Time.deltaTime);*/
        }
        private Vector2 GetBoostMovement()
        {
            float speed = standardMoveSpeed + moveSpeed.Evaluate(moveTimer.GetCurrentTime()) * boostMoveSpeed;
            moveTimer.Update(Time.deltaTime);
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
            if (!moving)
            {
                //rb2D.velocity = Vector2.Lerp(Vector2.zero, rb2D.velocity, velocityDropPercentAfterLaunch);
            }
            else
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
            TryExecuteAction(UseActionButton.Action);
        }

        public override void OnMoveAssistPerformed()
        {
            base.OnMoveAssistPerformed();
            if (!moveTimer.IsRunning())
            {
                moveTimer.Start();
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
            oxygenSlider.UpdateValue(oxygen.current);
            if (!oxygen.HasOxygen())
            {
                Die();
            }
        }
        public void Die()
        {
            charController.Die();
        }
        #endregion
    }

}
