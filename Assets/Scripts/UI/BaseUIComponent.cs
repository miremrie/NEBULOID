using System.Collections;
using System.Collections.Generic;
using NBLD.Input;
using UnityEngine;

namespace NBLD.UI
{
    public class BaseUIComponent : MonoBehaviour
    {
        public GameObject[] activeOnFocus;
        protected PlayerUIInputManager uiInputManager;
        public virtual void Focus(PlayerUIInputManager uiInputManager)
        {
            SetAllFocusElementsVisibility(true);
            this.uiInputManager = uiInputManager;
            SubscribeToInput();
        }

        public virtual void LoseFocus()
        {
            SetAllFocusElementsVisibility(false);
            UnsubscribeFromInput();
        }

        private void SetAllFocusElementsVisibility(bool visible)
        {
            for (int i = 0; i < activeOnFocus.Length; i++)
            {
                activeOnFocus[i].SetActive(visible);
            }
        }

        protected virtual void SubscribeToInput()
        {

        }
        protected virtual void UnsubscribeFromInput()
        {

        }
    }
}

