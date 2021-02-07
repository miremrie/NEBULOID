using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.Character
{
    public interface IOxygenProvider
    {
        float GetOxygenPerSecond();
    }
    [System.Serializable]
    public class Oxygen
    {
        public float min = 0;
        public float max = 100;
        private float current;
        public float Current
        {
            get => current;
            set => current = Mathf.Clamp(value, min, max);
        }
        private IOxygenProvider oxygenProvider;
        public void SetProvider(IOxygenProvider oxygenProvider)
        {
            this.oxygenProvider = oxygenProvider;
        }

        public void UpdateOxygen(float time)
        {
            if (oxygenProvider != null)
            {
                Current += oxygenProvider.GetOxygenPerSecond() * time;
            }
            else
            {
                Debug.Log("Oxygen provider not attached!");
            }
        }
        public void ReduceByTotalPercent(float oxygenPercent)
        {
            float value = oxygenPercent * (max - min);
            Current -= value;
        }
        public bool HasOxygen()
        {
            return Current > min;
        }
        public void ResetToMax()
        {
            Current = max;
        }
    }
}

