using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    public Transform ship;
    public float animTime;
    private Timer leftTimer;
    private Timer rightTimer;
    public Transform leftPivot;
    public Transform rightPivot;
    public float rotateSpeed;
    public float moveSpeed;
    public Transform shipInterior;


    // Start is called before the first frame update
    void Start()
    {
        leftTimer = new Timer(animTime);
        rightTimer = new Timer(animTime);
    }

    // Update is called once per frame
    void Update()
    {
        leftTimer.Update(Time.deltaTime);
        rightTimer.Update(Time.deltaTime);

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

        if (leftTimer.IsRunning() && rightTimer.IsRunning())
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
        }
        shipInterior.rotation = Quaternion.identity;
    }

    public void Rotate(bool left)
    {
        if (left) leftTimer.Start();
        else rightTimer.Start();
      
    }
}

class Timer {

    private float time;
    private float currTime;
    
    public Timer(float time)
    {
        this.time = time;
        currTime = time;
    }

    public void Update(float dt)
    {
        currTime += dt;
    }

    public bool IsRunning() {

        return (currTime < time);
    }

    public void Start()
    {
        currTime = 0f;
    }

}