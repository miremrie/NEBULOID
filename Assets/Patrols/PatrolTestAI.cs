using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolTestAI : MonoBehaviour
{
    public float moveSpeed = 2;
    public PatrolPath path;

    public bool circular;
    public bool forward;
    public int startIndex;
    public float nearDistance = 0.1f;

    PatrolWalk walk;
    public Motor motor;
    public float slowingDistance = 1f;
    public float radius = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        motor.trans = transform; 
        UpdatePath();
    }

    // Update is called once per frame
    void Update()
    {

        // arrival
        var targetOffset = walk.Target() - transform.position;
        var distance = targetOffset.magnitude;
        var rampedSpeed = motor.maxSpeed * (distance / slowingDistance);
        var clippedSpeed = Math.Min(rampedSpeed, motor.maxSpeed);

        var desiredVelocity = (clippedSpeed / distance) * targetOffset;
        var steering = desiredVelocity - motor.velocity;

        var steering_force = Vector3.ClampMagnitude(steering, motor.maxForce);

        var acceleration = steering_force / motor.mass;
        motor.velocity = Vector3.ClampMagnitude(motor.velocity + acceleration, motor.maxSpeed);
        transform.position += motor.velocity * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(motor.velocity, Vector3.back);
    }

    [ContextMenu("Update Path")]
    public void UpdatePath()
    {
        var cong = new PatrolWalkConfig
        {
            path = path,
            trans = transform,
            circular = circular,
            forward = forward,
            nearDistance = nearDistance,
            startIndex = startIndex
        };
        walk = new PatrolWalk(cong);

    }

    [Serializable]
    public class Motor
    {
        public Transform trans;
        public float mass = 1f;
        public Vector3 velocity;
        public float maxSpeed = 5f;
        public float maxForce = 5f;
    }

}
