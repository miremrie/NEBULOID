﻿using NBLD.Audio;
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
        protected CharAudioController charAudio;
        public string spriteSortingLayer;
        public Material spriteMaterial;
        protected bool initialized = false;

        public void Initialize(CharController charController, Rigidbody2D rigidbody, SpriteRenderer spriteRenderer, Animator animator, CharAudioController charAudio)
        {
            this.animator = animator;
            this.charController = charController;
            this.spriteRenderer = spriteRenderer;
            this.rb2D = rigidbody;
            this.charAudio = charAudio;
            initialized = true;
        }

        protected virtual void OnEnable()
        {
            if (initialized)
            {
                //spriteRenderer.sortingLayerID = SortingLayer.NameToID(spriteSortingLayer);
                //spriteRenderer.material = spriteMaterial;
            }
        }

        protected virtual void OnDisable()
        {
            
        }
        protected virtual void Start()
        {

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