using NBLD.ShipCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Gameplay, Garage
}
public class CampaignGame : MonoBehaviour
{
    public GameState currentState;

    public ShipCreator shipCreator;
    public List<GameObject> gameplayOnlyObjects = new List<GameObject>();
    public List<GameObject> garageOnlyObjects = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        ChangeState(GameState.Gameplay);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGameState();
    }


    public void UpdateGameState()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (currentState == GameState.Gameplay)
            {
                ChangeState(GameState.Garage);
            }
            else
            {
                ChangeState(GameState.Gameplay);
            }
        }
    }

    public void ChangeState(GameState newState)
    {
        GameState oldState = currentState;
        currentState = newState;
        bool gameplayActive = newState == GameState.Gameplay;
        bool garageActive = newState == GameState.Garage;

        for (int i = 0; i < gameplayOnlyObjects.Count; i++)
        {
            gameplayOnlyObjects[i].SetActive(gameplayActive);
        }
        for (int i = 0; i < garageOnlyObjects.Count; i++)
        {
            garageOnlyObjects[i].SetActive(garageActive);
        }

        if (garageActive)
        {
            shipCreator.LoadData();
        }
    }
}
