using System;
using NBLD.Ship;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public static ShipAudioController audioController;
    public float Damage { get; set; }
    public Game game;


    void OnTriggerEnter2D(Collider2D col)
    {
        //Debug.Log("ObstacleHit");
        //if (col.tag == Tags.SHIP_OUTER)
        //{
        //    Destroy(this.gameObject);
        //}
        //else
        if (col.tag == Tags.SHIP_SHIELD)
        {
            game.BulletHit(this);
            Destroy(this.gameObject);
        }
        else if (col.tag == Tags.SHIP_BODY)
        {
            ShipStatus shipStatus = col.GetComponentInParent<ShipStatus>();
            shipStatus.ObstacleHit(this);
            Destroy(this.gameObject);
        }
        else if (col.tag == Tags.BULLET)
        {
            Destroy(col.gameObject);
            Destroy(this.gameObject);
            game.BulletHit(this);
        }
    }

    public void Initialize(float size, Game game)
    {
        Damage = size;
        this.game = game;
    }
}
