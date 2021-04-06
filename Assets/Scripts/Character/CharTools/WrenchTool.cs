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
        public float GetRepairSpeedMultiplier() => repairSpeedMultiplier;
        public override bool IsAvailable()
        {
            return base.IsAvailable() && equiped;
        }
    }
}

