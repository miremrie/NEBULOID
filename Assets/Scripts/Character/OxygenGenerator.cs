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
        public float current;

        private IOxygenProvider oxygenProvider;
        public void SetProvider(IOxygenProvider oxygenProvider)
        {
            this.oxygenProvider = oxygenProvider;
        }

        public void UpdateOxygen(float time)
        {
            if (oxygenProvider != null)
            {
                current += oxygenProvider.GetOxygenPerSecond() * time;
                current = Mathf.Clamp(current, min, max);
            }
            else
            {
                Debug.Log("Oxygen provider not attached!");
            }
        }
        public bool HasOxygen()
        {
            return current > min;
        }
    }
}

