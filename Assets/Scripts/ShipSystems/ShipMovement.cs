using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NBLD.Ship
{
    public class ShipMovement : MonoBehaviour
    {
        public Rigidbody2D rb;
        public Transform shipRoot;
        public float animTime;
        public float rotateSpeed;
        public float moveSpeed;
        public Transform shipInterior;
        public ShipAudioController audioController;
        public bool hookLocked = false;
        private List<ShipArmSystem> armSystems = new List<ShipArmSystem>();
        public bool movementLocked;
        public List<Transform> posDependentTransforms = new List<Transform>();
        public List<Transform> rotDependentTransforms = new List<Transform>();
        public Vector2 prevFramePos;
        public Quaternion prevFrameRot;
        private void Start()
        {
            prevFramePos = transform.position.ToVector2();
            prevFrameRot = transform.rotation;
        }
        void FixedUpdate()
        {

            if (Keyboard.current.cKey.wasPressedThisFrame)
            {
                armSystems[0].DoAction();
            }

            if (Keyboard.current.vKey.wasPressedThisFrame)
            {
                armSystems[1].DoAction();
            }

            MovementUpdate();

        }
        private void LateUpdate()
        {
            Vector2 deltaPos = transform.position.ToVector2() - prevFramePos;
            Quaternion deltaRot = transform.rotation * Quaternion.Inverse(prevFrameRot);
            UpdateDependentTransforms(deltaPos, deltaRot);
            prevFramePos = transform.position.ToVector2();
            prevFrameRot = transform.rotation;
        }

        void MovementUpdate()
        {
            if (!IsShipMovementLocked())
            {
                int currentlyRunning = 0;
                //Vector3 movementVec = Vector3.zero;
                Vector2 originalInteriorPosition = shipInterior.position;
                //Quaternion originalInteriorRotation = shipInterior.rotation;
                Vector3 nextPos = rb.transform.position;
                Quaternion nextRot = rb.transform.rotation;

                foreach (ShipArmSystem arm in armSystems)
                {
                    if (arm.IsMoving())
                    {
                        currentlyRunning++;
                        //movementVec += arm.pivot.up * -1;

                        float speed = rotateSpeed;
                        if (!arm.isLeft)
                        {
                            speed *= -1;
                        }
                        Vector3 axis = Vector3.forward;
                        float angle = speed * Time.deltaTime;
                        Vector3 pivot = arm.pivot.position;
                        //ship.RotateAround(origin, axis, angle);
                        Quaternion q = Quaternion.AngleAxis(angle, axis);
                        Vector2 forceDir = ((q * (rb.transform.position - pivot) + pivot) - rb.transform.position).normalized;
                        //Vector2 force = (q * Vector3.right);
                        Vector2 force = forceDir * arm.moveForce * Time.deltaTime;
                        rb.AddForceAtPosition(force, transform.position, ForceMode2D.Impulse);

                        nextPos += (q * (rb.transform.position - pivot) + pivot) - rb.transform.position;
                        nextRot *= q;
                        //RotateRigidBodyAroundPointBy(rb, origin, axis, angle);
                    }
                }
                //rb.MovePosition(nextPos);
                rb.MoveRotation(nextRot);

                /*if (currentlyRunning > 1)
                {
                    movementVec = -1 * movementVec.normalized;
                    ship.Translate(movementVec * moveSpeed * Time.deltaTime, Space.World);
                }*/
                //movementLocked = currentlyRunning >= 1;
            }
        }
        public void MoveShip(Vector2 deltaMovement)
        {
            Vector3 pos = new Vector3(transform.position.x + deltaMovement.x,
            transform.position.y + deltaMovement.y,
            transform.position.z);
            rb.MovePosition(pos);

            //UpdateDependentTransforms(deltaMovement);
        }

        private void UpdateDependentTransforms(Vector2 deltaPos, Quaternion deltaRot)
        {
            for (int i = 0; i < posDependentTransforms.Count; i++)
            {
                posDependentTransforms[i].position = (Vector2)posDependentTransforms[i].position + deltaPos;
            }
            for (int i = 0; i < rotDependentTransforms.Count; i++)
            {
                rotDependentTransforms[i].Rotate(deltaRot.eulerAngles);
            }
        }

        public void Rotate(bool left)
        {
            if (left)
            {
                audioController.PlayLeftArm();
            }
            else
            {
                audioController.PlayRightArm();
            }
        }

        public void LockHook()
        {
            hookLocked = true;
        }

        public bool AreHooksLocked()
        {
            return hookLocked;
        }

        public void UnlockHook()
        {
            hookLocked = false;
        }

        public void LockMovement()
        {
            movementLocked = true;
        }

        public void UnlockMovement()
        {
            movementLocked = false;
        }

        public bool IsShipMovementLocked()
        {
            return movementLocked;
        }

        public Vector3 GetShipCentralPoint()
        {
            return transform.position;
        }

        public void RegisterArm(ShipArmSystem shipArmSystem)
        {
            armSystems.Add(shipArmSystem);
        }

        public void AddPosDependentTransform(Transform transform)
        {
            if (!posDependentTransforms.Contains(transform))
            {
                posDependentTransforms.Add(transform);
            }
        }
        public void RemovePosDependentTransform(Transform transform)
        {
            if (posDependentTransforms.Contains(transform))
            {
                posDependentTransforms.Remove(transform);
            }
        }
    }
}