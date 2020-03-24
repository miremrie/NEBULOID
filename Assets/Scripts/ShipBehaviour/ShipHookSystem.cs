using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipHookSystem : ShipSystem
{
    public Animator hookAnimator;
    public float fireMaxSpeed, retractSpeed;
    public float accelerationTime;
    private Timer fireTimer;
    public AnimationCurve acceleration;
    public float shipPullSpeed;
    public float maxDistance;
    private float minDistance = 0.1f;
    public Transform shipCenterOrigin;
    public Transform hookOrigin;
    public Transform rightHookPivot, leftHookPivot;
    public ShipMovement ship;
    public Collider2D triggerCollider;
    bool wasShot = false;
    bool retracting = false;
    bool hitSomething = false;
    private Timer hookOpenTimer;
    private Quaternion systemRotWhenFired, hookRotWhenFired;
    public float hookOpenAnimationTime = 1f;
    public float previousHookRotation;
    public float originalZRotation = 90;


    private const string hookOpenAnim = "HookOpen";
    private const string hookCloseAnim = "HookClose";
    private const string hookGrabAnim = "HookGrab";
    private const float hookGrabAnimOffset = 0.6667f;
    private GameObject grabbedObject;
    private bool isGrabbedObjectDraggable = false;

    public override void Initialize()
    {
        if (!initialized)
        {
            base.Initialize();
            hookOpenTimer = new Timer(hookOpenAnimationTime);
            fireTimer = new Timer(accelerationTime);
            OffsetColliderBasedOnCollision();
            previousHookRotation = rightHookPivot.eulerAngles.z;
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
        fireTimer.Update(Time.deltaTime);
        if (wasShot && !hookOpenTimer.IsRunning())
        {
            if (!retracting) //Hook flying
            {
                transform.position = transform.position + (GetDirFromBase() * GetFireSpeed() * Time.deltaTime);
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
                UpdateGrabDrag();
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

    private void LateUpdate()
    {
        AdjustRotation();
    }

    private float GetFireSpeed()
    {
        return acceleration.Evaluate(fireTimer.GetCurrentTimePercentClamped()) * fireMaxSpeed;
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
        isGrabbedObjectDraggable = false;
        hookAnimator.SetTrigger(hookCloseAnim);
        ship.UnlockHook();
        ship.UnlockMovement();
        grabbedObject = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (wasShot && !retracting && collision.CompareTag(Tags.OBSTACLE))
        {
            PrepareGrab(collision.gameObject);
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
            fireTimer.Start();
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

    private void PrepareGrab(GameObject grabbed)
    {
        grabbedObject = grabbed;
        Obstacle obs = grabbedObject.GetComponent<Obstacle>();
        isGrabbedObjectDraggable = obs != null;
        hookAnimator.SetTrigger(hookGrabAnim);
    }

    private void Grab()
    {
        previousHookRotation = rightHookPivot.eulerAngles.z;
        hitSomething = true;
        retracting = true;
        ship.LockMovement();
    }

    private void UpdateGrabDrag()
    {
        if (hitSomething && grabbedObject != null && isGrabbedObjectDraggable)
        {
            float angle = rightHookPivot.eulerAngles.z - previousHookRotation;
            grabbedObject.transform.RotateAround(transform.position, Vector3.forward, angle);
            previousHookRotation = rightHookPivot.eulerAngles.z;
        }
    }

    private void OffsetColliderBasedOnCollision()
    {
        float initalOffsetX = triggerCollider.offset.x;
        float initalOffsetY = triggerCollider.offset.y;
        float animationOffsetX = hookGrabAnimOffset * fireMaxSpeed;
        if (initalOffsetX < 0)
        {
            animationOffsetX = -animationOffsetX;
        }
        triggerCollider.offset = new Vector2(initalOffsetX + animationOffsetX, initalOffsetY);
    }
}
