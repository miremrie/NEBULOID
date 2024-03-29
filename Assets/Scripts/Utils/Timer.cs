﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.Utils
{
    public class Timer
    {
        private float maxTime;
        private float minTime;
        private float currentTime;
        private bool timeStarted;
        private float lastUpdatedTime;
        private bool clamp;
        private float delayTime;
        private bool reverseTime;

        public Timer(float time, bool startNow = false, bool clamp = true)
        {
            this.maxTime = time;
            this.minTime = 0;
            this.currentTime = 0;
            lastUpdatedTime = Time.time;
            this.clamp = clamp;
            timeStarted = false;
            delayTime = 0;
            reverseTime = false;
            if (startNow)
            {
                Start();
            }
        }

        public void Start() { timeStarted = true; lastUpdatedTime = Time.time; }
        public void Pause() => timeStarted = false;
        public void Stop() { timeStarted = false; currentTime = 0; }
        public void Restart() { timeStarted = true; currentTime = 0; lastUpdatedTime = Time.time; }
        public void Finish() { Stop(); currentTime = maxTime; }
        public void StartReversed() { Start(); reverseTime = !reverseTime; }
        public void StartForward() { Start(); reverseTime = false; }
        public void StartBackward() { Start(); reverseTime = true; }
        public void RestartDelayed(float delay)
        {
            timeStarted = true;
            delayTime = delay;
            currentTime = 0;
            lastUpdatedTime = Time.time;
        }
        private void UpdateTime()
        {
            if (timeStarted)
            {
                if (delayTime > 0)
                {
                    delayTime -= Time.time - lastUpdatedTime;
                    lastUpdatedTime = Time.time;
                }
                else
                {
                    if (!reverseTime)
                    {
                        currentTime += Time.time - lastUpdatedTime;
                    }
                    else
                    {
                        currentTime -= Time.time - lastUpdatedTime;
                    }
                    lastUpdatedTime = Time.time;
                    if (clamp)
                    {
                        if ((!reverseTime && currentTime >= maxTime) || (reverseTime && currentTime <= minTime))
                        {
                            timeStarted = false;
                        }
                        currentTime = Mathf.Clamp(currentTime, minTime, maxTime);
                    }
                }
            }
        }

        public float GetCurrentTime() { UpdateTime(); return currentTime; }
        public float GetCurrentTimePercent() { UpdateTime(); return currentTime / maxTime; }
        public float GetCurrentTimePercentClamped() { UpdateTime(); return Mathf.Clamp01(currentTime / maxTime); }
        public float GetTimeLeft() { UpdateTime(); return maxTime - currentTime; }
        public bool IsTimerDone() { UpdateTime(); return currentTime - maxTime >= 0; }
        public float GetCycles() { UpdateTime(); return currentTime / maxTime; }
        public int GetFullCycles() { UpdateTime(); return Mathf.FloorToInt(currentTime / maxTime); }
        public bool IsRunning() { UpdateTime(); return delayTime <= 0 && timeStarted; }
    }
}
