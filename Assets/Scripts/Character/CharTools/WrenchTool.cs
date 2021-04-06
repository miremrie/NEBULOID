using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.Character
{
    public class WrenchTool : CharTool
    {
        public override CharToolType GetCharToolType() => CharToolType.Wrench;
        [SerializeField]
        private float repairSpeedMultiplier;
        public float drillMovementSpeedFactor = 0.5f;
        public Transform drillRoot;
        public Collider2D drillCollider;
        private bool drillActive = false;
        public float GetRepairSpeedMultiplier() => repairSpeedMultiplier;
        public override void Initialize()
        {
            base.Initialize();
            SetDrillActive(false);
        }
        public override bool IsAvailable()
        {
            return base.IsAvailable() && equiped && wielder.GetState() == CharState.Outside;
        }
        public override void Unequip()
        {
            SetDrillActive(false);
            base.Unequip();
        }
        public override void Activate()
        {
            base.Activate();
            ToggleDrillActive();
        }
        protected override void Update()
        {
            drillRoot.transform.rotation = Quaternion.FromToRotation(Vector3.up, wielder.GetLookDirection());
        }
        private void SetDrillActive(bool active)
        {
            drillActive = active;
            drillRoot.gameObject.SetActive(drillActive);
            if (equiped)
            {
                if (active)
                {
                    wielder.outsideBehaviour.SetOverridenSpeedFactor(drillMovementSpeedFactor);
                }
                else
                {
                    wielder.outsideBehaviour.ReleaseOverrideSpeed();
                }
            }

        }
        private void ToggleDrillActive()
        {
            SetDrillActive(!drillActive);
        }
    }
}

