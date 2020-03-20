using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using System;

public class PathAnimator : MonoBehaviour
{
    public PathCreator pathCreator;
    public PathDrawer pathDrawer;
    public Vector3[] originalPoints;
    public float speed = 1f;
    public float rangeMin = -0.9f, rangeMax = 0.9f;
    public float period = 1f;
    public Transform startFollow, endFollow;
    public Vector3 startInertia, endInertia;
    public float inertiaDropFactor;
    public float maxInertiaVector;
    public float cumulativeLength;
    private VertexPath path;
    public float[] distancesBetweenAnchors;
    public bool animatePoints = true;

    private void Start()
    {
        SetOriginalPoints();
    }

    private void SetOriginalPoints()
    {
        originalPoints = new Vector3[pathCreator.bezierPath.NumPoints];
        distancesBetweenAnchors = new float[pathCreator.bezierPath.NumAnchorPoints - 1];
        for (int i = 0; i < pathCreator.bezierPath.NumPoints; i++)
        {
            originalPoints[i] = pathCreator.bezierPath.GetPoint(i);
        }
        for (int i = 0; i < pathCreator.bezierPath.NumAnchorPoints - 3; i += 3)
        {
            distancesBetweenAnchors[i / 3] = Vector3.Distance(originalPoints[i], originalPoints[i + 3]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        MovePoints();
        if (animatePoints)
        {
            AnimatePoints();
            UpdateInertia();
            UpdateOriginalPointsByInertia();
            AnimatePoints();
        }
        pathCreator.bezierPath.NotifyPathModified();
        pathDrawer.UpdateMesh(pathCreator.bezierPath);

        UpdateSegmentCount();
    }


    private void UpdateOriginalPointsByInertia()
    {
        int numAnchorPoints = pathCreator.bezierPath.NumAnchorPoints - 1;
        for (int i = 3; i < pathCreator.bezierPath.NumPoints - 2; i+= 3)
        {
            int anchorPointNum = i / 3;
            Vector3 startInfluence = Vector3.Lerp(startInertia, Vector3.zero, anchorPointNum / (float)numAnchorPoints);
            Vector3 endInfluence = Vector3.Lerp(Vector3.zero, endInertia, anchorPointNum / (float)numAnchorPoints);
            Vector3 newPosition = startInfluence + endInfluence + originalPoints[i];
            pathCreator.bezierPath.MovePoint(i, newPosition, true);
            originalPoints[i] = newPosition;
        }

    }
    private void MovePoints()
    {
        Vector3 startDirection = startFollow.position - pathCreator.bezierPath.GetPoint(0);
        Vector3 endDirection = endFollow.position - pathCreator.bezierPath.GetPoint(pathCreator.bezierPath.NumPoints - 1);
        Vector3 startPosition = startFollow.position;
        Vector3 endPosition = endFollow.position;
        ResetPositionAndOrientation();
        if (animatePoints)
        {
            AddInertia(startDirection, endDirection);
        }
        if (startPosition != pathCreator.bezierPath.GetPoint(0))
        {
            pathCreator.bezierPath.MovePoint(0, startPosition, true);
        }
        if (endPosition != pathCreator.bezierPath.GetPoint(pathCreator.bezierPath.NumPoints - 1))
        {
            pathCreator.bezierPath.MovePoint(pathCreator.bezierPath.NumPoints - 1, endPosition, true);

        }
        pathCreator.path.UpdateTransform(transform);
        //SetOriginalPoints();
    }

    private void UpdateInertia()
    {
        startInertia = Vector3.Lerp(startInertia, Vector3.zero, Mathf.Exp(inertiaDropFactor) * Time.deltaTime);
        endInertia = Vector3.Lerp(endInertia, Vector3.zero, Mathf.Exp(inertiaDropFactor) * Time.deltaTime);
    }

    private void AddInertia(Vector3 start, Vector3 end)
    {
        startInertia = Vector3.ClampMagnitude(start + startInertia, maxInertiaVector);
        endInertia = Vector3.ClampMagnitude(end + endInertia, maxInertiaVector);

    }

    private void UpdateSegmentCount()
    {
        cumulativeLength = pathDrawer.vPath.length;
    }


    void AnimatePoints()
    {
        float curTime = (Time.time % period) - (period * 0.5f);

        /*float x = Mathf.Sin(Time.time);
        x = Mathf.Clamp(x, rangeMin, rangeMax);*/
        for (int i = 3; i < pathCreator.bezierPath.NumPoints - 3; i+=3)
        {
            Vector3 position = originalPoints[i];
            float pX = Mathf.PerlinNoise(position.x, curTime);
            pX = pX - 0.5f;
            pX *= speed;
            float pY = Mathf.PerlinNoise(position.y, curTime);
            pY = -0.5f;
            pY *= speed;
            position.x += pX;
            position.y += pY;
            pathCreator.bezierPath.MovePoint(i, position, true);
        }
    }
    private void ResetPositionAndOrientation()
    {
        this.transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }

}
