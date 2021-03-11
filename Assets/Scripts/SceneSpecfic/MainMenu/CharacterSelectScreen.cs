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
        public bool playerActive = false;
        public bool playerReady = false;

        public void CopyData(SelectScreenPlayerData ssPlayerData)
        {
            this.playerSessionData = ssPlayerData.playerSessionData;
            this.skinIndex = ssPlayerData.skinIndex;
            this.devotionNameIndex = ssPlayerData.devotionNameIndex;
            this.spiritNameIndex = ssPlayerData.spiritNameIndex;
            this.playerActive = ssPlayerData.playerActive;
            this.playerReady = ssPlayerData.playerReady;
        }
    }
    public class CharacterSelectScreen : MonoBehaviour
    {
        public MainMenu mainMenu;
        public InputManager inputManager;
        public CharacterSelectSinglePanel[] csPanels;
        public CharacterNames characterNames;
        public CharacterSkins characterSkins;
        public Audio.UIAudioController audioController;
        private SelectScreenPlayerData[] selectScreenPlayerDatas;
        private List<SelectScreenPlayerController> playerControllers;
        private bool initialized = false;
        private bool subscribed = false;
        private void Initialize()
        {
            inputManager = InputManager.Instance;
            if (!initialized)
            {
                initialized = true;
                playerControllers = new List<SelectScreenPlayerController>();
                for (int i = 0; i < inputManager.GetDeviceCount(); i++)
                {
                    CreateNewPossiblePlayer(inputManager.GetDevice(i), inputManager.GetUIInputManager(i));
                }
                for (int i = 0; i < csPanels.Length; i++)
                {
                    csPanels[i].Deactivate();
                }
                selectScreenPlayerDatas = new SelectScreenPlayerData[inputManager.maxNumberOfPlayers];
                if (enabled)
                {
                    InitScreen();
                    Subscribe();
                }

            }
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
            InitScreen();
            Subscribe();
        }


        private void OnDisable()
        {
            InputManager.OnInputInitialized -= Initialize;
            Unsubscribe();
        }
        private void InitScreen()
        {
            if (initialized)
            {
                for (int i = 0; i < InputManager.Instance.GetPlayerCount(); i++)
                {
                    var psd = InputManager.Instance.GetPlayerSessionData(i);
                    OnPlayerRegistered(psd);
                }
            }
        }
        private void Subscribe()
        {
            if (initialized && !subscribed)
            {
                SubscribeToInput();
                for (int i = 0; i < playerControllers.Count; i++)
                {
                    playerControllers[i].Subscribe();
                }
                subscribed = true;
            }
        }
        private void Unsubscribe()
        {
            if (initialized && subscribed)
            {
                UnsubscribeFromInput();
                for (int i = 0; i < playerControllers.Count; i++)
                {
                    playerControllers[i].Unsubscribe();
                }
                subscribed = false;
            }
        }
        private void SubscribeToInput()
        {
            inputManager.OnDeviceRegistered += OnDeviceRegistered;
            inputManager.OnPlayerRegistered += OnPlayerRegistered;
            inputManager.OnPlayerRemoved += OnPlayerRemoved;
            inputManager.OnPlayerChangedIndex += OnPlayerChangedIndex;
        }
        private void UnsubscribeFromInput()
        {
            inputManager.OnDeviceRegistered -= OnDeviceRegistered;
            inputManager.OnPlayerRegistered -= OnPlayerRegistered;
            inputManager.OnPlayerRemoved -= OnPlayerRemoved;
            inputManager.OnPlayerChangedIndex -= OnPlayerChangedIndex;
        }
        private void OnDeviceRegistered(UserDevice userDevice, PlayerGameplayInputManager gameplayInputManager, UIInputManager uiInputManager)
        {
            CreateNewPossiblePlayer(userDevice, uiInputManager);
        }
        private void OnPlayerRegistered(PlayerSessionData playerSessionData)
        {
            SelectScreenPlayerData ssPlayerData = new SelectScreenPlayerData();
            selectScreenPlayerDatas[playerSessionData.playerIndex] = ssPlayerData;
            ssPlayerData.playerSessionData = playerSessionData;
            ssPlayerData.playerActive = true;
            ssPlayerData.playerReady = false;
            AssignRandomNames(ssPlayerData);
            AssignRandomSkin(ssPlayerData);
            csPanels[playerSessionData.playerIndex].Activate(playerSessionData.playerIndex, playerSessionData.uiInputManager);
            audioController.PlayEnter();
        }
        private void OnPlayerRemoved(PlayerSessionData playerSessionData)
        {
            for (int i = 0; i < playerControllers.Count; i++)
            {
                if (playerControllers[i].playerIndex == playerSessionData.playerIndex)
                {
                    playerControllers[i].SetPlayerActive(false);
                    break;
                }
            }
            for (int i = 0; i < selectScreenPlayerDatas.Length; i++)
            {
                if (selectScreenPlayerDatas[i] != null)
                {
                    selectScreenPlayerDatas[i].playerReady = false;
                    csPanels[selectScreenPlayerDatas[i].playerSessionData.playerIndex].UpdatePanel(selectScreenPlayerDatas[i]);
                }
            }
            selectScreenPlayerDatas[playerSessionData.playerIndex].playerActive = false;
            selectScreenPlayerDatas[playerSessionData.playerIndex].playerReady = false;
            audioController.PlayExit();
            csPanels[playerSessionData.playerIndex].Deactivate();

            if (inputManager.GetPlayerCount() == 0)
            {
                mainMenu.ChangeScreenToRootMainMenu();
            }
        }
        private void OnPlayerChangedIndex(PlayerSessionData playerSessionData, int oldIndex)
        {
            selectScreenPlayerDatas[playerSessionData.playerIndex].CopyData(selectScreenPlayerDatas[oldIndex]);
            selectScreenPlayerDatas[oldIndex].playerReady = false;
            selectScreenPlayerDatas[oldIndex].playerActive = false;
            selectScreenPlayerDatas[playerSessionData.playerIndex].playerActive = true;
            for (int i = 0; i < playerControllers.Count; i++)
            {
                if (playerControllers[i].playerIndex == oldIndex)
                {
                    playerControllers[i].playerIndex = playerSessionData.playerIndex;
                }
            }

            csPanels[playerSessionData.playerIndex].UpdatePanel(selectScreenPlayerDatas[playerSessionData.playerIndex]);
            csPanels[playerSessionData.playerIndex].Activate(playerSessionData.playerIndex, playerSessionData.uiInputManager);
            csPanels[oldIndex].UpdatePanel(selectScreenPlayerDatas[oldIndex]);
            csPanels[oldIndex].Deactivate();

        }
        private void CreateNewPossiblePlayer(UserDevice userDevice, UIInputManager uiInputManager)
        {
            while (userDevice.deviceIndex >= playerControllers.Count)
            {
                playerControllers.Add(null);
            }
            userDevice.EnableUIInput(true);
            SelectScreenPlayerController playerSelectController = new SelectScreenPlayerController(userDevice.deviceIndex, uiInputManager, this);
            playerControllers[userDevice.deviceIndex] = playerSelectController;
            playerSelectController.Subscribe();
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
            currentIndex = MMath.SumAllowFlow(currentIndex, step, 0, characterSkins.GetSkinsCount() - 1);
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
            currentIndex = MMath.SumAllowFlow(currentIndex, step, 0, characterNames.GetDevotionNamesCount() - 1);
            playerData.devotionNameIndex = currentIndex;
            csPanels[playerIndex].UpdatePanel(playerData);

        }
        public void ChangeSpiritName(int playerIndex, int step)
        {
            SelectScreenPlayerData playerData = selectScreenPlayerDatas[playerIndex];
            int currentIndex = playerData.spiritNameIndex;
            currentIndex = MMath.SumAllowFlow(currentIndex, step, 0, characterNames.GetSpiritNamesCount() - 1);
            playerData.spiritNameIndex = currentIndex;
            csPanels[playerIndex].UpdatePanel(playerData);
        }
        #endregion
        #region Player Ready/Active states
        public void SetPlayerReady(int playerIndex, bool isReady)
        {
            SelectScreenPlayerData playerData = selectScreenPlayerDatas[playerIndex];
            if (playerData.playerReady != isReady)
            {
                playerData.playerReady = isReady;
                csPanels[playerIndex].UpdatePanel(playerData);
                if (isReady)
                {
                    CheckIfAllPlayersAreReady();
                }
            }
        }
        public void PlayerClickedCancel(int playerIndex)
        {
            SelectScreenPlayerData playerData = selectScreenPlayerDatas[playerIndex];
            if (playerData.playerReady)
            {
                playerData.playerReady = false;
                csPanels[playerIndex].UpdatePanel(playerData);
            }
            else
            {
                inputManager.RemovePlayer(playerIndex);
            }
        }
        private void CheckIfAllPlayersAreReady()
        {
            bool playersReady = true;
            for (int i = 0; i < selectScreenPlayerDatas.Length; i++)
            {
                var sspData = selectScreenPlayerDatas[i];
                if (sspData != null && sspData.playerActive && !sspData.playerReady)
                {
                    playersReady = false;
                    break;
                }
            }
            if (playersReady)
            {
                //Save playerData?
                RecordPlayerSessionData();
                mainMenu.LoadArcadeLevel();
            }
        }
        private void RecordPlayerSessionData()
        {
            for (int i = 0; i < selectScreenPlayerDatas.Length; i++)
            {
                if (selectScreenPlayerDatas[i] != null && selectScreenPlayerDatas[i].playerActive)
                {
                    var sspData = selectScreenPlayerDatas[i];
                    sspData.playerSessionData.skin = characterSkins.GetSkinData(sspData.skinIndex);
                    sspData.playerSessionData.devotionName = characterNames.GetDevotionName(sspData.devotionNameIndex);
                    sspData.playerSessionData.spiritName = characterNames.GetSpiritName(sspData.spiritNameIndex);
                }
            }
        }
        #endregion
    }
}

