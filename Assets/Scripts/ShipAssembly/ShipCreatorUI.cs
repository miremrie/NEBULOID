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
        [SerializeField]
        public UIRoomID[] uiRooms;

        public Color selectedTint, unusedTint, usedTint;
        public Text actionTitleText;
        public CreationStageMessage[] stageMessages;
        private GameObject currentPlacementSystem;
        public float placementRotSpeed = 10f, placementTowardsRotSpeed = 0.05f;
        public Text[] roomNameDisplayTable;
        public Text[] systemNameDisplayTable;
        public Color normalTextColor, currentlyPickingTextColor;
        private int curEditIndexDisplayTable = 0;
        public InputField shipNameInputField;
        //private bool reselectInputField = false;
        public int minCharShipName = 3;
        public float heldVerticalChangeSelectionTime = 0.05f;
        private Timer heldVerticalTimer;

        private int currentlySelectedRoom;
        private int currentlySelectedSystem = 0;
        private List<RoomName> roomTextIndex = new List<RoomName>();
        private Dictionary<RoomName, GameObject> roomToAttachedSystem = new Dictionary<RoomName, GameObject>();
        public bool placeSystemTowardsDirection;

        private void Awake()
        {
            shipCreator.onCreationStageChanged += ChangeStage;
            heldVerticalTimer = new Timer(heldVerticalChangeSelectionTime);
            shipNameInputField.Select();
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
            //UI Items
            shipNameInputField.onEndEdit.AddListener(OnShipNameEntered);
            //Input
            NBLD.Input.UIInputManager.onSubmit += OnSubmit;
            NBLD.Input.UIInputManager.onCancel += OnCancel;
            NBLD.Input.UIInputManager.onEscape += OnCancel;
            NBLD.Input.UIInputManager.onChangeSelect += OnChangeSelect;
            NBLD.Input.UIInputManager.onNavigationChangedInt += OnNavigationChangedInt;
            NBLD.Input.UIInputManager.verticalHoldProcessor.onAxisBeingHeldInt += OnNavigationHeldInt;
            NBLD.Input.UIInputManager.onNavigation += OnNavigation;
        }

        private void Unsubscribe()
        {
            //UI Items
            shipNameInputField.onEndEdit.RemoveListener(OnShipNameEntered);
            //Input
            NBLD.Input.UIInputManager.onSubmit -= OnSubmit;
            NBLD.Input.UIInputManager.onCancel -= OnCancel;
            NBLD.Input.UIInputManager.onEscape -= OnCancel;
            NBLD.Input.UIInputManager.onChangeSelect -= OnChangeSelect;
            NBLD.Input.UIInputManager.onNavigationChangedInt -= OnNavigationChangedInt;
            NBLD.Input.UIInputManager.verticalHoldProcessor.onAxisBeingHeldInt -= OnNavigationHeldInt;
            NBLD.Input.UIInputManager.onNavigation -= OnNavigation;

        }


        private void ChangeStage(CreationStage stage, CreationStage previousStage)
        {
            actionTitleText.text = stageMessages.First(sm => sm.stage == stage).message;
            switch (shipCreator.currentStage)
            {
                case (CreationStage.CreateName):
                    break;
                case (CreationStage.SelectRoom):
                {
                    if (previousStage == CreationStage.PlaceSystem)
                    {
                        RotatePlaceSystem(shipCreator.GetCurrentEditRotation());
                    }
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
                    if (previousStage == CreationStage.PlaceSystem)
                    {
                        RotatePlaceSystem(shipCreator.GetCurrentEditRotation());
                    }
                    ResetRoomSelection();
                    break;
                case (CreationStage.Confirmed):
                    if (previousStage == CreationStage.PlaceSystem)
                    {
                        RotatePlaceSystem(shipCreator.GetCurrentEditRotation());
                    }
                    ResetRoomSelection();
                    ResetPlacedSystems();
                    break;
                case (CreationStage.Canceled):
                    ResetRoomSelection();
                    ResetPlacedSystems();
                    break;
            }
        }

        //Events
        private void OnSubmit()
        {
            if (shipCreator.currentStage == CreationStage.SelectRoom)
            {
                OnSelectRoomSubmit();
            }
            else if (shipCreator.currentStage == CreationStage.SelectSystem)
            {
                OnSelectSystemSubmit();
            }
            else if (shipCreator.currentStage == CreationStage.PlaceSystem)
            {
                OnPlaceSystemSubmit();
            }
        }
        private void OnNavigation(Vector2 navigation)
        {
            if (shipCreator.currentStage == CreationStage.PlaceSystem)
            {
                OnPlaceSystemChange(navigation);
            }
        }
        private void OnNavigationHeldInt(int value, float time)
        {
            if (!heldVerticalTimer.IsRunning())
            {
                IntNavigationChange(value);
                heldVerticalTimer.Start();
            }
            heldVerticalTimer.Update(Time.deltaTime);
        }
        private void OnNavigationChangedInt(Vector2Int navigation)
        {
            IntNavigationChange(navigation.y);
        }
        private void OnChangeSelect()
        {
            if (shipCreator.currentStage == CreationStage.SelectRoom
                || shipCreator.currentStage == CreationStage.Finished)
            {
                ConfirmShip();
            }
        }
        private void OnCancel()
        {
            if (shipCreator.currentStage != CreationStage.CreateName)
            {
                CancelShip();
            }
        }

        //FinishEditing Event
        private void ConfirmShip()
        {
            shipCreator.ConfirmShip();
        }
        private void CancelShip()
        {
            shipCreator.CancelShip();
        }
        private void IntNavigationChange(int direction)
        {
            if (shipCreator.currentStage == CreationStage.SelectRoom)
            {
                OnSelectRoomChange(direction);
            }
            else if (shipCreator.currentStage == CreationStage.SelectSystem)
            {
                OnSelectSystemChange(direction);
            }
        }
        //Place System
        private void OnPlaceSystemSubmit()
        {
            shipCreator.PlaceSystem(currentPlacementSystem.transform.rotation.eulerAngles);
        }
        private void OnPlaceSystemChange(Vector2 changeDirection)
        {
            Vector3 eulerAngles = currentPlacementSystem.transform.rotation.eulerAngles;
            if (placeSystemTowardsDirection)
            {
                if (Mathf.Abs(changeDirection.x) > 0 || Mathf.Abs(changeDirection.y) > 0)
                {
                    float curAngle = currentPlacementSystem.transform.rotation.eulerAngles.z;
                    float changeAngle = Mathf.Atan2(changeDirection.y, changeDirection.x) * Mathf.Rad2Deg - 180;
                    float deltaAngle = Mathf.DeltaAngle(curAngle, changeAngle);
                    deltaAngle *= placementTowardsRotSpeed * Time.deltaTime;
                    eulerAngles += Vector3.forward * deltaAngle;
                    //Debug.Log($"a {curAngle} cA = {changeAngle}, dA {deltaAngle}");
                    //Vector3 curDir = new Vector3(Mathf.Cos(curAngle), Mathf.Sin(curAngle));
                    //Vector3 newDir = Vector3.Slerp(curDir.normalized, changeDirection.normalized, placementTowardsRotSpeed * Time.deltaTime);
                    //Vector3 newDir = Vector3.RotateTowards(curDir.normalized, changeDirection.normalized, placementTowardsRotSpeed * Time.deltaTime, 0);
                    
                    //eulerAngles = currentPlacementSystem.transform.rotation.eulerAngles + Vector3.forward * Vector3.SignedAngle(curDir.normalized, newDir.normalized, Vector3.forward);
                    //Debug.Log($"D {curDir.normalized}, cD {changeDirection.normalized}, nD {newDir}, eA {eulerAngles}");

                }
            } else
            {
                float rotSpeed = placementRotSpeed * changeDirection.y;
                eulerAngles += Vector3.forward * rotSpeed * Time.deltaTime;
            }
            RotatePlaceSystem(eulerAngles);

        }
        //Select System Events
        private void OnSelectSystemSubmit()
        {
            shipCreator.SelectSystem(shipCreator.GetSystemNameForID(currentlySelectedSystem));
        }
        private void OnSelectSystemChange(int changeDirection)
        {
            SetNewSelectedSystem(currentlySelectedSystem, changeDirection);
            ShowSystemSelection();
        }
        //Select Room Events
        private void OnSelectRoomSubmit()
        {
            shipCreator.SelectRoom(uiRooms[currentlySelectedRoom].roomName);
        }
        private void OnSelectRoomChange(int direction)
        {
            int oldSelectedRoom = currentlySelectedRoom;
            SetRoomHighlightState(oldSelectedRoom, false);

            SetNewSelectedRoom(oldSelectedRoom, direction);

            SetRoomHighlightState(currentlySelectedRoom, true);
        }
        //Reset Values
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
            foreach (KeyValuePair<RoomName, GameObject> pair in roomToAttachedSystem)
            {
                Destroy(pair.Value);
            }
            roomToAttachedSystem.Clear();
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

        private void RotatePlaceSystem(Vector3 eulerRotation)
        {
            currentPlacementSystem.transform.rotation = Quaternion.Euler(eulerRotation);
        }

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
    }

}
