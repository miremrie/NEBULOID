using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipHookSystem : ShipSystem
{
    public Animator hookAnimator;
    public float fireSpeed, retractSpeed;
    public float shipPullSpeed;
    public float maxDistance;
    private float minDistance = 0.1f;
    public Transform shipCenterOrigin;
    public Transform hookOrigin;
    public ShipMovement ship;
    bool wasShot = false;
    bool retracting = false;
    bool hitSomething = false;
    private Timer hookOpenTimer;
    private Quaternion systemRotWhenFired, hookRotWhenFired;
    public float hookOpenAnimationTime = 1f;

    private const string hookOpenAnim = "HookOpen";
    private const string hookCloseAnim = "HookClose";
    private const string hookGrabAnim = "HookGrab";

    public override void Initialize()
    {
        if (!initialized)
        {
            base.Initialize();
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
            AdjustRotation();
            if (!retracting) //Hook flying
            {
                transform.position = transform.position + (GetDirFromBase() * fireSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, hookOrigin.position) >= maxDistance)
                {
                    retracting = true;
                    hookAnimator.SetTrigger(hookCloseAnim);
                }
            } else if (hitSomething) //Pull ship to hook
            {
                Vector3 keepPosition = this.transform.position;
                ship.MoveShip(GetDirFromBase() * shipPullSpeed * Time.deltaTime);
                this.transform.position = keepPosition;
                if (Vector3.Distance(transform.position, hookOrigin.position) <= minDistance)
                {
                    ResetHook();
                }
            } else //Hook retracting
            {
                transform.position = transform.position + (-GetDirFromBase() * retractSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, hookOrigin.position) <= minDistance)
                {
                    ResetHook();
                }
            }
        }
    }

    private void AdjustRotation()
    {
        //hookOrigin.rotation
        //transform.rotation = hookRotWhenFired;
        shipCenterOrigin.rotation = systemRotWhenFired;
    }

    private void ResetHook()
    {
        transform.position = hookOrigin.position;
        wasShot = false;
        retracting = false;
        hitSomething = false;
        hookAnimator.SetTrigger(hookCloseAnim);
        ship.UnlockHook();
        ship.UnlockMovement();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (wasShot && !retracting && collision.CompareTag(Tags.OBSTACLE))
        {

            hitSomething = true;
            retracting = true;
            hookAnimator.SetTrigger(hookGrabAnim);
            ship.LockMovement();
        }
    }

    public override void DoAction()
    {
        if (!ship.AreHooksLocked())
        {
            systemRotWhenFired = shipCenterOrigin.rotation;
            hookRotWhenFired = transform.rotation;
            wasShot = true;
            ship.LockHook();
            hookOpenTimer.Start();
            hookAnimator.ResetTrigger(hookCloseAnim);
            hookAnimator.ResetTrigger(hookGrabAnim);
            hookAnimator.SetTrigger(hookOpenAnim);
        }
    }

    private Vector3 GetDirFromBase()
    {
        return (hookOrigin.transform.position - ship.transform.position).normalized;
    }

    public override bool ReadyToUse()
    {
        return base.ReadyToUse() && !wasShot && !ship.AreHooksLocked();
    }
}
