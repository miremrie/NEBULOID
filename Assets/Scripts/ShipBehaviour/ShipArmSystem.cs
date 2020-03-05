using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipArmSystem : ShipSystem
{
    public Animator animator;
    public ShipMovement shipMovement;
    public Transform pivot;
    public bool isLeft;
    public float moveDuration = 0.3f;
    public float moveDelayTime = 0.416666f;
    private Timer moveTimer;
    private const string animArmMoveName = "ArmMove";

    public override void Initialize()
    {
        if (!initialized)
        {
            base.Initialize();
            moveTimer = new Timer(moveDuration);
            shipMovement.RegisterArm(this);
        }
    }

    protected override void Update()
    {
        base.Update();
        moveTimer.Update(Time.deltaTime);
    }

    public bool IsMoving()
    {
        return moveTimer.IsRunning();
    }

    public override void DoAction()
    {
        base.DoAction();
        animator.SetTrigger(animArmMoveName);
        moveTimer.StartDelayed(moveDelayTime);
        shipMovement.Rotate(isLeft);
    }

    public override bool ReadyToUse()
    {
        return base.ReadyToUse() && !shipMovement.IsShipMovementLocked();
    }
}