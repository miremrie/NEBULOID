using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ShipAudioController : MonoBehaviour
{
    private const string leftArmEv = "Play_Arm_L", rightArmEv = "Play_Arm_R";
    public GameObject leftArmPlayer, rightArmPlayer;
    private const string alarmStartEv = "Play_Alarm", alarmStopEv = "Stop_Alarm";
    public GameObject alarmPlayer;
    private const string gunshotEv = "Play_Gun_Shot", bulletHitEv = "Play_Bullet_Hit", shipHitEv = "Play_Ship_Hit";
    public GameObject gunShotPlayer, bulletHitPlayer, shipHitPlayer;
    private const string fuelRefillEv = "Play_Fuel_Refill";
    public GameObject fuelRefillPlayer;
    private const string gameOverEv = "Play_Game_Over";
    public GameObject gameOverPlayer;
    private const string sonarStartEv = "Play_Sonar", sonarStopEv = "Stop_Sonar";
    public GameObject sonarPlayer;

    private void Awake()
    {
        Obstacle.audioController = this;
    }

    void Update()
    {

        //if (Input.GetKeyDown(KeyCode.T)) {
        //    PlayHitClip();
        //}

        /*if (sonarOverride)
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
        }*/
    }

    public void ActivateDeathLowPass()
    {

    }
    
    public void ActivateNormalLowpass()
    {
    }

    public void ResetMixer()
    {
        ActivateNormalLowpass();
    }

    public void ActivateSonar(float time)
    {
        /*audioMixer.GetFloat(gameplayLowpass, out sonarLowPassAtStart);
        sonarTimer = new Timer(time);
        sonarTimer.Start();
        sonarOverride = true;
        sonarSource.Play();*/
    }
    public void RevertSonar(float time)
    {
        /*sonarTimer = new Timer(time);
        sonarTimer.Start();
        sonarRevert = true;
        sonarSource.Stop();*/
    }

    public void PlayShipHit()
    {
        AkSoundEngine.PostEvent(shipHitEv, shipHitPlayer);
    }

    public void PlayShootClip()
    {
        AkSoundEngine.PostEvent(gunshotEv, gunShotPlayer);
    }

    public void PlayHitClip()
    {
        AkSoundEngine.PostEvent(bulletHitEv, bulletHitPlayer);
    }

    public void PlayAlarm()
    {
        AkSoundEngine.PostEvent(alarmStartEv, alarmPlayer);
    }

    public void StopAlarm() {
        AkSoundEngine.PostEvent(alarmStopEv, alarmPlayer);
    }

    public void PlayFuelRefill()
    {
        AkSoundEngine.PostEvent(fuelRefillEv, fuelRefillPlayer);
    }

    public void PlayGameOver()
    {
        AkSoundEngine.PostEvent(gameOverEv, gameOverPlayer);
    }

    public void PlayLeftArm()
    {
        AkSoundEngine.PostEvent(leftArmEv, leftArmPlayer);
    }

    public void PlayRightArm()
    {
        AkSoundEngine.PostEvent(rightArmEv, rightArmPlayer);
    }
}