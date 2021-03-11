// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using GeneratedInputActions;
// using System;
// using UnityEngine.InputSystem;

// namespace NBLD.Input
// {
//     public class UIInputManager
//     {
//         public float holdDuration = 1f;
//         public float navigationDeadzoneValue = 0.4f;

//         public event Action<Vector2> onNavigation, onNavigationChanged;
//         public event Action<Vector2Int> onNavigationChangedInt;
//         public event Action onSubmit, onCancel, onChangeSelect, onEscape;
//         private UIInput uiInput;
//         //Input values
//         private Vector2 navigation = Vector2.zero;
//         //Hold Values
//         public AxisHeldInputProc horizontalHold, verticalHold;

//         public UIInputManager(float holdDuration = 0.6f, float deadZone = 0.4f)
//         {
//             this.holdDuration = holdDuration;
//             this.navigationDeadzoneValue = deadZone;
//             Initialize();
//         }
//         private void Initialize()
//         {
//             uiInput = new UIInput();
//             horizontalHold = new AxisHeldInputProc(holdDuration, 0, navigationDeadzoneValue);
//             verticalHold = new AxisHeldInputProc(holdDuration, 0, navigationDeadzoneValue);
//             /*uiInput.Enable();
//             SubscribeToInput();*/
//         }
//         public void Enable()
//         {
//             uiInput.Enable();
//             SubscribeToInput();
//         }
//         public void Disable()
//         {
//             uiInput.Disable();
//             UnsubscribeFromInput();
//         }
//         private void SubscribeToInput()
//         {
//             uiInput.UI.Navigate.performed += InputNavigationChanged;
//             uiInput.UI.Submit.performed += OnInputSubmit;
//             uiInput.UI.Cancel.performed += OnInputCancel;
//             uiInput.UI.ChangeSelect.performed += OnInputChangeSelect;
//             uiInput.UI.Escape.performed += OnEscape;
//         }
//         private void UnsubscribeFromInput()
//         {
//             uiInput.UI.Navigate.performed -= InputNavigationChanged;
//             uiInput.UI.Submit.performed -= OnInputSubmit;
//             uiInput.UI.Cancel.performed -= OnInputCancel;
//             uiInput.UI.ChangeSelect.performed -= OnInputChangeSelect;
//             uiInput.UI.Escape.performed -= OnEscape;
//         }

//         public void Update(float deltaTime)
//         {
//             UpdateNavigation();
//             UpdateHoldEventsTime(deltaTime);
//         }

//         //Events
//         private void UpdateNavigation()
//         {
//             if (onNavigation != null)
//             {
//                 onNavigation(navigation);
//             }
//         }

//         private void UpdateHoldEventsTime(float deltaTime)
//         {
//             horizontalHold.UpdateTime(deltaTime);
//             verticalHold.UpdateTime(deltaTime);
//         }
//         private void UpdateAxisHoldValues(Vector2 navigation)
//         {
//             horizontalHold.SetValue(navigation.x);
//             verticalHold.SetValue(navigation.y);
//         }

//         private void TryNavigationChangeInt(Vector2 oldNav, Vector2 newNav)
//         {
//             Vector2Int oldNavInt = GetIntNavigation(oldNav);
//             Vector2Int newNavInt = GetIntNavigation(newNav);
//             if (oldNav.x != newNav.x || oldNav.y != newNav.y)
//             {
//                 onNavigationChangedInt?.Invoke(newNavInt);
//             }
//         }

//         private void InputNavigationChanged(InputAction.CallbackContext context)
//         {
//             Vector2 oldNav = navigation;
//             navigation = context.ReadValue<Vector2>();
//             TryNavigationChangeInt(oldNav, navigation);
//             UpdateAxisHoldValues(navigation);
//             onNavigationChanged?.Invoke(navigation);
//         }
//         private void OnInputSubmit(InputAction.CallbackContext context)
//         {
//             //Debug.Log("Submit");
//             onSubmit?.Invoke();
//         }
//         private void OnInputCancel(InputAction.CallbackContext context)
//         {
//             //Debug.Log("Cancel");
//             onCancel?.Invoke();
//         }
//         private void OnInputChangeSelect(InputAction.CallbackContext context)
//         {
//             //Debug.Log("ChangeSelect");
//             onChangeSelect?.Invoke();
//         }

//         private void OnEscape(InputAction.CallbackContext context)
//         {
//             //Debug.Log("Escape");
//             onEscape?.Invoke();
//         }

//         //Helper
//         private Vector2Int GetIntNavigation(Vector2 navigation)
//         {
//             Vector2Int output = Vector2Int.zero;
//             if (Mathf.Abs(navigation.x) >= Mathf.Abs(navigationDeadzoneValue))
//             {
//                 output.x = (int)Mathf.Sign(navigation.x);
//             }
//             if (Mathf.Abs(navigation.y) >= Mathf.Abs(navigationDeadzoneValue))
//             {
//                 output.y = (int)Mathf.Sign(navigation.y);
//             }
//             return output;
//         }
//     }
// }

