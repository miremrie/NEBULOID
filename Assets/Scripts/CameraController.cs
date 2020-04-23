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
    public List<CamZone> zones = new List<CamZone>();
    public float smooth = 5;

    public CamZone limitZone; 

    private void Awake()
    {
        standardSize = mainCam.orthographicSize;
    }

    private void LateUpdate()
    {
        var tr = mainCam.transform;
        Vector3 pos = Vector3.zero;
        Vector3 largest = Vector3.zero;
        float maxX, maxY;
        maxX = maxY = 0;
        float weightSum = 0;
        foreach (var z in zones)
        {
            var lPos = (z.Pos - limitZone.Pos); // unclamped relative pos
            var clampedPos = lPos.magnitude + z.radius > limitZone.radius ? // clamped absolute pos
                limitZone.Pos + Vector3.ClampMagnitude(lPos, limitZone.radius - z.radius) :
                z.Pos;
            pos += clampedPos * z.weight;
            weightSum += z.weight;

            var rPos = clampedPos - tr.position; // clamped relative

            // size
            var y = Math.Abs(rPos.y) + z.radius;
            maxY = y > maxY ? y : maxY;

            var x = Math.Abs(rPos.x) + z.radius;
            maxX = x > maxX ? x : maxX;
        }
        pos /= weightSum;

        var tarPos = new Vector3(pos.x, pos.y, tr.position.z); //discard z

        maxX /= mainCam.aspect;
        var newSize = Math.Max(maxX, maxY);
        var finSize = Mathf.Lerp(mainCam.orthographicSize, newSize, smooth * Time.deltaTime);
        var finPos = Vector3.Lerp(tr.position, tarPos, smooth * Time.deltaTime);

        tr.position = finPos;
        mainCam.orthographicSize = finSize;
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

   
    [Serializable]
    public class CamZone
    {
        public Transform trans;
        public Vector3 Pos => trans.position;
        public float radius;
        [Range(0,1)]
        public float weight;
    }
}
