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


    void Start()
    {
        gameOverScreen.SetActive(false);
        currentFuel = maxFuel;
    }

    void Update()
    {
        currentFuel -= fuelBurnRate * Time.deltaTime;

        UpdateDeath();

        UpdateUI();
    }

    void UpdateDeath()
    {
        if (currentFuel < 0 && !dead)
        {
            dead = true;
            FindObjectsOfType<InputController>().ToList().ForEach(x => x.enabled = false);
            ShowGameOverScreen();
        }
    }

    void UpdateUI()
    {
        fuelFillImage.fillAmount = currentFuel / maxFuel;
    }

    public void FuelCollected(Fuel fuel) {
        currentFuel = maxFuel;
    }

    void ShowGameOverScreen()
    {
        gameOverScreen.SetActive(true);
    }

    public void RestartGame() {
        int scene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    public void ObstacleHit(Obstacle obs)
    {
        shake.TriggerShake(0.5f);
    }
    
}
