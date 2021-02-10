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
    public interface ICharInputListener
    {
        void OnMovement(Vector2 movement);
        void OnUp();
        void OnDown();
        void OnAction();
        void OnSubAction();
        void OnTalk();
        void OnMoveAssistStarted();
        void OnMoveAssistPerformed();
    }
    public class CharInputManager
    {
        public int index;
        public List<ICharInputListener> listeners;
        private CharacterInput input;


        public CharInputManager(CharacterInput input)
        {
            listeners = new List<ICharInputListener>();
            this.input = input;
            Subscribe();

        }

        /*private void OnDestroy()
        {
            input.Dispose();
        }*/

        public void RegisterListener(ICharInputListener listener)
        {
            if (!listeners.Contains(listener))
            {
                listeners.Add(listener);
            }
        }
        public void RemoveListener(ICharInputListener listener)
        {
            listeners.Remove(listener);
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
            for (int i = 0; i < listeners.Count; i++)
            {
                listeners[i].OnMovement(movement);
            }
        }

        private void OnUp(InputAction.CallbackContext context)
        {
            //Debug.Log($"{index}: Up");
            for (int i = 0; i < listeners.Count; i++)
            {
                listeners[i].OnUp();
            }
        }

        private void OnDown(InputAction.CallbackContext context)
        {
            //Debug.Log($"{index}: Down");
            for (int i = 0; i < listeners.Count; i++)
            {
                listeners[i].OnDown();
            }
        }

        private void OnAction(InputAction.CallbackContext context)
        {
            //Debug.Log($"{index}: Action");
            for (int i = 0; i < listeners.Count; i++)
            {
                listeners[i].OnAction();
            }
        }

        private void OnSubAction(InputAction.CallbackContext context)
        {
            //Debug.Log($"{index}: SubAction");
            for (int i = 0; i < listeners.Count; i++)
            {
                listeners[i].OnSubAction();
            }
        }

        private void OnTalk(InputAction.CallbackContext context)
        {
            //Debug.Log($"{index}: Talk");
            for (int i = 0; i < listeners.Count; i++)
            {
                listeners[i].OnTalk();
            }
        }

        private void OnMoveAssistStarted(InputAction.CallbackContext context)
        {
            //Debug.Log($"{index}: MoveAssist");
            for (int i = 0; i < listeners.Count; i++)
            {
                listeners[i].OnMoveAssistStarted();
            }
        }

        private void OnMoveAssistPerformed(InputAction.CallbackContext context)
        {
            //Debug.Log($"{index}: MoveAssistPerformed");
            for (int i = 0; i < listeners.Count; i++)
            {
                listeners[i].OnMoveAssistPerformed();
            }
        }

    }
}
