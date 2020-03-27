using CustomInput;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using NBLD.Input;

namespace NBLD.Character
{
    public class InsideCharController : MonoBehaviour
    {
        public int index;
        private Vector2 movement;
        private bool actionPress, subActionPress, escapePress, talkPress;
        private int playerIndex = 0;

        public void Awake()
        {
        }

        public void InitializeInput(UserDevice userDevice)
        {
            CharacterInput input = (CharacterInput)userDevice.inputActions;
            input.InsideControls.Move.performed += ctx => OnMovement(ctx.ReadValue<Vector2>());
            input.InsideControls.Action.performed += _ => OnAction();
            input.InsideControls.SubAction.performed += _ => OnSubAction();
            input.InsideControls.UpAction.performed += _ => OnUpAction();
            input.InsideControls.DownAction.performed += _ => OnDownAction();
            input.InsideControls.Talk.performed += _ => OnTalk();
        }

        private void OnMovement(Vector2 movement)
        {
            Debug.Log($"{index}: Move {movement}");
        }

        private void OnUpAction()
        {
            Debug.Log($"{index}: Up Action");
        }

        private void OnDownAction()
        {
            Debug.Log($"{index}: Down Action");
        }

        private void OnAction()
        {
            Debug.Log($"{index}: Action");
        }

        private void OnSubAction()
        {
            Debug.Log($"{index}: SubAction");
        }

        private void OnTalk()
        {
            Debug.Log($"{index}: Talk");
        }
    }
}
