using System.Collections;
using System.Collections.Generic;
using NBLD.Data;
using NBLD.Input;
using NBLD.UI;
using UnityEngine;
using UnityEngine.UI;

namespace NBLD.MainMenu
{
    public class CharacterSelectSinglePanel : MonoBehaviour
    {
        public int playerIndex;
        public CharacterSelectScreen characterSelectScreen;
        public GameObject inactivePanel;
        public GameObject activePanel;
        public BaseUIComponent[] activeUIElements;
        public SingleOptionSelectImage skinOptionSelect;
        public SingleOptionSelectText devotionNameOptionSelect, spiritNameOptionSelect;
        public int currentlySelectedElement = 0;
        public GameObject playerReadyObject;
        private PlayerUIInputManager uiInputManager;
        private bool subscribedToInput = false;
        private bool subscribedToUI = false;

        public void Activate(int playerIndex, PlayerUIInputManager uiInputManager)
        {
            this.playerIndex = playerIndex;
            this.uiInputManager = uiInputManager;
            inactivePanel.SetActive(false);
            activePanel.SetActive(true);
            SubscribeToUI();
            SubscribeToInput();
            for (int i = 0; i < activeUIElements.Length; i++)
            {
                activeUIElements[i].LoseFocus();
            }
            activeUIElements[currentlySelectedElement].Focus(uiInputManager);
        }

        public void Deactivate()
        {
            inactivePanel.SetActive(true);
            activePanel.SetActive(false);
            UnsubscribeFromUI();
            UnsubscribeFromInput();
        }
        private void OnDisable()
        {
            UnsubscribeFromInput();
            UnsubscribeFromUI();
        }
        private void SubscribeToInput()
        {
            if (!subscribedToInput)
            {
                uiInputManager.OnNavigationIntChanged += OnNavigationIntChanged;
                uiInputManager.OnSubmit += OnSubmit;
                uiInputManager.OnCancel += OnCancel;
                subscribedToInput = true;
            }
        }
        private void UnsubscribeFromInput()
        {
            if (subscribedToInput)
            {
                subscribedToInput = false;
                uiInputManager.OnNavigationIntChanged -= OnNavigationIntChanged;
                uiInputManager.OnSubmit -= OnSubmit;
                uiInputManager.OnCancel -= OnCancel;
            }
        }
        private void SubscribeToUI()
        {
            if (!subscribedToUI)
            {
                subscribedToUI = true;
                skinOptionSelect.OnSelectionChanged += OnSkinSelectChanged;
                devotionNameOptionSelect.OnSelectionChanged += OnDevotionSelectChanged;
                spiritNameOptionSelect.OnSelectionChanged += OnSpiritSelectChanged;
            }

        }
        private void UnsubscribeFromUI()
        {
            if (subscribedToUI)
            {
                subscribedToUI = false;
                skinOptionSelect.OnSelectionChanged -= OnSkinSelectChanged;
                devotionNameOptionSelect.OnSelectionChanged -= OnDevotionSelectChanged;
                spiritNameOptionSelect.OnSelectionChanged -= OnSpiritSelectChanged;
            }

        }
        #region Input Events
        private void OnNavigationIntChanged(Vector2Int navigation)
        {
            //Debug.Log(navigation);
            if (navigation.x != 0)
            {
                int oldSelection = currentlySelectedElement;
                currentlySelectedElement += navigation.x;
                if (currentlySelectedElement >= activeUIElements.Length)
                {
                    currentlySelectedElement = 0;
                }
                else if (currentlySelectedElement < 0)
                {
                    currentlySelectedElement = activeUIElements.Length - 1;
                }
                activeUIElements[oldSelection].LoseFocus();
                activeUIElements[currentlySelectedElement].Focus(uiInputManager);
            }
        }
        private void OnSubmit()
        {
            characterSelectScreen.SetPlayerReady(playerIndex, true);
        }
        private void OnCancel()
        {
            characterSelectScreen.PlayerClickedCancel(playerIndex);
        }

        #endregion
        #region UIEvents
        private void OnSkinSelectChanged(int step)
        {
            characterSelectScreen.ChangeCharacterSkin(playerIndex, step);
        }
        private void OnDevotionSelectChanged(int step)
        {
            characterSelectScreen.ChangeDevotionName(playerIndex, step);
        }
        private void OnSpiritSelectChanged(int step)
        {
            characterSelectScreen.ChangeSpiritName(playerIndex, step);
        }
        #endregion

        public void UpdatePanel(SelectScreenPlayerData playerData)
        {
            string devotionName = characterSelectScreen.characterNames.GetDevotionName(playerData.devotionNameIndex);
            devotionNameOptionSelect.UpdateText(devotionName);
            string spiritName = characterSelectScreen.characterNames.GetSpiritName(playerData.spiritNameIndex);
            spiritNameOptionSelect.UpdateText(spiritName);
            CharacterSkinData skinData = characterSelectScreen.characterSkins.GetSkinData(playerData.skinIndex);
            skinOptionSelect.UpdateImage(skinData.defaultImage);
            playerReadyObject.SetActive(playerData.playerReady);
            //Debug.Log(devotionName + " " + spiritName);
        }

    }
}

