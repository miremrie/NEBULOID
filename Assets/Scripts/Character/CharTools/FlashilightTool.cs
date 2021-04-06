using System.Collections;
using System.Collections.Generic;
using NBLD.Utils;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace NBLD.Character
{
    public class FlashilightTool : CharTool
    {
        public Transform lightRoot;
        public Light2D light2D;
        public AnimationCurve intensityPerChargePercent;
        public float maxIntensity;
        public float normalIntensity;
        public float minCharge = 0, maxCharge = 100;
        [SerializeField]
        private float currentCharge;
        public float CurrentCharge
        {
            get => currentCharge;
            set => currentCharge = Mathf.Clamp(value, minCharge, maxCharge);
        }
        public float chargePerSecond;
        public float dropoffPerSecond;
        private bool lightOn = false;
        private Timer dropoffTimer;
        public float ChargePercent => (CurrentCharge - minCharge) / (maxCharge - minCharge);

        public override CharToolType GetCharToolType() => CharToolType.Flashlight;

        public override void Initialize()
        {
            base.Initialize();
            SetLight(false);
            CurrentCharge = minCharge;
        }
        public override void EquipToCharacter(CharController charController)
        {
            base.EquipToCharacter(charController);
            CurrentCharge = minCharge;
        }
        public override void Activate()
        {
            base.Activate();
            ToggleLight();
            if (lightOn)
            {
                if (Mathf.Approximately(CurrentCharge, maxCharge))
                {
                    //TODO: Stun in front
                }
            }
            else
            {
                CurrentCharge = minCharge;
            }
        }

        protected override void Update()
        {
            if (equiped)
            {
                if (wielder.GetState() != CharState.Outside)
                {
                    SetLight(false);
                }
                if (!lightOn)
                {
                    CurrentCharge += chargePerSecond * Time.deltaTime;
                }
                else
                {
                    CurrentCharge -= dropoffPerSecond * Time.deltaTime;
                    light2D.intensity = Mathf.Clamp(maxIntensity * intensityPerChargePercent.Evaluate(ChargePercent), normalIntensity, maxIntensity);
                }
                lightRoot.transform.rotation = Quaternion.FromToRotation(Vector3.up, wielder.GetLookDirection());
            }
        }

        public override bool IsAvailable()
        {
            return base.IsAvailable() && equiped && wielder.GetState() == CharState.Outside;
        }

        private void ToggleLight()
        {
            SetLight(!lightOn);
        }
        private void SetLight(bool on)
        {
            lightOn = on;
            light2D.gameObject.SetActive(lightOn);
        }
    }
}

