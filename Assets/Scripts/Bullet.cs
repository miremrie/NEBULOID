using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 direction;
    private bool wasShot = false;
    public float speed = 4f;
    public Collider2D col;
    public float bombTime = 2f;
    private Timer bombTimer;
    public float bombRadius = 5f;
    private Vector3 origin;
    private bool isBulletNotBomb;

    private void Awake()
    {
        bombTimer = new Timer(bombTime);
    }

    private void Update()
    {
        if (wasShot)
        {
            transform.position = transform.position + ((direction).normalized * speed * Time.deltaTime);
            if (!isBulletNotBomb) {
                if (!bombTimer.IsRunning())
                {
                    ActivateBomb();
                }
                bombTimer.Update(Time.deltaTime);

            }
        }
    }

    public void Shoot(Vector3 direction, bool isBullet = true)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

        origin = transform.position;
        isBulletNotBomb = isBullet;
        bombTimer.Start();
        if (!isBullet)
        {
            ((CircleCollider2D)col).radius = bombRadius;
            col.enabled = false;
        }
        this.direction = direction;
        this.wasShot = true;
    }

    public void ShootBullet(Vector3 destination)
    {

    }

    public void ShootBomb(Vector3 destination)
    {
        wasShot = true;
        this.direction = destination;

    }

    private void ActivateBomb()
    {
        col.enabled = true;

    }
}
