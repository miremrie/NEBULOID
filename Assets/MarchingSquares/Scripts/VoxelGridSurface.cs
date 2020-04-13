using UnityEngine;
using System.Collections.Generic;
using System;

namespace MarchingSquares
{
    public class VoxelGridSurface : MonoBehaviour
    {

        private Mesh mesh;

        private List<Vector3> vertices;
        private List<int> triangles;
        private List<Vector2> uvs;

        private Voxel dummyX, dummyY, dummyT;

        private int[] cornersMin, cornersMax;
        private int[] xEdgesMin, xEdgesMax;
        private int yEdgeMin, yEdgeMax;

        private Vector2 chunkPos;
        private int voxelResolution;
        private int chunkResolution;
        //private MeshCollider mCollider;
        PolygonCollider2D mCollider;
        private MeshFilter mFilter;

        private VoxelMap map;

        public void Initialize(VoxelMap map, int resolution)
        {
            mCollider = GetComponent<PolygonCollider2D>();
            mFilter = GetComponent<MeshFilter>();
            this.map = map;

            mFilter.mesh = mesh = new Mesh();
            mesh.name = "VoxelGridSurface Mesh";
            vertices = new List<Vector3>();
            triangles = new List<int>();
            uvs = new List<Vector2>();
            cornersMax = new int[resolution + 1];
            cornersMin = new int[resolution + 1];
            xEdgesMin = new int[resolution];
            xEdgesMax = new int[resolution];
            voxelResolution = resolution;
        }

        internal void EnableCollision(bool enabled)
        {
            mCollider.enabled = enabled;
        }

        public void Clear()
        {
            vertices.Clear();
            triangles.Clear();
            mesh.Clear();
            uvs.Clear();
        }


        void AddVertexWithUV(Vector2 pos)
        {
            vertices.Add(pos);
            var p = transform.TransformPoint(pos);
            uvs.Add(map.CalcUV(p));
        }

        public void Apply()
        {
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            //RefreshCollider();
            //mCollider.sharedMesh = mFilter.sharedMesh;
        }

        public void CacheFirstCorner(Voxel voxel)
        {
            cornersMax[0] = vertices.Count;
            AddVertexWithUV(voxel.position);
        }

        public void CacheNextCorner(int i, Voxel voxel)
        {
            cornersMax[i + 1] = vertices.Count;
            AddVertexWithUV(voxel.position);
        }

        public void CacheXEdge(int i, Voxel voxel)
        {
            xEdgesMax[i] = vertices.Count;
            AddVertexWithUV(voxel.XEdgePoint);
        }

        public void CacheYEdge(Voxel voxel)
        {
            yEdgeMax = vertices.Count;
            AddVertexWithUV(voxel.YEdgePoint);
        }

        public void PrepareCacheForNextCell()
        {
            yEdgeMin = yEdgeMax;
        }

        public void PrepareCacheForNextRow()
        {
            int[] rowSwap = cornersMin;
            cornersMin = cornersMax;
            cornersMax = rowSwap;
            rowSwap = xEdgesMin;
            xEdgesMin = xEdgesMax;
            xEdgesMax = rowSwap;
        }

        public void AddQuadABCD(int i)
        {
            AddQuad(
                cornersMin[i], cornersMax[i], cornersMax[i + 1], cornersMin[i + 1]);
        }

        public void AddTriangleA(int i)
        {
            AddTriangle(cornersMin[i], yEdgeMin, xEdgesMin[i]);
        }

        public void AddQuadA(int i, Vector2 extraVertex)
        {
            AddQuad(vertices.Count, xEdgesMin[i], cornersMin[i], yEdgeMin);
            AddVertexWithUV(extraVertex);
        }

        public void AddTriangleB(int i)
        {
            AddTriangle(cornersMin[i + 1], xEdgesMin[i], yEdgeMax);
        }

        public void AddQuadB(int i, Vector2 extraVertex)
        {
            AddQuad(vertices.Count, yEdgeMax, cornersMin[i + 1], xEdgesMin[i]);
            AddVertexWithUV(extraVertex);
        }

        public void AddTriangleC(int i)
        {
            AddTriangle(cornersMax[i], xEdgesMax[i], yEdgeMin);
        }

        public void AddQuadC(int i, Vector2 extraVertex)
        {
            AddQuad(vertices.Count, yEdgeMin, cornersMax[i], xEdgesMax[i]);
            AddVertexWithUV(extraVertex);
        }

        public void AddTriangleD(int i)
        {
            AddTriangle(cornersMax[i + 1], yEdgeMax, xEdgesMax[i]);
        }

        public void AddQuadD(int i, Vector2 extraVertex)
        {
            AddQuad(vertices.Count, xEdgesMax[i], cornersMax[i + 1], yEdgeMax);
            AddVertexWithUV(extraVertex);
        }

