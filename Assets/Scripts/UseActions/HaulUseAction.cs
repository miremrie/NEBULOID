using System.Collections;
using System.Collections.Generic;
using NBLD.Character;
using UnityEngine;

namespace NBLD.UseActions
{
    public class HaulUseAction : OutsideUseAction
    {
        public Transform haulPivot;
        public float haulFollowSpeed = 10f;
        bool hauling = false;
        OutsideCharBehaviour hauler;
        public override void DoAction(OutsideCharBehaviour behaviour)
        {
            if (!hauling)
            {
                hauling = true;
                hauler = behaviour;
            }
            else
            {
                StopHauling();
            }
        }

        private void LateUpdate()
        {
            if (hauling)
            {
                Vector3 newPivotPos = Vector3.Lerp(haulPivot.position, hauler.transform.position, haulFollowSpeed * Time.deltaTime);
                //transform.rotation = Quaternion.LookRotation(hauler.transform.position - newPivotPos, Vector3.back);
                transform.position = transform.position + newPivotPos - haulPivot.position;
            }
        }

        public void StopHauling()
        {
            hauling = false;
            hauler.StopHauling();
        }

    }
}

