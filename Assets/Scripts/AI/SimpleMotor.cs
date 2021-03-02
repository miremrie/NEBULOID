using System;
using UnityEngine;

public abstract class Motor : ScriptableObject
{
     public abstract MotorResult UpdateMotorVelocity(Transform trans, Vector3 target, Vector3 prevVelocity);
     public abstract bool AbsoluteRotation();
}

public struct MotorResult
{ 
    public Vector3 velocity;
    public Quaternion rotation;
}

[Serializable]
[CreateAssetMenu(fileName = "SimpleMotor", menuName = "Motors/SimpleMotor", order = 1)]
public class SimpleMotor :  Motor
{
    public float mass = 1f;
    public float maxSpeed = 5f;
    public float maxForce = 5f;
    public float slowingDistance = 1f;

    public override bool AbsoluteRotation() { return true; }

    public override MotorResult UpdateMotorVelocity(Transform trans, Vector3 target, Vector3 prevVelocity)
    {
        Vector3 desiredVelocity;
        var targetDirection = target - trans.position;
        var distance = targetDirection.magnitude;
        float rampedSpeed = maxSpeed * Math.Min(1f, distance / slowingDistance);

        desiredVelocity = rampedSpeed * targetDirection.normalized;
        var steering = desiredVelocity - prevVelocity;
        var steering_force = Vector3.ClampMagnitude(steering, maxForce);
        var acceleration = steering_force / mass;
        var finalVelocity =  Vector3.ClampMagnitude(prevVelocity + acceleration, maxSpeed);

        var rotation = Quaternion.LookRotation(finalVelocity, Vector3.back);
        return new MotorResult { velocity = finalVelocity, rotation = rotation};
    }
}

