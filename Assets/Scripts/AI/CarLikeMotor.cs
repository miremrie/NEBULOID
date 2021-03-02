using UnityEngine;
using System;

[CreateAssetMenu(fileName = "CarLikeMotor", menuName = "Motors/CarLikeMotor", order = 1)]
public class CarLikeMotor : SimpleMotor
{
    public float rotationSpeed;

    public override bool AbsoluteRotation() { return false; }
    public override MotorResult UpdateMotorVelocity(Transform trans, Vector3 target, Vector3 prevVelocity)
    {
        Vector3 desiredVelocity;
        var forward = trans.forward;

        // arrival
        var targetDirection = target - trans.position;
        var distance = targetDirection.magnitude;
        float rampedSpeed = maxSpeed * Math.Min(1f, distance / slowingDistance);
        desiredVelocity = rampedSpeed * targetDirection.normalized;

        var left = PathFinding.Rotate90Left(forward);
        var sin = Vector2.Dot(desiredVelocity.normalized, left.normalized); // left magnitude forw
        var forwardDot = Vector2.Dot(forward, desiredVelocity.normalized);

        float rotAmount = forwardDot > 0 ? sin : Math.Sign(sin);

        var finalRotation = Quaternion.AngleAxis(rotAmount * rotationSpeed, Vector3.down);

        var gas = Vector2.Dot(desiredVelocity, forward);
        var steering = forward * gas;
        steering = steering - prevVelocity;
        var steering_force = Vector3.ClampMagnitude(steering, maxForce);

        var acceleration = steering_force / mass;

        var finalVelocity =  Vector3.ClampMagnitude(prevVelocity + acceleration, maxSpeed);
        return new MotorResult { velocity = finalVelocity, rotation = finalRotation};

    }
}
