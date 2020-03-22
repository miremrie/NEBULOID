using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NBLD.ShipCreation
{

    [System.Serializable]
    public struct UIRoomID
    {
        public RoomName roomName;
        public Image uiImage;
        public GameObject alarmIndicator;
    }
    [System.Serializable]
    public struct CreationStageMessage
    {
        public CreationStage stage;
        public string message;
    }

    public class ShipCreatorUI : MonoBehaviour
    {
        public ShipCreator shipCreator;
        public int primaryInput, altInput;
        private InputHandler handler;
        private int lastHorInputState = 0;
        [SerializeField]
        public UIRoomID[] uiRooms;

        public Color selectedTint, unusedTint, usedTint;
        public Text actionTitleText;
        public CreationStageMessage[] stageMessages;
        private GameObject currentPlacementSystem;
        public float placementRotSpeed = 10f;
        public Text[] roomNameDisplayTable;
        public Text[] systemNameDisplayTable;
        public Color normalTextColor, currentlyPickingTextColor;
        private int curEditIndexDisplayTable = 0;
        public InputField shipNameInputField;
        //private bool reselectInputField = false;
        public int minCharShipName = 3;

        private int currentlySelectedRoom;
        private int currentlySelectedSystem = 0;
        private List<RoomName> roomTextIndex = new List<RoomName>();
        private Dictionary<RoomName, GameObject> roomToAttachedSystem = new Dictionary<RoomName, GameObject>();

        private void Awake()
        {
            RegisterController();
            shipCreator.onCreationStageChanged += ChangeStage;

            shipNameInputField.onEndEdit.AddListener(OnShipNameEntered);
            shipNameInputField.Select();
        }


        private void ChangeStage(CreationStage stage)
        {
            actionTitleText.text = stageMessages.First(sm => sm.stage == stage).message;
            switch (shipCreator.currentStage)
            {
                case (CreationStage.CreateName):
                    break;
                case (CreationStage.SelectRoom):
                {
                    shipNameInputField.transform.parent.gameObject.SetActive(false);
                    ResetRoomSelection();
                    ReengageRoomSelection();
                    break;
                }
                case (CreationStage.SelectSystem):
                {
                    RoomName currentRoom = shipCreator.GetCurrentEditRoom();
                    RegisterNewRoomDisplayText(currentRoom);
                    if (roomToAttachedSystem.ContainsKey(currentRoom))
                    {
                        Destroy(roomToAttachedSystem[currentRoom]);
                        roomToAttachedSystem.Remove(currentRoom);
                    }
                    ChangeRoomText(curEditIndexDisplayTable, shipCreator.GetCurrentEditRoom().ToString(), false);
                    ReengageSystemSelection();
                    break;
                }
                case (CreationStage.PlaceSystem):
                {
                    shipCreator.HideAllSystems();
                    RoomName currentRoom = shipCreator.GetCurrentEditRoom();
                    SystemName currentSystem = shipCreator.GetCurrentEditSystem();
                    currentPlacementSystem = shipCreator.CreateSampleSystem(currentSystem);
                    roomToAttachedSystem.Add(currentRoom, currentPlacementSystem);
                    ChangeSystemText(curEditIndexDisplayTable, currentSystem, false);
                    break;
                }

                case (CreationStage.Finished):
                    ResetRoomSelection();
                    break;
                case (CreationStage.Confirmed):
                    ResetRoomSelection();
                    ResetPlacedSystems();
                    break;
                case (CreationStage.Canceled):
                    ResetRoomSelection();
                    ResetPlacedSystems();
                    break;
            }
        }

        private void Update()
        {
            CheckForSecondInput();
            CheckForFinishEditing();
            if (shipCreator.currentStage == CreationStage.SelectRoom)
            {
                HandleRoomSelection();
            }
            else if (shipCreator.currentStage == CreationStage.SelectSystem)
            {
                HandleSelectSystem();
            }
            else if (shipCreator.currentStage == CreationStage.PlaceSystem)
            {
                HandlePlaceSystem();
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
            if (handler.GetEscapeKeyPressed())
            {
                shipCreator.CancelShip();
            }
        }

        private void HandlePlaceSystem()
        {
            if (handler.GetActionPressed())
            {
                shipCreator.PlaceSystem(currentPlacementSystem.transform.rotation.eulerAngles);
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
                shipCreator.SelectSystem(shipCreator.GetSystemNameForID(currentlySelectedSystem));
                
                return;
            }
            int curHor = GetDiscreteHor();
            if (curHor != 0)
            {
                SetNewSelectedSystem(currentlySelectedSystem, curHor);
                ShowSystemSelection();
            }
        }

        private void ReengageSystemSelection()
        {
            SetNewSelectedSystem(0);
            ShowSystemSelection();
        }

        private void ReengageRoomSelection()
        {
            SetNewSelectedRoom(0);
            SetRoomHighlightState(currentlySelectedRoom, true);
        }
        private void ResetRoomSelection()
        {
            SetRoomHighlightState(currentlySelectedRoom, false);
        }
        private void ResetPlacedSystems()
        {
            foreach(KeyValuePair<RoomName, GameObject> pair in roomToAttachedSystem)
            {
                Destroy(pair.Value);
            }
            roomToAttachedSystem.Clear();
        }

        private void HandleRoomSelection()
        {
            if (handler.GetActionPressed())
            {
                shipCreator.SelectRoom(uiRooms[currentlySelectedRoom].roomName);
                return;
            }
            int curHor = GetDiscreteHor();
            if (curHor != 0)
            {
                int oldSelectedRoom = currentlySelectedRoom;
                SetRoomHighlightState(oldSelectedRoom, false);

                SetNewSelectedRoom(oldSelectedRoom, curHor);

                SetRoomHighlightState(currentlySelectedRoom, true);
            }
        }

        private void SetNewSelectedRoom(int previousRoom, int dir = 1)
        {
            for (int i = dir; Mathf.Abs(i) < uiRooms.Length; i += dir)
            {
                currentlySelectedRoom = (previousRoom + i);
                if (currentlySelectedRoom < 0)
                    currentlySelectedRoom = uiRooms.Length + currentlySelectedRoom;
                if (currentlySelectedRoom >= uiRooms.Length)
                    currentlySelectedRoom = currentlySelectedRoom % uiRooms.Length;

                if (shipCreator.IsRoomAvailable(uiRooms[currentlySelectedRoom].roomName))
                {
                    break;
                }
            }
        }

        private void SetNewSelectedSystem(int previousSystem, int dir = 1)
        {
            int systemCount = shipCreator.GetAllSystemsCount();
            for (int i = dir; Mathf.Abs(i) < systemCount; i += dir)
            {
                currentlySelectedSystem = (previousSystem + i);
                if (currentlySelectedSystem < 0)
                    currentlySelectedSystem = systemCount + currentlySelectedSystem;
                if (currentlySelectedSystem >= systemCount)
                    currentlySelectedSystem = currentlySelectedSystem % systemCount;

                if (shipCreator.IsSystemAvailable(shipCreator.GetSystemNameForID(currentlySelectedSystem)))
                {
                    break;
                }
            }
        }

        private void RegisterNewRoomDisplayText(RoomName name)
        {
            if (!roomTextIndex.Contains(name))
            {
                roomTextIndex.Add(name);
            }
            curEditIndexDisplayTable = roomTextIndex.IndexOf(name);
        }

        //UI Changes

        private void ShowSystemSelection()
        {
            SystemName name = shipCreator.GetSystemNameForID(currentlySelectedSystem);
            shipCreator.ShowSystem(name);
            ChangeSystemText(curEditIndexDisplayTable, name, true);
        }

        private void SetRoomHighlightState(int roomID, bool highlighted)
        {
            UIRoomID uiRoom = uiRooms[roomID];
            Color tint = highlighted ? selectedTint : unusedTint;
            ChangeImageTint(uiRoom.uiImage, tint);
            uiRoom.alarmIndicator.SetActive(highlighted);

            if (roomTextIndex.Contains(uiRoom.roomName))
            {
                int textIndex = roomTextIndex.IndexOf(uiRoom.roomName);
                ChangeRoomText(textIndex, uiRoom.roomName, highlighted);
            }
            else
            {
                if (highlighted)
                {
                    ChangeRoomText(roomTextIndex.Count, uiRoom.roomName, highlighted);
                }
                else
                {
                    ChangeRoomText(roomTextIndex.Count, "", highlighted);
                }
            }
        }

        private void ChangeImageTint(Image image, Color tint)
        {
            image.color = tint;
        }
        private void ChangeRoomText(int roomOrderIndex, RoomName room, bool isBeingEdited = false)
        {
            ChangeRoomText(roomOrderIndex, room.ToString(), isBeingEdited);
        }
        private void ChangeRoomText(int roomOrderIndex, string text, bool isBeingEdited = false)
        {
            roomNameDisplayTable[roomOrderIndex].text = text;
            roomNameDisplayTable[roomOrderIndex].color = (isBeingEdited) ? currentlyPickingTextColor : normalTextColor;
        }
        private void ChangeSystemText(int systemOrderIndex, SystemName name, bool isBeingEdited = false)
        {
            systemNameDisplayTable[systemOrderIndex].text = name.ToString();
            systemNameDisplayTable[systemOrderIndex].color = (isBeingEdited) ? currentlyPickingTextColor : normalTextColor;
        }

        private void OnShipNameEntered(string name)
        {
            if (name.Length >= minCharShipName)
            {
                shipCreator.SetShipName(name);
            }
            else
            {
                shipNameInputField.Select();
            }
        }

        //Input
        private void RegisterController()
        {
            handler = InputHandler.RegisterController(primaryInput);
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

}
