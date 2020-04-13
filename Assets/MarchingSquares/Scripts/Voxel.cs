using UnityEngine;
using System;

namespace MarchingSquares
{
    [Serializable]
    public class Voxel
    {

        public bool state;

        public Vector2 position;

        public float xEdge, yEdge;
        private Transform parentTrans;
        private Vector3 localPos;

        public Vector3 WorldPos { get {
               return parentTrans.TransformPoint(localPos);
            }
        }
        public Vector2 xNormal, yNormal;

        public Vector2 XEdgePoint {
            get {
                return new Vector2(xEdge, position.y);
            }
        }

        public Vector2 YEdgePoint {
            get {
                return new Vector2(position.x, yEdge);
            }
        }

        public Voxel(int x, int y, float size, Transform worldPos, Vector3 localPos)
        {
            position.x = (x + 0.5f) * size;
            position.y = (y + 0.5f) * size;

            xEdge = float.MinValue;
            yEdge = float.MinValue;

            this.parentTrans = worldPos;
            this.localPos = localPos;
        }

        public Voxel()
        {
        }

        public void SetHorizontalEdge(EdgeData edge)
        {
            if (edge.EdgeOffset == float.MinValue) return;

            xEdge = edge.EdgeOffset;
            xNormal = edge.Normal;
        }

        public void SetVerticalEdge(EdgeData edge)
        {
            if (edge.EdgeOffset == float.MinValue) return;

            yEdge = edge.EdgeOffset;
            yNormal = edge.Normal;
        }

        //public Voxel(Transform worldPos)
        //{
        //    this.worldPos = worldPos;
        //}

        public void BecomeXDummyOf(Voxel voxel, float offset)
        {
            state = voxel.state;
            position = voxel.position;
            parentTrans = voxel.parentTrans;
            localPos = voxel.localPos;
            position.x += offset;
            xEdge = voxel.xEdge + offset;
            yEdge = voxel.yEdge;
            yNormal = voxel.yNormal;
        }

        public void BecomeYDummyOf(Voxel voxel, float offset)
        {
            state = voxel.state;
            position = voxel.position;
            parentTrans = voxel.parentTrans;
            localPos = voxel.localPos;
            position.y += offset;
            xEdge = voxel.xEdge;
            yEdge = voxel.yEdge + offset;
            xNormal = voxel.xNormal;
        }

        public void BecomeXYDummyOf(Voxel voxel, float offset)
        {
            state = voxel.state;
            position = voxel.position;
            parentTrans = voxel.parentTrans;
            localPos = voxel.localPos;
            position.x += offset;
            position.y += offset;
            xEdge = voxel.xEdge + offset;
            yEdge = voxel.yEdge + offset;
        }

    }

    public struct EdgeData
    {
        public float EdgeOffset { get; set; }
        public Vector2 Normal { get; set; }

        public EdgeData(float edgeOffset, Vector2 normal) : this()
        {
            EdgeOffset = edgeOffset;
            Normal = normal;
        }
    }
}