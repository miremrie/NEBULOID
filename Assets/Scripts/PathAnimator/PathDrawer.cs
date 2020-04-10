using PathCreation;
using PathCreation.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PathCreation.Utility.VertexPathUtility;

namespace NBLD.Graphics.Path
{
    public class PathDrawer : MonoBehaviour
    {
        public GameObject meshHolder;
        public MeshRenderer meshRenderer;
        public MeshFilter meshFilter;
        private Mesh mesh;
        [Header("Path settings")]
        public float pathWidth = .4f;
        [Header("Material settings")]
        public Material pathMaterial;
        public float textureTilingPerMeter = 1;
        public float maxAngleError = 0.4f;
        public VertexPath vPath;
        private const string TilingParam = "_Tiling";
        Vector3[] verts;
        Vector2[] uvs;
        Vector3[] normals;
        public int numberOfVerts;
        private int[] triangleMap = { 0, 2, 1, 1, 2, 3 };
        private int[] roadTriangles;

        private void Start()
        {
            AssignMeshComponents();
        }
        public void Initalize(BezierPath bPath)
        {
            vPath = new VertexPath(bPath, transform, maxAngleError, 1);
            verts = new Vector3[vPath.NumPoints * 8];
            uvs = new Vector2[verts.Length];
            normals = new Vector3[verts.Length];
            numberOfVerts = verts.Length;
            int numTris = 2 * (vPath.NumPoints - 1);
            roadTriangles = new int[numTris * 3];
        }
        // Add MeshRenderer and MeshFilter components to this gameobject if not already attached
        void AssignMeshComponents()
        {
            mesh = meshFilter.mesh;
            if (mesh == null)
            {
                mesh = new Mesh();
            }
            meshFilter.sharedMesh = mesh;
            meshRenderer.material = pathMaterial;

            meshRenderer.material = new Material(meshRenderer.material);
        }

        void UpdateMaterials()
        {
            meshRenderer.material.SetVector(TilingParam, new Vector2(1, textureTilingPerMeter * vPath.length));
        }

        public void UpdateMesh(BezierPath bPath)
        {
            Initalize(bPath);
            int vertIndex = 0;
            int triIndex = 0;
            // Vertices for the top of the road are layed out:
            // 0  1
            // 2  3
            // and so on... So the triangle map 0,8,1 for example, defines a triangle from top left to bottom left to bottom right.

            for (int i = 0; i < vPath.NumPoints; i++)
            {
                Vector3 localUp = vPath.up;
                Vector3 localRight = Vector3.Cross(localUp, vPath.GetTangent(i));

                // Find position to left and right of current path vertex
                Vector3 vertSideA = vPath.GetPoint(i) - localRight * Mathf.Abs(pathWidth);
                Vector3 vertSideB = vPath.GetPoint(i) + localRight * Mathf.Abs(pathWidth);

                // Add top of road vertices
                verts[vertIndex + 0] = vertSideA;
                verts[vertIndex + 1] = vertSideB;

                // Set uv on y axis to path time (0 at start of path, up to 1 at end of path)
                uvs[vertIndex + 0] = new Vector2(0, vPath.times[i]);
                uvs[vertIndex + 1] = new Vector2(1, vPath.times[i]);

                // Top of road normals
                normals[vertIndex + 0] = localUp;
                normals[vertIndex + 1] = localUp;

                // Set triangle indices
                if (i < vPath.NumPoints - 1 || vPath.isClosedLoop)
                {
                    for (int j = 0; j < triangleMap.Length; j++)
                    {
                        roadTriangles[triIndex + j] = (vertIndex + triangleMap[j]) % verts.Length;
                    }
                }
                vertIndex += 2;
                triIndex += 6;
            }

            mesh.Clear();
            mesh.vertices = verts;
            mesh.uv = uvs;
            mesh.normals = normals;
            mesh.SetTriangles(roadTriangles, 0);
            mesh.RecalculateBounds();
            meshFilter.sharedMesh = mesh;
            UpdateMaterials();
        }
    }
}

