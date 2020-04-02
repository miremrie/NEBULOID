using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeneratedInputActions;
using System;
using UnityEngine.InputSystem;

namespace NBLD.Input
{
    public class UIInputManager : MonoBehaviour
    {
        public float holdDuration = 1f;
        public float navigationDeadzoneValue = 0.4f;

        public delegate void UINavigation(Vector2 navigation);
        public delegate void UINavigationInt(Vector2Int navigation);
        public delegate void UIAction();
        public static event UINavigation onNavigation, onNavigationChanged;
        public static event UINavigationInt onNavigationChangedInt;
        public static event UIAction onSubmit, onCancel, onChangeSelect, onEscape;
        private UIInput uiInput;
        private bool inputInitialized = false;
        //Input values
        private Vector2 navigation = Vector2.zero;
        //Hold Values
        public static AxisHeldInputProc horizontalHoldProcessor, verticalHoldProcessor;

        private void OnEnable()
        {
            if (!inputInitialized)
            {
                Initialize();
            }
            SubscribeToInput();
        }
        private void OnDisable()
        {
            if (inputInitialized)
            {
                UnsubscribeToInput();
            }
        }
        private void Initialize()
        {
            uiInput = new UIInput();
            horizontalHoldProcessor = new AxisHeldInputProc(holdDuration, 0, navigationDeadzoneValue);
            verticalHoldProcessor = new AxisHeldInputProc(holdDuration, 0, navigationDeadzoneValue);
            uiInput.Enable();
            inputInitialized = true;
        }

        private void SubscribeToInput()
        {
            uiInput.UI.Navigate.performed += InputNavigationChanged;
            uiInput.UI.Submit.performed += OnInputSubmit;
            uiInput.UI.Cancel.performed += OnInputCancel;
            uiInput.UI.ChangeSelect.performed += OnInputChangeSelect;
            uiInput.UI.Escape.performed += OnEscape;
        }
        private void UnsubscribeToInput()
        {
            uiInput.UI.Navigate.performed -= InputNavigationChanged;
            uiInput.UI.Submit.performed -= OnInputSubmit;
            uiInput.UI.Cancel.performed -= OnInputCancel;
            uiInput.UI.ChangeSelect.performed -= OnInputChangeSelect;
            uiInput.UI.Escape.performed -= OnEscape;
        }

        private void Update()
        {
            UpdateNavigation();
            UpdateHoldEventsTime();
        }

        //Events
        private void UpdateNavigation()
        {
            if (onNavigation != null)
            {
                onNavigation(navigation);
            }
        }

        private void UpdateHoldEventsTime()
        {
            horizontalHoldProcessor.UpdateTime(Time.deltaTime);
            verticalHoldProcessor.UpdateTime(Time.deltaTime);
        }
        private void UpdateAxisHoldValues(Vector2 navigation)
        {
            horizontalHoldProcessor.SetValue(navigation.x);
            verticalHoldProcessor.SetValue(navigation.y);
        }

        private void TryNavigationChangeInt(Vector2 oldNav, Vector2 newNav)
        {
            Vector2Int oldNavInt = GetIntNavigation(oldNav);
            Vector2Int newNavInt = GetIntNavigation(newNav);
            if (oldNav.x != newNav.x || oldNav.y != newNav.y)
            {
                if (onNavigationChangedInt != null)
                {
                    onNavigationChangedInt(newNavInt);
                }
            }
        }

        private void InputNavigationChanged(InputAction.CallbackContext context)
        {
            Vector2 oldNav = navigation;
            navigation = context.ReadValue<Vector2>();
            TryNavigationChangeInt(oldNav, navigation);
            UpdateAxisHoldValues(navigation);
            if (onNavigationChanged != null)
            {
                onNavigationChanged(navigation);
            }
        }
        private void OnInputSubmit(InputAction.CallbackContext context)
        {
            Debug.Log("Submit");
            if (onSubmit != null)
            {
                onSubmit();
            }
        }
        private void OnInputCancel(InputAction.CallbackContext context)
        {
            Debug.Log("Cancel");
            if (onCancel != null)
            {
                onCancel();
            }
        }
        private void OnInputChangeSelect(InputAction.CallbackContext context)
        {
            Debug.Log("ChangeSelect");
            if (onChangeSelect != null)
            {
                onChangeSelect();
            } 
        }

        private void OnEscape(InputAction.CallbackContext context)
        {
            Debug.Log("Escape");
            if (onEscape != null)
            {
                onEscape();
            }
        }

        //Helper
        private Vector2Int GetIntNavigation(Vector2 navigation)
        {
            Vector2Int output = Vector2Int.zero;
            if (Mathf.Abs(navigation.x) >= Mathf.Abs(navigationDeadzoneValue))
            {
                output.x = (int)Mathf.Sign(navigation.x);
            }
            if (Mathf.Abs(navigation.y) >= Mathf.Abs(navigationDeadzoneValue))
            {
                output.y = (int)Mathf.Sign(navigation.y);
            }
            return output;
        }
    }
}

