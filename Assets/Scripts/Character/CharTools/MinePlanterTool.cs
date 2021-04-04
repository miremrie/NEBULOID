using System.Collections;
using System.Collections.Generic;
using NBLD.Utils;
using UnityEngine;

namespace NBLD.Character
{
    public class MinePlanterTool : CharTool
    {
        public Transform minePlacementRoot;
        public Mine minePrefab;
        public float cooldownTime = 5f;
        private Timer cooldownTimer;


        public override void Initialize()
        {
            base.Initialize();
            cooldownTimer = new Timer(cooldownTime);
        }
        public override void Activate()
        {
            base.Activate();
            if (IsAvailable())
            {
                Mine mine = GameObject.Instantiate(minePrefab, wielder.transform.position, Quaternion.identity);
                mine.transform.parent = minePlacementRoot;
                mine.Activate();
                cooldownTimer.Restart();
            }
        }
        public override bool IsAvailable()
        {
            return base.IsAvailable() && wielder.GetState() == CharState.Outside && !cooldownTimer.IsRunning() && equiped;
        }
    }
}

