using System.Collections;
using System.Collections.Generic;
using NBLD.UseActions;
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

        private bool lockedByTransport;

        //Animation
        private const string moveAnimName = "Move";
        private const string moveVerAnimName = "MoveVer";

        protected override void Start()
        {
            base.Start();
            transportTimer = new Timer(transportAudioFrequency);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            ResetMovement();
            ResetAnimations();
        }

        private void Update()
        {
            if (!lockedByTransport)
            {
                UpdateMovement(GetCurrentMoveDirection());
            }
            UpdateTransport();

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
                spriteRenderer.flipX = moveDirection > 0;
                //charAudio.PlayFootsteps();
            }
        }
        //Movement
        private void UpdateMovement(int direction)
        {
            transform.Translate(Vector3.right * direction * moveSpeed * Time.deltaTime);
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
                    } else
                    {
                        UpdateMovement((int)Mathf.Sign(xDistance));
                    }
                    return;
                } else
                {
                    if (transportTimer.IsRunning())
                    {
                        transportTimer.Update(Time.deltaTime);
                    } else
                    {
                        if (transportAscending)
                        {
                            charAudio.PlayLadderAsc();
                        }
                        else
                        {
                            charAudio.PlayerLadderDsc();
                        }
                        transportTimer.Start();
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
        public override void ExecuteAction(UseAction action)
        {
            InsideUseAction iuAction = (InsideUseAction)action;
            iuAction.DoAction(this);
        }
        public override void DismissAction(UseAction action)
        {
            InsideUseAction iuAction = (InsideUseAction)action;
            iuAction.OnExitAction(this);
        }

        //Events
        public override void OnMovement(Vector2 movement)
        {
            currentMovement = movement;
        }
        public override void OnAction()
        {
            base.OnAction();
            TryExecuteAction(UseActionButton.Action);
        }
        public override void OnUp()
        {
            base.OnUp();
            TryExecuteAction(UseActionButton.Up);
        }
        public override void OnDown()
        {
            base.OnDown();
            Debug.Log("Down Action");
            TryExecuteAction(UseActionButton.Down);
        }

        //Audio
        public void AudioPlayFootstep()
        {
            charAudio.PlayFootstep();
        }
    }
}