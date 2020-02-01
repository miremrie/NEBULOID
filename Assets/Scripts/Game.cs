using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{

    private float currentFuel;
    public float maxFuel;
    public float fuelBurnRate;
    private bool dead;
    public GameObject gameOverScreen;

    void Start()
    {
        gameOverScreen.SetActive(false);
        currentFuel = maxFuel;
    }

    void Update()
    {
        currentFuel -= fuelBurnRate * Time.deltaTime;

        if (currentFuel < 0 && !dead) {
            dead = true;
            ShowGameOverScreen();
        }
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

    }
}
