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
        public CharController charController;
        private CharacterInput input;
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
            this.charController = charController;
        }

        public void InitializeInput(UserDevice userDevice)
        {
            input = userDevice.inputActions;
            
            Subscribe();
            inputInitialized = true;
        }

        public void Subscribe()
        {
            input.Character.Move.performed += OnMovement;
            input.Character.Action.performed += OnAction;
            input.Character.SubAction.performed += OnSubAction;
            input.Character.Up.performed += OnUp;
            input.Character.Down.performed += OnDown;
            input.Character.Talk.performed += OnTalk;
            input.Character.MoveAssist.performed += OnMoveAssistStarted;
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

        private void OnMovement(InputAction.CallbackContext context)
        {
            Vector2 movement = context.ReadValue<Vector2>();
            //Debug.Log($"{index}: Move {movement}");

            charController.OnMovement(movement);
        }

        private void OnUp(InputAction.CallbackContext context)
        {
            //Debug.Log($"{index}: Up");
            charController.OnUp();
        }

        private void OnDown(InputAction.CallbackContext context)
        {
            //Debug.Log($"{index}: Down");
            charController.OnDown();
        }

        private void OnAction(InputAction.CallbackContext context)
        {
            //Debug.Log($"{index}: Action");
            charController.OnAction();
        }

        private void OnSubAction(InputAction.CallbackContext context)
        {
            //Debug.Log($"{index}: SubAction");
            charController.OnSubAction();
        }

        private void OnTalk(InputAction.CallbackContext context)
        {
            //Debug.Log($"{index}: Talk");
            charController.OnTalk();
        }

        private void OnMoveAssistStarted(InputAction.CallbackContext context)
        {
            //Debug.Log($"{index}: MoveAssist");
            charController.OnMoveAssistStarted();
        }

        private void OnMoveAssistPerformed(InputAction.CallbackContext context)
        {
            //Debug.Log($"{index}: MoveAssistPerformed");
            charController.OnMoveAssistPerformed();
        }
        
    }
}
