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
    public class PlayerGameplayInputManager
    {
        private CharacterInput input;

        public delegate void ButtonActionHandler();
        public delegate void Axis2DActionHandler(Vector2 value);
        //public delegate void Axis2DIntActionHandler(Vector2Int value);
        public delegate void AxisActionHandler(float value);

        public event ButtonActionHandler OnAction, OnSubAction, OnUp, OnDown, OnTalk, OnMoveAssistStarted, OnMoveAssistPerformed;
        public event Axis2DActionHandler OnMove;
        public event AxisActionHandler OnMoveHorizontal, OnMoveVertical;


        public PlayerGameplayInputManager(CharacterInput input)
        {
            this.input = input;
            Subscribe();
        }

        /*private void OnDestroy()
        {
            input.Dispose();
        }*/

        public void Subscribe()
        {
            input.Character.Move.performed += OnInputMovement;
            input.Character.Action.performed += OnInputAction;
            input.Character.SubAction.performed += OnInputSubAction;
            input.Character.Up.performed += OnUpInput;
            input.Character.Down.performed += OnInputDown;
            input.Character.Talk.performed += OnInputTalk;
            input.Character.MoveAssist.performed += OnInputMoveAssistStarted;
            input.Character.MoveAssist.performed += OnInputMoveAssistPerformed;
        }

        public void Unsubscribe()
        {
            input.Character.Move.performed -= OnInputMovement;
            input.Character.Action.performed -= OnInputAction;
            input.Character.SubAction.performed -= OnInputSubAction;
            input.Character.Up.performed -= OnUpInput;
            input.Character.Down.performed -= OnInputDown;
            input.Character.Talk.performed -= OnInputTalk;
            input.Character.MoveAssist.started -= OnInputMoveAssistStarted;
            input.Character.MoveAssist.performed -= OnInputMoveAssistPerformed;
        }

        private void OnInputMovement(InputAction.CallbackContext context)
        {
            Vector2 movement = context.ReadValue<Vector2>();
            //Debug.Log($"{index}: Move {movement}");
            OnMove?.Invoke(movement);
            OnMoveHorizontal?.Invoke(movement.x);
            OnMoveVertical?.Invoke(movement.y);
        }

        private void OnUpInput(InputAction.CallbackContext context)
        {
            //Debug.Log($"{index}: Up");
            OnUp?.Invoke();
        }

        private void OnInputDown(InputAction.CallbackContext context)
        {
            //Debug.Log($"{index}: Down");
            OnDown?.Invoke();
        }

        private void OnInputAction(InputAction.CallbackContext context)
        {
            //Debug.Log($"{index}: Action");
            OnAction?.Invoke();
        }

        private void OnInputSubAction(InputAction.CallbackContext context)
        {
            //Debug.Log($"{index}: SubAction");
            OnSubAction?.Invoke();
        }

        private void OnInputTalk(InputAction.CallbackContext context)
        {
            //Debug.Log($"{index}: Talk");
            OnTalk?.Invoke();
        }

        private void OnInputMoveAssistStarted(InputAction.CallbackContext context)
        {
            //Debug.Log($"{index}: MoveAssist");
            OnMoveAssistStarted?.Invoke();
        }

        private void OnInputMoveAssistPerformed(InputAction.CallbackContext context)
        {
            //Debug.Log($"{index}: MoveAssistPerformed");
            OnMoveAssistPerformed?.Invoke();
        }

    }
}
