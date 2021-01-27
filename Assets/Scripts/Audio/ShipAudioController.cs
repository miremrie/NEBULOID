using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ShipAudioController : MonoBehaviour
{
    //Arm
    private const string leftArmEv = "Play_Arm_L", rightArmEv = "Play_Arm_R";
    public GameObject leftArmPlayer, rightArmPlayer;
    //Alarm
    private const string alarmStartEv = "Play_Alarm", alarmStopEv = "Stop_Alarm";
    public GameObject alarmPlayer;
    //Gun, bullet, expolosions
    private const string gunshotEv = "Play_Gun_Shot", bulletHitEv = "Play_Bullet_Hit", shipHitEv = "Play_Ship_Hit";
    public GameObject gunShotPlayer, bulletHitPlayer, shipHitPlayer;

    //Sonar
    private const string sonarStartEv = "Play_Sonar", sonarStopEv = "Stop_Sonar";
    //public GameObject sonarPlayer;

    //Hook
    public GameObject hookCablePlayer;
    private const string hookCableFwdEv = "Play_Hook_Cable_Grab", hookCableFwdStopEv = "Stop_Hook_Cable";
    private const string hookCableBackEv = "Play_Hook_Cable_Back", hookCableBackStopEv = "Stop_Hook_Cable_Back";
    private const string hookSpeedPitchRTPC = "Speed_Pitch";
    public GameObject hookClawPlayer;
    private const string hookClawOpenEv = "Play_Hook_ClawOpen", hookClawOpenAndLockEv = "Play_Hook_ClawLock";
    public GameObject hookClawBitePlayer;
    private const string hookClawBitePrepEv = "Play_Hook_ClawBitePrep", hookClawBiteEv = "Play_Hook_ClawBite";
    //Shield
    public GameObject shieldOpenExtrudePlayer;
    private const string shieldOpenExtrudeEv = "Play_Shield_Open_Extrude";
    public GameObject shieldOpenIntrudeFanOutPlayer;
    private const string shieldOpenIntrudeFanOutEv = "Play_Shield_Open_IntrudeFanOut";
    public GameObject shieldCloseExtrudeFanInPlayer;
    private const string shieldCloseExtrudeFanInEv = "Play_Shield_Close_ExtrudeFanIn";
    public GameObject shieldCloseIntrudePlayer;
    private const string shieldCloseIntrudeEv = "Play_Shield_Close_Intrude";

    //Fuel
    private const string fuelRefillEv = "Play_Fuel_Refill", fuelRefillStopEv = "Stop_Fuel_Refill";
    private const string fuelAmountRTPC = "Fuel_Amount";
    public GameObject fuelRefillPlayer;
    //Gameover
    private const string gameOverEv = "Play_Game_Over";
    public GameObject gameOverPlayer;

    public float fuelRefillTime;
    private Timer fuelRefillTimer;
    private bool refillingFuel = false;
    private bool alarmPlaying = false;
    private float minSpeedPitchValue = 0, maxSpeedPitchValue = 50;

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
        //StopAlarm();
        //StopFuelRefill();
        //StopSonar(audioEmitter);
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

    public void ActivateSonar(float time, GameObject sonarEmitter)
    {
        AkSoundEngine.PostEvent(sonarStartEv, sonarEmitter);
    }
    public void StopSonar(GameObject sonarEmitter)
    {
        AkSoundEngine.PostEvent(sonarStopEv, sonarEmitter);

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

    public void StopAlarm()
    {
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

    public void PlayCableForward()
    {
        AkSoundEngine.PostEvent(hookCableFwdEv, hookCablePlayer);
    }
    public void StopCableForward()
    {
        AkSoundEngine.PostEvent(hookCableFwdStopEv, hookCablePlayer);
    }
    public void PlayCableBack()
    {
        AkSoundEngine.PostEvent(hookCableBackEv, hookCablePlayer);
    }
    public void StopCableBack()
    {
        AkSoundEngine.PostEvent(hookCableBackStopEv, hookCablePlayer);
    }
    public void UpdateHookSpeedPitch(float speed, float maxSpeed, float minSpeed = 0)
    {
        float t = (speed - minSpeed) / (maxSpeed - minSpeed);
        float pitchValue = Mathf.Lerp(minSpeedPitchValue, maxSpeedPitchValue, t);
        AkSoundEngine.SetRTPCValue(hookSpeedPitchRTPC, pitchValue);
    }

    public void PlayHookOpen()
    {
        AkSoundEngine.PostEvent(hookClawOpenEv, hookClawPlayer);
    }
    public void PlayHookOpenAndLock()
    {
        AkSoundEngine.PostEvent(hookClawOpenAndLockEv, hookClawPlayer);
    }
    public void PlayHookBitePrep()
    {
        AkSoundEngine.PostEvent(hookClawBitePrepEv, hookClawBitePlayer);
    }
    public void PlayHookBite()
    {
        AkSoundEngine.PostEvent(hookClawBiteEv, hookClawBitePlayer);
    }

    //Shield
    public void PlayShieldOpenExtrude()
    {
        AkSoundEngine.PostEvent(shieldOpenExtrudeEv, shieldOpenExtrudePlayer);

    }
    public void PlayShieldOpenIntrudeFanOut()
    {
        AkSoundEngine.PostEvent(shieldOpenIntrudeFanOutEv, shieldOpenIntrudeFanOutPlayer);
    }
    public void PlayShieldCloseExtrudeFanIn()
    {
        AkSoundEngine.PostEvent(shieldCloseExtrudeFanInEv, shieldCloseExtrudeFanInPlayer);
    }
    public void PlayShieldCloseIntrude()
    {
        AkSoundEngine.PostEvent(shieldCloseIntrudeEv, shieldCloseIntrudePlayer);
    }
}