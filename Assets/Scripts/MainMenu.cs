using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public int primaryInput, altInput;
    public GameObject shipSelectionScreen, shipSelectionPrompt;
    public InputHandler handler;
    public bool inShipSelectionMode = false;
    // Start is called before the first frame update
    void Start()
    {
        RegisterController();
        ChangeShipSelectionMode(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!inShipSelectionMode)
        {
            if (handler.GetActionPressed())
            {
                SceneManager.LoadScene(1, LoadSceneMode.Single);
            }
            if (handler.GetSubActionPressed())
            {
                ChangeShipSelectionMode(true);
            }
        }
        if (handler.GetEscapeKeyPressed())
        {
            Application.Quit();
        }
        CheckForSecondInput();
    }

    private void CheckForSecondInput()
    {
        if (altInput != -1 && Input.GetButtonDown("Action" + altInput.ToString()))
        {
            int tmp = altInput;
            altInput = primaryInput;
            primaryInput = tmp;
            RegisterController();
        }
    }

    private void RegisterController()
    {
        handler = InputHandler.RegisterController(primaryInput);
    }

    public void ChangeShipSelectionMode(bool inShipSelectionMode)
    {
        this.inShipSelectionMode = inShipSelectionMode;
        shipSelectionPrompt.SetActive(!this.inShipSelectionMode);
        shipSelectionScreen.SetActive(this.inShipSelectionMode);
    }
}
