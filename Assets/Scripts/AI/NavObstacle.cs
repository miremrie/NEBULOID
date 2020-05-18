using UnityEngine;

public class NavObstacle : MonoBehaviour
{
    public float radius = 1f;
    public Vector2 velocity = Vector2.one;

    void Update()
    {
        transform.position += (Vector3) velocity * Time.deltaTime; 
    }
}
