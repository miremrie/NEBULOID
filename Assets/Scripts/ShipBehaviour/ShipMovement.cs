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
    public ArmAudioController armAudioController;
    private bool movementLocked = false;
    private List<ShipArmSystem> armSystems = new List<ShipArmSystem>();


    void Start()
    {
    }

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
        if (!movementLocked)
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
            if (currentlyRunning > 1)
            {
                movementVec = -1 * movementVec.normalized;
                ship.Translate(movementVec * moveSpeed * Time.deltaTime, Space.World);
            }

            /*if (leftTimer.IsRunning() && rightTimer.IsRunning())
            {
                ship.Translate(ship.up * moveSpeed * Time.deltaTime, Space.World);
            }
            else if (leftTimer.IsRunning())
            {
                ship.RotateAround(leftPivot.position, Vector3.forward, rotateSpeed * Time.deltaTime);
            }
            else if (rightTimer.IsRunning())
            {
                ship.RotateAround(rightPivot.position, Vector3.forward, -rotateSpeed * Time.deltaTime);
            }*/

            shipInterior.rotation = Quaternion.identity;
        }

    }

    public void Rotate(bool left)
    {
        if (left)
        {
            armAudioController.PlayLeftArm();
        }
        else
        {
            armAudioController.PlayRightArm();
        }
    }

    public void LockShip()
    {
        movementLocked = true;
    }

    public void UnlockShip()
    {
        movementLocked = false;
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