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
        private float transportMinOffset = 0.1f;
        private Transform transportDestination;
        private Vector2 currentMovement = Vector2.zero;

        private bool lockedByTransport;


        //Animation
        private const string moveAnimName = "Move";
        private const string moveVerAnimName = "MoveVer";

        private void Update()
        {
            UpdateMovement();
            UpdateTransport();

            AnimateMovement();
        }
        //Animations
        private void AnimateMovement()
        {
            int moveDirection = GetCurrentMoveDirection();
            animator.SetBool(moveAnimName, moveDirection != 0);
            if (moveDirection != 0)
            {
                spriteRenderer.flipX = moveDirection > 0;
            }
            animator.SetBool(moveVerAnimName, isTransporting);
        }
        //Movement
        private void UpdateMovement()
        {
            if (!lockedByTransport)
            {
                transform.Translate(Vector3.right * GetCurrentMoveDirection() * moveSpeed * Time.deltaTime);
            }
        }
        private int GetCurrentMoveDirection()
        {
            return (int)currentMovement.normalized.x;
        }

        //Transport
        public void TransportTo(Transform destination)
        {
            lockedByTransport = true;
            isTransporting = true;
            transportDestination = destination;
        }

        private void UpdateTransport()
        {
            if (isTransporting)
            {
                if (Vector3.Distance(transportDestination.position, transform.position) <= transportMinOffset)
                {
                    transform.position = transportDestination.position;
                    isTransporting = false;
                    lockedByTransport = false;
                }
                else
                {
                    transform.Translate((transportDestination.position - transform.position).normalized * transportSpeed * Time.deltaTime);
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
    }
}