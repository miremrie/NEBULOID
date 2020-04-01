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
        public delegate void UIAction();
        public delegate void UIButtonHeld(int value, float time);
        public static event UINavigation onNavigation, onNavigationChanged;
        public static event UIAction onSubmit, onCancel, onChangeSelect, onEscape;
        public static event UIButtonHeld onHorizontalHeld, onVerticalHeld;
        private UIInput uiInput;
        private bool inputInitialized = false;
        //Input values
        private Vector2 navigation = Vector2.zero;
        //Hold Values
        private int xHoldValue = 0, yHoldValue = 0;
        private Timer xHoldTimer, yHoldTimer;

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
            xHoldTimer = new Timer(holdDuration);
            xHoldTimer.Start();
            yHoldTimer = new Timer(holdDuration);
            yHoldTimer.Start();
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
            UpdateHeldEvents();
        }

        //Events
        private void UpdateNavigation()
        {
            if (onNavigation != null)
            {
                onNavigation(navigation);
            }
        }
        private void UpdateHeldEvents()
        {
            UpdateNavigationHeldTime();
            if (IsXBeingHeld())
            {
                if (onHorizontalHeld != null)
                {
                    //TODO: Should we pass real time since press, or time since it started holding
                    Debug.Log($"Horizontal Hold {xHoldValue} for {xHoldTimer.GetCurrentTime()}");
                    onHorizontalHeld(xHoldValue, xHoldTimer.GetCurrentTime());
                }
            }
            if (IsYBeingHeld())
            {
                if (onVerticalHeld != null)
                {
                    //TODO: Should we pass real time since press, or time since it started holding
                    Debug.Log($"Vertical Hold {yHoldValue} for {yHoldTimer.GetCurrentTime()}");
                    onVerticalHeld(yHoldValue, yHoldTimer.GetCurrentTime());
                }
            }
        }

        private void InputNavigationChanged(InputAction.CallbackContext context)
        {
            navigation = context.ReadValue<Vector2>();
            Debug.Log($"Navigation Changed {navigation}");
            CheckNavigationHeldStatus(navigation);

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

        private void UpdateNavigationHeldTime()
        {
            if (xHoldValue != 0)
            {
                xHoldTimer.Update(Time.deltaTime);
            }
            if (yHoldValue != 0)
            {
                yHoldTimer.Update(Time.deltaTime);
            }
        }

        private bool IsXBeingHeld()
        {
            return xHoldValue != 0 && !xHoldTimer.IsRunning();
        }

        private bool IsYBeingHeld()
        {
            return yHoldValue != 0 && !yHoldTimer.IsRunning();
        }

        private void CheckNavigationHeldStatus(Vector2 newNavigation)
        {
            int newXHoldValue = GetNewHoldValue(newNavigation.x);
            if (xHoldValue == 0 || xHoldValue != newXHoldValue)
            {
                xHoldValue = newXHoldValue;
                xHoldTimer.Start();
            }
            int newYHoldValue = GetNewHoldValue(newNavigation.y);
            if (yHoldValue == 0 || yHoldValue != newYHoldValue)
            {
                yHoldValue = newYHoldValue;
                yHoldTimer.Start();
            }
        }

        private int GetNewHoldValue(float newValue)
        {
            int newDiscreteValue = 0;
            if (Mathf.Abs(newValue) > navigationDeadzoneValue)
            {
                newDiscreteValue = (newValue > 0) ? 1 : -1;
            }
            return newDiscreteValue;
        }
    }
}

