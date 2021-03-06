using NBLD.MainMenu;
using UnityEngine;

namespace NBLD.Input
{
    public class SelectScreenPlayerController
    {
        public int playerIndex = -1;
        public CharacterSelectScreen characterSelectScreen;
        private PlayerUIInputManager playerUIInputManager;
        private bool playerActive = false;
        private bool subscribed = false;
        public void SetPlayerActive(bool active)
        {
            playerActive = active;
            if (!playerActive)
            {
                playerIndex = -1;
            }
        }

        #region Initialization
        public SelectScreenPlayerController(int index, PlayerUIInputManager playerUIInputManager, CharacterSelectScreen characterSelectScreen)
        {
            this.characterSelectScreen = characterSelectScreen;
            this.playerUIInputManager = playerUIInputManager;
            this.playerActive = false;
            //Subscribe();
        }
        ~SelectScreenPlayerController()
        {
            Unsubscribe();
        }
        public void Subscribe()
        {
            if (!subscribed)
            {
                subscribed = true;
                playerUIInputManager.OnSubmit += OnSubmit;
            }
        }
        public void Unsubscribe()
        {
            if (subscribed)
            {
                subscribed = false;
                playerUIInputManager.OnSubmit -= OnSubmit;
            }
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
                var playerSessionData = characterSelectScreen.GetPlayerByDevice(playerUIInputManager.deviceIndex);
                playerIndex = playerSessionData.playerIndex;
                playerActive = true;
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