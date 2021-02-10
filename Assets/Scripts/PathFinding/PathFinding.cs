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
    public float targetDistance;


    void Update()
    {
        if (Keyboard.current.oKey.IsPressed())
        { 
            output = FindPath(agents[0].position, target.position);
            output.Reverse();
        }
    }

    List<Vector2> FindPath(Vector2 from, Vector2 target)
    {
        var tree = new List<Node>();
        var result = new List<Vector2>();
        RaycastHit2D[] hits = new RaycastHit2D[1];

        var first = new Node { position = from, parentIndex = -1 };

        tree.Add(first);

        for (int i = 0; i < tree.Count && i < 100; i++)
        {
            var node = tree[i];
            var direction = ToVector2(target - node.position);
            float dist = direction.magnitude;
            direction.Normalize();
            var hit = Physics2D.RaycastNonAlloc(node.position, direction, hits, dist, layerMask);

            Debug.DrawLine(node.position, node.position + (direction * dist), Color.red);

            if (hit > 0)
            {
                // hit something
                var center = ToVector2(hits[0].transform.position);
                var cross = Rotate90AntiClockwise(direction);
                var offset = obstacleSize + margin;
                var newPosAntiClock = center + (cross * offset);
                var newPosClock = center + (cross * (-offset));

                CheckAndAdd(newPosAntiClock);
                CheckAndAdd(newPosClock);

                void CheckAndAdd(Vector2 newPos)
                {
                    direction = newPos - node.position;
                    dist = direction.magnitude; 
                    var lHit = Physics2D.RaycastNonAlloc(node.position, direction, hits, dist, layerMask);
                    if (lHit == 0)
                    {
                        tree.Add(new Node { position = newPos, parentIndex = i });
                    }
                }
            }
            else
            {
                // found path
                var final = target - (direction * targetDistance);
                result.Add(final);

                for (int j = i; j > 0; j = tree[j].parentIndex)
                {
                    result.Add(tree[j].position);
                }

                return result;
            }
        }

        // reached limit
        return result;

    }

    struct Node {
        public Vector2 position;
        public int parentIndex;
    }

    Vector2 Rotate90AntiClockwise(Vector2 v) => new Vector2(-v.y, v.x);
    Vector2 Rotate90Clockwise(Vector2 v) => new Vector2(v.y, -v.x);
    Vector2 ToVector2(Vector3 v) => new Vector2(v.x, v.y);

    public void OnDrawGizmos()
    {
        for (int i = 0; i < output.Count; ++i)
        {
            Gizmos.color = Color.Lerp(Color.green, Color.red, i/(float)output.Count);
            Gizmos.DrawSphere(output[i], .2f);
        }


        if (output.Count > 0)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(agents[0].position, output[0]);
        }
        for (int i = 0; i < output.Count - 1; ++i)
        {
            Gizmos.color = Color.Lerp(Color.green, Color.red, i + 1/(float)output.Count);
            Gizmos.DrawLine(output[i], output[i + 1]);
        }
    }


}
