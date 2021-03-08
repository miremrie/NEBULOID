using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NBLD.MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        public int primaryInput, altInput;
        public GameObject charSelectionScreen;
        public GameObject shipSelectionScreen;
        public GameObject mainMenuRootScreen;
        public bool onShipSelectScreen = false;
        public bool onCharSelectScreen = false;
        private Input.UIInputManager uiInput;
        public int gameSceneIndex = 1;
        public int garageSceneIndex = 2;

        private void Awake()
        {
            uiInput = new Input.UIInputManager();
        }
        void Start()
        {

            ChangeScreenToRootMainMenu();
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
        private bool IsOnMainMenuRootScreen() => !onShipSelectScreen && !onCharSelectScreen;

        public void ChangeScreenToShipSelect()
        {
            this.onShipSelectScreen = true;
            this.onCharSelectScreen = false;
            ChangeScreen();
        }
        public void ChangeScreenToCharSelect()
        {
            this.onCharSelectScreen = true;
            this.onShipSelectScreen = false;
            ChangeScreen();
        }
        public void ChangeScreenToRootMainMenu()
        {
            this.onShipSelectScreen = false;
            this.onCharSelectScreen = false;
            ChangeScreen();
        }
        private void ChangeScreen()
        {
            mainMenuRootScreen.SetActive(IsOnMainMenuRootScreen());
            charSelectionScreen.SetActive(this.onCharSelectScreen);
            shipSelectionScreen.SetActive(this.onShipSelectScreen);
        }
        //Scene Changers
        public void LoadArcadeLevel()
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
            if (IsOnMainMenuRootScreen())
            {
                ChangeScreenToCharSelect();
            }
            else if (onShipSelectScreen)
            {
                LoadGarage();
            }
        }
        private void OnChangeSelect()
        {
            if (IsOnMainMenuRootScreen())
            {
                ChangeScreenToShipSelect();
            }
        }
        private void OnEscape()
        {
            QuitGame();
        }
    }
}
