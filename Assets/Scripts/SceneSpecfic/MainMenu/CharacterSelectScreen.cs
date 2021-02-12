using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NBLD.Input;

namespace NBLD.MainMenu
{
    public class CharacterSelectScreen : MonoBehaviour
    {
        public InputManager inputManager;
        public CharacterSelectSinglePanel[] csPanels;
        private List<SelectScreenPlayerController> playerControllers;
        private bool initialized = false;
        private bool subscribed = false;
        private void Awake()
        {
            if (inputManager.Initialized)
            {
                Initialize();
            }
        }
        private void Initialize()
        {
            if (!initialized)
            {
                initialized = true;
                playerControllers = new List<SelectScreenPlayerController>();
                for (int i = 0; i < inputManager.GetDeviceCount(); i++)
                {
                    CreateNewController(inputManager.GetDevice(i), inputManager.GetUIInputManager(i));
                }
                SubscribeToInput();
                for (int i = 0; i < csPanels.Length; i++)
                {
                    csPanels[i].Deactivate();
                }
            }
        }
        private void OnEnable()
        {
            inputManager.OnInputInitialized += Initialize;
            if (initialized && !subscribed)
            {
                SubscribeToInput();
            }

        }
        private void OnDisable()
        {
            inputManager.OnInputInitialized -= Initialize;
            if (initialized && subscribed)
            {
                UnsubscribeFromInput();
            }
        }
        private void SubscribeToInput()
        {
            inputManager.OnDeviceRegistered += OnDeviceRegistered;
        }
        private void UnsubscribeFromInput()
        {
            inputManager.OnDeviceRegistered -= OnDeviceRegistered;
        }
        private void OnDeviceRegistered(UserDevice userDevice, PlayerGameplayInputManager gameplayInputManager, PlayerUIInputManager uiInputManager)
        {
            CreateNewController(userDevice, uiInputManager);
        }
        private void CreateNewController(UserDevice userDevice, PlayerUIInputManager uiInputManager)
        {
            while (userDevice.deviceIndex >= playerControllers.Count)
            {
                playerControllers.Add(null);
            }
            userDevice.EnableUIInput(true);
            SelectScreenPlayerController playerSelectController = new SelectScreenPlayerController(userDevice.deviceIndex, uiInputManager, this);
            playerControllers[userDevice.deviceIndex] = playerSelectController;
        }

        public bool RegisterPlayer(int deviceIndex)
        {
            return inputManager.RegisterPlayer(deviceIndex);
        }
        public PlayerSessionData GetPlayerByDevice(int deviceIndex)
        {
            return inputManager.GetPlayerByDevice(deviceIndex);
        }

        public CharacterSelectSinglePanel GetPanelForPlayer(int playerIndex)
        {
            return csPanels[playerIndex];
        }
    }
}

