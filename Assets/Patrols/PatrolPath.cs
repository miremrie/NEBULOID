using System;
using UnityEngine;

public class PatrolPath : MonoBehaviour
{
    public Transform[] waypoints;
    public Color color;
}

public struct PatrolWalkConfig
{
    public PatrolPath path;
    public Transform trans;
    public float nearDistance;
    public bool circular;
    public bool forward;
    public int startIndex;
    public Func<Vector3, bool> IsNear;

    public static PatrolWalkConfig Default(PatrolPath path, Transform trans)
    {
        return new PatrolWalkConfig
        {
            path = path,
            trans = trans,
            circular = true,
            forward = true,
            nearDistance = 0.1f,
        };
    }
}

public class PatrolWalk
{
    PatrolPath path;
    public int wayIndex = 0;
    private Transform trans;
    private float nearDistance;
    private bool circular;
    private bool forward;
    private int startIndex;
    private Func<Vector3, bool> IsNear;
    private int last;

    public PatrolWalk(PatrolWalkConfig c)
    {
        if (c.trans == null && c.IsNear == null) {
            Debug.LogError("Patrol walk needs transform or IsNear function");
        }

        last = c.path.waypoints.Length - 1;
        startIndex =  Mathf.Clamp(c.startIndex, 0, last);
        IsNear = c.IsNear ?? DefaultIsNear;
        wayIndex = startIndex;

        path = c.path;
        trans = c.trans;
        nearDistance = c.nearDistance;
        circular = c.circular;
        forward = c.forward;
    }

    internal Vector3 Target()
    {
        if (IsNear(Current())) NextWaypoint();
        return Current();
    }

    private void NextWaypoint()
    {
        if (!circular)
        {
            if ((forward && wayIndex == Prev(startIndex))
                || (!forward && wayIndex == startIndex))
            {
                forward = !forward;
            }
        }

        wayIndex = forward ? Next(wayIndex) : Prev(wayIndex);
    }

    Vector3 Current() => path.waypoints[wayIndex].position;

    int Next(int i) => (i == last) ? 0 : ++i;
    int Prev(int i) => (i == 0) ? last : --i;

    private bool DefaultIsNear(Vector3 v) => (v - trans.position).sqrMagnitude < nearDistance;
}
