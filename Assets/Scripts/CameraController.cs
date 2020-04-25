using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DynamicCamera;

public class CameraController : MonoBehaviour
{
    public Camera cam;
    private float standardSize;
    public AnimationCurve sizeOverTimeCurve;
    public AnimationCurve revertingSizeCurve;
    private float dstStandardSizeMultiplier;
    private float dstSize;
    private bool changingSize = false;
    public bool revertingSize = false;
    private Timer animTimer;
    private Action callback;
    public List<CamSet> sets = new List<CamSet>();
    private CamSet activeSet;
    public string initalSetName;
    public float smooth = 5;

    private void Awake()
    {
        standardSize = cam.orthographicSize;
        ActivateCamSet(initalSetName);
    }

    private void LateUpdate()
    {
        var newTarget = DynamicCam.Calculate(cam, activeSet);

        var s = smooth * Time.deltaTime;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newTarget.size, s);
        cam.transform.position = Vector3.Lerp(cam.transform.position, newTarget.pos, s);
    }

    public void ActivateCamSet(CamSet set) => activeSet = set;
    public void ActivateCamSet(string setName) => activeSet = FindSet(setName); 

    public CamSet FindSet(string name)
    {
        return sets.FirstOrDefault(s => s.name == name)
            ?? throw new Exception("There is no set with that name");
    }

    private void Update()
    {
        UpdateSonarLegacy();
    }

    [Obsolete("Use new camera system")]
    private void UpdateSonarLegacy()
    {
        if (changingSize)
        {
            animTimer.Update(Time.deltaTime);
            cam.orthographicSize = standardSize + sizeOverTimeCurve.Evaluate(animTimer.GetCurrentTimePercent()) * dstStandardSizeMultiplier;
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
                cam.orthographicSize = standardSize;
                revertingSize = false;
                callback();
            }
            cam.orthographicSize = standardSize + sizeOverTimeCurve.Evaluate(1 - animTimer.GetCurrentTimePercent()) * dstStandardSizeMultiplier;
        }
    }

    [Obsolete("Use the new camera system")]
    public void ChangeSizeOverTime(float newSize, float time)
    {
        dstSize = newSize;
        changingSize = true;
        this.dstStandardSizeMultiplier = newSize - standardSize;
        animTimer = new Timer(time);
        animTimer.Start();
    }

    [Obsolete("Use the new camera system")]
    public void RevertToStandardSize(float time, Action callback)
    {
        this.callback = callback;
        revertingSize = true;
        animTimer = new Timer(time);
        animTimer.Start();
    }

}

namespace DynamicCamera
{
    public static class DynamicCam
    {
        public static CamTarget Calculate(Camera cam, CamSet set)
        {
            var zones = set.camZones;
            var limitZone = set.limitZone;
            var tr = cam.transform;
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

            maxX /= cam.aspect;
            var newSize = Math.Max(maxX, maxY);

            return new CamTarget { pos = tarPos, size = newSize };
        }
    }

    public struct CamTarget
    {
        public float size;
        public Vector3 pos;
    }

    [Serializable]
    public class CamZone
    {
        public Transform trans;
        public Vector3 Pos => trans.position;
        public float radius;
        [Range(0, 1)]
        public float weight;
    }

    [Serializable]
    public class CamSet
    {
        public string name;
        public CamZone limitZone;
        public List<CamZone> camZones = new List<CamZone>();

        public CamZone FindZone(Transform trans)
        {
            return camZones.FirstOrDefault(s => s.trans == trans)
                ?? throw new Exception("There is no zone with that transform");
        }
    }
}
