using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    public AudioMixer audioMixer;
    public AnimationCurve deathLowPassPerFuel;
    private bool deathLowPassActive = false;
    private float normalLowPass = 20000;
    private const string gameplayLowpass = "GameplayLowpass";
    public Game game;

    private void Awake()
    {
    }

    void Update()
    {
        audioMixer.SetFloat(gameplayLowpass, deathLowPassPerFuel.Evaluate(game.GetFuelPercent()));
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
}
