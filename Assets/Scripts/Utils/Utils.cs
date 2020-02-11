using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class Timer
{
    private float time;
    private float currTime;
    private float delayTime;
    private bool playDelayed = false;

    public Timer(float time)
    {
        this.time = time;
        currTime = time;
    }

    public void Update(float dt)
    {
        if (playDelayed)
        {
            delayTime += dt;
            if (delayTime > 0f)
            {
                Start();
                playDelayed = false;
            }
        }

        currTime += dt;
    }

    public bool IsRunning()
    {

        return (currTime < time);
    }

    public float GetCurrentTime()
    {
        return currTime;
    }

    public float GetCurrentTimePercent()
    {
        return currTime / time;
    }

    public void Start()
    {
        currTime = 0f;
    }

    public void StartDelayed(float delay)
    {
        delayTime = -delay;
        playDelayed = true;
    }
}
