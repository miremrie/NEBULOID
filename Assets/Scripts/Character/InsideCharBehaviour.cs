using System.Collections;
using System.Collections.Generic;
using NBLD.UseActions;
using NBLD.Utils;
using UnityEngine;

namespace NBLD.Character
{

    public class InsideCharBehaviour : CharBehaviour
    {
        [Header("Movement")]
        public float moveSpeed;
        public float transportSpeed = 5f;
        private bool isTransporting;
        private bool transportInCorrectX;
        private float transportMinOffset = 0.1f;
        private Transform transportDestination;
        private Vector2 currentMovement = Vector2.zero;
        private int currentFloor = 1;
        private float transportAudioFrequency = 0.05f;
        private Timer transportTimer;
        private bool transportAscending;
        private Vector3 currentLookDirection = Vector3.left;

        private bool lockedByTransport;
        [Header("Idle Behaviour")]
        public float idleBehaviourTriggerTime = 20f;
        private Timer idleBehaviourTimer;

        //Animation
        private const string moveAnimName = "Move";
        private const string moveVerAnimName = "MoveVer";

        protected void Awake()
        {
            transportTimer = new Timer(transportAudioFrequency, true);
            idleBehaviourTimer = new Timer(idleBehaviourTriggerTime);
        }
        public override void Activate()
        {
            base.Activate();
            idleBehaviourTimer.Restart();
        }
        public override void Deactivate()
        {
            if (!initialized)
            {
                return;
            }
            base.Deactivate();
            ResetMovement();
            ResetAnimations();
        }
        protected override void BehaviourUpdate()
        {
            if (!lockedByTransport)
            {
                UpdateMovement(GetCurrentMoveDirection());
            }
            UpdateTransport();
            UpdateIdleBehaviour();

        }
        public override Vector3 GetLookDirection()
        {
            return currentLookDirection;
        }
        //Reset
        private void ResetMovement()
        {
            currentMovement = Vector3.zero;

        }
        private void ResetAnimations()
        {
            animator.SetBool(moveAnimName, false);
            animator.SetBool(moveVerAnimName, false);
        }
        //Animations
        private void AnimateMovement()
        {
            int moveDirection = GetCurrentMoveDirection();
            animator.SetBool(moveAnimName, moveDirection != 0);
            if (moveDirection != 0)
            {
                bool lookingRight = moveDirection > 0;
                spriteRenderer.flipX = lookingRight;
                currentLookDirection = (lookingRight) ? Vector3.right : Vector3.left;
                //charAudio.PlayFootsteps();
            }
        }
        //Movement
        private void UpdateMovement(int direction)
        {
            transform.Translate(Vector3.right * direction * moveSpeed * Time.deltaTime);
            //Debug.Log($"Move{Vector3.right * direction*moveSpeed*Time.deltaTime}");
            AnimateMovement();
        }
        private int GetCurrentMoveDirection()
        {
            return (int)currentMovement.normalized.x;
        }

        //Transport
        public void TransportToFloor(int newFloor, Transform destination)
        {
            lockedByTransport = true;
            isTransporting = true;
            transportInCorrectX = false;
            transportDestination = destination;
            transportAscending = newFloor > currentFloor;
            currentFloor = newFloor;
            charAudio.SetEnvironmentBasedOnFloor(currentFloor);
        }

        private void UpdateTransport()
        {
            if (isTransporting)
            {
                if (!transportInCorrectX)
                {
                    float xDistance = transportDestination.position.x - transform.position.x;
                    if (Mathf.Abs(xDistance) <= transportMinOffset)
                    {
                        transform.position = new Vector2(transportDestination.position.x, transform.position.y);
                        transportInCorrectX = true;
                    }
                    else
                    {
                        UpdateMovement((int)Mathf.Sign(xDistance));
                    }
                    return;
                }
                else
                {
                    if (transportTimer.IsTimerDone())
                    {
                        if (transportAscending)
                        {
                            charAudio.PlayLadderAsc();
                        }
                        else
                        {
                            charAudio.PlayLadderDsc();
                        }
                        transportTimer.Restart();
                    }
                    if (Vector3.Distance(transportDestination.position, transform.localPosition) <= transportMinOffset)
                    {
                        transform.position = transportDestination.position;
                        isTransporting = false;
                        lockedByTransport = false;
                    }
                    else
                    {
                        transform.Translate((transportDestination.position - transform.position).normalized * transportSpeed * Time.deltaTime);
                    }
                    animator.SetBool(moveVerAnimName, isTransporting);
                }
            }
        }

        //Actions


        //Events
        public override void OnMovement(Vector2 movement)
        {
            currentMovement = movement;
            ResetIdleBehaviour();
        }
        public override void OnAction()
        {
            base.OnAction();
            bool actionExecuted = false;
            if (TryExecuteAction(UseActionButton.Action))
            {
                actionExecuted = true;
                //Action available and executed
            }
            else if (charController.IsToolAvailable())
            {
                actionExecuted = true;
                charController.ActivateTool();
            }
            if (actionExecuted)
            {
                ResetIdleBehaviour();
            }
        }
        public override void OnSubAction()
        {
            base.OnSubAction();
            if (TryExecuteAction(UseActionButton.SubAction))
            {
                //Action executed;
            }
        }
        public override void OnUp()
        {
            base.OnUp();
            TryExecuteAction(UseActionButton.Up);
            ResetIdleBehaviour();
        }
        public override void OnDown()
        {
            base.OnDown();
            TryExecuteAction(UseActionButton.Down);
            ResetIdleBehaviour();
        }
        //Idle Behaviour
        private void UpdateIdleBehaviour()
        {
            if (idleBehaviourTimer.IsTimerDone())
            {
                idleBehaviourTimer.Restart();
                charAudio.PlayIdleBehaviour();
            }
        }
        private void ResetIdleBehaviour()
        {
            idleBehaviourTimer.Restart();
        }
        //Audio
        public void AudioPlayFootstep()
        {
            charAudio.PlayFootstep();
        }
    }
}