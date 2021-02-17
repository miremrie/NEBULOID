using NBLD.MainMenu;
using UnityEngine;

namespace NBLD.Input
{
    public class SelectScreenPlayerController
    {
        public int playerIndex = -1;
        public CharacterSelectScreen characterSelectScreen;
        public CharacterSelectSinglePanel csPanel;
        private PlayerUIInputManager playerUIInputManager;
        private bool playerActive = false;
        private PlayerSessionData playerSessionData;

        #region Initialization
        public SelectScreenPlayerController(int index, PlayerUIInputManager playerUIInputManager, CharacterSelectScreen characterSelectScreen)
        {
            this.characterSelectScreen = characterSelectScreen;
            this.playerUIInputManager = playerUIInputManager;
            this.playerActive = false;
            Subscribe();
        }
        ~SelectScreenPlayerController()
        {
            Unsubscribe();
        }
        private void Subscribe()
        {
            playerUIInputManager.OnSubmit += OnSubmit;
        }
        private void Unsubscribe()
        {
            playerUIInputManager.OnSubmit -= OnSubmit;
        }
        #endregion
        #region Inactive Player
        #endregion
        #region Active Player
        private void TryRegisteringPlayer()
        {
            bool success = characterSelectScreen.RegisterPlayer(playerUIInputManager.deviceIndex);
            if (success)
            {
                playerSessionData = characterSelectScreen.GetPlayerByDevice(playerUIInputManager.deviceIndex);
                playerActive = true;
                playerIndex = playerSessionData.playerIndex;
                csPanel = characterSelectScreen.GetPanelForPlayer(playerIndex);
                csPanel.Activate(playerIndex, playerUIInputManager);
            }
        }
        #endregion
        #region Input Events
        public void OnSubmit()
        {
            PrintInput("OnSubmit");
            if (!playerActive)
            {
                TryRegisteringPlayer();
            }
        }
        #endregion
        #region Debug Help
        private void PrintInput(string methodName, string additionalParam = "")
        {
            string playerName = (playerActive) ? (playerIndex.ToString()) : "Inactive";
            Debug.Log($"Device {playerUIInputManager.deviceIndex} Called {methodName} - Player {playerName} with Param: ({additionalParam})");
        }
        #endregion
    }
}