        public void AddPentagonABC(int i)
        {
            AddPentagon(
                cornersMin[i], cornersMax[i], xEdgesMax[i],
                yEdgeMax, cornersMin[i + 1]);
        }

        public void AddHexagonABC(int i, Vector2 extraVertex)
        {
            AddHexagon(
                vertices.Count, yEdgeMax, cornersMin[i + 1],
                cornersMin[i], cornersMax[i], xEdgesMax[i]);

            AddVertexWithUV(extraVertex);
        }

        public void AddPentagonABD(int i)
        {
            AddPentagon(
                cornersMin[i + 1], cornersMin[i], yEdgeMin,
                xEdgesMax[i], cornersMax[i + 1]);
        }

        public void AddHexagonABD(int i, Vector2 extraVertex)
        {
            AddHexagon(
                vertices.Count, xEdgesMax[i], cornersMax[i + 1],
                cornersMin[i + 1], cornersMin[i], yEdgeMin);

            AddVertexWithUV(extraVertex);
        }

        public void AddPentagonACD(int i)
        {
            AddPentagon(
                cornersMax[i], cornersMax[i + 1], yEdgeMax,
                xEdgesMin[i], cornersMin[i]);
        }

        public void AddHexagonACD(int i, Vector2 extraVertex)
        {
            AddHexagon(
                vertices.Count, xEdgesMin[i], cornersMin[i],
                cornersMax[i], cornersMax[i + 1], yEdgeMax);

            AddVertexWithUV(extraVertex);
        }

        public void AddPentagonBCD(int i)
        {
            AddPentagon(
                cornersMax[i + 1], cornersMin[i + 1], xEdgesMin[i],
                yEdgeMin, cornersMax[i]);
        }

        public void AddHexagonBCD(int i, Vector2 extraVertex)
        {
            AddHexagon(
                vertices.Count, yEdgeMin, cornersMax[i],
                cornersMax[i + 1], cornersMin[i + 1], xEdgesMin[i]);

            AddVertexWithUV(extraVertex);
        }

        public void AddQuadAB(int i)
        {
            AddQuad(cornersMin[i], yEdgeMin, yEdgeMax, cornersMin[i + 1]);
        }

        public void AddPentagonAB(int i, Vector2 extraVertex)
        {
            AddPentagon(
                vertices.Count, yEdgeMax, cornersMin[i + 1],
                cornersMin[i], yEdgeMin);

            AddVertexWithUV(extraVertex);
        }

        public void AddQuadAC(int i)
        {
            AddQuad(cornersMin[i], cornersMax[i], xEdgesMax[i], xEdgesMin[i]);
        }

        public void AddPentagonAC(int i, Vector2 extraVertex)
        {
            AddPentagon(
                vertices.Count, xEdgesMin[i], cornersMin[i],
                cornersMax[i], xEdgesMax[i]);

            AddVertexWithUV(extraVertex);
        }

        public void AddQuadBD(int i)
        {
            AddQuad(
                xEdgesMin[i], xEdgesMax[i], cornersMax[i + 1], cornersMin[i + 1]);
        }

        public void AddPentagonBD(int i, Vector2 extraVertex)
        {
            AddPentagon(
                vertices.Count, xEdgesMax[i], cornersMax[i + 1],
                cornersMin[i + 1], xEdgesMin[i]);

            AddVertexWithUV(extraVertex);
        }

        public void AddQuadCD(int i)
        {
            AddQuad(yEdgeMin, cornersMax[i], cornersMax[i + 1], yEdgeMax);
        }

        public void AddPentagonCD(int i, Vector2 extraVertex)
        {
            AddPentagon(
                vertices.Count, yEdgeMin, cornersMax[i],
                cornersMax[i + 1], yEdgeMax);

            AddVertexWithUV(extraVertex);
        }

        public void AddQuadBCToA(int i)
        {
            AddQuad(yEdgeMin, cornersMax[i], cornersMin[i + 1], xEdgesMin[i]);
        }

        public void AddPentagonBCToA(int i, Vector2 extraVertex)
        {
            AddPentagon(
                vertices.Count, yEdgeMin, cornersMax[i],
                cornersMin[i + 1], xEdgesMin[i]);

            AddVertexWithUV(extraVertex);
        }

        public void AddQuadBCToD(int i)
        {
            AddQuad(yEdgeMax, cornersMin[i + 1], cornersMax[i], xEdgesMax[i]);
        }

        public void AddPentagonBCToD(int i, Vector2 extraVertex)
        {
            AddPentagon(
                vertices.Count, yEdgeMax, cornersMin[i + 1],
                cornersMax[i], xEdgesMax[i]);

            AddVertexWithUV(extraVertex);
        }

