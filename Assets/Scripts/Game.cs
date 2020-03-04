using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    private float currentFuel;
    public float maxFuel;
    public float fuelBurnRate;
    private bool dead;
    public GameObject gameOverScreen;
    public Image fuelFillImage;
    public ScreenShake shake;
    private Repairable[] repairables;
    public GameObject smokePrefab;

    public Texture2D emptyTex;
    public Texture2D fullTex;

    public GUIStyle progress_empty, progress_full;

    public float guiOffset;
    public float healthWidth;
    public float healthHeight;


  
    public float fuelThresholdForDeathLowPass = 10;

    public GameObject explosionParticle;

    public InputHandler firstPlayerHandler;


    public AudioController audioController;

    void Start()
    {
        gameOverScreen.SetActive(false);
        currentFuel = maxFuel;
        repairables = FindObjectsOfType<Repairable>();
    }

    void Update()
    {
        currentFuel -= fuelBurnRate * Time.deltaTime;

        UpdateDeath();

        UpdateUI();

        UpdateRestart();
    }

    internal void BulletHit(Obstacle obstacle)
    {
        Instantiate(explosionParticle, obstacle.transform.position, Quaternion.identity);
         audioController.PlayHitClip();
    }

    void UpdateDeath()
    {
        if (currentFuel < fuelThresholdForDeathLowPass)
        {
            audioController.ActivateDeathLowPass();
        }
        if (currentFuel < 0 && !dead)
        {
            audioController.PlayDeath();
            dead = true;
            FindObjectsOfType<CharController>().ToList().ForEach(x => x.enabled = false);
            ShowGameOverScreen();
            audioController.ActivateDeathLowPass();
        }
    }

    void UpdateUI()
    {
        fuelFillImage.fillAmount = currentFuel / maxFuel;
    }

    public void FuelCollected(Fuel fuel)
    {
        audioController.PlayFuelRefill();
        currentFuel = maxFuel;
    }

    void ShowGameOverScreen()
    {
        gameOverScreen.SetActive(true);
    }

    public void RestartGame()
    {
        audioController.ResetMixer();
        int scene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    public void ObstacleHit(Obstacle obs)
    {
        shake.TriggerShake(0.5f);
        var rand = new System.Random();
        var dmgIndex = rand.Next(0, repairables.Length);
        var rp = repairables[dmgIndex];
        rp.TakeDamage(obs.Damage);
        audioController.PlayObstacleHit();
        audioController.PlayAlarm();
        Instantiate(explosionParticle, obs.transform.position, Quaternion.identity);
    }

    public void Repaired(Repairable r)
    {
        if (repairables.Any(rp => !rp.IsRepaired())) return;
        audioController.StopAlarm();
    }

    public float GetCurrentFuel()
    {
        return currentFuel;
    }

    public float GetFuelPercent()
    {
        return currentFuel / maxFuel;
    }

    public void UpdateRestart()
    {
        if (dead && (Input.GetButtonDown("Action0") || Input.GetButtonDown("Action1") || Input.GetButtonDown("Action2"))) {
            RestartGame();
        }

    }

    public void GoBackToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
