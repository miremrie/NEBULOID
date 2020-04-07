using NBLD.Input;
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
        [Header("Input")]
        public CharInputManager charInputManager;

        [Header("Animation")]
        public Animator animator;
        public SpriteRenderer charSpriteRenderer;

        [Header("Physics")]
        public List<Collider2D> colliders = new List<Collider2D>();

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
        private bool isTransitioning = false, transitionStartReached = false, transitionPrepAnimationOver;
        private const string transitionPrepAnimKey = "PrepTransition";
        private const string transitionEndAnimKey = "EndTransition";


        //Actions
        protected Dictionary<UseActionButton, UseAction> availableActions = new Dictionary<UseActionButton, UseAction>();

        public void Start()
        {
            charInputManager.RegisterController(this);
            insideBehaviour.Initialize(this, charSpriteRenderer, animator);
            outsideBehaviour.Initialize(this, charSpriteRenderer, animator);
            activeBehaviour = insideBehaviour;
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
                activeBehaviour = outsideBehaviour;
            } else if (state == CharacterState.Inside)
            {
                activeBehaviour = insideBehaviour;
            } else if (state == CharacterState.Dead)
            {
                //isDead = true;
            }
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
            isTransitioning = true;
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
            isTransitioning = false;
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
            if (isTransitioning)
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
            activeBehaviour.OnMovement(movement);
        }
        public void OnUp()
        {
            activeBehaviour.OnUp();
        }

        public void OnDown()
        {
            activeBehaviour.OnDown();
        }

        public void OnAction()
        {
            activeBehaviour.OnAction();
        }

        public void OnSubAction()
        {
            activeBehaviour.OnSubAction();
        }

        public void OnTalk()
        {
            activeBehaviour.OnTalk();
        }

        public void OnMoveAssistStarted()
        {
            activeBehaviour.OnMoveAssistStarted();
        }

        public void OnMoveAssistPerformed()
        {
            activeBehaviour.OnMoveAssistPerformed();
        }
    }
}