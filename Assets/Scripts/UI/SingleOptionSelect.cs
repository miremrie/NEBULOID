using System.Collections;
using System.Collections.Generic;
using NBLD.Input;
using NBLD.Utils;
using UnityEngine;

namespace NBLD.UI
{
    public class SingleOptionSelect : BaseUIComponent
    {
        public enum ChangeDirection { Up, Down, Left, Right };
        public TMPro.TextMeshProUGUI currentlySelected;
        public ChangeDirection forwardDirection;
        public ChangeDirection backwardsDirection;
        public float holdOffsetTime = 0.1f;
        private Timer heldTimer;
        public delegate void ChangeHandler(int index);
        public event ChangeHandler OnSelectionChanged;
        private bool subscribedToInput = false;
        private void Awake()
        {
            heldTimer = new Timer(holdOffsetTime);
        }


        protected override void SubscribeToInput()
        {
            base.SubscribeToInput();
            if (!subscribedToInput)
            {
                subscribedToInput = true;
                uiInputManager.OnNavigationIntChanged += OnNavigationIntChanged;
                uiInputManager.OnNavigationHeld += OnNavigationHeld;
            }

        }
        protected override void UnsubscribeFromInput()
        {
            base.UnsubscribeFromInput();
            if (subscribedToInput)
            {
                uiInputManager.OnNavigationIntChanged -= OnNavigationIntChanged;
                uiInputManager.OnNavigationHeld -= OnNavigationHeld;
                subscribedToInput = false;
            }

        }
        private void OnNavigationIntChanged(Vector2Int navigation)
        {
            if (IsNavigationMatchingDirection(navigation, forwardDirection))
            {
                OnSelectionChanged?.Invoke(1);
            }
            if (IsNavigationMatchingDirection(navigation, backwardsDirection))
            {
                OnSelectionChanged?.Invoke(-1);
            }
        }
        private void OnNavigationHeld(Vector2 navigation)
        {
            if (heldTimer.IsTimerDone())
            {
                heldTimer.Restart();
                if (IsNavigationMatchingDirection(navigation, forwardDirection))
                {
                    OnSelectionChanged?.Invoke(1);
                }
                if (IsNavigationMatchingDirection(navigation, backwardsDirection))
                {
                    OnSelectionChanged?.Invoke(-1);
                }
            }
        }

        private bool IsNavigationMatchingDirection(Vector2 navigation, ChangeDirection direction)
        {
            return (navigation.x > 0 && direction == ChangeDirection.Right)
                    || (navigation.x < 0 && direction == ChangeDirection.Left)
                    || (navigation.y > 0 && direction == ChangeDirection.Up)
                    || (navigation.y < 0 && direction == ChangeDirection.Down);
        }

        public void UpdateText(string text)
        {
            currentlySelected.text = text;
        }

    }
}

