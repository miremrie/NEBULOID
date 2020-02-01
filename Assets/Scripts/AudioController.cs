﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    public AudioMixer audioMixer;
    public AnimationCurve deathLowPassPerFuel;
    public AnimationCurve sonarPassPerTime;
    public float sonarLowPassDst;
    public float sonarHighPassDst;
    private float regularHighPass = 0;
    private Timer sonarTimer;
    private bool deathLowPassActive = false;
    private float normalLowPass = 20000;
    private float sonarLowPassAtStart;
    private const string gameplayLowpass = "GameplayLowpass";
    private const string gameplayHighpass = "GameplayHighpass";
    public Game game;
    private bool sonarOverride;
    private bool sonarRevert;

    private void Awake()
    {
    }

    void Update()
    {
        if (sonarOverride)
        {
            sonarTimer.Update(Time.deltaTime);
            if (sonarRevert && sonarTimer.GetCurrentTimePercent() >= 1f)
            {
                sonarOverride = false;
                sonarRevert = false;
                audioMixer.SetFloat(gameplayHighpass, regularHighPass);
                return;

            }
            float y = sonarPassPerTime.Evaluate(sonarTimer.GetCurrentTimePercent());

            if (sonarRevert)
            {
                y = 1 - y;
            }
            audioMixer.SetFloat(gameplayLowpass, Mathf.Lerp(sonarLowPassAtStart, sonarLowPassDst, y));
            audioMixer.SetFloat(gameplayHighpass, Mathf.Lerp(regularHighPass, sonarHighPassDst, y));
        } else
        {
            audioMixer.SetFloat(gameplayLowpass, deathLowPassPerFuel.Evaluate(game.GetFuelPercent()));
        }
    }

    public void ActivateDeathLowPass()
    {

    }
    
    public void ActivateNormalLowpass()
    {
        deathLowPassActive = false;
        audioMixer.SetFloat(gameplayLowpass, normalLowPass);
    }

    public void ResetMixer()
    {
        ActivateNormalLowpass();
    }

    public void ActivateLowAndHighPass(float time)
    {
        audioMixer.GetFloat(gameplayLowpass, out sonarLowPassAtStart);
        sonarTimer = new Timer(time);
        sonarTimer.Start();
        sonarOverride = true;
    }
    public void RevertLowAndHighPass(float time)
    {
        sonarTimer = new Timer(time);
        sonarTimer.Start();
        sonarRevert = true;
    }
}
