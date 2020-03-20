using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class ControlPointsAnimator : MonoBehaviour
{
    public PathCreator pathCreator;
    public Vector3[] originalPoints;
    public float speed = 1f;
    public float rangeMin = -0.9f, rangeMax = 0.9f;
    public float period = 1f;

    void Start()
    {
    }

    private void SetOriginalControlPoints()
    {
        originalPoints = new Vector3[pathCreator.bezierPath.NumAnchorPoints];
        for (int i = 0; i < pathCreator.bezierPath.NumAnchorPoints; i += 3)
        {
            originalPoints[i] = pathCreator.bezierPath.GetPoint(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        AnimatePoints();
    }

    void AnimatePoints()
    {
        if (originalPoints == null || originalPoints.Length != pathCreator.bezierPath.NumAnchorPoints)
        {
            SetOriginalControlPoints();
        }
        float curTime = (Time.time % period) - (period * 0.5f);

        /*float x = Mathf.Sin(Time.time);
        x = Mathf.Clamp(x, rangeMin, rangeMax);*/
        for (int i = 0; i < pathCreator.bezierPath.NumAnchorPoints; i++)
        {
            Vector3 position = originalPoints[i];
            float pX = Mathf.PerlinNoise(originalPoints[i].x, curTime);
            pX = pX - 0.5f;
            pX *= speed;
            float pY = Mathf.PerlinNoise(originalPoints[i].y, curTime);
            pY = -0.5f;
            pY *= speed;
            position.x = originalPoints[i].x + pX;
            position.y = originalPoints[i].y + pY;
            pathCreator.bezierPath.MovePoint(i, position, true);
        }
        pathCreator.bezierPath.NotifyPathModified();

    }
}
