using System.Collections;
using System.Collections.Generic;
using GeneratedInputActions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace NBLD.Input
{
    public class PlayerUIInputManager
    {
        public float navigationDeadZoneValue = 0.4f;
        public int deviceIndex;
        private UIInput input;
        public delegate void NavigationEventHandler(Vector2 navigation);
        public delegate void NavigationIntEventHandler(Vector2Int navigation);
        public delegate void ClickEventHandler();
        public event NavigationEventHandler OnNavigation;
        public event NavigationIntEventHandler OnNavigationIntChanged;
        public event ClickEventHandler OnSubmit;
        public event ClickEventHandler OnCancel;
        public event ClickEventHandler OnChangeSelect;
        public event ClickEventHandler OnEscape;

        private Vector2 navigation = Vector2.zero;
        private float holdDuration = 0.6f;
        public AxisHeldInputProc horizontalHold, verticalHold;
        //Hold Values
        public PlayerUIInputManager(UIInput input, int deviceIndex)
        {
            this.input = input;
            this.deviceIndex = deviceIndex;
            horizontalHold = new AxisHeldInputProc(holdDuration, 0, navigationDeadZoneValue);
            verticalHold = new AxisHeldInputProc(holdDuration, 0, navigationDeadZoneValue);
            Subscribe();
        }

        private void Subscribe()
        {
            Debug.Log("Subscribing to input");
            InputManager.OnInputTick += OnInputTick;
            input.UI.Navigate.performed += InputNavigationChanged;
            input.UI.Submit.performed += OnInputSubmit;
            input.UI.Cancel.performed += OnInputCancel;
            input.UI.ChangeSelect.performed += OnInputChangeSelect;
            input.UI.Escape.performed += OnInputEscape;
        }
        private void Unsubscribe()
        {
            InputManager.OnInputTick -= OnInputTick;
            input.UI.Navigate.performed -= InputNavigationChanged;
            input.UI.Submit.performed -= OnInputSubmit;
            input.UI.Cancel.performed -= OnInputCancel;
            input.UI.ChangeSelect.performed -= OnInputChangeSelect;
            input.UI.Escape.performed -= OnInputEscape;
        }

        private void OnInputTick(float deltaTime)
        {
            horizontalHold.UpdateTime(deltaTime);
            verticalHold.UpdateTime(deltaTime);
            OnNavigation?.Invoke(navigation);
        }

        private void TryNavigationChangedInt(Vector2 oldNav, Vector2 newNav)
        {
            Vector2Int oldNavInt = InputUtils.Axis2DToInt(oldNav, navigationDeadZoneValue);
            Vector2Int newNavInt = InputUtils.Axis2DToInt(newNav, navigationDeadZoneValue);
            if (oldNav.x != newNav.x || oldNav.y != newNav.y)
            {
                OnNavigationIntChanged?.Invoke(newNavInt);
            }
        }

        private void InputNavigationChanged(InputAction.CallbackContext context)
        {
            Vector2 oldNav = navigation;
            navigation = context.ReadValue<Vector2>();
            horizontalHold.SetValue(navigation.x);
            verticalHold.SetValue(navigation.y);
            //Default to press
            TryNavigationChangedInt(oldNav, navigation);
        }
        private void OnInputSubmit(InputAction.CallbackContext context)
        {
            OnSubmit?.Invoke();
        }
        private void OnInputCancel(InputAction.CallbackContext context)
        {
            //Debug.Log("Cancel");
            OnCancel?.Invoke();
        }
        private void OnInputChangeSelect(InputAction.CallbackContext context)
        {
            //Debug.Log("ChangeSelect");
            OnChangeSelect?.Invoke();
        }
        private void OnInputEscape(InputAction.CallbackContext context)
        {
            //Debug.Log("Escape");
            OnEscape?.Invoke();
        }
    }
}

