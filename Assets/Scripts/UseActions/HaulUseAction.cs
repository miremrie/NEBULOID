using System.Collections;
using System.Collections.Generic;
using NBLD.Character;
using UnityEngine;

namespace NBLD.UseActions
{
    public class HaulUseAction : OutsideUseAction
    {
        public Transform haulPivot;
        public float haulFollowSpeed = 30f;
        public float haulRotationSpeed = 2f;
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
                Vector3 delta = newPivotPos - haulPivot.position;

                newPivotPos.z = transform.position.z;
                transform.position = transform.position + newPivotPos - haulPivot.position;
                
                float angle = Mathf.Atan2(delta.normalized.y, delta.normalized.x) * Mathf.Rad2Deg;
                Quaternion targetRot = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, delta.magnitude * haulRotationSpeed * Time.deltaTime);
            }
        }

        public void StopHauling()
        {
            hauling = false;
            hauler.StopHauling();
        }

    }
}

