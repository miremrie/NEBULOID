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
        private Input.UIInputManager uiInput;
        public int gameSceneIndex = 1;
        public int garageSceneIndex = 2;

        private void Awake()
        {
            uiInput = new Input.UIInputManager();
        }
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
            uiInput.Enable();
            uiInput.onSubmit += OnSubmit;
            uiInput.onChangeSelect += OnChangeSelect;
            uiInput.onEscape += OnEscape;
        }
        private void Unsubscribe()
        {
            uiInput.Disable();
            uiInput.onSubmit -= OnSubmit;
            uiInput.onChangeSelect -= OnChangeSelect;
            uiInput.onEscape -= OnEscape;
        }

        private void Update()
        {
            uiInput.Update(Time.deltaTime);
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
            SceneManager.LoadScene(gameSceneIndex, LoadSceneMode.Single);
        }
        public void LoadGarage()
        {
            AkSoundEngine.StopAll();
            SceneManager.LoadScene(garageSceneIndex, LoadSceneMode.Single);
        }
        private void QuitGame()
        {
            Application.Quit();
        }


        //Events
        private void OnSubmit()
        {
            /*if (!inShipSelectionMode)
            {
                LoadArcadeLevel();
            } else
            {
                LoadGarage();
            }*/
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
