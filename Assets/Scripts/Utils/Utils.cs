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

    public float GetCurrentTimePercentClamped()
    {
        return Mathf.Clamp01(GetCurrentTimePercent());
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
public class MMath
{
    public static bool LineSegmentsIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 intersection)
    {
        intersection = Vector2.zero;

        var d = (p2.x - p1.x) * (p4.y - p3.y) - (p2.y - p1.y) * (p4.x - p3.x);

        if (d == 0.0f)
        {
            return false;
        }

        var u = ((p3.x - p1.x) * (p4.y - p3.y) - (p3.y - p1.y) * (p4.x - p3.x)) / d;
        var v = ((p3.x - p1.x) * (p2.y - p1.y) - (p3.y - p1.y) * (p2.x - p1.x)) / d;

        if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
        {
            return false;
        }

        intersection.x = p1.x + u * (p2.x - p1.x);
        intersection.y = p1.y + u * (p2.y - p1.y);

        return true;
    }


}
