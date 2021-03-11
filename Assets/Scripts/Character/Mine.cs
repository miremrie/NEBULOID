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
        public CircleCollider2D circleCollider;
        private Timer timer;
        [Header("Damage")]
        public float oxygenPercentDamage = 0.4f;
        public float minRoomPercentDamage = 0.6f, maxRoomPercentDamage = 1f;
        private bool started = false;
        private bool exploded = false;
        private const string tickAnimKey = "Tick";
        private const string explodeAnimKey = "Explode";
        private const string explodeSFXKey = "Play_Bullet_Hit";
        private int explosionFrameCount = 0;
        private int explosionDisableColliderFrameCount = 2;

        private void Awake()
        {
            timer = new Timer(time);
            circleCollider.gameObject.SetActive(false);
        }
        public void Activate()
        {
            if (!started)
            {
                circleCollider.gameObject.SetActive(false);
                circleCollider.radius = radius;
                timer.Restart();
                started = true;
                animator.SetTrigger(tickAnimKey);
            }
        }
        private void Update()
        {
            if (started)
            {
                if (!exploded)
                {
                    if (timer.IsTimerDone())
                    {
                        Explode();
                    }
                }
            }
        }
        private void FixedUpdate()
        {
            if (started && exploded)
            {
                if (explosionFrameCount == explosionDisableColliderFrameCount)
                {
                    circleCollider.gameObject.SetActive(false);
                }
                explosionFrameCount++;
            }
        }
        private void Explode()
        {
            explosionFrameCount = 0;
            exploded = true;
            animator.SetTrigger(explodeAnimKey);
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
            AkSoundEngine.PostEvent(explodeSFXKey, gameObject);
            circleCollider.gameObject.SetActive(true);
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
                else if (colliders[i].tag == Tags.OBSTACLE)
                {
                    Obstacle obstacle = colliders[i].gameObject.GetComponentInParent<Obstacle>();
                    obstacle.DestroyObstacle();
                }
                //Debug.Log(colliders[i].gameObject.name);
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

