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
    }

    void UpdateDeath()
    {
        if (currentFuel < fuelThresholdForDeathLowPass)
        {
            audioController.ActivateDeathLowPass();
        }
        if (currentFuel < 0 && !dead)
        {
            dead = true;
            FindObjectsOfType<InputController>().ToList().ForEach(x => x.enabled = false);
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
    }

    public void Repaired(Repairable r)
    {
        if (repairables.Any(rp => !rp.IsRepaired())) return;
        audioController.StopAlarm();
    }

    void OnGUI() {
        // Repairables 

        foreach (var r in repairables)
        {

            if (r.IsRepaired()) continue;
            //var width = 60;

            var pos = Camera.main.WorldToScreenPoint(r.transform.position);
            var gPos = GUIUtility.ScreenToGUIPoint(pos);
            gPos = new Vector2(gPos.x - healthWidth * 0.5f, Screen.height - gPos.y + guiOffset);
            //Debug.Log($"repairable {gPos} {r.RepairedAmount}");
            OnGUIHealth(gPos, new Vector2(healthWidth, healthHeight), r.RepairedAmount);
        }
    }

    void OnGUIHealth(Vector2 pos, Vector2 size, float amount)
    {
        //draw the background:
        GUI.BeginGroup(new Rect(pos.x, pos.y, size.x, size.y));
        GUI.Box(new Rect(0, 0, size.x, size.y), emptyTex, progress_empty);

        //draw the filled-in part:
        GUI.BeginGroup(new Rect(0, 0, size.x * amount, size.y));
        GUI.Box(new Rect(0, 0, size.x, size.y), fullTex, progress_full);
        GUI.EndGroup();
        GUI.EndGroup();
    }

    public float GetCurrentFuel()
    {
        return currentFuel;
    }

    public float GetFuelPercent()
    {
        return currentFuel / maxFuel;
    }
}
