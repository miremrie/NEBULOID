using UnityEngine;

public class MovingObstacle : Obstacle
{
    public float minSpeed, maxSpeed;
    private float speed;
    private Vector3 direction;

    public void Initialize(Transform flyTo)
    {
        speed = Random.Range(minSpeed, maxSpeed);
        direction = (flyTo.position - transform.position).normalized;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    public void Initialize(Transform ship, float size, Game game)
    {
        Initialize(ship);
        Initialize(size, game);
    }
}
