using UnityEngine;

public class Obstacle : MonoBehaviour
{
    
    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("ObstacleHit");
        if (col.tag == Tags.SHIP_OUTER || col.tag == Tags.SHIP_BODY)
        {
            FindObjectOfType<Game>().ObstacleHit(this);
           
            Destroy(this.gameObject);
        }
    }
}
