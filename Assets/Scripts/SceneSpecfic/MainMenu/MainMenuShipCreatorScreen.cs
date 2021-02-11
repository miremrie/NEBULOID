using System.Collections;
using System.Collections.Generic;
using NBLD.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NBLD.MainMenu
{
    public class MainMenuShipCreatorScreen : MonoBehaviour
    {
        AllSaveData saveData;

        public MainMenu mainMenu;
        public Image shipEntrySample;
        public float entryHeight;
        private Image[] allShipEntries;
        public RectTransform shipEntriesTable;
        public Color evenEntryColor, oddEntryColor;
        public Color selectedColor;
        public int currentlySelectedShip;
        public ScrollRect scrollRect;
        public Scrollbar scrollBar;
        public float heldVerticalChangeSelectionTime = 0.1f;
        private Timer heldVerticalTimer;
        private Input.UIInputManager uiInput;

        private void Awake()
        {
            uiInput = new Input.UIInputManager();
        }

        private void Start()
        {
            if (saveData == null)
            {
                saveData = SaveSystem.LoadData();
            }
            FillShipEntries();
            heldVerticalTimer = new Timer(heldVerticalChangeSelectionTime, true);
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
            uiInput.onNavigationChangedInt += OnNavigationChanged;
            uiInput.onCancel += OnCancel;
            uiInput.onChangeSelect += OnChangeSelect;
            uiInput.verticalHold.onAxisBeingHeldInt += OnNavigationHeld;
        }
        private void Unsubscribe()
        {
            uiInput.Disable();
            uiInput.onNavigationChangedInt -= OnNavigationChanged;
            uiInput.onCancel -= OnCancel;
            uiInput.onChangeSelect -= OnChangeSelect;
            uiInput.verticalHold.onAxisBeingHeldInt -= OnNavigationHeld;
        }

        private void Update()
        {
            uiInput.Update(Time.deltaTime);
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
                }
                else
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

        private void ChangeSelection(int direction)
        {
            if (direction != 0)
            {
                int oldSelectedShip = currentlySelectedShip;
                currentlySelectedShip += direction;
                currentlySelectedShip = Mathf.Clamp(currentlySelectedShip, (int)0, (int)saveData.shipData.Count - 1);
                HighlightShip(oldSelectedShip);
                UpdateScrollBar();
            }
        }
        private void ConfirmSelection()
        {
            saveData.currentShipID = currentlySelectedShip;
            SaveSystem.SaveData(saveData);
        }
        private void DismissSelection()
        {
            int dismissedShip = currentlySelectedShip;
            currentlySelectedShip = saveData.currentShipID;
            HighlightShip(dismissedShip);
        }

        //Events
        private void OnChangeSelect()
        {
            ConfirmSelection();
            mainMenu.ChangeShipSelectionMode(false);
        }
        private void OnCancel()
        {
            DismissSelection();
            mainMenu.ChangeShipSelectionMode(false);
        }
        private void OnNavigationChanged(Vector2Int navigation)
        {
            ChangeSelection(-Mathf.RoundToInt(navigation.y));
        }
        private void OnNavigationHeld(int value, float time)
        {
            if (heldVerticalTimer.IsTimerDone())
            {
                ChangeSelection(-value);
                heldVerticalTimer.Restart();
            }
        }

        //Visual UI
        private void UpdateScrollBar()
        {
            scrollBar.value = 1 - (float)currentlySelectedShip / ((float)allShipEntries.Length - 1);
        }

        private void HighlightShip(int oldSelectedShip)
        {
            allShipEntries[oldSelectedShip].color = (oldSelectedShip % 2 == 0) ? evenEntryColor : oddEntryColor;
            allShipEntries[currentlySelectedShip].color = selectedColor;
        }
    }
}

