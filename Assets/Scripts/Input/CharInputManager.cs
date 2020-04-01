using GeneratedInputActions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using NBLD.Input;
using NBLD.Character;

namespace NBLD.Input
{
    public class CharInputManager : MonoBehaviour
    {
        public int index;
        public List<CharController> charControllers = new List<CharController>();
        public CharController activeChar;
        private bool activeCharExists = false;
        private CharacterInput input;
        public CharacterState state;
        private bool inputInitialized = false;

        private void OnEnable()
        {
            if (inputInitialized)
            {
                Subscribe();
            }
        }

        private void OnDisable()
        {
            if (inputInitialized)
            {
                Unsubscribe();
                input.Dispose();
            }
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
            input = userDevice.inputActions;
            
            Subscribe();
            inputInitialized = true;
            ChangeState(state);
        }

        public void Subscribe()
        {
            input.Character.Move.performed += OnMovement;
            input.Character.Action.performed += OnAction;
            input.Character.SubAction.performed += OnSubAction;
            input.Character.Up.performed += OnUp;
            input.Character.Down.performed += OnDown;
            input.Character.Talk.performed += OnTalk;
            input.Character.MoveAssist.started += OnMoveAssistStarted;
            input.Character.MoveAssist.performed += OnMoveAssistPerformed;
        }

        public void Unsubscribe()
        {
            input.Character.Move.performed -= OnMovement;
            input.Character.Action.performed -= OnAction;
            input.Character.SubAction.performed -= OnSubAction;
            input.Character.Up.performed -= OnUp;
            input.Character.Down.performed -= OnDown;
            input.Character.Talk.performed -= OnTalk;
            input.Character.MoveAssist.started -= OnMoveAssistStarted;
            input.Character.MoveAssist.performed -= OnMoveAssistPerformed;
        } 

        public void ChangeState(CharacterState newState)
        {
            activeChar = charControllers.Find(c => c.activeState == newState);
            activeCharExists = activeChar != null;
            state = newState;
        }

        private void OnMovement(InputAction.CallbackContext context)
        {
            Vector2 movement = context.ReadValue<Vector2>();
            Debug.Log($"{index}: Move {movement}");
            if (activeCharExists)
            {
                activeChar.OnMovement(movement);
            }
        }

        private void OnUp(InputAction.CallbackContext context)
        {
            Debug.Log($"{index}: Up");
            if (activeCharExists)
            {
                activeChar.OnUp();
            }
        }

        private void OnDown(InputAction.CallbackContext context)
        {
            Debug.Log($"{index}: Down");
            if (activeCharExists)
            {
                activeChar.OnDown();
            }
        }

        private void OnAction(InputAction.CallbackContext context)
        {
            Debug.Log($"{index}: Action");
            if (activeCharExists)
            {
                activeChar.OnAction();
            }
        }

        private void OnSubAction(InputAction.CallbackContext context)
        {
            Debug.Log($"{index}: SubAction");
            if (activeCharExists)
            {
                activeChar.OnSubAction();
            }
        }

        private void OnTalk(InputAction.CallbackContext context)
        {
            Debug.Log($"{index}: Talk");
            if (activeCharExists)
            {
                activeChar.OnTalk();
            }
        }

        private void OnMoveAssistStarted(InputAction.CallbackContext context)
        {
            Debug.Log($"{index}: MoveAssist");
            if (activeCharExists)
            {
                activeChar.OnMoveAssistStarted();
            }
        }

        private void OnMoveAssistPerformed(InputAction.CallbackContext context)
        {
            Debug.Log($"{index}: MoveAssistPerformed");
            if (activeCharExists)
            {
                activeChar.OnMoveAssistPerformed();
            }
        }
    }
}
