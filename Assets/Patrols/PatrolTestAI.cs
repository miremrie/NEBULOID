using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PatrolTestAI : MonoBehaviour
{
    public float moveSpeed = 2;
    public PatrolPath path;

    public bool circular;
    public bool forward;
    public int startIndex;
    public float nearDistance = 0.1f;

    PatrolWalk patrolWalk;
    public Motor motor;
    public float slowingDistance = 1f;
    public float radius = 0.5f;
    private Vector3 obsAvoidance;

    public Transform attackTarget;
    public bool attack;
    public PathFinding pathfinding;
    public List<Vector2> attackPath;
    public Walk attackWalk;

    public float attackRadius = 10;
    public float agentSize;
    public float randomDelta;
    public FindPathInput pathfindingInput;

    void Start()
    {
        motor.trans = transform;
        Randomize(ref motor.maxForce, randomDelta);
        Randomize(ref motor.maxSpeed, randomDelta);
        UpdatePatrolPathWalk();
    }

    void Randomize (ref float val, float delta) { val = UnityEngine.Random.Range(val - delta, val + delta);}

    void Update()
    {

        attack = (transform.position - attackTarget.position).magnitude < attackRadius;
        if (attack)
        {
            pathfindingInput.from = transform.position;
            pathfindingInput.target = attackTarget.position;

            attackPath = PathFinding.FindPath(pathfindingInput);
            attackWalk = new Walk(attackPath, transform, nearDistance);
        }

        var target = SelectTarget();
        if (target != Vector2.zero)
        {
            UpdateMotorVelocity(motor, SelectTarget(), obsAvoidance);
            transform.position += motor.velocity * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(motor.velocity, Vector3.back);
        }
    }

    private Vector2 SelectTarget()
    {
        Vector2 target;
        if (attack && attackWalk != null)
        {
            target = attackWalk.Target();
        }
        else if (!attack && patrolWalk != null)
        {
            target = patrolWalk.Target();
        }
        else
        {
            target = Vector2.zero;
        }

        return target;
    }

    // Simple steering with bullshit added
    private void UpdateMotorVelocity(Motor motor, Vector3 target, Vector3 obsAvoidance)
    {
        Vector3 desiredVelocity;
        {
            // arrival
            var targetDirection = target - transform.position;
            var distance = targetDirection.magnitude;
            float rampedSpeed = motor.maxSpeed * Math.Min(1f, distance / slowingDistance);
            desiredVelocity = rampedSpeed * targetDirection.normalized;
            desiredVelocity += obsAvoidance;
        }

        var steering = desiredVelocity - motor.velocity;

        var steering_force = Vector3.ClampMagnitude(steering, motor.maxForce);

        var acceleration = steering_force / motor.mass;
        motor.velocity = Vector3.ClampMagnitude(motor.velocity + acceleration, motor.maxSpeed);
    }

    public void SetObstacleAvoidance(Vector3 v)
    {
        obsAvoidance = v;
    }

    [ContextMenu("Update Patrol Path Walk")]
    public void UpdatePatrolPathWalk()
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
        patrolWalk = new PatrolWalk(cong);

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

    // TODO: Overlap functionality with patrol path, merge behind interface
    // Walks backwards because pathfinding returns the path backwards
    public class Walk
    {
        private readonly List<Vector2> path;
        private readonly Transform trans;
        private int index;
        private int lastIndex;
        private float nearDistance;

        public Walk(List<Vector2> path, Transform trans, float nearDistance)
        {
            this.path = path;
            this.trans = trans;
            this.nearDistance = nearDistance;

            lastIndex = path.Count - 1;

            index = lastIndex;
        }

        internal Vector2 Target()
        {
            if (path.Count == 0) return trans.position;
            if (IsNear() && index > 0) --index;
            return path[index];
        }

        private bool IsNear() => (path[index] - trans.position.ToVector2()).sqrMagnitude < nearDistance;
    }

    public void OnDrawGizmos()
    {
        if (attack)
        {
            var displayList = new List<Vector2>(attackPath);
            displayList.Reverse();

            for (int i = 0; i < displayList.Count; ++i)
            {
                Gizmos.color = Color.Lerp(Color.green, Color.red, i / (float)displayList.Count);
                Gizmos.DrawSphere(displayList[i], .2f);
            }

            if (displayList.Count > 0)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, displayList[0]);
            }
            for (int i = 0; i < displayList.Count - 1; ++i)
            {
                Gizmos.color = Color.Lerp(Color.green, Color.red, i + 1 / (float)displayList.Count);
                Gizmos.DrawLine(displayList[i], displayList[i + 1]);
            }
        }
    }
}

