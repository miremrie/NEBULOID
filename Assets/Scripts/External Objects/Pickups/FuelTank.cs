using NBLD.Ship;
using UnityEngine;

namespace NBLD.Pickups
{
    public class FuelTank : MonoBehaviour
    {
        public float Amount { get; set; }

        void OnTriggerEnter2D(Collider2D col)
        {
            /*if (col.tag == Tags.SHIP_BODY)
            {
                col.GetComponentInParent<ShipStatus>().FuelCollected(this);
                Destroy(this.gameObject);
            }*/
        }
    }
}