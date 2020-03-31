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
        public CharacterState activeState;
        public CharInputManager charInputManager;

        [Header("Animation")]
        public Animator animator;
        public SpriteRenderer charSpriteRenderer;

        //Actions
        protected Dictionary<UseActionButton, UseAction> availableActions = new Dictionary<UseActionButton, UseAction>();

        public void Start()
        {
            charInputManager.RegisterController(this);
        }
        //Actions
        protected virtual bool CheckIfActionAvailable(UseActionButton useButton)
        {
            return availableActions.ContainsKey(useButton) 
                && availableActions[useButton].AvailableForCharacterState() == charInputManager.state;
        }
        protected virtual void ExecuteAction(UseAction action)
        {

        }
        protected virtual void DismissAction(UseAction action)
        {

        }
        protected virtual void CheckThenExecuteAction(UseActionButton useButton)
        {
            if (CheckIfActionAvailable(useButton))
            {
                ExecuteAction(availableActions[useButton]);
            }
        }
        //Collisions
        protected virtual void OnTriggerEnter2D(Collider2D col)
        {
            if (col.tag == Tags.ACTION_OBJECT)
            {
                UseAction newControl = col.gameObject.GetComponent<UseAction>();
                if (newControl.AvailableForCharacterState() == charInputManager.state)
                {
                    if (availableActions.ContainsKey(newControl.actionControl))
                    {
                        availableActions[newControl.actionControl] = newControl;
                    }
                    else
                    {
                        availableActions.Add(newControl.actionControl, newControl);
                    }
                }
            }
        }

        protected virtual void OnTriggerExit2D(Collider2D col)
        {
            if (col.tag == Tags.ACTION_OBJECT)
            {
                InsideUseAction leavingActionControl = col.gameObject.GetComponent<InsideUseAction>();
                if (leavingActionControl.AvailableForCharacterState() == charInputManager.state)
                {
                    if (availableActions.ContainsKey(leavingActionControl.actionControl))
                    {
                        DismissAction(availableActions[leavingActionControl.actionControl]);
                        availableActions.Remove(leavingActionControl.actionControl);
                    }
                }
            }
        }

        //Input Events
        public virtual void OnMovement(Vector2 movement)
        {
            Debug.Log($"{activeState}: {movement}");
        }

        public virtual void OnUp()
        {
            Debug.Log($"{activeState}: Up Action");
        }

        public virtual void OnDown()
        {
            Debug.Log($"{activeState}: Down Action");
        }

        public virtual void OnAction()
        {
            Debug.Log($"{activeState}: Action");
        }

        public virtual void OnSubAction()
        {
            Debug.Log($"{activeState}: SubAction");
        }

        public virtual void OnTalk()
        {
            Debug.Log($"{activeState}: Talk");
        }

        public virtual void OnMoveAssistStarted()
        {
            Debug.Log($"{activeState}: MoveAssistStarted");
        }

        public virtual void OnMoveAssistPerformed()
        {
            Debug.Log($"{activeState}: MoveAssistPerformed");
        }
    }
}