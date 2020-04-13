using System;
using System.Collections.Generic;
using UnityEngine;

namespace MarchingSquares
{
    
    public class VoxelMap : MonoBehaviour
    {
        public int voxelResolution = 8;
        public int chunkResolution = 2;

        public float maxFeatureAngle = 135f;

        public VoxelGrid voxelGridPrefab;

        private VoxelGrid[] chunks;

        private float chunkSize, voxelSize, halfSize;

        Voxel bottomLeft, topRight;

        public List<VoxelStencilCollider> stencilsInRange;

        public VoxelStencilCollider initialStencil;
        private List<VoxelGridSurface> surfaceColliderUpdateList = new List<VoxelGridSurface>();

        int framesToSkipCount;
        public int framesToSkipCollisionUpdate = 10;

        public Material overrideMaterial;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var c = collision.GetComponent<VoxelStencilCollider>();
            if (c != null) stencilsInRange.Add(c);
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            var c = collision.GetComponent<VoxelStencilCollider>();
            if (c != null && stencilsInRange.Contains(c)) stencilsInRange.Remove(c);
        }

        private void Awake()
        {
            stencilsInRange = new List<VoxelStencilCollider>();
            halfSize = 0.5f;
            chunkSize = 1f / chunkResolution;
            voxelSize = chunkSize / voxelResolution;

            chunks = new VoxelGrid[chunkResolution * chunkResolution];
            for (int i = 0, y = 0; y < chunkResolution; y++)
            {
                for (int x = 0; x < chunkResolution; x++, i++)
                {
                    CreateChunk(i, x, y);
                }
            }
        }

        private void Start()
        {
            if (initialStencil != null) CreateCircleMesh(); 

        }

        private void FixedUpdate()
        {
            stencilsInRange.RemoveAll(x => x == null);
            ApplyStencils();
            UpdateMeshes();
            UpdateColliders();
        }

        private void ApplyStencils()
        {
            foreach (var s in stencilsInRange)
            {
                EditVoxels(s);
            }
        }

        private void UpdateMeshes()
        {
            foreach (var c in chunks)
            {
                c.RefreshIfDirty();
            }
        }

        private void UpdateColliders()
        {
            framesToSkipCount++;
            if (framesToSkipCount > framesToSkipCollisionUpdate)
            {
                framesToSkipCount = 0;
                foreach (var s in surfaceColliderUpdateList)
                {
                    s.SetPolygonColliderPath();
                }
                surfaceColliderUpdateList.Clear();
            }
        }

        private void CreateCircleMesh()
        {
            EditVoxels(initialStencil, true);
            foreach (var c in chunks) { c.RefreshIfDirty(); }

            Destroy(initialStencil.gameObject);
        }

        private void CreateChunk(int i, int x, int y)
        {
            VoxelGrid chunk = Instantiate(voxelGridPrefab) as VoxelGrid;
            var chunkPos = new Vector2Int(x, y);
            chunk.Initialize(this, voxelResolution, chunkResolution, chunkPos, chunkSize * transform.lossyScale.x, maxFeatureAngle);
            chunk.transform.parent = transform;
            chunk.transform.localPosition =
                new Vector3(x * chunkSize - halfSize, y * chunkSize - halfSize);
            chunks[i] = chunk;
            if (x > 0)
            {
                chunks[i - 1].xNeighbor = chunk;
            }
            if (y > 0)
            {
                chunks[i - chunkResolution].yNeighbor = chunk;
                if (x > 0)
                {
                    chunks[i - chunkResolution - 1].xyNeighbor = chunk;
                }
            }
        }

     
        internal void SetTopRightTrans(Voxel transform)
        {
            topRight = transform;
        }

        internal void SetBottomLeftTrans(Voxel transform)
        {
            bottomLeft = transform;
        }

        public Vector2 CalcUV(Vector2 v)
        {
            var vert = transform.InverseTransformPoint(v.x, v.y, 0);

            var sPos = transform.InverseTransformPoint(bottomLeft.WorldPos);
            var ePos = transform.InverseTransformPoint(topRight.WorldPos);
            var x = remap(vert.x, sPos.x, ePos.x, 0, 1);
            var y = remap(vert.y, sPos.y, ePos.y, 0, 1);
            return new Vector2(x, y);
        }

        float remap(float s, float a1, float a2, float b1, float b2)
        {
            return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        }

        private void EditVoxels(IStencil activeStencil, bool ignoreFilter = false)
        {
            int xStart, xEnd, yStart, yEnd;
            xStart = yStart = 0;
            xEnd = yEnd = chunkResolution - 1;

            for (int y = yEnd; y >= yStart; y--)
            {
                int i = y * chunkResolution + xEnd;
                for (int x = xEnd; x >= xStart; x--, i--)
                {
                    activeStencil.SetGridTransform(chunks[i].transform);
                    chunks[i].Apply(activeStencil, ignoreFilter);
                }
            }
        }

        internal void AddSurfaceToUpdate(VoxelGridSurface surface)
        {
            if (!surfaceColliderUpdateList.Contains(surface)) surfaceColliderUpdateList.Add(surface);
        }
    }
}