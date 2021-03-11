using NBLD.Character;
using NBLD.Input;
using NBLD.Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public ShipStatus shipStatus;
    private bool gameIsOver;
    public GameObject gameOverScreen;
    public GameObject smokePrefab;
    private NBLD.Input.UIInputManager uiInput;

    public GameObject explosionParticle;

    public ShipAudioController audioController;
    private bool initialized = false;
    private bool subscribed = false;
    private void Initialize()
    {
        if (!initialized)
        {
            uiInput = InputManager.Instance.generalUIInputManager;
            shipStatus.Initialize(this);
            initialized = true;
            Subscribe();
        }
    }
    void Start()
    {
        gameOverScreen.SetActive(false);
    }
    private void OnEnable()
    {
        if (InputManager.Instance != null && InputManager.Instance.Initialized)
        {
            Initialize();
        }
        else
        {
            InputManager.OnInputInitialized += Initialize;
        }
        Subscribe();
    }
    private void OnDisable()
    {
        Unsubscribe();
    }
    private void Subscribe()
    {
        if (initialized && !subscribed)
        {
            uiInput.OnSubmit += OnSubmit;
            uiInput.OnEscape += OnEscape;
            subscribed = true;
        }
    }
    private void Unsubscribe()
    {
        if (initialized && subscribed)
        {
            uiInput.OnSubmit -= OnSubmit;
            uiInput.OnEscape -= OnEscape;
            subscribed = false;
        }
    }

    internal void BulletHit(Obstacle obstacle)
    {
        Instantiate(explosionParticle, obstacle.transform.position, Quaternion.identity);
        audioController.PlayHitClip();
    }
    public void GameOver()
    {
        if (!gameIsOver)
        {
            audioController.PlayGameOver();
            gameIsOver = true;
            FindObjectsOfType<CharController>().ToList().ForEach(x => x.enabled = false);
            ShowGameOverScreen();
        }
    }
    public bool IsGameOver() => gameIsOver;

    void ShowGameOverScreen()
    {
        gameOverScreen.SetActive(true);
    }

    public void RestartGame()
    {
        audioController.StopAllFX();
        int scene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    public void OnSubmit()
    {
        if (gameIsOver)
        {
            RestartGame();
        }

    }

    public void OnEscape()
    {
        GoBackToMenu();
    }

    public void GoBackToMenu()
    {
        AkSoundEngine.StopAll();
        SceneManager.LoadScene(0);
    }

}
