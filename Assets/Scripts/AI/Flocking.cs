using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flocking : MonoBehaviour
{
    public List<Transform> agents;
    public float multiplier;

    public Vector2 Nudge (Transform trans)
    {
        Vector2 result = Vector2.zero;
        foreach (var a in agents)
        {
            var vec = (trans.position - a.position).ToVector2();
            if (vec.sqrMagnitude > 0)
            {
                vec *= 1 / vec.sqrMagnitude;
                result += vec;
            }
        }
        return result * multiplier;
    }
}
