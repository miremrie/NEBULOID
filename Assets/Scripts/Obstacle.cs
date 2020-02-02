using System;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public static AudioController audioController;
    public float Damage { get; set; }

    void OnTriggerEnter2D(Collider2D col)
    {
        //Debug.Log("ObstacleHit");
        //if (col.tag == Tags.SHIP_OUTER)
        //{
        //    Destroy(this.gameObject);
        //}
        //else
        if (col.tag == Tags.SHIP_BODY)
        {
            FindObjectOfType<Game>().ObstacleHit(this);
            Destroy(this.gameObject);
        } else if (col.tag == Tags.BULLET)
        {
            Destroy(col.gameObject);
            Destroy(this.gameObject);
            if (audioController != null) audioController.PlayHitClip();
        }
    }

    internal void Initialize(float size)
    {
        Damage = size;
    }
}
