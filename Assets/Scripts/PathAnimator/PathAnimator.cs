using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using System;

namespace NBLD.Graphics.Path
{
    public class PathAnimator : MonoBehaviour
    {
        public PathCreator pathCreator;
        public PathDrawer pathDrawer;
        public Vector3[] originalPoints;
        public float speed = 1f;
        public float perlinAnimScale = 0.4f;
        public float minDistancePerlinAnim = 3f;
        public bool scalePerlinWithDistance;
        public float period = 1f;
        public Transform startFollow, endFollow;
        public Vector3 startInertia, endInertia;
        public float inertiaDropFactor;
        public float maxInertiaVector;
        private VertexPath path;
        public float[] distancesBetweenAnchors;
        public bool animatePoints = true;


        private void Start()
        {
            SetOriginalPoints();
            pathDrawer.Initalize(pathCreator.bezierPath);
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
                UpdateInertia();
                UpdateOriginalPointsByInertia();
                AnimateControlPoints();
            }
            //pathCreator.bezierPath.NotifyPathModified();
            pathDrawer.UpdateMesh(pathCreator.bezierPath);
        }


        private void UpdateOriginalPointsByInertia()
        {
            int numAnchorPoints = pathCreator.bezierPath.NumAnchorPoints - 1;
            for (int i = 3; i < pathCreator.bezierPath.NumPoints - 2; i += 3)
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
        void AnimateControlPoints()
        {
            Vector2 previousAnchor = pathCreator.bezierPath.GetPoint(0);
            Vector2 nextAnchor = pathCreator.bezierPath.GetPoint(3);
            float curTime = (Time.time % period) - (period * 0.5f);
            for (int i = 1; i < pathCreator.bezierPath.NumPoints - 1; i++)
            {
                if (i % 3 == 0)
                {
                    previousAnchor = nextAnchor;
                    nextAnchor = pathCreator.bezierPath.GetPoint(i + 3);
                } else
                {
                    Vector2 position;
                    float distance = Vector2.Distance(previousAnchor, nextAnchor);
                    if (distance < minDistancePerlinAnim)
                    {
                        float percentDistance = 0.5f;
                        position = Vector2.Lerp(previousAnchor, nextAnchor, percentDistance);
                    } else
                    {
                        float percentDistance = (i % 3) * 0.33f * speed;
                        position = Vector2.Lerp(previousAnchor, nextAnchor, percentDistance);

                        float pX = Mathf.PerlinNoise(position.x, curTime);
                        pX = pX - 0.5f;
                        float pY = Mathf.PerlinNoise(position.y, curTime);
                        pY = -0.5f;
                        pY *= perlinAnimScale;
                        if (scalePerlinWithDistance)
                        {
                            pY *= perlinAnimScale * distance;
                            pX *= perlinAnimScale * distance;
                        }
                        else
                        {
                            pY *= perlinAnimScale;
                            pX *= perlinAnimScale;
                        }
                        position += new Vector2(pX, pY);
                    }


                    //midPoint = midPoint.normalized * speed;

                    pathCreator.bezierPath.MovePoint(i, position, true);
                }
            }
        }
        void AnimatePoints()
        {
            float curTime = (Time.time % period) - (period * 0.5f);

            /*float x = Mathf.Sin(Time.time);
            x = Mathf.Clamp(x, rangeMin, rangeMax);*/
            for (int i = 3; i < pathCreator.bezierPath.NumPoints - 3; i += 3)
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
        private bool ShouldPosAndRotReset()
        {
            return transform.position != Vector3.zero || transform.rotation != Quaternion.identity;
        }

    }
}
