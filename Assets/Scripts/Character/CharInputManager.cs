using CustomInput;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using NBLD.Input;
using NBLD.Character;

namespace NBLD.Input
{
    public enum CharacterState
    {
        Inside, Outside, Dead
    }

    public class CharInputManager : MonoBehaviour
    {
        public int index;
        private Vector2 movement;
        private bool actionPress, subActionPress, escapePress, talkPress;
        private int playerIndex = 0;
        public List<CharController> charControllers = new List<CharController>();
        public CharController activeChar;
        private bool activeCharExists = false;
        private CustomInput.CharacterInput input;
        public CharacterState state;

        public void Awake()
        {

        }

        public void RegisterController(CharController charController)
        {
            if (!charControllers.Contains(charController))
            {
                charControllers.Add(charController);
            }
        }

        public void InitializeInput(UserDevice userDevice)
        {
            input = (CustomInput.CharacterInput)userDevice.inputActions;
            Subscribe();
            ChangeState(state);
        }

        public void Subscribe()
        {
            input.Character.Move.performed += ctx => OnMovement(ctx.ReadValue<Vector2>());
            input.Character.Action.performed += _ => OnAction();
            input.Character.SubAction.performed += _ => OnSubAction();
            input.Character.Up.performed += _ => OnUp();
            input.Character.Down.performed += _ => OnDown();
            input.Character.Talk.performed += _ => OnTalk();
            input.Character.MoveAssist.started += _ => OnMoveAssistStarted();
            input.Character.MoveAssist.performed += _ => OnMoveAssistPerformed();
        }

        public void Unsubscribe()
        {
            input.Character.Move.performed -= ctx => OnMovement(ctx.ReadValue<Vector2>());
            input.Character.Action.performed -= _ => OnAction();
            input.Character.SubAction.performed -= _ => OnSubAction();
            input.Character.Up.performed -= _ => OnUp();
            input.Character.Down.performed -= _ => OnDown();
            input.Character.Talk.performed -= _ => OnTalk();
            input.Character.MoveAssist.started -= _ => OnMoveAssistStarted();
            input.Character.MoveAssist.performed -= _ => OnMoveAssistPerformed();
        } 

        public void ChangeState(CharacterState newState)
        {
            activeChar = charControllers.Find(c => c.activeState == newState);
            activeCharExists = activeChar != null;
            state = newState;
        }

        private void OnMovement(Vector2 movement)
        {
            Debug.Log($"{index}: Move {movement}");
            if (activeCharExists)
            {
                activeChar.OnMovement(movement);
            }
        }

        private void OnUp()
        {
            Debug.Log($"{index}: Up");
            if (activeCharExists)
            {
                activeChar.OnUp();
            }
        }

        private void OnDown()
        {
            Debug.Log($"{index}: Down");
            if (activeCharExists)
            {
                activeChar.OnDown();
            }
        }

        private void OnAction()
        {
            Debug.Log($"{index}: Action");
            if (activeCharExists)
            {
                activeChar.OnAction();
            }
        }

        private void OnSubAction()
        {
            Debug.Log($"{index}: SubAction");
            if (activeCharExists)
            {
                activeChar.OnSubAction();
            }
        }

        private void OnTalk()
        {
            Debug.Log($"{index}: Talk");
            if (activeCharExists)
            {
                activeChar.OnTalk();
            }
        }

        private void OnMoveAssistStarted()
        {
            Debug.Log($"{index}: MoveAssist");
            if (activeCharExists)
            {
                activeChar.OnMoveAssistStarted();
            }
        }

        private void OnMoveAssistPerformed()
        {
            Debug.Log($"{index}: MoveAssistPerformed");
            if (activeCharExists)
            {
                activeChar.OnMoveAssistPerformed();
            }
        }
    }
}
