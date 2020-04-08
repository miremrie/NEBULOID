﻿using NBLD.UI;
using NBLD.UseActions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.Character
{
    public class OutsideCharBehaviour : CharBehaviour
    {
        public Transform hoseAttachSpot;
        [Header("Movement")]
        public float rotationSpeed = 2f;
        public float moveSpeedMultiplier = 1f;
        public AnimationCurve moveSpeed;
        public MaskedSlider moveIntensityUI;
        private bool isRotating = false;
        private Quaternion targetRotation;
        private Timer moveTimer;

        private void Start()
        {
            moveTimer = new Timer(moveSpeed.keys[moveSpeed.length - 1].time);
            moveTimer.Start();
            moveIntensityUI.Initalize(0, moveSpeedMultiplier);
        }

        private void OnEnable()
        {
            moveIntensityUI.gameObject.SetActive(true);
        }
        private void OnDisable()
        {
            moveIntensityUI.gameObject.SetActive(false);
        }
        private void Update()
        {
            UpdateRotation();
            UpdateMoveTimer();
        }
        //Movement
        private void UpdateMoveTimer()
        {
            if (!moveTimer.IsRunning())
            {
                moveTimer.Start();
            }
            moveIntensityUI.UpdateValue(GetCurrentSpeed());
            moveTimer.Update(Time.deltaTime);
        }
        private void LaunchMovement()
        {

            Vector2 force = Vector2.left * GetCurrentSpeed();
            rb2D.AddRelativeForce(force);
            moveTimer.Start();
        }
        private float GetCurrentSpeed()
        {
            return moveSpeed.Evaluate(moveTimer.GetCurrentTime()) * moveSpeedMultiplier;
        }
        private void UpdateRotation()
        {
            if (isRotating)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
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
            if (movement.x != 0 || movement.y != 0)
            {
                targetRotation = Quaternion.Euler(0, 0, (Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg) - 180);
                isRotating = true;
            } else
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
            LaunchMovement();
        }
        public override void OnMoveAssistStarted()
        {
            base.OnMoveAssistStarted();
        }
    }

}
