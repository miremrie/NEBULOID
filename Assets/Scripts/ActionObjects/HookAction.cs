using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookAction : ActionObject
{
    public HookBehaviour hook;
    public float timeBetweenShots;
    private Timer timer;
    public Repairable repairable;
    public AudioController audioController;
    public ScreenShake shake;
    public Animator pistonButtonAnimator;
    private const string buttonAnimName = "ButtonPush";

    private void Awake()
    {
        timer = new Timer(timeBetweenShots);
    }

    private void Update()
    {
        timer.Update(Time.deltaTime);
        if (!timer.IsRunning())
        {
            pistonButtonAnimator.SetBool(buttonAnimName, false);
        }
    }

    public override bool IsActionObjectReady()
    {
        bool isReady = base.IsActionObjectReady();
        isReady = isReady && !timer.IsRunning() && hook.IsReady();
        return isReady;
    }

    public override void DoAction(InputController controller)
    {
        if (repairable.IsRepaired())
        {
            shake.TriggerShake(0.5f);
            timer.Start();
            hook.ShootHook();
            audioController.PlayShootClip();
            pistonButtonAnimator.SetTrigger(buttonAnimName);
        }
        else
        {
            repairable.StartRepairing();
        }

    }

    public override void OnExitAction()
    {
        base.OnExitAction();
        repairable.StopRepairing();
    }
}
