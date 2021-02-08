using System.Collections;
using System.Collections.Generic;
using NBLD.Ship;
using NBLD.Utils;
using UnityEngine;

namespace NBLD.Character
{
    public class Mine : MonoBehaviour
    {
        public float radius;
        public float time;

        public Animator animator;
        private Timer timer;
        [Header("Damage")]
        public float oxygenPercentDamage = 0.4f;
        public float minRoomPercentDamage = 0.6f, maxRoomPercentDamage = 1f;
        private bool started = false;
        private bool exploded = false;
        private const string tickAnimKey = "Tick";
        private const string explodeAnimKey = "Explode";

        private void Awake()
        {
            timer = new Timer(time);
        }
        public void Activate()
        {
            if (!started)
            {
                timer.Restart();
                started = true;
                animator.SetTrigger(tickAnimKey);
            }
        }
        private void Update()
        {
            if (started && !exploded)
            {
                if (timer.IsTimerDone())
                {
                    Explode();
                }
            }

        }
        private void Explode()
        {
            exploded = true;
            animator.SetTrigger(explodeAnimKey);
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
            Debug.Log("Exploded!");
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].tag == Tags.CHARACTER)
                {
                    OutsideCharBehaviour charBehaviour = colliders[i].gameObject.GetComponentInParent<OutsideCharBehaviour>();
                    if (charBehaviour != null)
                    {
                        charBehaviour.GetHit(oxygenPercentDamage);
                    }
                }
                else if (colliders[i].tag == Tags.SHIP_BODY)
                {
                    ShipStatus ship = colliders[i].gameObject.GetComponentInParent<ShipStatus>();
                    ship.MineHit(this);

                }
                Debug.Log(colliders[i].gameObject.name);
            }
        }

        public float GetRandomRoomDamage()
        {
            return Random.Range(minRoomPercentDamage, maxRoomPercentDamage);
        }

        public void OnExplodeAnimationFinished()
        {
            Destroy(gameObject);
        }
    }
}

