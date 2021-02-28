using System.Collections;
using System.Collections.Generic;
using DynamicCamera;
using NBLD.Cameras;
using NBLD.Data;
using NBLD.Input;
using NBLD.Ship;
using UnityEngine;

namespace NBLD.Character
{
    public class CharAssembler : MonoBehaviour
    {
        public CharacterSkins characterSkins;
        public CharController characterPrefab;
        public List<Transform> spawnLocations;
        public List<int> spawnFloors;
        public List<ContextualCamera> contextCams = new List<ContextualCamera>();
        public bool allowRecordedInput = false;
        public List<RecordedInputPlayer> recordedInputPlayers;
        public CameraController mainCameraController;
        public CamZone gameplayCamZone;
        public CamZone characterCamZone;
        public string gameplayCamSetName = "gameplay";
        public string charactersCamSetName = "characters";
        public Transform charactersRoot;
        [Header("Controller Assignables")]
        public ShipMovement shipMovement;
        [Header("Outside Behaviour Assignables")]
        public ShipEjectSystem shipEjectSystem;
        public Transform minesRoot;
        private List<CharController> playerCharacters = new List<CharController>();
        private CamSet gameplaySet;
        private CamSet charactersSet;
        private bool initialized = false;
        private bool subscribed = false;

        private void Awake()
        {
            if (InputManager.Instance.Initialized)
            {
                Initialize();
            }
            else
            {
                InputManager.Instance.OnInputInitialized += Initialize;
            }
        }
        private void Initialize()
        {
            initialized = true;
            CreateInitialCharacters();
            InputManager.Instance.OnInputInitialized -= Initialize;
            Subscribe();
        }

        private void Subscribe()
        {
            if (initialized && !subscribed)
            {
                subscribed = true;
                //InputManager.Instance.OnPlayerRegistered += CreateCharacter;
            }
        }
        private void Unsubscribe()
        {
            if (initialized && subscribed)
            {
                subscribed = false;
                //InputManager.Instance.OnPlayerRegistered -= CreateCharacter;
            }
        }
        private void OnEnable()
        {
            Subscribe();
        }
        private void OnDisable()
        {
            Unsubscribe();
        }

        private void CreateInitialCharacters()
        {
            int playerCount = InputManager.Instance.GetPlayerCount();
            gameplaySet = mainCameraController.FindSet(gameplayCamSetName);
            charactersSet = mainCameraController.FindSet(charactersCamSetName);
            playerCharacters = new List<CharController>();
            for (int i = 0; i < playerCount; i++)
            {
                CreateCharacter(InputManager.Instance.GetPlayerSessionData(i));
            }
        }

        public void CreateCharacter(PlayerSessionData psData)
        {

            CharController newChar = GameObject.Instantiate(characterPrefab, spawnLocations[psData.playerIndex % spawnLocations.Count]);
            newChar.ship = shipMovement;
            newChar.outsideBehaviour.shipEjectSystem = shipEjectSystem;
            newChar.outsideBehaviour.minesRoot = minesRoot;
            newChar.transform.parent = charactersRoot;
            GameplayInputEvents giEvents;
            if (allowRecordedInput && recordedInputPlayers.Count > psData.playerIndex && recordedInputPlayers[psData.playerIndex] != null)
            {
                giEvents = recordedInputPlayers[psData.playerIndex].inputEvents;
            }
            else
            {
                giEvents = psData.gameplayInputManager;
            }
            newChar.Initialize(psData, spawnFloors[psData.playerIndex % spawnFloors.Count], giEvents);
            //Cameras
            contextCams[psData.playerIndex].SetFollowTarget(newChar.transform);
            contextCams[psData.playerIndex].Activate();
            CamZone charGameplayCamZone = gameplayCamZone.Copy();
            charGameplayCamZone.trans = newChar.transform;
            gameplaySet.camZones.Add(charGameplayCamZone);
            CamZone charCharactersCamZone = characterCamZone.Copy();
            charCharactersCamZone.trans = newChar.transform;
            charactersSet.camZones.Add(charCharactersCamZone);

            playerCharacters.Add(newChar);
        }
    }
}