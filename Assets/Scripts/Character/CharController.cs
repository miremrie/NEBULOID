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
        Inside, Outside, Dead, Transition
    }
    public class CharController : MonoBehaviour
    {
        public ShipSystems.ShipMovement ship;
        public Audio.CharAudioController charAudio;

        [Header("Input")]
        public CharInputManager charInputManager;

        [Header("Animation")]
        public Animator animator;
        public SpriteRenderer charSpriteRenderer;
        public Transform characterPivot;

        [Header("Physics")]
        public List<Collider2D> colliders = new List<Collider2D>();
        public Rigidbody2D rb2D;


        [Header("Behaviours")]
        public InsideCharBehaviour insideBehaviour;
        public OutsideCharBehaviour outsideBehaviour;
        private CharBehaviour activeBehaviour;
        private CharBehaviour behaviourAfterTransition;

        [Header("States")]
        public CharacterState state;

        [Header("Transition")]
        public float transitionSpeed;
        public float transitionMinOffset = 0.01f;
        public float transitionMinAngleOffset = 5;
        public float rotationSpeed = 45;
        public float targetAngle = 0;
        public Vector3 pivotWhenExiting, pivotWhenEntering;
        public Transform characterCenter;
        private Transform transitionStart, transitionDst;
        private bool inTransition = false, transitionStartReached = false;
        private bool transitionPrepAnimStarted = false, transitionPrepAnimationOver = false, transitionPrepRotationOver = false;
        private const string transitionPrepOutAnimKey = "PrepOutTransition";
        private const string transitionEndOutAnimKey = "EndOutTransition";
        private const string transitionPrepInAnimKey = "PrepInTransition";
        private const string transitionEndInAnimKey = "EndInTransition";
        private const string exitAnimKey = "Exit";
        private const string enterAnimKey = "Enter";
        private const string dieAnimKey = "Die";
        private CharacterState transitionNewState;
        private bool CanRecieveInput => !inTransition && state != CharacterState.Dead;


        //Actions
        protected Dictionary<UseActionButton, UseAction> availableActions = new Dictionary<UseActionButton, UseAction>();

        public void Awake()
        {
            charInputManager.RegisterController(this);
            insideBehaviour.Initialize(this, rb2D, charSpriteRenderer, animator, charAudio);
            outsideBehaviour.Initialize(this, rb2D, charSpriteRenderer, animator, charAudio);
            outsideBehaviour.enabled = false;
            ChangeState(CharacterState.Inside);
            charAudio.SetEnvironmentBasedOnFloor(1);
        }

        private void Update()
        {
            UpdateEjectTransition();
        }

        //States
        private void ChangeState(CharacterState state)
        {
            if (state == CharacterState.Outside)
            {
                ActivateBehaviour(outsideBehaviour);
                ship.RemoveDependentTransform(transform);
            }
            else if (state == CharacterState.Inside)
            {
                ActivateBehaviour(insideBehaviour);
                ship.AddDependentTransform(transform);
            }
            else if (state == CharacterState.Dead)
            {
                animator.SetTrigger(dieAnimKey);
                DeactivateBehaviour();
            }
            else if (state == CharacterState.Transition)
            {
                if (this.state == CharacterState.Outside)
                {
                    ship.AddDependentTransform(transform);
                }
                DeactivateBehaviour();
                //ship.RemoveDependentTransform(transform);
            }
            this.state = state;
        }
        private void DeactivateBehaviour()
        {
            activeBehaviour.enabled = false;
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
        public void Die()
        {
            ChangeState(CharacterState.Dead);
        }

        //Actions
        public bool TryGetAction(UseActionButton useButton, out UseAction action)
        {
            if (CheckIfActionAvailable(useButton))
            {
                action = availableActions[useButton];
                return true;
            }
            else
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
        //Movement
        public void ApplyForceMovement(Vector2 velocity, bool clearPrevious = true)
        {
            if (clearPrevious)
            {
                rb2D.velocity = Vector2.zero;
            }
            rb2D.AddForce(velocity);
        }
        //Eject Transition
        public void PerformTransition(Transform start, Transform dst, CharacterState newState, bool clearActions = true)
        {
            inTransition = true;
            transitionStart = start;
            transitionDst = dst;
            activeBehaviour.DisableCollisions();
            ChangeState(CharacterState.Transition);
            transitionNewState = newState;
            if (newState == CharacterState.Inside)
            {
                behaviourAfterTransition = insideBehaviour;
            }
            else if (newState == CharacterState.Outside)
            {
                behaviourAfterTransition = outsideBehaviour;
            }
            if (clearActions)
            {
                availableActions.Clear();
            }
            /*if (newState == CharacterState.Inside)
            {
                animator.SetTrigger(enterAnimKey);
            } else
            {
                animator.SetTrigger(exitAnimKey);
            }*/
            //If going inside, character should be facing right
            //If going outside, character should be facing left
            rb2D.velocity = Vector2.zero;
            //charSpriteRenderer.flipX = newState == CharacterState.Inside; 
        }
        public void ChangeSpriteLayerDuringTransition()
        {
            behaviourAfterTransition.SetSortingLayer();
        }
        public void FinishTransition()
        {
            inTransition = false;
            transitionStartReached = false;
            transitionPrepAnimationOver = false;
            transitionPrepRotationOver = false;
            transitionPrepAnimStarted = false;
            /*animator.ResetTrigger(transitionEndInAnimKey);
            animator.ResetTrigger(transitionEndOutAnimKey);
            animator.ResetTrigger(transitionPrepInAnimKey);
            animator.ResetTrigger(transitionPrepOutAnimKey);*/
            transform.position = transitionDst.position;
            //transform.position += characterPivot.position;
            characterPivot.localPosition = Vector3.zero;
            ChangeState(transitionNewState);
            activeBehaviour.EnableCollisions();
        }
        private void UpdateTransitionStartPos()
        {
            if (transitionMinOffset >= Vector3.Distance(transform.position, transitionStart.position))
            {
                transitionStartReached = true;
                transform.position = transitionStart.position;
            }
            else
            {
                Vector3 direction = (transitionStart.position - transform.position).normalized;
                transform.Translate(direction * transitionSpeed * Time.deltaTime);

            }
        }
        private void UpdateTransitionStartRot()
        {
            Quaternion targetRot = Quaternion.Euler(0, 0, targetAngle);
            if (transitionMinAngleOffset >= Mathf.Abs(Mathf.DeltaAngle(transform.rotation.eulerAngles.z, targetAngle)))
            {
                transform.rotation = targetRot;
                transitionPrepRotationOver = true;
            }
            else
            {
                float angle = Mathf.LerpAngle(transform.rotation.eulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime);
                transform.RotateAround(characterCenter.position, Vector3.forward, Mathf.DeltaAngle(transform.rotation.eulerAngles.z, angle));
                //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
        }
        private void TransitionPrepAnimationOver()
        {
            transitionPrepAnimationOver = true;
        }
        public void UpdateEjectTransition()
        {
            if (inTransition)
            {
                if (!transitionPrepRotationOver)
                {
                    UpdateTransitionStartRot();
                }
                if (transitionPrepRotationOver && !transitionStartReached)
                {
                    UpdateTransitionStartPos();
                    //transitionStartReached = true;
                }
                if (transitionStartReached && transitionPrepRotationOver && !transitionPrepAnimStarted)
                {
                    transitionPrepAnimStarted = true;
                    if (transitionNewState == CharacterState.Outside)
                    {
                        //animator.SetTrigger(transitionPrepOutAnimKey);
                        characterPivot.localPosition = pivotWhenExiting;
                        animator.SetTrigger(exitAnimKey);

                    }
                    else
                    {
                        //animator.SetTrigger(transitionPrepInAnimKey);
                        characterPivot.localPosition = pivotWhenEntering;
                        animator.SetTrigger(enterAnimKey);
                    }
                }
                /*if (transitionPrepAnimationOver) {
                    FinishTransition();
                    /*if (transitionMinOffset >= Vector3.Distance(transform.position, transitionDst.position))
                    {
                        
                        transform.position = transitionDst.position;
                        /*if (transitionNewState == CharacterState.Outside)
                        {
                            animator.SetTrigger(transitionEndOutAnimKey);
                        }
                        else
                        {
                            animator.SetTrigger(transitionEndInAnimKey);
                        }*/

                /*} else
                {
                    Vector3 direction = (transitionDst.position - transform.position).normalized;
                    transform.Translate(direction * transitionSpeed * Time.deltaTime);
                }*/
                //}
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
            if (CanRecieveInput)
            {
                activeBehaviour.OnMovement(movement);
            }
        }
        public void OnUp()
        {
            if (CanRecieveInput)
            {
                activeBehaviour.OnUp();
            }
        }

        public void OnDown()
        {
            if (CanRecieveInput)
            {
                activeBehaviour.OnDown();
            }
        }

        public void OnAction()
        {
            if (CanRecieveInput)
            {
                activeBehaviour.OnAction();
            }
        }

        public void OnSubAction()
        {
            if (CanRecieveInput)
            {
                activeBehaviour.OnSubAction();
            }
        }

        public void OnTalk()
        {
            if (CanRecieveInput)
            {
                activeBehaviour.OnTalk();
            }
        }

        public void OnMoveAssistStarted()
        {
            if (CanRecieveInput)
            {
                activeBehaviour.OnMoveAssistStarted();
            }
        }

        public void OnMoveAssistPerformed()
        {
            if (CanRecieveInput)
            {
                activeBehaviour.OnMoveAssistPerformed();
            }
        }
    }
}