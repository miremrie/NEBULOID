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
    public class GameplayInputEvents
    {
        public delegate void ButtonActionHandler();
        public delegate void Axis2DActionHandler(Vector2 value);
        //public delegate void Axis2DIntActionHandler(Vector2Int value);
        public delegate void AxisActionHandler(float value);

        public event ButtonActionHandler OnAction, OnSubAction, OnUp, OnDown, OnTalk, OnMoveAssistStarted, OnMoveAssistPerformed;
        public event Axis2DActionHandler OnMove;
        public event AxisActionHandler OnMoveHorizontal, OnMoveVertical;

        public void RaiseAction()
        {
            OnAction?.Invoke();
        }
        public void RaiseSubAction()
        {
            OnSubAction?.Invoke();
        }
        public void RaiseUp()
        {
            OnUp?.Invoke();
        }
        public void RaiseDown()
        {
            OnDown?.Invoke();
        }
        public void RaiseTalk()
        {
            OnTalk?.Invoke();
        }
        public void RaiseMoveAssistStarted()
        {
            OnMoveAssistStarted?.Invoke();
        }
        public void RaiseMoveAssistPerformed()
        {
            OnMoveAssistPerformed?.Invoke();
        }
        public void RaiseMove(Vector2 nav)
        {
            OnMove?.Invoke(nav);
        }
        public void RaiseMoveHorizontal(float value)
        {
            OnMoveHorizontal?.Invoke(value);
        }
        public void RaiseMoveVertical(float value)
        {
            OnMoveVertical?.Invoke(value);
        }
    }
    public class PlayerGameplayInputManager : GameplayInputEvents
    {
        private CharacterInput input;
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
            RaiseMove(movement);
            RaiseMoveHorizontal(movement.x);
            RaiseMoveVertical(movement.y);
        }

        private void OnUpInput(InputAction.CallbackContext context)
        {
            //Debug.Log($"{index}: Up");
            RaiseUp();
        }

        private void OnInputDown(InputAction.CallbackContext context)
        {
            //Debug.Log($"{index}: Down");
            RaiseDown();
        }

        private void OnInputAction(InputAction.CallbackContext context)
        {
            //Debug.Log($"{index}: Action");
            RaiseAction();
        }

        private void OnInputSubAction(InputAction.CallbackContext context)
        {
            //Debug.Log($"{index}: SubAction");
            RaiseSubAction();
        }

        private void OnInputTalk(InputAction.CallbackContext context)
        {
            //Debug.Log($"{index}: Talk");
            RaiseTalk();
        }

        private void OnInputMoveAssistStarted(InputAction.CallbackContext context)
        {
            //Debug.Log($"{index}: MoveAssist");
            RaiseMoveAssistStarted();
        }

        private void OnInputMoveAssistPerformed(InputAction.CallbackContext context)
        {
            //Debug.Log($"{index}: MoveAssistPerformed");
            RaiseMoveAssistPerformed();
        }
    }
}
