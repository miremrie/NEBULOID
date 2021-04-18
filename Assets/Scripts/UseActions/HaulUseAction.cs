using System.Collections;
using System.Collections.Generic;
using NBLD.Character;
using UnityEngine;

namespace NBLD.UseActions
{
    public class HaulUseAction : UseAction
    {
        public List<UseAction> attachedActions = new List<UseAction>();
        public Vector2 insideOffset;
        public Transform haulPivot;
        public float haulFollowSpeed = 30f;
        public float haulRotationSpeed = 2f;
        public float haulOutsideSpeedFactor = 0.2f;
        [Header("State Based")]
        public List<GameObject> activeOutside;
        public List<GameObject> activeInside;
        public Vector3 outsideScale = Vector3.one;
        public Vector3 insideScale;
        bool hauling = false;
        CharController hauler;
        private bool isInsideShip = false;
        public bool IsInsideShip => isInsideShip;

        public override int DefaultActionPriority => 90;


        private void Start()
        {
            UpdateStateBasedObjects();
        }
        public override bool AvailableForChar(CharController charController)
        {
            return ((isInsideShip && charController.GetState() == CharState.Inside) || (!isInsideShip && charController.GetState() == CharState.Outside));
        }
        public override void DoAction(CharController user)
        {
            if (!hauling)
            {
                hauling = true;
                hauler = user;
                user.SetHauling(this);
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
                if (!isInsideShip)
                {
                    Vector3 newPivotPos = Vector3.Lerp(haulPivot.position, hauler.transform.position, haulFollowSpeed * Time.deltaTime);
                    Vector3 delta = newPivotPos - haulPivot.position;

                    newPivotPos.z = transform.position.z;
                    transform.position = transform.position + newPivotPos - haulPivot.position;

                    float angle = Mathf.Atan2(delta.normalized.y, delta.normalized.x) * Mathf.Rad2Deg;
                    Quaternion targetRot = Quaternion.AngleAxis(angle, Vector3.forward);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, delta.magnitude * haulRotationSpeed * Time.deltaTime);
                }
                else
                {
                    Vector2 lookDir = hauler.GetLookDirection();
                    transform.position = hauler.transform.position + new Vector3(insideOffset.x * lookDir.x, insideOffset.y, 0);
                }
            }
        }

        public void ChangeInsideShipState(bool isInsideShip)
        {
            if (this.isInsideShip != isInsideShip)
            {
                transform.rotation = Quaternion.identity;
                //transform.position = hauler.transform.position + insideOffset;
            }
            this.isInsideShip = isInsideShip;
            UpdateStateBasedObjects();
        }
        private void UpdateStateBasedObjects()
        {
            foreach (var obj in activeInside)
            {
                obj.SetActive(isInsideShip);
            }
            foreach (var obj in activeOutside)
            {
                obj.SetActive(!isInsideShip);
            }
            transform.localScale = (isInsideShip) ? insideScale : outsideScale;

        }

        public void StopHauling()
        {
            hauling = false;
            hauler.StopHauling();
        }

        private void OnDestroy()
        {
            if (IsBeingHauled())
            {
                hauler.RemoveDestroyedAction(this);
                StopHauling();
            }
        }

        public bool IsBeingHauled() => hauling;
        public CharController GetHauler()
        {
            return (hauling) ? hauler : null;
        }

    }
}

