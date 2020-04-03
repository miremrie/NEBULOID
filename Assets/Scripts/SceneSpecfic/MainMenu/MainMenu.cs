using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NBLD.MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        public int primaryInput, altInput;
        public GameObject shipSelectionScreen, shipSelectionPrompt;
        public bool inShipSelectionMode = false;

        void Start()
        {
            ChangeShipSelectionMode(false);
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
            NBLD.Input.UIInputManager.onSubmit += OnSubmit;
            NBLD.Input.UIInputManager.onChangeSelect += OnChangeSelect;
            NBLD.Input.UIInputManager.onEscape += OnEscape;
        }
        private void Unsubscribe()
        {
            NBLD.Input.UIInputManager.onSubmit -= OnSubmit;
            NBLD.Input.UIInputManager.onChangeSelect -= OnChangeSelect;
            NBLD.Input.UIInputManager.onEscape -= OnEscape;
        }

        public void ChangeShipSelectionMode(bool inShipSelectionMode)
        {
            this.inShipSelectionMode = inShipSelectionMode;
            shipSelectionPrompt.SetActive(!this.inShipSelectionMode);
            shipSelectionScreen.SetActive(this.inShipSelectionMode);
        }
        //Scene Changers
        private void LoadArcadeLevel()
        {
            AkSoundEngine.StopAll();
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
        public void LoadGarage()
        {
            AkSoundEngine.StopAll();
            SceneManager.LoadScene(2, LoadSceneMode.Single);
        }
        private void QuitGame()
        {
            Application.Quit();
        }


        //Events
        private void OnSubmit()
        {
            if (!inShipSelectionMode)
            {
                LoadArcadeLevel();
            } else
            {
                LoadGarage();
            }
        }
        private void OnChangeSelect()
        {
            if (!inShipSelectionMode)
            {
                ChangeShipSelectionMode(true);
            }
        }
        private void OnEscape()
        {
            QuitGame();
        }
    }
}
