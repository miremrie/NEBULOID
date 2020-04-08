﻿using NBLD.Input;
using NBLD.UseActions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.Character
{
    public enum UseActionButton
    {
        Action, SubAction, Up, Down
    }
    public enum CharacterState
    {
        Inside, Outside, Dead
    }
    public class CharController : MonoBehaviour
    {
        public ShipSystems.ShipMovement ship;
        [Header("Input")]
        public CharInputManager charInputManager;

        [Header("Animation")]
        public Animator animator;
        public SpriteRenderer charSpriteRenderer;

        [Header("Physics")]
        public List<Collider2D> colliders = new List<Collider2D>();
        public Rigidbody2D rb2D;

        [Header("Behaviours")]
        public InsideCharBehaviour insideBehaviour;
        public OutsideCharBehaviour outsideBehaviour;
        private CharBehaviour activeBehaviour;

        [Header("States")]
        public CharacterState state;

        [Header("Transition")]
        public float transitionSpeed;
        public float transitionMinOffset = 0.01f;
        private Transform transitionStart, transitionDst;
        private bool inTransition = false, transitionStartReached = false, transitionPrepAnimationOver;
        private const string transitionPrepAnimKey = "PrepTransition";
        private const string transitionEndAnimKey = "EndTransition";


        //Actions
        protected Dictionary<UseActionButton, UseAction> availableActions = new Dictionary<UseActionButton, UseAction>();

        public void Start()
        {
            charInputManager.RegisterController(this);
            insideBehaviour.Initialize(this, rb2D, charSpriteRenderer, animator);
            outsideBehaviour.Initialize(this, rb2D, charSpriteRenderer, animator);
            outsideBehaviour.enabled = false;
            ChangeState(CharacterState.Inside);
        }

        private void Update()
        {
            UpdateEjectTransition();
        }

        //States
        public void ChangeState(CharacterState state)
        {
            if (state == CharacterState.Outside)
            {
                ActivateBehaviour(outsideBehaviour);
                ship.RemoveDependentTransform(transform);
            } else if (state == CharacterState.Inside)
            {
                ActivateBehaviour(insideBehaviour);
                ship.AddDependentTransform(transform);
            } else if (state == CharacterState.Dead)
            {
                //isDead = true;
            }
        }
        private void ActivateBehaviour(CharBehaviour newActive)
        {
            if (activeBehaviour != null)
            {
                activeBehaviour.enabled = false;
            }
            activeBehaviour = newActive;
            activeBehaviour.enabled = true;
        }

        //Actions
        public bool TryGetAction(UseActionButton useButton, out UseAction action)
        {
            if (CheckIfActionAvailable(useButton))
            {
                action = availableActions[useButton];
                return true;
            } else
            {
                action = null;
                return false;
            }
        }
        public virtual bool CheckIfActionAvailable(UseActionButton useButton)
        {
            return availableActions.ContainsKey(useButton) 
                && availableActions[useButton].AvailableForCharacterState() == state;
        }
        /*public virtual void CheckThenExecuteAction(UseActionButton useButton)
        {
            if (CheckIfActionAvailable(useButton))
            {
                ExecuteAction(availableActions[useButton]);
            }
        }*/

        public void SetCollidersActive(bool active)
        {
            foreach(Collider2D col in colliders)
            {
                col.enabled = active;
            }
        }

        //Eject Transition
        public void PerformTransition(Transform start, Transform dst, CharacterState newState, bool clearActions = true)
        {
            inTransition = true;
            transitionStart = start;
            transitionDst = dst;
            SetCollidersActive(false);
            ChangeState(newState);
            if (clearActions)
            {
                availableActions.Clear();
            }
        }
        public void StopTransition()
        {
            animator.SetTrigger(transitionEndAnimKey);
            inTransition = false;
            transitionStartReached = false;
            transitionPrepAnimationOver = false;
            SetCollidersActive(true);
        }
        private void UpdateTransitionStartPos()
        {
            if (transitionMinOffset >= Vector3.Distance(transform.position, transitionStart.position))
            {
                transitionStartReached = true;
                transform.position = transitionStart.position;
                animator.SetTrigger(transitionPrepAnimKey);
            }
            Vector3 direction = (transitionStart.position - transform.position).normalized;
            transform.Translate(direction * transitionSpeed * Time.deltaTime);
        }
        private void TransitionPrepAnimationOver()
        {
            transitionPrepAnimationOver = true;
        }
        public void UpdateEjectTransition()
        {
            if (inTransition)
            {
                if (!transitionStartReached)
                {
                    UpdateTransitionStartPos();
                } else if (transitionPrepAnimationOver) {
                    if (transitionMinOffset >= Vector3.Distance(transform.position, transitionDst.position))
                    {
                        transform.position = transitionDst.position;
                        StopTransition();
                    }
                    Vector3 direction = (transitionDst.position - transform.position).normalized;
                    transform.Translate(direction * transitionSpeed * Time.deltaTime);
                }
            }
        }

        //Collisions
        protected void OnTriggerEnter2D(Collider2D col)
        {
            if (col.tag == Tags.ACTION_OBJECT)
            {
                UseAction newControl = col.gameObject.GetComponent<UseAction>();
                if (newControl.AvailableForCharacterState() == state)
                {
                    if (availableActions.ContainsKey(newControl.actionButton))
                    {
                        availableActions[newControl.actionButton] = newControl;
                    }
                    else
                    {
                        availableActions.Add(newControl.actionButton, newControl);
                    }
                }
            }
        }

        protected void OnTriggerExit2D(Collider2D col)
        {
            if (col.tag == Tags.ACTION_OBJECT)
            {
                UseAction leavingActionControl = col.gameObject.GetComponent<UseAction>();
                if (leavingActionControl.AvailableForCharacterState() == state)
                {
                    if (availableActions.ContainsKey(leavingActionControl.actionButton))
                    {
                        activeBehaviour.DismissAction(availableActions[leavingActionControl.actionButton]);
                        availableActions.Remove(leavingActionControl.actionButton);
                    }
                }
            }
        }

        //Input Events
        public void OnMovement(Vector2 movement)
        {
            if (!inTransition)
            {
                activeBehaviour.OnMovement(movement);
            }
        }
        public void OnUp()
        {
            if (!inTransition)
            {
                activeBehaviour.OnUp();
            }
        }

        public void OnDown()
        {
            if (!inTransition)
            {
                activeBehaviour.OnDown();
            }
        }

        public void OnAction()
        {
            if (!inTransition)
            {
                activeBehaviour.OnAction();
            }
        }

        public void OnSubAction()
        {
            if (!inTransition)
            {
                activeBehaviour.OnSubAction();
            }
        }

        public void OnTalk()
        {
            if (!inTransition)
            {
                activeBehaviour.OnTalk();
            }
        }

        public void OnMoveAssistStarted()
        {
            if (!inTransition)
            {
                activeBehaviour.OnMoveAssistStarted();
            }
        }

        public void OnMoveAssistPerformed()
        {
            if (!inTransition)
            {
                activeBehaviour.OnMoveAssistPerformed();
            }
        }
    }
}