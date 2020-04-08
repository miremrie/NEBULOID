using NBLD.UseActions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.Character
{
    public class CharBehaviour : MonoBehaviour
    {
        protected Animator animator;
        public CharController charController { get; protected set; }
        protected SpriteRenderer spriteRenderer;
        protected Rigidbody2D rb2D;

        public void Initialize(CharController charController, Rigidbody2D rigidbody, SpriteRenderer spriteRenderer, Animator animator)
        {
            this.animator = animator;
            this.charController = charController;
            this.spriteRenderer = spriteRenderer;
            this.rb2D = rigidbody;
        }

        public virtual void ExecuteAction(UseAction action)
        {
        }
        public virtual void DismissAction(UseAction action)
        {
        }
        protected void TryExecuteAction(UseActionButton button)
        {
            UseAction action;
            if (charController.TryGetAction(button, out action))
            {
                ExecuteAction(action);
            }
        }

        //Input methods
        public virtual void OnMovement(Vector2 movement)
        {
        }

        public virtual void OnUp()
        {
        }

        public virtual void OnDown()
        {
        }

        public virtual void OnAction()
        {
        }

        public virtual void OnSubAction()
        {
        }

        public virtual void OnTalk()
        {
        }

        public virtual void OnMoveAssistStarted()
        {
        }

        public virtual void OnMoveAssistPerformed()
        {
        }
    }
}