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
    private const string fuelRefillEv = "Play_Fuel_Refill", fuelRefillStopEv = "Stop_Fuel_Refill";
    public GameObject fuelRefillPlayer;
    private const string gameOverEv = "Play_Game_Over";
    public GameObject gameOverPlayer;
    private const string sonarStartEv = "Play_Sonar", sonarStopEv = "Stop_Sonar";
    public GameObject sonarPlayer;
    private const string fuelAmountRTPC = "Fuel_Amount";
    public float fuelRefillTime;
    private Timer fuelRefillTimer;
    private bool refillingFuel = false;
    private bool alarmPlaying = false;

    private void Awake()
    {
        Obstacle.audioController = this;
        fuelRefillTimer = new Timer(fuelRefillTime);
    }

    void Update()
    {
        UpdateFuelRefill();
    }

    public void StopAllFX()
    {
        StopAlarm();
        StopFuelRefill();
        StopSonar();
        AkSoundEngine.StopAll();
        //StopMusic();
    }
    public void StartFuelRefill()
    {
        AkSoundEngine.PostEvent(fuelRefillEv, fuelRefillPlayer);
        refillingFuel = true;
        fuelRefillTimer.Start();
    }


    private void UpdateFuelRefill()
    {
        if (refillingFuel)
        {
            fuelRefillTimer.Update(Time.deltaTime);
            if (fuelRefillTimer.GetCurrentTimePercent() >= 1)
            {
                StopFuelRefill();
                refillingFuel = false;
            }
        }
    }

    public void StopFuelRefill()
    {
        AkSoundEngine.PostEvent(fuelRefillStopEv, fuelRefillPlayer);
    }

    public void SetFuelFX(float fuelPercent)
    {
        AkSoundEngine.SetRTPCValue(fuelAmountRTPC, (1 - fuelPercent) * 100);
    }

    public void ActivateSonar(float time)
    {
        AkSoundEngine.PostEvent(sonarStartEv, sonarPlayer);
    }
    public void StopSonar()
    {
        AkSoundEngine.PostEvent(sonarStopEv, sonarPlayer);

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
        if (!alarmPlaying)
        {
            alarmPlaying = true;
            AkSoundEngine.PostEvent(alarmStartEv, alarmPlayer);
        }
    }

    public void StopAlarm() {
        alarmPlaying = false;
        AkSoundEngine.PostEvent(alarmStopEv, alarmPlayer);
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