using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera mainCam;
    private float standardSize;
    public AnimationCurve sizeOverTimeCurve;
    public AnimationCurve revertingSizeCurve;
    private float dstStandardSizeMultiplier;
    private float dstSize;
    private bool changingSize = false;
    public bool revertingSize = false;
    private Timer animTimer;
    private Action callback;

    private void Awake()
    {
        standardSize = mainCam.orthographicSize;
    }

    private void Update()
    {
        if(changingSize)
        {
            animTimer.Update(Time.deltaTime);
            mainCam.orthographicSize = standardSize + sizeOverTimeCurve.Evaluate(animTimer.GetCurrentTimePercent()) * dstStandardSizeMultiplier;
            if (animTimer.GetCurrentTimePercent() >= 1f)
            {
                changingSize = false;
            }
        }
        if (revertingSize)
        {
            animTimer.Update(Time.deltaTime);
            if (animTimer.GetCurrentTimePercent() >= 1f)
            {
                mainCam.orthographicSize = standardSize;
                revertingSize = false;
                callback();
            }
            mainCam.orthographicSize = standardSize + sizeOverTimeCurve.Evaluate(1 - animTimer.GetCurrentTimePercent()) * dstStandardSizeMultiplier;
        }
    }


    public void ChangeSizeOverTime(float newSize, float time)
    {
        dstSize = newSize;
        changingSize = true;
        this.dstStandardSizeMultiplier = newSize - standardSize;
        animTimer = new Timer(time);
        animTimer.Start();
    }

    public void RevertToStandardSize(float time, Action callback)
    {
        this.callback = callback;
        revertingSize = true;
        animTimer = new Timer(time);
        animTimer.Start();
    }

}
