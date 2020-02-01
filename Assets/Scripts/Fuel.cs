using UnityEngine;

public class Fuel : MonoBehaviour
{
    public float Amount { get; set; }

    

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == Tags.SHIP_BODY)
        {
            FindObjectOfType<Game>().FuelCollected(this);
            Debug.Log("FuelHit");
            Destroy(this.gameObject);
        }
    }
}


