using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    public List<Transform> agents;
    public float rayDistance;
    public LayerMask layerMask;
    public float obstacleSize;
    public float margin;
    public Transform target;
    public List<Vector2> output = new List<Vector2>();
    public float targetDistance;

    Vector2 LeftEdge (Vector2 direction, Vector2 position, float halfSize)
    {
        var leftDir = Rotate90AntiClockwise(direction);
        return position + leftDir * halfSize;
    }

    Vector2 RightEdge (Vector2 direction, Vector2 position, float halfSize)
    {
        var leftDir = Rotate90Clockwise(direction);
        return position + leftDir * halfSize;
    }

    /// <summary>
    /// Returns a path in reverse order (final target first)
    /// </summary>
    public List<Vector2> FindPath(Vector2 from, Vector2 target, float agentSize = 0.1f)
    {
        var agentHalfSize = agentSize * 0.5f;
        var tree = new List<Node>();
        var result = new List<Vector2>();
        RaycastHit2D[] lHits = new RaycastHit2D[1];
        RaycastHit2D[] rHits = new RaycastHit2D[1];

        var first = new Node { position = from, parentIndex = -1 };

        tree.Add(first);

        for (int i = 0; i < tree.Count && i < 100; i++)
        {

            // TODO: direction is recalculated in edge raycast
            var node = tree[i];
            var direction = (target - node.position);
            direction.Normalize();
            var raycast = EdgeRaycast(target, node.position, agentHalfSize, ref lHits, ref rHits);

            if (raycast.Valid)
            {
                // hit something
                var center = raycast.Item.transform.position.ToVector2();
                var cross = Rotate90AntiClockwise(direction);
                var offset = obstacleSize + margin;
                var newPosAntiClock = center + (cross * offset);
                var newPosClock = center + (cross * (-offset));

                // redundant check, also in CheckAndAdd
                var acDist = (newPosAntiClock - node.position).sqrMagnitude;
                var cDist = (newPosClock - node.position).sqrMagnitude;

                // pick closer first
                // come on, you are better then this... maybe...
                if (acDist > cDist)
                {
                    CheckAndAdd(newPosClock);
                    CheckAndAdd(newPosAntiClock);
                }
                else
                {
                    CheckAndAdd(newPosAntiClock);
                    CheckAndAdd(newPosClock);
                }

                void CheckAndAdd(Vector2 newPos)
                {
                    // TODO: Don't need the hit info, just to see if hit anything, but convenience
                    var raycastIn = EdgeRaycast(newPos, node.position, agentHalfSize, ref lHits, ref rHits);
                    if (!raycastIn.Valid)
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

    private static Maybe<RaycastHit2D> EdgeHit(RaycastHit2D lHits, RaycastHit2D rHits, int lHit, int rHit)
    {
        var ret = new Maybe<RaycastHit2D>();
        if (HitOrInside(lHit, lHits)) ret.Item = lHits;
        if (HitOrInside(rHit, rHits)) ret.Item = rHits;
        return ret;
    }

    private Maybe<RaycastHit2D> EdgeRaycast(Vector2 target, Vector2 position, float agentHalfSize, ref RaycastHit2D[] lHits, ref RaycastHit2D[] rHits)
    {
        var direction = (target - position);
        float dist = direction.magnitude;
        direction.Normalize();
        var lEdge = LeftEdge(direction, position, agentHalfSize);
        var rEdge = RightEdge(direction, position, agentHalfSize);
        var lHit = Physics2D.RaycastNonAlloc(lEdge, direction, lHits, dist, layerMask);
        var rHit = Physics2D.RaycastNonAlloc(rEdge, direction, rHits, dist, layerMask);
        return EdgeHit(lHits[0], rHits[0], lHit, rHit);
    }

    private static bool HitOrInside(int result, RaycastHit2D hit)
    {
        return result > 0 && hit.fraction != 0;
    }

    struct Node
    {
        public Vector2 position;
        public int parentIndex;
    }

    Vector2 Rotate90AntiClockwise(Vector2 v) => new Vector2(-v.y, v.x);
    Vector2 Rotate90Clockwise(Vector2 v) => new Vector2(v.y, -v.x);

    public void OnDrawGizmos()
    {
        for (int i = 0; i < output.Count; ++i)
        {
            Gizmos.color = Color.Lerp(Color.green, Color.red, i / (float)output.Count);
            Gizmos.DrawSphere(output[i], .2f);
        }

        if (output.Count > 0)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(agents[0].position, output[0]);
        }
        for (int i = 0; i < output.Count - 1; ++i)
        {
            Gizmos.color = Color.Lerp(Color.green, Color.red, i + 1 / (float)output.Count);
            Gizmos.DrawLine(output[i], output[i + 1]);
        }
    }
}
