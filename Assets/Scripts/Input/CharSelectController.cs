using UnityEngine;

namespace NBLD.Input
{
    public class CharSelectController : ICharInputListener
    {
        public int deviceIndex;
        public InputManager inputManager;
        private bool playerActive = false;
        private PlayerSessionData playerSessionData;
        public CharSelectController(int index, InputManager inputManager)
        {
            this.deviceIndex = index;
            this.inputManager = inputManager;
            this.playerActive = false;
        }
        #region Inactive Player
        #endregion
        #region Active Player
        private void TryRegisteringPlayer()
        {
            bool success = inputManager.RegisterPlayer(deviceIndex);
            if (success)
            {
                playerSessionData = inputManager.GetPlayerByDevice(deviceIndex);
                playerActive = true;
            }
        }
        #endregion
        #region Input Events
        public void OnMovement(Vector2 movement)
        {
        }
        public void OnUp()
        {
        }
        public void OnDown()
        {
        }
        public void OnAction()
        {
            if (!playerActive)
            {
                TryRegisteringPlayer();
            }
        }
        public void OnSubAction()
        {
        }

        public void OnTalk()
        {
        }
        public void OnMoveAssistStarted()
        {
        }
        public void OnMoveAssistPerformed()
        {
        }
        #endregion
    }
}