using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.Input
{
    public abstract class BeingHeldInputProc
    {
        private Timer holdTimer;
        private float timeBeforeHeld;

        protected void InitializeTimer(float time)
        {
            this.timeBeforeHeld = time;
            holdTimer = new Timer(time);
            holdTimer.Start();
        }

        public float GetFullTime()
        {
            return holdTimer.GetCurrentTime();
        }
        public float GetHoldTime()
        {
            return GetFullTime() - timeBeforeHeld;
        }
        public float GetTimePercent()
        {
            return holdTimer.GetCurrentTimePercentClamped();
        }
        public void ResetTimer()
        {
            holdTimer.Start();
        }

        public void UpdateTime(float deltaTime)
        {
            if (IsHoldInitiated())
            {
                holdTimer.Update(deltaTime);
                if (!holdTimer.IsRunning())
                {
                    InvokeHeldEvent();
                } else
                {
                    InvokeHoldStartedEvent(); 
                }
            }
        }

        protected abstract void InvokeHeldEvent();
        protected abstract void InvokeHoldStartedEvent();

        public bool IsBeingHeld()
        {
            return IsHoldInitiated() && !holdTimer.IsRunning();
        }

        public abstract bool IsHoldInitiated();
    }

    public class ButtonHeldInputProc : BeingHeldInputProc
    {
        public delegate void ButtonHoldEvent(bool value, float time);
        public event ButtonHoldEvent onButtonBeingHeld, onButtonHoldStarted;
        bool value;
        private bool activeValue;

        public ButtonHeldInputProc(float holdTime, bool startingValue = false, bool activeValue = true)
        {
            this.value = startingValue;
            this.activeValue = activeValue;
            InitializeTimer(holdTime);
        }

        public void SetValue(bool value)
        {
            this.value = value;
            if (value != activeValue)
            {
                ResetTimer();
            }
        }

        public override bool IsHoldInitiated()
        {
            return value == activeValue;
        }

        protected override void InvokeHeldEvent()
        {
            if (onButtonBeingHeld != null)
            {
                onButtonBeingHeld(value, GetHoldTime());
            }
        }

        protected override void InvokeHoldStartedEvent()
        {
            if (onButtonHoldStarted != null)
            {
                onButtonHoldStarted(value, GetTimePercent());
            }
        }
    }

    public class AxisHeldInputProc : BeingHeldInputProc
    {
        public delegate void AxisHoldEvent(float value, float time);
        public delegate void IntAxisHoldEvent(int value, float time);
        public event AxisHoldEvent onAxisBeingHeld, onAxisHoldStarted;
        public event IntAxisHoldEvent onAxisBeingHeldInt, onAxisHoldStartedInt;
        private float value;
        private float deadZoneValue;

        public AxisHeldInputProc(float holdTime, float startingValue, float deadZoneValue)
        {
            value = startingValue;
            this.deadZoneValue = deadZoneValue;
            InitializeTimer(holdTime);
        }

        public void SetValue(float newValue)
        {
            CheckHoldStatus(newValue, value);
            value = newValue;
        }

        public float GetValue()
        {
            return value;
        }

        private void CheckHoldStatus(float newValue, float oldValue)
        {
            int oldDiscreteValue = GetDiscreteValue(oldValue);
            int newDiscreteValue = GetDiscreteValue(newValue);
            if (oldDiscreteValue == 0 || oldDiscreteValue != newDiscreteValue)
            {
                ResetTimer();
            }
        }

        private int GetDiscreteValue(float value)
        {
            int discreteValue = 0;
            if (Mathf.Abs(value) >= deadZoneValue)
            {
                discreteValue = (value > 0) ? 1 : -1;
            }
            return discreteValue;
        }

        public override bool IsHoldInitiated()
        {
            return Mathf.Abs(value) >= Mathf.Abs(deadZoneValue);
        }

        protected override void InvokeHeldEvent()
        {
            if (onAxisBeingHeld != null)
            {
                onAxisBeingHeld(value, GetHoldTime());
            }
            if (onAxisBeingHeldInt != null)
            {
                onAxisBeingHeldInt(GetDiscreteValue(value), GetHoldTime());
            }
        }

        protected override void InvokeHoldStartedEvent()
        {
            if (onAxisHoldStarted != null)
            {
                onAxisHoldStarted(value, GetTimePercent());
            }
            if (onAxisHoldStartedInt != null)
            {
                onAxisHoldStartedInt(GetDiscreteValue(value), GetTimePercent());
            }
        }
    }
}