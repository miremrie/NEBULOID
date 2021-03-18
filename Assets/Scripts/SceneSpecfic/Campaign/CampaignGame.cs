using NBLD.Input;
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
    private NBLD.Input.UIInputManager uiInput;
    public bool enableOnDemandGarage = false;
    private bool initialized = false;
    private bool subscribed = false;
    // Start is called before the first frame update
    private void Initialize()
    {
        if (!initialized)
        {
            uiInput = InputManager.Instance.generalUIInputManager;
            initialized = true;
            Subscribe();
        }
    }
    void Start()
    {
        ChangeState(GameState.Gameplay);
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

            uiInput.OnChangeSelect += UpdateGameState;
            //Garage
            shipCreator.onCreationStageChanged += OnGarageCreationStageChanged;
            subscribed = true;
        }

    }
    private void Unsubscribe()
    {
        if (initialized && subscribed)
        {
            uiInput.OnChangeSelect -= UpdateGameState;
            subscribed = true;
        }
    }

    public void EnterGarage()
    {
        ChangeState(GameState.Garage);
    }

    private void UpdateGameState()
    {
        if (enableOnDemandGarage && currentState == GameState.Gameplay)
        {
            ChangeState(GameState.Garage);
        }
        /*else
        {
            ChangeState(GameState.Gameplay);
        }*/
    }
    private void OnGarageCreationStageChanged(CreationStage creationStage, CreationStage prevStage)
    {
        if (creationStage == CreationStage.Canceled || creationStage == CreationStage.Confirmed)
        {
            ChangeState(GameState.Gameplay);
        }
    }
    private void ChangeState(GameState newState)
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
