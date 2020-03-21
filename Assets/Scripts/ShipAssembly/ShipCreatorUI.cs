using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public struct UIRoomID
{
    public RoomName roomName;
    public Image uiImage;
    public GameObject alarmIndicator;
}

public class ShipCreatorUI : MonoBehaviour
{
    public ShipCreator shipCreator;
    public int primaryInput, altInput;
    private InputHandler handler;
    private int lastHorInputState = 0;
    [SerializeField]
    public UIRoomID[] uiRooms;
    private int currentShipSystem = 0;
    public SystemName[] systemNames;
    private List<UIRoomID> availableRooms = new List<UIRoomID>();
    private List<SystemName> availableSystems = new List<SystemName>();
    public Color selectedTint, unusedTint, usedTint;
    public Text actionTitleText;
    private string selectRoomTitle = "SELECT ROOM";
    private string selectSystemTitle = "SELECT SYSTEM";
    private string placeSystemTitle = "PLACE SYSTEM";
    private string finishedSystemTitle = "NO MORE";
    private string enterShipNameTitle = "ENTER NAME";
    private GameObject currentPlacementSystem;
    public float placementRotSpeed = 10f;
    public Text[] roomNameDisplayTable;
    public Text[] systemNameDisplayTable;
    public Color normalTextColor, currentlyPickingTextColor;
    private int currentlyModifying = 0;
    public InputField shipNameInputField;
    //private bool reselectInputField = false;
    public int minCharShipName = 3;

    private int currentlySelectedRoom;

    private void Start()
    {
        RegisterController();
        shipCreator.onShipDataPrepared += OnShipDataPrepared;
        foreach (UIRoomID room in uiRooms)
        {
            availableRooms.Add(room);
        }
        foreach (SystemName system in systemNames)
        {
            availableSystems.Add(system);
        }
        shipNameInputField.onEndEdit.AddListener(OnShipNameEntered);
        shipNameInputField.Select();
        ChangeStage();
    }

