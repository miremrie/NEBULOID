using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[Serializable]
public class FindPathInput
{
    public Vector2 from;
    public Vector2 target;
    public float agentSize;
    public LayerMask layerMask;
    public float margin = 0.5f;          // scaled with obstacle size
    public float targetDistance = 1f;    // stop at this distance to target
    public float obstacleSize = 1f;      // default value if circle collider not found
}

public class PathFinding
{
    const int PATH_NODES_MAX = 50;
    /// <summary>
    /// Returns a path in reverse order (final target first)
    /// </summary>
    public static List<Vector2> FindPath(FindPathInput input)
    {
        var agentHalfSize = input.agentSize * 0.5f;
        var tree = new List<Node>();
        var result = new List<Vector2>();
        // TODO PERF: Don't need allocations per call if not running in parallel
        RaycastHit2D[] lHits = new RaycastHit2D[2];
        RaycastHit2D[] rHits = new RaycastHit2D[2];

        var first = new Node { position = input.from, parentIndex = -1 };

        tree.Add(first);

        for (int i = 0; i < tree.Count && i < PATH_NODES_MAX; i++)
        {
            // TODO PERF: direction is recalculated in edge raycast
            var node = tree[i];
            var direction = (input.target - node.position);
            direction.Normalize();
            var raycast = EdgeRaycast(input.target, node.position, agentHalfSize, ref lHits, ref rHits, input.layerMask);

            if (raycast.Valid)
            {
                // hit something
                var center = raycast.Item.transform.position.ToVector2();
                var leftDir = Rotate90Left(direction);

                // get obstacle size from input or circle collider
                var obstacleRadius = input.obstacleSize;
                var col = raycast.Item.collider as CircleCollider2D;
                if (col != null)
                {
                    var wScale = col.transform.lossyScale;
                    Assert.AreEqual(wScale.x, wScale.y, "Scale is not uniform");
                    obstacleRadius = col.radius * wScale.x;
                }

                var marginAbs = input.margin * obstacleRadius;
                var distanceFromCenter = obstacleRadius + marginAbs;
                var lOffsetVec = leftDir * distanceFromCenter;
                bool canReachSide;
                {
                    // find potential positions
                    // try sides of obstacle
                    var newPosLeft = center + lOffsetVec;
                    var newPosRight = center - lOffsetVec;
                    canReachSide = FindCloserAndAdd(newPosLeft, newPosRight);
                }

                if (!canReachSide)
                {
                    // try sides of current node
                    var offsetVec = lOffsetVec * 0.5f;
                    var newPosLeft = center - direction * distanceFromCenter + offsetVec;
                    var newPosRight = center - direction * distanceFromCenter - offsetVec;
                    FindCloserAndAdd(newPosLeft, newPosRight);
                }

                bool FindCloserAndAdd (Vector2 lhs, Vector2 rhs)
                {
                    // pick closer first
                    // TODO PERF: no need for two branches, done for readability
                    // TODO PERF: redundant check, also in CheckAndAdd
                    var leftDist = (lhs - node.position).sqrMagnitude;
                    var rightDist = (rhs - node.position).sqrMagnitude;
                    bool leftCloser = leftDist < rightDist;
                    Vector2 closerPos = leftCloser ? lhs : rhs;
                    Vector2 furtherPos = !leftCloser ? lhs : rhs;

                    // Add New Nodes
                    bool added = false;
                    added |= CheckAndAdd(closerPos);
                    added |= CheckAndAdd(furtherPos);
                    return added;
                }

                // If way between currnet node and the new position is clear, add the node to tree
                // return true if added
                bool CheckAndAdd(Vector2 newPos)
                {
                    // TODO PERF: Don't need the hit info, just to see if hit anything, but for convenience sake
                    var raycastIn = EdgeRaycast(newPos, node.position, agentHalfSize, ref lHits, ref rHits, input.layerMask);
                    if (!raycastIn.Valid)
                    {
                        tree.Add(new Node { position = newPos, parentIndex = i });
                        return true;
                    }
                    return false;
                }

            }
            else  // found valid path
            {
                // add last node to resulting path
                var final = input.target - (direction * input.targetDistance);
                result.Add(final);

                // traverse the tree in reverse order and add to result
                for (int j = i; j > 0; j = tree[j].parentIndex)
                {
                    result.Add(tree[j].position);
                }

                return result;
            }
        }

        // could not find a valid path
        return result;

    }

    public static Maybe<RaycastHit2D> EdgeHit(ref RaycastHit2D[] lHits, ref RaycastHit2D[] rHits, int lHit, int rHit)
    {
        var ret = new Maybe<RaycastHit2D>();

        var lValHit = NonInsideHit(lHit,ref lHits);
        if (lValHit.Valid) return lValHit;

        var rValHit = NonInsideHit(rHit,ref rHits);
        if (rValHit.Valid) return rValHit;
        
        return ret;
    }

    // Shoot a ray from both sides of an agent and return if any hits
    public static Maybe<RaycastHit2D> EdgeRaycast(Vector2 target, Vector2 position, float agentHalfSize, ref RaycastHit2D[] lHits, ref RaycastHit2D[] rHits, LayerMask layerMask)
    {
        var direction = (target - position);
        float dist = direction.magnitude;
        direction.Normalize();
        var lEdge = FindLeftEdge(direction, position, agentHalfSize);
        var rEdge = FindRightEdge(direction, position, agentHalfSize);
        var lHit = Physics2D.RaycastNonAlloc(lEdge, direction, lHits, dist, layerMask);
        var rHit = Physics2D.RaycastNonAlloc(rEdge, direction, rHits, dist, layerMask);
        return EdgeHit(ref lHits, ref rHits, lHit, rHit);
    }

    static Vector2 FindLeftEdge (Vector2 direction, Vector2 position, float halfSize)
    {
        var leftDir = Rotate90Left(direction);
        return position + leftDir * halfSize;
    }

    static  Vector2 FindRightEdge (Vector2 direction, Vector2 position, float halfSize)
    {
        var leftDir = Rotate90Right(direction);
        return position + leftDir * halfSize;
    }

    private static Maybe<RaycastHit2D> NonInsideHit(int hitResult, ref RaycastHit2D[] hits)
    {
        var result = new Maybe<RaycastHit2D>();
        if      (hitResult > 0 && hits[0].fraction != 0)   result.Item = hits[0]; 
        else if (hitResult > 1 && hits[0].fraction == 0)   result.Item = hits[1]; 
        return result;
    }

    struct Node
    {
        public Vector2 position;
        public int parentIndex;
    }

    public static Vector2 Rotate90Left(Vector2 v) => new Vector2(-v.y, v.x);
    public static Vector2 Rotate90Right(Vector2 v) => new Vector2(v.y, -v.x);
}
