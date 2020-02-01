using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public static AudioController audioController;

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("ObstacleHit");
        if (col.tag == Tags.SHIP_OUTER || col.tag == Tags.SHIP_BODY)
        {
            FindObjectOfType<Game>().ObstacleHit(this);
           
            Destroy(this.gameObject);
        }
        if (col.tag == Tags.BULLET)
        {
            Destroy(col.gameObject);
            Destroy(this.gameObject);
            if (audioController != null) audioController.PlayHitClip();
        }
    }
}
