using System;
using System.Collections.Generic;

namespace UnityEngine.AI
{
    public enum NavMeshCollectGeometryNBLD
    {
        RenderMeshes = 0,
        PhysicsColliders = 1,
        Both = 2
    }
    class NavMeshBuilderNBLDWrapper
    {
        public Dictionary<Sprite, Mesh> map;
        public Dictionary<uint, Mesh> coliderMap;
        public int defaultArea;
        public int layerMask;
        public int agentID;
        public bool overrideByGrid;
        public GameObject useMeshPrefab;
        public bool compressBounds;
        public Vector3 overrideVector;
        public NavMeshCollectGeometry CollectGeometry;
        public CollectObjectsNBLD CollectObjects;
        public GameObject parent;
        internal bool CollectBoth;

        public NavMeshBuilderNBLDWrapper()
        {
            map = new Dictionary<Sprite, Mesh>();
            coliderMap = new Dictionary<uint, Mesh>();
        }

        public Mesh GetMesh(Sprite sprite)
        {
            Mesh mesh;
            if (map.ContainsKey(sprite))
            {
                mesh = map[sprite];
            }
            else
            {
                mesh = new Mesh();
                NavMeshBuilderNBLD.sprite2mesh(sprite, mesh);
                map.Add(sprite, mesh);
            }
            return mesh;
        }

        internal Mesh GetMesh(Collider2D collider)
        {
#if UNITY_2019_3_OR_NEWER
            Mesh mesh;
            uint hash = collider.GetShapeHash();
            if (coliderMap.ContainsKey(hash))
            {
                mesh = coliderMap[hash];
            }
            else
            {
                mesh = collider.CreateMesh(false, false);
                coliderMap.Add(hash, mesh);
            }
            return mesh;
#else
            throw new InvalidOperationException("PhysicsColliders supported in Unity 2019.3 and higher.");
#endif
        }

        internal IEnumerable<GameObject> GetRoot()
        {
            switch (CollectObjects)
            {
                case CollectObjectsNBLD.Children: return new[] { parent };
                case CollectObjectsNBLD.Volume: 
                case CollectObjectsNBLD.All: 
                default:
                    return SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            }
        }
    }

    class NavMeshBuilderNBLD
    {
        internal static void CollectSources(List<NavMeshBuildSource> sources, NavMeshBuilderNBLDWrapper builder)
        {
            var root = builder.GetRoot();
            foreach (var it in root)
            {
                CollectSources(it, sources, builder);
            }
        }

        private static void CollectSources(GameObject root, List<NavMeshBuildSource> sources, NavMeshBuilderNBLDWrapper builder)
        {
            foreach (var modifier in root.GetComponentsInChildren<NavMeshModifier>())
            {
                if (((0x1 << modifier.gameObject.layer) & builder.layerMask) == 0)
                {
                    continue;
                }
                if (!modifier.AffectsAgentType(builder.agentID))
                {
                    continue;
                }
                int area = builder.defaultArea;

                if (modifier.overrideArea)
                {
                    area = modifier.area;
                }
                if (!modifier.ignoreFromBuild)
                {
                    if (builder.CollectGeometry == NavMeshCollectGeometry.PhysicsColliders ||
                        builder.CollectBoth)
                    {
                        CollectSources(sources, modifier, area, builder);
                    }
                    if (builder.CollectGeometry == NavMeshCollectGeometry.RenderMeshes ||
                        builder.CollectBoth)
                    {
                        var sprite = modifier.GetComponent<SpriteRenderer>();
                        if (sprite != null)
                        {
                            CollectSources(sources, sprite, area, builder);
                        }
                    }
                }
            }
            Debug.Log("Sources " + sources.Count);
        }

        private static void CollectSources(List<NavMeshBuildSource> sources, SpriteRenderer sprite, int area, NavMeshBuilderNBLDWrapper builder)
        {
            if (sprite == null)
            {
                return;
            }
            var src = new NavMeshBuildSource();
            src.shape = NavMeshBuildSourceShape.Mesh;
            src.area = area;

            Mesh mesh;
            mesh = builder.GetMesh(sprite.sprite);
            if (mesh == null)
            {
                Debug.Log($"{sprite.name} mesh is null");
                return;
            }
            src.transform = Matrix4x4.TRS(Vector3.Scale(sprite.transform.position, builder.overrideVector), sprite.transform.rotation, sprite.transform.lossyScale);
            src.sourceObject = mesh;
            sources.Add(src);
        }

        private static void CollectSources(List<NavMeshBuildSource> sources, NavMeshModifier modifier, int area, NavMeshBuilderNBLDWrapper builder)
        {
            var collider = modifier.GetComponent<Collider2D>();
            if (collider == null)
            {
                return;
            }

            if (collider.usedByComposite)
            {
                collider = collider.GetComponent<CompositeCollider2D>();
            }

            var src = new NavMeshBuildSource();
            src.shape = NavMeshBuildSourceShape.Mesh;
            src.area = area;

            Mesh mesh;
            mesh = builder.GetMesh(collider);
            if (mesh == null)
            {
                Debug.Log($"{collider.name} mesh is null");
                return;
            }
            if (collider.attachedRigidbody)
            {
                src.transform = Matrix4x4.TRS(Vector3.Scale(collider.transform.position, builder.overrideVector), collider.transform.rotation, Vector3.one);
            }
            else
            {
                src.transform = Matrix4x4.identity;
            }
            src.sourceObject = mesh;
            sources.Add(src);
        }


        internal static void sprite2mesh(Sprite sprite, Mesh mesh)
        {
            Vector3[] vert = new Vector3[sprite.vertices.Length];
            for (int i = 0; i < sprite.vertices.Length; i++)
            {
                vert[i] = new Vector3(sprite.vertices[i].x, sprite.vertices[i].y, 0);
            }
            mesh.vertices = vert;
            mesh.uv = sprite.uv;
            int[] tri = new int[sprite.triangles.Length];
            for (int i = 0; i < sprite.triangles.Length; i++)
            {
                tri[i] = sprite.triangles[i];
            }
            mesh.triangles = tri;
        }

        static private NavMeshBuildSource BoxBoundSource(Bounds localBounds)
        {
            var src = new NavMeshBuildSource();
            src.transform = Matrix4x4.Translate(localBounds.center);
            src.shape = NavMeshBuildSourceShape.Box;
            src.size = localBounds.size;
            src.area = 0;
            return src;
        }
    }
}
