using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    public Transform ship;
    public float animTime;
    public float rotateSpeed;
    public float moveSpeed;
    public Transform shipInterior;
    public ShipAudioController audioController;
    public bool hookLocked = false;
    private List<ShipArmSystem> armSystems = new List<ShipArmSystem>();
    public bool movementLocked;

    void Update()
    {

        /*if (Input.GetKeyDown(KeyCode.A))
        {
            Rotate(true);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            Rotate(false);
        }*/

        MovementUpdate();
    }

    void MovementUpdate() {
        if (!IsShipMovementLocked())
        {
            int currentlyRunning = 0;
            Vector3 movementVec = Vector3.zero;
            foreach (ShipArmSystem arm in armSystems)
            {
                if(arm.IsMoving())
                {
                    currentlyRunning++;
                    movementVec += arm.pivot.up * -1;

                    float speed = rotateSpeed;
                    if (!arm.isLeft)
                    {
                        speed *= -1;
                    }

                    ship.RotateAround(arm.pivot.position, Vector3.forward, speed * Time.deltaTime);
                }
            }
            /*if (currentlyRunning > 1)
            {
                movementVec = -1 * movementVec.normalized;
                ship.Translate(movementVec * moveSpeed * Time.deltaTime, Space.World);
            }*/
            //movementLocked = currentlyRunning >= 1;


            shipInterior.rotation = Quaternion.identity;
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
}