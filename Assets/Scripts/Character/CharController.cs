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
    public enum CharState
    {
        Inside, Outside, Dead, Transition
    }
    public class CharController : MonoBehaviour
    {
        public Ship.ShipMovement ship;
        public Audio.CharAudioController charAudio;

        [Header("Input")]
        public PlayerGameplayInputManager charInputManager;
        private PlayerSessionData playerSessionData;
        private const string graphicsObjectName = "Character Graphics";
        [Header("Char Tool")]
        public Transform equipedToolRoot;
        public CharTool equipedTool;
        private bool hasEquipedTool;
        [Header("Animation")]
        public CharAnimator charAnimator;
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
        private CharState state;
        [Header("Repair")]
        public float baseRepairSpeed = 0.4f;

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
        private const string dieAnimKey = "DiePlayed";
        private const string deadAnimKey = "Dead";
        private CharState transitionNewState;
        private bool CanRecieveInput => !inTransition && state != CharState.Dead;
        private bool initialized = false;
        private bool subscribed = false;
        private bool isHauling = false;
        private HaulUseAction currentlyHauling;


        //Actions
        protected Dictionary<UseActionButton, List<UseAction>> availableActions = new Dictionary<UseActionButton, List<UseAction>>();

        public void Initialize(PlayerSessionData playerSessionData, int startingFloor)
        {
            initialized = true;
            this.playerSessionData = playerSessionData;
            this.charInputManager = playerSessionData.gameplayInputManager;
            this.charAnimator.sprites = playerSessionData.skin.sprites.ToArray();
            animator.runtimeAnimatorController = playerSessionData.skin.animatorController;
            ship.AddDependentTransform(transform);
            SetGameplayInputActive();
            insideBehaviour.Initialize(this, rb2D, charSpriteRenderer, animator, charAudio);
            outsideBehaviour.Initialize(this, rb2D, charSpriteRenderer, animator, charAudio);
            outsideBehaviour.Deactivate();
            ChangeState(CharState.Inside);
            charAudio.SetEnvironmentBasedOnFloor(startingFloor);
            animator.keepAnimatorControllerStateOnDisable = true;
            //Initialize Actions
            availableActions = new Dictionary<UseActionButton, List<UseAction>>();
            Subscribe();
        }
        private void SetGameplayInputActive()
        {
            InputManager.Instance.SetPlayerGameplayEnabled(playerSessionData.playerIndex, true);
        }
        private void OnEnable()
        {
            Subscribe();
            if (initialized)
            {
                ChangeState(state);
            }
        }
        private void OnDisable()
        {
            Unsubscribe();
            insideBehaviour.Deactivate();
            outsideBehaviour.Deactivate();
        }

        private void Update()
        {
            UpdateEjectTransition();
        }

        private void Subscribe()
        {
            if (initialized && !subscribed)
            {

                charInputManager.OnMove += OnMovement;
                charInputManager.OnUp += OnUp;
                charInputManager.OnDown += OnDown;
                charInputManager.OnAction += OnAction;
                charInputManager.OnSubAction += OnSubAction;
                charInputManager.OnTalk += OnTalk;
                charInputManager.OnMoveAssistPerformed += OnMoveAssistPerformed;
                charInputManager.OnMoveAssistStarted += OnMoveAssistStarted;
                subscribed = true;
            }

        }
        private void Unsubscribe()
        {
            if (initialized && subscribed)
            {
                charInputManager.OnMove -= OnMovement;
                charInputManager.OnUp -= OnUp;
                charInputManager.OnDown -= OnDown;
                charInputManager.OnAction -= OnAction;
                charInputManager.OnSubAction -= OnSubAction;
                charInputManager.OnTalk -= OnTalk;
                charInputManager.OnMoveAssistPerformed -= OnMoveAssistPerformed;
                charInputManager.OnMoveAssistStarted -= OnMoveAssistStarted;
                subscribed = false;
            }
        }

        //States
        private void ChangeState(CharState state)
        {
            if (state == CharState.Outside)
            {
                ActivateBehaviour(outsideBehaviour);
                ship.RemoveDependentTransform(transform);
            }
            else if (state == CharState.Inside)
            {
                ActivateBehaviour(insideBehaviour);
                ship.AddDependentTransform(transform);
            }
            else if (state == CharState.Dead)
            {
                animator.SetBool(dieAnimKey, true);
                animator.SetBool(deadAnimKey, true);
                DeactivateBehaviour();
            }
            else if (state == CharState.Transition)
            {
                if (this.state == CharState.Outside)
                {
                    ship.AddDependentTransform(transform);
                }
                DeactivateBehaviour();
                //ship.RemoveDependentTransform(transform);
            }
            this.state = state;
        }
        public CharState GetState()
        {
            return state;
        }
        private void DeactivateBehaviour()
        {
            if (activeBehaviour.enabled != false)
            {
                activeBehaviour.Deactivate();
            }
        }
        private void ActivateBehaviour(CharBehaviour newActive)
        {
            if (activeBehaviour != null)
            {
                DeactivateBehaviour();
            }
            activeBehaviour = newActive;
            activeBehaviour.Activate();
        }
        public void Die()
        {
            ChangeState(CharState.Dead);
        }

        //Actions
        public bool TryGetAction(UseActionButton useButton, out UseAction action)
        {
            int maxPriority = int.MinValue;
            int maxPriorityActionIndex = -1;
            if (availableActions.ContainsKey(useButton))
            {
                for (int i = 0; i < availableActions[useButton].Count; i++)
                {
                    var curAction = availableActions[useButton][i];
                    if (curAction.AvailableForChar(this) && curAction.Priority > maxPriority)
                    {
                        //action = availableActions[useButton][i];
                        maxPriorityActionIndex = i;
                        maxPriority = curAction.Priority;
                    }
                }
            }
            if (maxPriorityActionIndex != -1)
            {
                action = availableActions[useButton][maxPriorityActionIndex];
                return true;
            }
            else
            {
                action = null;
                return false;
            }
        }
        private void AddAction(UseAction action)
        {
            if (!availableActions.ContainsKey(action.actionButton))
            {
                availableActions.Add(action.actionButton, new List<UseAction>());
            }
            if (!availableActions[action.actionButton].Contains(action))
            {
                availableActions[action.actionButton].Add(action);
            }
        }
        private bool RemoveAction(UseAction action)
        {
            if (availableActions.ContainsKey(action.actionButton))
            {
                return availableActions[action.actionButton].Remove(action);
            }
            return false;
        }
        public void RemoveDestroyedAction(UseAction action)
        {
            bool removed = RemoveAction(action);
            if (isHauling && action == currentlyHauling)
            {
                ship.RemoveDependentTransform(action.transform);
            }
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
        public void PerformTransition(Transform start, Transform dst, CharState newState, bool clearActions = true)
        {
            inTransition = true;
            transitionStart = start;
            transitionDst = dst;
            activeBehaviour.DisableCollisions();
            ChangeState(CharState.Transition);
            transitionNewState = newState;
            if (newState == CharState.Inside)
            {
                behaviourAfterTransition = insideBehaviour;
            }
            else if (newState == CharState.Outside)
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
            if (IsHauling())
            {
                currentlyHauling.ChangeInsideShipState(state == CharState.Inside);
                if (state == CharState.Inside)
                {
                    ship.AddDependentTransform(currentlyHauling.transform);
                }
                else
                {
                    ship.RemoveDependentTransform(currentlyHauling.transform);
                }
                //Restore hauling actions
                for (int i = 0; i < currentlyHauling.attachedActions.Count; i++)
                {
                    var action = currentlyHauling.attachedActions[i];
                    AddAction(action);
                }
                AddAction(currentlyHauling);
            }
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
                    if (transitionNewState == CharState.Outside)
                    {
                        //animator.SetTrigger(transitionPrepOutAnimKey);
                        characterPivot.localPosition = pivotWhenExiting;
                        animator.SetTrigger(exitAnimKey);
                        charAudio.PlayShipExit();
                    }
                    else
                    {
                        //animator.SetTrigger(transitionPrepInAnimKey);
                        characterPivot.localPosition = pivotWhenEntering;
                        animator.SetTrigger(enterAnimKey);
                        charAudio.PlayShipEnter();
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
                UseAction[] newControls = col.gameObject.GetComponents<UseAction>();
                for (int i = 0; i < newControls.Length; i++)
                {
                    var newControl = newControls[i];
                    AddAction(newControl);
                }
            }
        }

        protected void OnTriggerExit2D(Collider2D col)
        {
            if (col.tag == Tags.ACTION_OBJECT)
            {
                UseAction[] leavingActions = col.gameObject.GetComponents<UseAction>();
                for (int i = 0; i < leavingActions.Length; i++)
                {
                    var leavingAction = leavingActions[i];
                    if (RemoveAction(leavingAction))
                    {
                        activeBehaviour.DismissAction(leavingAction);
                    }
                }


            }
        }

        #region InputEvents
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
        #endregion
        #region AnimationEvents
        public void OnDieAnimPlayed()
        {
            animator.SetBool(dieAnimKey, false);
        }
        #endregion
        #region Char Tool
        public void EquipTool(CharTool charTool)
        {
            if (hasEquipedTool)
            {
                //Drop tool
                equipedTool.Unequip();
            }
            charTool.EquipToCharacter(this);
            hasEquipedTool = true;
            equipedTool = charTool;
            equipedTool.transform.SetParent(equipedToolRoot, false);
            equipedTool.transform.localPosition = Vector3.zero;
        }
        public bool IsToolAvailable()
        {
            return hasEquipedTool && equipedTool.IsAvailable();
        }
        public void ActivateTool()
        {
            if (IsToolAvailable())
            {
                equipedTool.Activate();
            }
        }
        #endregion
        public Vector3 GetLookDirection()
        {
            return activeBehaviour.GetLookDirection();
        }
        public float GetRepairSpeed()
        {
            if (hasEquipedTool && equipedTool.GetCharToolType() == CharToolType.Wrench)
            {
                return baseRepairSpeed * ((WrenchTool)equipedTool).GetRepairSpeedMultiplier();
            }
            else
            {
                return baseRepairSpeed;
            }
        }
        #region Hauling
        public void SetHauling(HaulUseAction haulObject)
        {
            if (IsHauling())
            {
                currentlyHauling.StopHauling();
            }
            currentlyHauling = haulObject;
            isHauling = true;
            if (state == CharState.Outside)
            {
                outsideBehaviour.SetOverridenSpeedFactor(currentlyHauling.haulOutsideSpeedFactor);
            }
        }
        public bool IsHauling()
        {
            return isHauling;
        }
        public void StopHauling()
        {
            isHauling = false;
            outsideBehaviour.ReleaseOverrideSpeed();
        }

        #endregion
    }
}