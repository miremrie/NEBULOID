using System;
using System.Collections.Generic;
using System.Linq;
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
        List<VoxelGrid> surfaceColliderUpdateList = new List<VoxelGrid>();

        int framesToSkipCount;
        public int framesToSkipCollisionUpdate = 10;

        public Material overrideMaterial;
        
        ContactFilter2D gridFilter = new ContactFilter2D();


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
            if (initialStencil != null) CreateInitialShape();

            gridFilter.useTriggers = true;
            gridFilter.SetLayerMask(gridLayerMask);
            gridFilter.useLayerMask = true;
        }

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
        
        private void CreateInitialShape()
        {
            EditVoxels(initialStencil, true);
            foreach (var c in chunks) { c.RefreshIfDirty(); }
            UpdateMeshes();
            Destroy(initialStencil.gameObject);
        }

        private void Update()
        {
            stencilsInRange.RemoveAll(x => x == null);

            if (stencilsInRange.Count > 0)
            {
                FilterGrids();
                ApplyStencils();
                UpdateMeshes();
            }
            UpdateColliders();
        }

        Collider2D[] results = new Collider2D[1];
        public LayerMask gridLayerMask;
        HashSet<VoxelGrid> gridsToEdit = new HashSet<VoxelGrid>();

        private void FilterGrids()
        {
            foreach (var c in chunks)
            {
                results[0] = null;
                c.boundsTrigger.OverlapCollider(gridFilter,results);
                if (results[0] != null) { gridsToEdit.Add(c); }
                //Debug.Log(results);
            }
        }

        private void ApplyStencils()
        {
            foreach (var s in stencilsInRange)
            {
                EditVoxels(s);
            }

            gridsToEdit.Clear();
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
                    s.surface.SetPolygonColliderPath();
                    s.waitingForCollisionUpdate = false; 
                }
                surfaceColliderUpdateList.Clear();
            }
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

            // go in reverse order for neighbour handling
            for (int y = yEnd; y >= yStart; y--)
            {
                int i = y * chunkResolution + xEnd;
                for (int x = xEnd; x >= xStart; x--, i--)
                {
                    var c = chunks[i];
                    if (ignoreFilter || gridsToEdit.Contains(c))
                    {
                        c.Apply(activeStencil);
                    }
                }
            }
        }

        internal void AddSurfaceToUpdate(VoxelGrid surface)
        {
            if (!surfaceColliderUpdateList.Contains(surface)) surfaceColliderUpdateList.Add(surface);
        }
    }
}