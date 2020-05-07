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

    // Start is called before the first frame update
    private void Awake()
    {
        uiInput = new NBLD.Input.UIInputManager();
    }
    void Start()
    {
        ChangeState(GameState.Gameplay);
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
        uiInput.onChangeSelect += UpdateGameState;
        //Garage
        shipCreator.onCreationStageChanged += OnGarageCreationStageChanged;
    }
    private void Unsubscribe()
    {
        uiInput.Disable();
        uiInput.onChangeSelect -= UpdateGameState;
    }

    private void Update()
    {
        uiInput.Update(Time.deltaTime);
    }

    public void UpdateGameState()
    {
        if (currentState == GameState.Gameplay)
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
