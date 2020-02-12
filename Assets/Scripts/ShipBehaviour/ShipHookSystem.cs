﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipHookSystem : ShipSystem
{
    public Animator hookAnimator;
    public float fireSpeed, retractSpeed;
    public float shipPullSpeed;
    public float maxDistance;
    private float minDistance = 0.1f;
    public Transform origin;
    public ShipMovement ship;
    private Vector3 direction;
    bool wasShot = false;
    bool retracting = false;
    bool hitSomething = false;
    Vector3 shipDirection;
    private Timer hookOpenTimer;
    public float hookOpenAnimationTime = 1f;

    private const string hookOpenAnim = "HookOpen";
    private const string hookCloseAnim = "HookClose";
    private const string hookGrabAnim = "HookGrab";

    public override void Initialize()
    {
        if (!initialized)
        {
            base.Initialize();
            direction = (origin.position - ship.transform.position).normalized;
            direction.z = 0;
            hookOpenTimer = new Timer(hookOpenAnimationTime);
        }
    }

    protected override void Update()
    {
        base.Update();
        Move();
    }

    private void Move()
    {
        hookOpenTimer.Update(Time.deltaTime);
        if (wasShot && !hookOpenTimer.IsRunning())
        {
            if (!retracting)
            {
                transform.position = transform.position + (direction * fireSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, origin.position) >= maxDistance)
                {
                    retracting = true;
                    hookAnimator.SetTrigger(hookCloseAnim);
                }
            } else if (hitSomething)
            {
                Vector3 keepPosition = this.transform.position;
                ship.transform.position = ship.transform.position + (shipDirection * shipPullSpeed * Time.deltaTime);
                this.transform.position = keepPosition;
                if (Vector3.Distance(transform.position, origin.position) <= minDistance)
                {
                    ResetHook();
                }
            } else
            {
                transform.position = transform.position + (-direction * retractSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, origin.position) <= minDistance)
                {
                    ResetHook();
                }
            }
        }
    }

    private void ResetHook()
    {
        transform.position = origin.position;
        wasShot = false;
        retracting = false;
        hitSomething = false;
        hookAnimator.SetTrigger(hookCloseAnim);
        ship.UnlockShip();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!retracting && collision.CompareTag(Tags.OBSTACLE))
        {

            hitSomething = true;
            retracting = true;
            shipDirection = direction;
            hookAnimator.SetTrigger(hookGrabAnim);
        }
    }

    public override void DoAction()
    {
        if (!ship.AreHooksLocked())
        {
            direction = (origin.transform.position - ship.transform.position).normalized;
            wasShot = true;
            ship.LockShip();
            hookOpenTimer.Start();
            hookAnimator.ResetTrigger(hookCloseAnim);
            hookAnimator.ResetTrigger(hookGrabAnim);
            hookAnimator.SetTrigger(hookOpenAnim);
        }
    }

    public bool IsReady()
    {
        return !wasShot && !ship.AreHooksLocked();
    }
}
