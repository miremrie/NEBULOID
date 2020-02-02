using UnityEngine;

public class RotateAround : MonoBehaviour
{
    float rotationSpeed;
    // Start is called before the first frame update
    void Start()
    {
        rotationSpeed = Random.Range(-25f, 25f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

    }
}
