using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuShipCreatorScreen : MonoBehaviour
{
    InputHandler handler;
    AllSaveData saveData;

    public MainMenu mainMenu;
    public Image shipEntrySample;
    public float entryHeight;
    private Image[] allShipEntries;
    public RectTransform shipEntriesTable;
    public Color evenEntryColor, oddEntryColor;
    public Color selectedColor;
    public int currentlySelectedShip;
    private int lastHorInputState = 0;
    public ScrollRect scrollRect;
    public Scrollbar scrollBar;


    private void Start()
    {
        if (saveData == null)
        {
            saveData = SaveSystem.LoadData();
        }
        FillShipEntries();
    }
    private void OnEnable()
    {
        handler = mainMenu.handler;
    }

    private void Update()
    {
        ChangeSelection();
        ConfirmSelection();
        CheckForCreateShip();
    }


    private void FillShipEntries()
    {
        int shipCount = saveData.shipData.Count;
        allShipEntries = new Image[shipCount];
        currentlySelectedShip = saveData.currentShipID;
        for (int i = 0; i < shipCount; i++)
        {
            allShipEntries[i] = Instantiate(shipEntrySample, shipEntriesTable, false);
            float yPos = i * (entryHeight);
            RectTransform imageTransform = (RectTransform)allShipEntries[i].transform;
            imageTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, yPos, entryHeight);
            //imageTransform.localPosition = new Vector3(0, yPos, 0);
            if (i == currentlySelectedShip)
            {
                allShipEntries[i].color = selectedColor;
            } else
            {
                allShipEntries[i].color = (i % 2 == 0) ? evenEntryColor : oddEntryColor;
            }
            allShipEntries[i].GetComponentInChildren<Text>().text = saveData.shipData[i].shipName;
            allShipEntries[i].gameObject.SetActive(true);
        }
        shipEntriesTable.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, entryHeight * shipCount);
        scrollRect.verticalNormalizedPosition = 1;
        Canvas.ForceUpdateCanvases();
        UpdateScrollBar();
    }

    private void CheckForCreateShip()
    {
        if (handler.GetActionPressed())
        {
            AkSoundEngine.StopAll();
            SceneManager.LoadScene(2);
        }
    }

    private void ConfirmSelection()
    {
        if (handler.GetSubActionPressed())
        {
            saveData.currentShipID = currentlySelectedShip;
            SaveSystem.SaveData(saveData);
            mainMenu.ChangeShipSelectionMode(false);
        }
    }

    private void ChangeSelection()
    {
        int curHor = GetDiscreteHor();
        if (curHor != 0)
        {
            int oldSelectedShip = currentlySelectedShip;
            currentlySelectedShip += curHor;
            currentlySelectedShip = Mathf.Clamp(currentlySelectedShip, (int)0, (int)saveData.shipData.Count - 1);
            SelectShip(oldSelectedShip);
            UpdateScrollBar();
        }
    }

    private void UpdateScrollBar()
    {
        scrollBar.value = 1 - (float)currentlySelectedShip / ((float)allShipEntries.Length - 1);

    }

    private void SelectShip(int oldSelectedShip)
    {
        allShipEntries[oldSelectedShip].color = (oldSelectedShip % 2 == 0) ? evenEntryColor : oddEntryColor;
        allShipEntries[currentlySelectedShip].color = selectedColor;
    }

    private int GetDiscreteHor()
    {
        int newState = handler.GetHorizontal();
        if (newState != lastHorInputState)
        {
            lastHorInputState = newState;
            return lastHorInputState;
        }
        return 0;
    }
}
