using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PathFinding : MonoBehaviour
{
    public List<Transform> agents;
    public float rayDistance;
    public LayerMask layerMask;
    public Vector3 foundPos;
    public float obstacleSize;
    public float margin;
    public Transform target;
    public List<Vector2> output = new List<Vector2>();


    void Update()
    {
        if (Keyboard.current.oKey.IsPressed())
        { 
            output = FindPath(agents[0].position, target.position);
        }
    }

    // TODO: make a struct with vector and parent for better locality of reference
    List<Vector2> FindPath(Vector2 from, Vector2 target)
    {
        var tree = new List<Vector2>();
        var parents = new List<int>();
        var result = new List<Vector2>();
        RaycastHit2D[] hits = new RaycastHit2D[1];

        tree.Add(from);
        parents.Add(-1);

        for (int i = 0; i < tree.Count && i < 100; i++)
        {
            var currPos = tree[i];
            var direction = ToVector2(target - currPos);
            direction.Normalize();
            var hit = Physics2D.RaycastNonAlloc(currPos, direction, hits, rayDistance, layerMask);

            Debug.DrawLine(currPos, currPos + (direction * rayDistance), Color.red);

            if (hit > 0)
            {
                // hit something
                var center = ToVector2(hits[0].transform.position);
                var cross = Rotate90AntiClockwise(direction);
                var offset = obstacleSize + margin;
                var newPos = center + (cross * offset);
                var newPosClock = center + (cross * (-offset));

                CheckAndAdd(i, currPos, newPos);
                CheckAndAdd(i, currPos, newPosClock);
            }
            else
            {
                // found path
                for (int j = i; j > 0; j = parents[j])
                {
                    result.Add(tree[j]);
                }
                return result;
            }
        }

        // reached limit
        return result;

        void CheckAndAdd(int i, Vector2 currPos, Vector2 newPos)
        {
            var direction = newPos - currPos;
            var lHit = Physics2D.RaycastNonAlloc(currPos, direction, hits, rayDistance, layerMask);
            if (lHit == 0)
            {
                tree.Add(newPos);
                parents.Add(i);
            }
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach (var item in output)
        {
            Gizmos.DrawSphere(item, .2f);
        }
    }

    Vector2 Rotate90AntiClockwise(Vector2 v) => new Vector2(-v.y, v.x);
    Vector2 Rotate90Clockwise(Vector2 v) => new Vector2(v.y, -v.x);

    Vector2 ToVector2(Vector3 v) => new Vector2(v.x, v.y);
}
