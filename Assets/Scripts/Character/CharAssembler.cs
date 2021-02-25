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
        private bool initialized = false;
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
            CreateCharacters();
            InputManager.Instance.OnInputInitialized -= Initialize;
        }

        private void CreateCharacters()
        {
            int playerCount = InputManager.Instance.GetPlayerCount();
            CamSet gameplaySet = mainCameraController.FindSet(gameplayCamSetName);
            CamSet charactersSet = mainCameraController.FindSet(charactersCamSetName);
            for (int i = 0; i < playerCount; i++)
            {
                PlayerSessionData playerSessionData = InputManager.Instance.GetPlayerSessionData(i);
                CharController newChar = GameObject.Instantiate(characterPrefab, spawnLocations[i % spawnLocations.Count]);
                newChar.ship = shipMovement;
                newChar.outsideBehaviour.shipEjectSystem = shipEjectSystem;
                newChar.outsideBehaviour.minesRoot = minesRoot;
                newChar.transform.parent = charactersRoot;
                newChar.Initialize(playerSessionData, spawnFloors[i % spawnFloors.Count]);
                //Cameras
                contextCams[i].SetFollowTarget(newChar.transform);
                contextCams[i].Activate();
                CamZone charGameplayCamZone = gameplayCamZone.Copy();
                charGameplayCamZone.trans = newChar.transform;
                gameplaySet.camZones.Add(charGameplayCamZone);
                CamZone charCharactersCamZone = characterCamZone.Copy();
                charCharactersCamZone.trans = newChar.transform;
                charactersSet.camZones.Add(charCharactersCamZone);
            }
        }
    }
}