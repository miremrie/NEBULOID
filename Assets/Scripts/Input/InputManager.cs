using System.Collections;
using System.Collections.Generic;
using GeneratedInputActions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

namespace NBLD.Input
{
    public class PlayerData
    {

    }
    public class UserDevice
    {
        public InputUser user;
        public InputDevice device;
        private string schemeName;
        public CharacterInput inputActions;

        public UserDevice(InputDevice device, string schemeName)
        {
            this.user = InputUser.CreateUserWithoutPairedDevices();
            CharacterInput input = new CharacterInput();
            this.inputActions = input;
            user.AssociateActionsWithUser(inputActions);
            SetDevice(device, schemeName);
            //ActivateKeyboard();
        }
        public void SetDevice(InputDevice device, string schemeName)
        {
            this.device = device;
            this.schemeName = schemeName;
            ActivateDevice();
        }
        private void ActivateDevice()
        {
            InputUser.PerformPairingWithDevice(this.device, user);
            inputActions.Enable();
            user.AssociateActionsWithUser(inputActions);
            user.ActivateControlScheme(this.schemeName);
        }

        public void Dispose()
        {
            inputActions.Dispose();
            user.UnpairDevicesAndRemoveUser();
        }
    }
    public class PlayerSessionData
    {
        public int playerIndex;
        public PlayerData playerData;
        public CharInputManager charInputManager;
        public int deviceIndex;

        public PlayerSessionData(int playerIndex, int deviceIndex, CharInputManager charInputManager)
        {
            this.playerIndex = playerIndex;
            this.deviceIndex = deviceIndex;
            this.charInputManager = charInputManager;
            playerData = new PlayerData();
        }
    }
    public class InputManager : MonoBehaviour
    {
        public int maxNumberOfPlayers;
        public List<CharInputManager> characterManagers;
        public List<CharSelectController> charSelectControllers;
        public List<string> keyboardSchemeNames;
        public string gamepadSchemeName;
        private List<UserDevice> userDevices;
        private List<PlayerSessionData> activePlayers;
        private bool initialized = false;
        private bool subscribed = false;

        private void Awake()
        {
            Initialize();
        }
        private void Initialize()
        {
            if (!initialized)
            {
                initialized = true;
                InputUser.listenForUnpairedDeviceActivity++;
                userDevices = new List<UserDevice>();
                characterManagers = new List<CharInputManager>();
                charSelectControllers = new List<CharSelectController>();
                activePlayers = new List<PlayerSessionData>();
                if (Keyboard.current != null)
                {
                    for (int i = 0; i < keyboardSchemeNames.Count; i++)
                    {
                        InitializeUnusedDevice(Keyboard.current, keyboardSchemeNames[i]);
                    }
                }
                Subscribe();
            }

        }

        private void InitializeUnusedDevice(InputDevice device, string schemeName)
        {
            int playerIndex = userDevices.Count;
            UserDevice uDevice = new UserDevice(device, schemeName);
            userDevices.Add(uDevice);
            CharInputManager charInputManager = new CharInputManager(uDevice.inputActions);
            characterManagers.Add(charInputManager);
            CharSelectController charController = new CharSelectController(playerIndex, this);
            charInputManager.RegisterListener(charController);
            charSelectControllers.Add(charController);
        }
        private PlayerSessionData CreatePlayerWithDevice(int deviceIndex)
        {
            PlayerSessionData playerSessionData = new PlayerSessionData(activePlayers.Count, deviceIndex, characterManagers[deviceIndex]);
            activePlayers.Add(playerSessionData);
            return playerSessionData;
        }
        private void OnEnable()
        {
            Subscribe();
        }
        private void OnDisable()
        {
            Unsubscribe();
        }

        private void OnDestroy()
        {
            Unsubscribe();
            for (int i = 0; i < userDevices.Count; i++)
            {
                userDevices[i].Dispose();
            }
        }

        private void Subscribe()
        {
            if (initialized && !subscribed)
            {
                subscribed = true;
                //InputUser.onChange += OnControlsChanged;
                InputUser.onUnpairedDeviceUsed += ListenForUnpairedGamepads;
            }

        }
        private void Unsubscribe()
        {
            if (initialized && subscribed)
            {
                subscribed = false;
                //InputUser.onChange -= OnControlsChanged;
                InputUser.onUnpairedDeviceUsed -= ListenForUnpairedGamepads;
            }
        }

        /*void OnControlsChanged(InputUser user, InputUserChange change, InputDevice device)
        {
            if (device is Gamepad)
            {
                if (change == InputUserChange.DevicePaired)
                {
                    UserDevice userDevice = users.First(ud => ud.user == user);
                    userDevice.ActivateGamepad();
                }
                else if (change == InputUserChange.DeviceUnpaired)
                {
                    UserDevice userDevice = users.First(ud => ud.user == user);
                    userDevice.DeactivateGamepad();
                }
            }
        }*/

        private bool IsDeviceRegistered(InputDevice inputDevice)
        {
            for (int i = 0; i < userDevices.Count; i++)
            {
                if (userDevices[i].device == inputDevice)
                {
                    return true;
                }
            }
            return false;
        }
        public PlayerSessionData GetPlayerByDevice(int deviceIndex)
        {
            for (int i = 0; i < activePlayers.Count; i++)
            {
                if (activePlayers[i].deviceIndex == deviceIndex)
                {
                    return activePlayers[i];
                }
            }
            return null;
        }
        private bool IsDeviceUsed(int deviceIndex)
        {
            for (int i = 0; i < activePlayers.Count; i++)
            {
                if (activePlayers[i].deviceIndex == deviceIndex)
                {
                    return true;
                }
            }
            return false;
        }
        private void ListenForUnpairedGamepads(InputControl control, InputEventPtr inputEventPtr)
        {
            if (control.device is Gamepad)
            {
                InputDevice device = control.device;
                if (!IsDeviceRegistered(device))
                {
                    InitializeUnusedDevice(device, gamepadSchemeName);
                }
            }
        }

        public bool RegisterPlayer(int deviceIndex)
        {
            if (deviceIndex.IsBetween(-1, userDevices.Count) && !IsDeviceUsed(deviceIndex))
            {
                CreatePlayerWithDevice(deviceIndex);
                return true;
            }
            return false;
        }
    }
}