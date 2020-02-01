using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera camera;
    public float standardSize;
    public AnimationCurve sizeOverTimeCurve;
    public AnimationCurve revertingSizeCurve;
    private float dstStandardSizeMultiplier;
    private float dstSize;
    private bool changingSize = false;
    public bool revertingSize = false;
    private Timer animTimer;

    private void Update()
    {
        if(changingSize)
        {
            animTimer.Update(Time.deltaTime);
            camera.orthographicSize = standardSize + sizeOverTimeCurve.Evaluate(animTimer.GetCurrentTimePercent()) * dstStandardSizeMultiplier;
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
                camera.orthographicSize = standardSize;
                revertingSize = false;
            }
            camera.orthographicSize = standardSize + sizeOverTimeCurve.Evaluate(1 - animTimer.GetCurrentTimePercent()) * dstStandardSizeMultiplier;
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

    public void RevertToStandardSize(float time)
    {
        revertingSize = true;
        animTimer = new Timer(time);
        animTimer.Start();
    }

}