        public void AddQuadADToB(int i)
        {
            AddQuad(xEdgesMin[i], cornersMin[i], cornersMax[i + 1], yEdgeMax);
        }

        public void AddPentagonADToB(int i, Vector2 extraVertex)
        {
            AddPentagon(
                vertices.Count, xEdgesMin[i], cornersMin[i],
                cornersMax[i + 1], yEdgeMax);

            AddVertexWithUV(extraVertex);
        }

        public void AddQuadADToC(int i)
        {
            AddQuad(xEdgesMax[i], cornersMax[i + 1], cornersMin[i], yEdgeMin);
        }

        public void AddPentagonADToC(int i, Vector2 extraVertex)
        {
            AddPentagon(
                vertices.Count, xEdgesMax[i], cornersMax[i + 1],
                cornersMin[i], yEdgeMin);

            AddVertexWithUV(extraVertex);
        }

        private void AddTriangle(int a, int b, int c)
        {
            triangles.Add(a);
            triangles.Add(b);
            triangles.Add(c);
        }

        private void AddQuad(int a, int b, int c, int d)
        {
            triangles.Add(a);
            triangles.Add(b);
            triangles.Add(c);
            triangles.Add(a);
            triangles.Add(c);
            triangles.Add(d);
        }

        private void AddPentagon(int a, int b, int c, int d, int e)
        {
            triangles.Add(a);
            triangles.Add(b);
            triangles.Add(c);
            triangles.Add(a);
            triangles.Add(c);
            triangles.Add(d);
            triangles.Add(a);
            triangles.Add(d);
            triangles.Add(e);
        }

        private void AddHexagon(int a, int b, int c, int d, int e, int f)
        {
            triangles.Add(a);
            triangles.Add(b);
            triangles.Add(c);
            triangles.Add(a);
            triangles.Add(c);
            triangles.Add(d);
            triangles.Add(a);
            triangles.Add(d);
            triangles.Add(e);
            triangles.Add(a);
            triangles.Add(e);
            triangles.Add(f);
        }

        Dictionary<string, KeyValuePair<int, int>> edges = new Dictionary<string, KeyValuePair<int, int>>();
        Dictionary<int, int> lookup = new Dictionary<int, int>();
        List<Vector2> colliderPath = new List<Vector2>();

        public void SetPolygonColliderPath()
        {
            // Get just the outer edges from the mesh's triangles (ignore or remove any shared edges)
            edges.Clear();
            for (int i = 0; i < triangles.Count; i += 3)
            {
                for (int e = 0; e < 3; e++)
                {
                    int vert1 = triangles[i + e];
                    int vert2 = triangles[i + e + 1 > i + 2 ? i : i + e + 1];
                    string edge = Mathf.Min(vert1, vert2) + ":" + Mathf.Max(vert1, vert2);
                    if (edges.ContainsKey(edge))
                    {
                        edges.Remove(edge);
                    }
                    else
                    {
                        edges.Add(edge, new KeyValuePair<int, int>(vert1, vert2));
                    }
                }
            }

            // Create edge lookup (Key is first vertex, Value is second vertex, of each edge)
            lookup.Clear();
            foreach (KeyValuePair<int, int> edge in edges.Values)
            {
                if (!lookup.ContainsKey(edge.Key))
                {
                    lookup.Add(edge.Key, edge.Value);
                }
            }

            // Create empty polygon collider
            //PolygonCollider2D polygonCollider = gameObject.AddComponent<PolygonCollider2D>();
            var polygonCollider = mCollider;
            polygonCollider.pathCount = 0;

            if (vertices.Count == 0) return;

            // Loop through edge vertices in order
            int startVert = 0;
            int nextVert = startVert;
            int highestVert = startVert;

            colliderPath.Clear();
            while (true)
            {

                // Add vertex to collider path
                colliderPath.Add(vertices[nextVert]);

                // Get next vertex
                nextVert = lookup[nextVert];

                // Store highest vertex (to know what shape to move to next)
                if (nextVert > highestVert)
                {
                    highestVert = nextVert;
                }

                // Shape complete
                if (nextVert == startVert)
                {

                    // Add path to polygon collider
                    polygonCollider.pathCount++;
                    polygonCollider.SetPath(polygonCollider.pathCount - 1, colliderPath.ToArray());
                    colliderPath.Clear();

                    // Go to next shape if one exists
                    if (lookup.ContainsKey(highestVert + 1))
                    {

                        // Set starting and next vertices
                        startVert = highestVert + 1;
                        nextVert = startVert;

                        // Continue to next loop
                        continue;
                    }

                    // No more verts
                    break;
                }
            }
        }
    }
}