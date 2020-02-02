using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : ActionObject
{
    public float timeBetweenShots;
    private Timer timer;
    public Bullet bullet;
    public Transform bulletShoothole;
    public bool shootBuletsNotBombs = true;
    public Repairable repairable;
    public AudioController audioController;
    public Animator bigRedButtonAnimator;
    public ScreenShake shake;
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
            bigRedButtonAnimator.SetBool(buttonAnimName, false);
        }
    }

    public override bool IsActionObjectReady()
    {
        bool isReady = base.IsActionObjectReady();
        isReady = isReady && !timer.IsRunning();
        return isReady;
    }

    public override void DoAction(InputController controller)
    {
        if (repairable.IsRepaired())
        {
            shake.TriggerShake(0.5f);
            timer.Start();
            Bullet tmpBullet = Instantiate(bullet);
            tmpBullet.transform.position = bulletShoothole.position;
            tmpBullet.Shoot(bulletShoothole.up, shootBuletsNotBombs);
            audioController.PlayShootClip();
            bigRedButtonAnimator.SetBool(buttonAnimName, true);
        } else
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
