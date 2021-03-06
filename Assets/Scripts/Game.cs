﻿using NBLD.Character;
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
    private void Awake()
    {
        uiInput = new NBLD.Input.UIInputManager();
        shipStatus.Initialize(this);
    }
    void Start()
    {
        gameOverScreen.SetActive(false);
    }
    private void OnEnable()
    {
        Subscribe();
    }
    private void OnDisable()
    {
        Unsubscribe();
    }
    private void Subscribe()
    {
        uiInput.Enable();
        uiInput.onSubmit += OnSubmit;
        uiInput.onEscape += OnEscape;
    }
    private void Unsubscribe()
    {
        uiInput.Disable();
        uiInput.onSubmit -= OnSubmit;
        uiInput.onEscape -= OnEscape;

    }
    void Update()
    {
        uiInput.Update(Time.deltaTime);
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
