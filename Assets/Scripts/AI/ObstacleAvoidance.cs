using System;
using UnityEngine;

public class ObstacleAvoidance : MonoBehaviour
{
    struct CollisionDebug
    {
        public bool draw;
        public Vector3 agentPredictPos;
        public Vector3 obsPredictPos;
        public Vector2 agentPos;
        public Vector3 obsPos;
        public float agentRadius;
        public float obsRadius;
        public Color col;
    }

    public float amount;
    private NavObstacle[] obstacles;
    private CollisionDebug[] debugs = new CollisionDebug[0];
    private PatrolTestAI[] agents = new PatrolTestAI[0];
    public float time;

    private void Start()
    {
        obstacles = FindObjectsOfType<NavObstacle>();
        agents = FindObjectsOfType<PatrolTestAI>();

        debugs = new CollisionDebug[agents.Length * obstacles.Length];

        for (int i = 0; i < debugs.Length; i++)
        {
            debugs[i].col = Colors.RandomColor(1, 1);
        }
    }

    void Update()
    {
        for (int aIndex = 0, gIndex = 0; aIndex < agents.Length; aIndex++)
        {
            var agent = agents[aIndex];
            for (int oIndex = 0; oIndex < obstacles.Length; oIndex++, gIndex++)
            {
                var obs = obstacles[oIndex];
                var t = (float) PredictCollision(agent.transform.position, obs.transform.position, agent.motor.velocity, obs.velocity, agent.radius, obs.radius);

                if (t != -1 && t < time)
                {
                    var agentPredict = agent.transform.position + (t * agent.motor.velocity);
                    var obsPredict = obs.transform.position + (Vector3)(t * obs.velocity);

                    agent.SetObstacleAvoidance((agentPredict - obsPredict).normalized * (1 / (t * t)));
                    //agent.motor.velocity += (agentPredict - obsPredict).normalized * (1 / (t * t)) * amount;

                    var d = new CollisionDebug
                    {
                        draw = true,
                        agentPredictPos = agentPredict,
                        obsPredictPos = obsPredict,
                        agentPos = agent.transform.position,
                        obsPos = obs.transform.position,
                        agentRadius = agent.radius,
                        obsRadius = obs.radius,
                        col = debugs[oIndex].col
                    };
                    debugs[gIndex] = d;
                }
                else
                {
                    debugs[gIndex].draw = false;
                }
            }
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
        for (int i = 0; i < debugs.Length; i++)
        {
            var d = debugs[i];
            if (d.draw)
            {
                var c = d.col;
                Debug.DrawLine(d.agentPos, d.agentPredictPos, c);
                Debug.DrawLine(d.obsPos, d.obsPredictPos, c);
                DrawCircle(d.agentPredictPos, d.agentRadius, c);
                DrawCircle(d.obsPredictPos, d.obsRadius, c);
            }
        }

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

