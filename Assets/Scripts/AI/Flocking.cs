using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flocking : MonoBehaviour
{
    public List<Transform> agents;
    public List<CircleCollider2D> obstacles;
    public List<Transform> predators;
    public float multiplierAvoidAgents;
    public float sqrRadiusAvoidAgents;
    public float multiplierAvoidObstacles;
    public float sqrRadiusAvoidObstacles;
    public float multiplierAvoidPredators;
    public float sqrRadiusAvoidPredators;
    public Vector2 Velocity (Transform trans)
    {
        Vector2 agentResult = AvoidSimple(trans, agents, multiplierAvoidAgents, sqrRadiusAvoidAgents);
        Vector2 predatorResult = AvoidSimple(trans, predators, multiplierAvoidPredators, sqrRadiusAvoidPredators);
        Vector2 obsResult = AvoidCircles(trans, obstacles, multiplierAvoidObstacles, sqrRadiusAvoidObstacles);
        return agentResult + obsResult + predatorResult;
    }

    public Vector2 AvoidCircles(Transform trans, List<CircleCollider2D> circles,  float multiplier, float radius)
    {
        Vector2 result = Vector2.zero;
        if (multiplier == 0) return result;
        foreach (var o in circles)
        {
            var vec = (trans.position - o.transform.position).ToVector2();
            var obsRadius = o.radius * o.transform.lossyScale.x;
            var dist = vec.magnitude;
            var maxDist = obsRadius + radius;
            if (dist > 0 && dist > obsRadius && dist < maxDist)
            {
                vec = vec.normalized / (dist - obsRadius);
            }
            else if (dist > 0 && dist < obsRadius)
            {
                vec = vec.normalized;
            }
            else vec = Vector2.zero;

            result += vec;
        }

        result *= multiplier;
        return result;
    }

    public Vector2 AvoidSimple(Transform trans, List<Transform> agents, float multiplier, float radius)
    {
        Vector2 result = Vector2.zero;
        if (multiplier == 0) return result;
        foreach (var a in agents)
        {
            var vec = (trans.position - a.position).ToVector2();
            var mag = vec.magnitude;
            if (mag > 0 && mag < radius)
            {
                vec = vec.normalized / mag;
            }
            else vec = Vector2.zero;

            result += vec;
        }
        result *= multiplier;
        return result;
    }
}
