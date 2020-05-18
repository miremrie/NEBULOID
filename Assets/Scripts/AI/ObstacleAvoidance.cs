using System;
using UnityEditor;
using UnityEngine;

public class ObstacleAvoidance : MonoBehaviour
{

    public PatrolTestAI agent;
    public NavObstacle obstacle;

    Vector3 agentPredict;
    Vector3 obsPredict;
    public float amount;

    void Update()
    {
        var t = (float)PredictCollision(agent.transform.position, obstacle.transform.position, agent.motor.velocity, obstacle.velocity, agent.radius, obstacle.radius);
        if (t != -1) {
            agent.motor.velocity += (agentPredict - obsPredict).normalized * (1/t) * amount;
            agentPredict = agent.transform.position + (t * agent.motor.velocity);
            obsPredict = obstacle.transform.position + (Vector3)(t * obstacle.velocity);
            Debug.DrawLine(agent.transform.position, agentPredict, Color.green);
            Debug.DrawLine(obstacle.transform.position, obsPredict, Color.red);
        }

    }

    public double PredictCollision(Vector2 Oa, Vector2 Ob, Vector2 Da, Vector2 Db, float ra, float rb)
    {
        var a = (Da.x * Da.x) + (Db.x * Db.x) + (Da.y * Da.y) + (Db.y * Db.y) - (2 * Da.x * Db.x) - (2 * Da.y * Db.y);
        var b = (2 * Oa.x * Da.x) - (2 * Oa.x * Db.x) - (2 * Ob.x * Da.x) + (2 * Ob.x * Db.x) +(2 * Oa.y * Da.y) - (2 * Oa.y * Db.y) - (2 * Ob.y * Da.y) + (2 * Ob.y * Db.y);
        var c = (Oa.x * Oa.x) + (Ob.x * Ob.x) + (Oa.y * Oa.y) + (Ob.y * Ob.y) - (2 * Oa.x * Ob.x) - (2 * Oa.y * Ob.y) - ((ra + rb) * (ra + rb));


        var D = (b * b) - (4 * a * c);

        if ((a != 0) &&  (D >= 0)) {
            // collision exists
            var sqrt = Math.Sqrt(D);
            var pos = (-b + sqrt) / (2*a);
            var neg = (-b - sqrt) / (2*a);
            var min = Math.Min(pos, neg);
            if (min > 0) return min;
        }

        return -1;
    }

    private void OnDrawGizmos()
    {
        DrawCircle(agentPredict, agent.radius, Color.green);
        DrawCircle(obsPredict, obstacle.radius, Color.red);
    }

    private void DrawCircle(Vector3 position, float radius, Color color)
    {
        Gizmos.color = color;
        float theta = 0;
        float x = radius * Mathf.Cos(theta);
        float y = radius * Mathf.Sin(theta);
        Vector3 pos = position + new Vector3(x, y, 0);
        Vector3 newPos = pos;
        Vector3 lastPos = pos;
        for (theta = 0.1f; theta < Mathf.PI * 2; theta += 0.1f)
        {
            x = radius * Mathf.Cos(theta);
            y = radius * Mathf.Sin(theta);
            newPos = position + new Vector3(x, y, 0);
            Gizmos.DrawLine(pos, newPos);
            pos = newPos;
        }
        Gizmos.DrawLine(pos, lastPos);
    }
}

