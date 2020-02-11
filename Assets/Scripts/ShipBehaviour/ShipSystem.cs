using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSystem : MonoBehaviour
{
    public float cooldownTime = 1f;
    private Timer cooldownTimer;

    protected virtual void Start()
    {
        cooldownTimer = new Timer(cooldownTime);
    }

    protected virtual void Update()
    {
        UpdateCooldownTimer();
    }

    private void UpdateCooldownTimer()
    {
        cooldownTimer.Update(Time.deltaTime);
    }

    public virtual void DoAction()
    {
        cooldownTimer.Start();
    }

    public virtual bool ReadyToUse()
    {
        return !cooldownTimer.IsRunning();
    }
}
