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

        // Start is called before the first frame update
        void Start()
        {

            stencils = FindObjectsOfType<VoxelStencilCollider>();
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
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 100, 20), (Time.deltaTime * 1000).ToString());

        }
    }
}
