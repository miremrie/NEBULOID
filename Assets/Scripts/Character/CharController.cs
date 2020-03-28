using NBLD.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.Character
{
    public class CharController : MonoBehaviour
    {
        public CharacterState activeState;
        public CharInputManager charInputManager;

        public void Start()
        {
            charInputManager.RegisterController(this);
        }

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