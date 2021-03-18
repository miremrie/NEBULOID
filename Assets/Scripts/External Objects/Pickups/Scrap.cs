using System.Collections;
using System.Collections.Generic;
using NBLD.Ship;
using UnityEngine;

namespace NBLD.Pickups
{
    public class Scrap : MonoBehaviour
    {
        public SystemName systemName;

        void OnTriggerEnter2D(Collider2D col)
        {
            if (col.tag == Tags.SHIP_BODY)
            {
                col.GetComponentInParent<ShipAssembler>().AddAvailableSystem(systemName);
                Destroy(this.gameObject);
            }
        }
    }
}