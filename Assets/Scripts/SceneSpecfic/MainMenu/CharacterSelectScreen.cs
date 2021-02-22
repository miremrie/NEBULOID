using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NBLD.Input;
using NBLD.Data;

namespace NBLD.MainMenu
{
    public class SelectScreenPlayerData
    {
        public PlayerSessionData playerSessionData;
        public int skinIndex;
        public int devotionNameIndex;
        public int spiritNameIndex;
        public bool confirmedName;

    }
    public class CharacterSelectScreen : MonoBehaviour
    {

        public InputManager inputManager;
        public CharacterSelectSinglePanel[] csPanels;
        public CharacterNames characterNames;
        public CharacterSkins characterSkins;
        private SelectScreenPlayerData[] selectScreenPlayerDatas;
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
                    CreateNewPlayer(inputManager.GetDevice(i), inputManager.GetUIInputManager(i));
                }
                SubscribeToInput();
                for (int i = 0; i < csPanels.Length; i++)
                {
                    csPanels[i].Deactivate();
                }
                selectScreenPlayerDatas = new SelectScreenPlayerData[inputManager.maxNumberOfPlayers];
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
            inputManager.OnPlayerRegistered += OnPlayerRegistered;
        }
        private void UnsubscribeFromInput()
        {
            inputManager.OnDeviceRegistered -= OnDeviceRegistered;
            inputManager.OnPlayerRegistered -= OnPlayerRegistered;
        }
        private void OnDeviceRegistered(UserDevice userDevice, PlayerGameplayInputManager gameplayInputManager, PlayerUIInputManager uiInputManager)
        {
            CreateNewPlayer(userDevice, uiInputManager);
        }
        private void OnPlayerRegistered(PlayerSessionData playerSessionData)
        {
            SelectScreenPlayerData ssPlayerData = new SelectScreenPlayerData();
            selectScreenPlayerDatas[playerSessionData.playerIndex] = ssPlayerData;
            ssPlayerData.playerSessionData = playerSessionData;
            AssignRandomNames(ssPlayerData);
            AssignRandomSkin(ssPlayerData);
        }
        private void CreateNewPlayer(UserDevice userDevice, PlayerUIInputManager uiInputManager)
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
        #region Character Skins
        public void ChangeCharacterSkin(int playerIndex, int step)
        {
            SelectScreenPlayerData playerData = selectScreenPlayerDatas[playerIndex];
            int currentIndex = playerData.skinIndex;
            currentIndex += step;
            if (currentIndex >= characterSkins.GetSkinsCount())
            {
                currentIndex = 0;
            }
            else if (currentIndex < 0)
            {
                currentIndex = characterSkins.GetSkinsCount() - 1;
            }
            playerData.skinIndex = currentIndex;
            csPanels[playerData.playerSessionData.playerIndex].UpdatePanel(playerData);
        }
        #endregion
        #region Character Names
        private void AssignRandomNames(SelectScreenPlayerData playerData)
        {
            int devotionIndex = Random.Range(0, characterNames.GetDevotionNamesCount());
            int spiritIndex = Random.Range(0, characterNames.GetSpiritNamesCount());
            playerData.devotionNameIndex = devotionIndex;
            playerData.spiritNameIndex = spiritIndex;
            //Debug.Log($"indices {devotionIndex},{spiritIndex}");
            csPanels[playerData.playerSessionData.playerIndex].UpdatePanel(playerData);
        }
        private void AssignRandomSkin(SelectScreenPlayerData playerData)
        {
            int skinIndex = Random.Range(0, characterSkins.GetSkinsCount());
            playerData.skinIndex = skinIndex;
            csPanels[playerData.playerSessionData.playerIndex].UpdatePanel(playerData);
        }
        public void ChangeDevotionName(int playerIndex, int step)
        {
            SelectScreenPlayerData playerData = selectScreenPlayerDatas[playerIndex];
            int currentIndex = playerData.devotionNameIndex;
            currentIndex += step;
            if (currentIndex >= characterNames.GetDevotionNamesCount())
            {
                currentIndex = 0;

            }
            else if (currentIndex < 0)
            {
                currentIndex = characterNames.GetDevotionNamesCount() - 1;
            }
            playerData.devotionNameIndex = currentIndex;
            csPanels[playerIndex].UpdatePanel(playerData);

        }
        public void ChangeSpiritName(int playerIndex, int step)
        {
            SelectScreenPlayerData playerData = selectScreenPlayerDatas[playerIndex];
            int currentIndex = playerData.spiritNameIndex;
            currentIndex += step;
            if (currentIndex >= characterNames.GetSpiritNamesCount())
            {
                currentIndex = 0;

            }
            else if (currentIndex < 0)
            {
                currentIndex = characterNames.GetSpiritNamesCount() - 1;
            }
            playerData.spiritNameIndex = currentIndex;
            csPanels[playerIndex].UpdatePanel(playerData);
        }
        #endregion
    }
}