    private void OnShipDataPrepared(ShipData shipData, bool hardSaveShip)
    {
        foreach(SystemData sysData in shipData.systemDatas)
        {

        }
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

    private void Update()
    {
        CheckForSecondInput();
        CheckForFinishEditing();
        if (shipCreator.currentStage == CreationStage.SelectRoom)
        {
            HandleSelectRoom();
        } else if (shipCreator.currentStage == CreationStage.SelectSystem)
        {
            HandleSelectSystem();
        } else if (shipCreator.currentStage == CreationStage.PlaceSystem)
        {
            HandlePlaceSystem();
        }
        if (handler.GetEscapeKeyPressed())
        {
            AkSoundEngine.StopAll();
            SceneManager.LoadScene(0);
        }
        //ShouldReselectShipNameInput();
    }

    private void CheckForFinishEditing()
    {
        if (handler.GetSubActionPressed() 
            && (shipCreator.currentStage == CreationStage.SelectRoom
                || shipCreator.currentStage == CreationStage.Finished))
        {
            shipCreator.ConfirmShip();
        }
    }

    private void HandlePlaceSystem()
    {
        if (handler.GetActionPressed())
        {
            shipCreator.PlaceSystem(currentPlacementSystem.transform.rotation.eulerAngles);
            availableRooms[currentlySelectedRoom].alarmIndicator.SetActive(false);
            availableRooms.RemoveAt(currentlySelectedRoom);
            if (!shipCreator.IsSystemAvailable(availableSystems[currentShipSystem]))
            {
                availableSystems.RemoveAt(currentShipSystem);
            }
            currentlyModifying++;
            ChangeStage();
            return;
        }
        int curHor = handler.GetHorizontal();
        if (curHor != 0)
        {
            float rotSpeed = placementRotSpeed * curHor;
            currentPlacementSystem.transform.rotation = Quaternion.Euler(currentPlacementSystem.transform.rotation.eulerAngles + Vector3.forward * rotSpeed * Time.deltaTime);
        }


    }

    private void HandleSelectSystem()
    {
        if (handler.GetActionPressed())
        {
            shipCreator.SelectSystem(availableSystems[currentShipSystem]);
            currentPlacementSystem = shipCreator.CreateSampleSystem(availableSystems[currentShipSystem]);
            ChangeSystemText(availableSystems[currentShipSystem], false);
            ChangeStage();
            return;
        }
        int curHor = GetDiscreteHor();
        if (curHor != 0)
        {
            currentShipSystem += curHor;
            if (currentShipSystem >= availableSystems.Count)
            {
                currentShipSystem = 0;
            } else if (currentShipSystem < 0)
            {
                currentShipSystem = availableSystems.Count - 1;
            }
            shipCreator.ShowSystem(availableSystems[currentShipSystem]);
            ChangeSystemText(availableSystems[currentShipSystem], true);
        }
    }

    private void ResetSelectedSystem()
    {
        currentShipSystem = 0;
        shipCreator.ShowSystem(availableSystems[currentShipSystem]);
        ChangeSystemText(currentlyModifying, availableSystems[currentShipSystem], true);

    }

    private void ResetSelectedRoom()
    {

        currentlySelectedRoom = 0;
        availableRooms[currentlySelectedRoom].alarmIndicator.SetActive(true);
        ChangeRoomText(currentlyModifying, availableRooms[currentlySelectedRoom].roomName, true);
    }

    private void HandleSelectRoom()
    {
        ChangeRoomSelection();
    }



    private void ChangeRoomSelection()
    {
        if (handler.GetActionPressed())
        {
            shipCreator.SelectRoom(availableRooms[currentlySelectedRoom].roomName);
            ChangeRoomText(currentlyModifying, availableRooms[currentlySelectedRoom].roomName, false);
            ChangeStage();
            return;
        }
        int curHor = GetDiscreteHor();
        if (curHor != 0)
        {
            if (availableRooms.Count > 1)
            {
                int oldSelectedRoom = currentlySelectedRoom;
                ChangeImageTint(availableRooms[oldSelectedRoom].uiImage, unusedTint);
                availableRooms[oldSelectedRoom].alarmIndicator.SetActive(false);
                currentlySelectedRoom += curHor;
                if (currentlySelectedRoom < 0)
                {
                    currentlySelectedRoom = availableRooms.Count - 1;
                } else if (currentlySelectedRoom == availableRooms.Count)
                {
                    currentlySelectedRoom = 0;
                }
                ChangeImageTint(availableRooms[currentlySelectedRoom].uiImage, selectedTint);
                ChangeRoomText(currentlyModifying, availableRooms[currentlySelectedRoom].roomName, true);
                availableRooms[currentlySelectedRoom].alarmIndicator.SetActive(true);
            }
        }

    }


    private void ChangeStage()
    {
        switch (shipCreator.currentStage)
        {
            case (CreationStage.CreateName):
                actionTitleText.text = enterShipNameTitle;
                break;
            case (CreationStage.SelectRoom):
            {
                ResetSelectedRoom();
                actionTitleText.text = selectRoomTitle;
                break;
            }
            case (CreationStage.SelectSystem):
            {
                ResetSelectedSystem();
                actionTitleText.text = selectSystemTitle;
                break;
            }
            case (CreationStage.PlaceSystem):
                actionTitleText.text = placeSystemTitle;
                break;
            case (CreationStage.Finished):
                actionTitleText.text = finishedSystemTitle;
                break;
        }
    }

    private void ChangeImageTint(Image image, Color tint)
    {
        image.color = tint;
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


    private void ChangeRoomText(int roomOrderIndex, RoomName name, bool isBeingEdited = false)
    {
        roomNameDisplayTable[roomOrderIndex].text = name.ToString();
        roomNameDisplayTable[roomOrderIndex].color = (isBeingEdited) ? currentlyPickingTextColor : normalTextColor;
    }
    private void ChangeSystemText(int systemOrderIndex, SystemName name, bool isBeingEdited = false)
    {
        systemNameDisplayTable[systemOrderIndex].text = name.ToString();
        systemNameDisplayTable[systemOrderIndex].color = (isBeingEdited) ? currentlyPickingTextColor : normalTextColor;
    }

    private void RegisterController()
    {
        handler = InputHandler.RegisterController(primaryInput);
    }

    private void OnShipNameEntered(string name)
    {
        if (name.Length >= minCharShipName)
        {
            shipNameInputField.transform.parent.gameObject.SetActive(false);
            shipCreator.SetShipName(name);
            ChangeStage();
        } else
        {
            shipNameInputField.Select();            
        }
    }
}
