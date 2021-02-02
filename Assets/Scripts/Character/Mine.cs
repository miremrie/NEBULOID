using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.Character
{
    public class Mine : MonoBehaviour
    {
        public float radius;
        public float time;

        public Animator animator;
        private Timer timer;
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
                timer.Start();
                started = true;
                animator.SetTrigger(tickAnimKey);
            }
        }
        private void Update()
        {
            if (started && !exploded)
            {
                timer.Update(Time.deltaTime);
                if (!timer.IsRunning())
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
                Debug.Log(colliders[i].gameObject.name);
            }
        }

        public void OnExplodeAnimationFinished()
        {
            Destroy(gameObject);
        }
    }
}

