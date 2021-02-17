
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Editor;
using UnityEngine.Scripting;

namespace NBLD.Input
{
    [DisplayName("Held")]
    [UnityEditor.InitializeOnLoad]
    public class HeldInteraction : IInputInteraction
    {
        public float duration = InputSystem.settings.defaultHoldTime;
        public float pressPoint = InputSystem.settings.defaultButtonPressPoint;

        private float durationOrDefault => duration > 0.0 ? duration : InputSystem.settings.defaultHoldTime;
        private float pressPointOrDefault => pressPoint > 0.0 ? pressPoint : InputSystem.settings.defaultButtonPressPoint;

        private double m_TimePressed;

        /// <inheritdoc />
        public void Process(ref InputInteractionContext context)
        {
            if (context.timerHasExpired)
            {
                context.PerformedAndStayPerformed();
                return;
            }

            switch (context.phase)
            {
                case InputActionPhase.Waiting:
                    if (context.ControlIsActuated(pressPointOrDefault))
                    {
                        m_TimePressed = context.time;
                        Debug.Log($"timewait:{m_TimePressed}");
                        context.Started();
                        context.SetTimeout(durationOrDefault);
                    }
                    break;

                case InputActionPhase.Started:
                    // If we've reached our hold time threshold, perform the hold.
                    // We do this regardless of what state the control changed to.
                    Debug.Log($"started:{context.time}");
                    if (context.time - m_TimePressed >= durationOrDefault)
                    {
                        context.PerformedAndStayPerformed();
                    }
                    else if (!context.ControlIsActuated())
                    {
                        // Control is no longer actuated and we haven't performed a hold yet,
                        // so cancel.
                        context.Canceled();
                    }
                    break;

                case InputActionPhase.Performed:
                    if (!context.ControlIsActuated(pressPointOrDefault))
                        context.Canceled();
                    break;
            }


        }

        static HeldInteraction()
        {
            InputSystem.RegisterInteraction<HeldInteraction>();
        }
        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            //Calls static constructor as a side effect
        }
        public void Reset()
        {
            m_TimePressed = 0;
        }
    }
}