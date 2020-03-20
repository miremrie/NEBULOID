using PathCreation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public float vertexSpacing = 1;
    public VertexPath vPath;

    private void Start()
    {
        AssignMeshComponents();
    }

    private void ResetPositionAndOrientation()
    {
        meshHolder.transform.position = Vector3.zero;
        meshHolder.transform.rotation = Quaternion.identity;
    }
    // Add MeshRenderer and MeshFilter components to this gameobject if not already attached
    void AssignMeshComponents()
    {
        mesh = meshFilter.mesh;
        meshFilter.sharedMesh = mesh;
    }

    void AssignMaterials()
    {
        meshRenderer.sharedMaterial = pathMaterial;
        meshRenderer.sharedMaterial.mainTextureScale = new Vector3(1, textureTilingPerMeter * vPath.length);
    }

    public void UpdateMesh(BezierPath bPath)
    {
        ResetPositionAndOrientation();
        vPath = new VertexPath(bPath, transform, vertexSpacing);
        Vector3[] verts = new Vector3[vPath.NumPoints * 8];
        Vector2[] uvs = new Vector2[verts.Length];
        Vector3[] normals = new Vector3[verts.Length];

        int numTris = 2 * (vPath.NumPoints - 1) + ((vPath.isClosedLoop) ? 2 : 0);
        int[] roadTriangles = new int[numTris * 3];

        int vertIndex = 0;
        int triIndex = 0;

        // Vertices for the top of the road are layed out:
        // 0  1
        // 2  3
        // and so on... So the triangle map 0,8,1 for example, defines a triangle from top left to bottom left to bottom right.
        int[] triangleMap = { 0, 2, 1, 1, 2, 3 };

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
        mesh.subMeshCount = 1;
        mesh.SetTriangles(roadTriangles, 0);
        mesh.RecalculateBounds();
        meshFilter.sharedMesh = mesh;
        AssignMaterials();
    }
}
