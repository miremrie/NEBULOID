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
        public float boostDuration;
        public float moveSpeedMultiplier = 1f;
        public AnimationCurve moveSpeed;
        [Range(0, 1)]
        public float velocityDropPercentAfterLaunch;
        public float maxVelocityMag = 15f;
        public MaskedSlider moveIntensityUI;
        public GameObject boostVFX;
        private Timer moveTimer;
        private Vector2 moveDirection;
        private bool isBoostedMovement = false;




        protected override void Start()
        {
            base.Start();
            moveTimer = new Timer(moveSpeed.keys[moveSpeed.length - 1].time);
            //moveTimer.Start();
            moveIntensityUI.Initalize(0, moveSpeedMultiplier);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            moveIntensityUI.gameObject.SetActive(true);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            moveIntensityUI.gameObject.SetActive(false);
        }
        private void Update()
        {
            UpdateRotation();
            UpdateMoveTimer();
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
        private void UpdateMovement()
        {
            if (moveDirection.x != 0 || moveDirection.y != 0)
            {
                float speed = 0f;
                float maxMag = 0f;
                if (moveTimer.IsRunning())
                {
                    Debug.Log("Running");
                    speed = moveSpeed.Evaluate(moveTimer.GetCurrentTime()) * boostMoveSpeed;
                    maxMag = maxVelocityMag * (boostMoveSpeed / standardMoveSpeed);
                    moveTimer.Update(Time.deltaTime);
                    boostVFX.SetActive(true);
                } else
                {
                    boostVFX.SetActive(false);
                    speed = standardMoveSpeed;
                    maxMag = maxVelocityMag;
                }
                //float moveSpeed = isBoostedMovement ? moveSpeedMultiplier : standardMoveSpeed;
                Vector2 force = moveDirection * speed;
                rb2D.AddForce(force);
                rb2D.velocity = Vector2.ClampMagnitude(rb2D.velocity, maxMag);
            }
            else
            {
                rb2D.velocity = Vector2.Lerp(Vector2.zero, rb2D.velocity, velocityDropPercentAfterLaunch);
            }
            Debug.Log(rb2D.velocity);

        }
        private void LaunchMovement()
        {
            LaunchMovement(Vector2.left);
        }
        private void LaunchMovement(Vector2 direction)
        {
            rb2D.velocity = Vector2.Lerp(Vector2.zero, rb2D.velocity, velocityDropPercentAfterLaunch);
            Vector2 force = direction * GetCurrentSpeed();
            rb2D.AddRelativeForce(force);
            rb2D.velocity = Vector2.ClampMagnitude(rb2D.velocity, maxVelocityMag);
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

                    } else
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
                targetRotation = Quaternion.Euler(0, 0, (Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg) - 180);
                //LaunchMovement(movement);
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
    }

}
