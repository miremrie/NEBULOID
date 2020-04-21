using UnityEngine;

namespace MarchingSquares
{
    public class PlanetsTest : MonoBehaviour
    {
        public GameObject prefab;

        public Transform stencil;
        public VoxelStencilCollider[] stencils { get; private set; }
        public VoxelMap planet;
        public GameObject planetPrefab;

        public Collider2D movingCol, stationaryCol;
        public Transform colTestPos;
        public int colTestsNum;

        // Start is called before the first frame update
        void Start()
        {
            stencils = FindObjectsOfType<VoxelStencilCollider>();
            stationaryCol.transform.position = colTestPos.position;
            movingCol.transform.position = Vector3.zero;

        }


        void Update()
        {
            var point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var p = point + Vector3.forward * 9;
            stencil.position = p;

            if (Input.GetMouseButton(0))
            {
                var go = Instantiate(prefab, p + Vector3.forward, Quaternion.identity);
                Destroy(go, 5f);
            }

            if (Input.GetMouseButtonDown(1))
            {
                foreach (var s in stencils)
                {
                    s.fillType = !s.fillType;
                }
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                var stencils = FindObjectsOfType<VoxelStencilCollider>();
                foreach (var s in stencils)
                {
                    if (s.tag == "stencil") continue;
                    s.CarveOnce(() => Destroy(s.transform.parent.gameObject));
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                var pGo = Instantiate(planetPrefab, p + Vector3.forward, Quaternion.identity);
                Destroy(pGo, 15);
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                movingCol.transform.position = Vector3.zero;
                var pos = colTestPos.position;
                movingCol.transform.position = pos;
                var f = new ContactFilter2D();
                f.useTriggers = true;

                for (int i = 0; i < colTestsNum; i++)
                {
                stationaryCol.OverlapCollider(f, results);

                }
                movingCol.transform.position = Vector3.zero;
                Debug.Log(results[0].name);
                results[0] = null;
            }
        }

        public Collider2D[] results = new Collider2D[10];

        private void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 100, 20), (Time.deltaTime * 1000).ToString());

        }
    }
}
