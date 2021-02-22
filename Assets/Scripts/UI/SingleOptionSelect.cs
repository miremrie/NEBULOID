using System.Collections;
using System.Collections.Generic;
using NBLD.Input;
using NBLD.Utils;
using UnityEngine;

namespace NBLD.UI
{
    public class SingleOptionSelect : BaseUIComponent
    {
        public enum ChangeOrientation { Horizontal, Vertical };
        //public TMPro.TextMeshProUGUI currentlySelected;
        public ChangeOrientation changeOrientation;
        public bool forwardIsPositive;
        public float holdOffsetTime = 0.1f;
        private Timer holdTimer;
        public delegate void ChangeHandler(int index);
        public event ChangeHandler OnSelectionChanged;
        private bool subscribedToInput = false;
        private int holdValue;
        private bool isHolding = false;
        private void Awake()
        {
            holdTimer = new Timer(holdOffsetTime);
        }


        protected override void SubscribeToInput()
        {
            base.SubscribeToInput();
            if (!subscribedToInput)
            {
                subscribedToInput = true;
                uiInputManager.OnNavigationIntChanged += OnNavigationIntChanged;
                if (changeOrientation == ChangeOrientation.Horizontal)
                {
                    uiInputManager.horizontalHold.onAxisBeingHeldInt += OnNavigationHeld;
                }
                else if (changeOrientation == ChangeOrientation.Vertical)
                {
                    uiInputManager.verticalHold.onAxisBeingHeldInt += OnNavigationHeld;
                }
            }

        }
        protected override void UnsubscribeFromInput()
        {
            base.UnsubscribeFromInput();
            if (subscribedToInput)
            {
                uiInputManager.OnNavigationIntChanged -= OnNavigationIntChanged;
                if (changeOrientation == ChangeOrientation.Horizontal)
                {
                    uiInputManager.horizontalHold.onAxisBeingHeldInt -= OnNavigationHeld;
                }
                else if (changeOrientation == ChangeOrientation.Vertical)
                {
                    uiInputManager.verticalHold.onAxisBeingHeldInt -= OnNavigationHeld;
                }
                subscribedToInput = false;
            }

        }
        private void OnNavigationIntChanged(Vector2Int navigation)
        {
            int relevantNavValue = GetRelevantNavValue(navigation);
            ChangeSelection(relevantNavValue);
        }
        private void OnNavigationHeld(int value, float time)
        {
            Debug.Log($"{value}:{time}");
            holdValue = value;
            if (isHolding)
            {
                if (holdTimer.IsTimerDone())
                {
                    ChangeSelection(value);
                    holdTimer.Restart();
                }
            }
            else
            {
                holdTimer.Restart();
                isHolding = true;
            }
        }
        private void ChangeSelection(int relevantNavValue)
        {
            if (IsNavigationMatchingDirection(relevantNavValue, forwardIsPositive))
            {
                OnSelectionChanged?.Invoke(1);
            }
            if (IsNavigationMatchingDirection(relevantNavValue, !forwardIsPositive))
            {
                OnSelectionChanged?.Invoke(-1);
            }
        }

        private int GetRelevantNavValue(Vector2Int navigation)
        {
            int relevantValue = 0;
            if (navigation.y != 0 && changeOrientation == ChangeOrientation.Vertical)
            {
                relevantValue = navigation.y;
            }
            else if (navigation.x != 0 && changeOrientation == ChangeOrientation.Horizontal)
            {
                relevantValue = navigation.x;
            }
            return relevantValue;
        }
        private bool IsNavigationMatchingDirection(int relevantNavValue, bool directionIsPositive)
        {
            if (relevantNavValue != 0)
            {
                return directionIsPositive == (relevantNavValue > 0);
            }
            return false;
        }

        /*public void UpdateText(string text)
        {
            currentlySelected.text = text;
        }*/

    }
}

