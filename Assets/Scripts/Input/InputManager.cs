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
        public readonly int deviceIndex;
        public InputUser user;
        public InputDevice device;
        private string schemeName;
        public CharacterInput gameplayInput;
        public UIInput uiInput;

        public UserDevice(int deviceIndex, InputDevice device, string schemeName)
        {
            this.deviceIndex = deviceIndex;
            this.user = InputUser.CreateUserWithoutPairedDevices();
            this.gameplayInput = new CharacterInput();
            //user.AssociateActionsWithUser(gameplayInput);
            this.uiInput = new UIInput();
            //user.AssociateActionsWithUser(uiInput);
            SetDevice(device, schemeName);
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
            //gameplayInput.Enable();
            user.AssociateActionsWithUser(gameplayInput);
            user.AssociateActionsWithUser(uiInput);
            user.ActivateControlScheme(this.schemeName);
        }

        public void EnableGameplayInput(bool enabled)
        {
            if (enabled)
            {
                gameplayInput.Enable();
            }
            else
            {
                gameplayInput.Disable();
            }
        }
        public void EnableUIInput(bool enabled)
        {
            if (enabled)
            {
                uiInput.Enable();
            }
            else
            {
                uiInput.Disable();
            }
        }

        public void Dispose()
        {
            gameplayInput.Dispose();
            uiInput.Dispose();
            user.UnpairDevicesAndRemoveUser();
        }
    }
    public class PlayerSessionData
    {
        public int playerIndex;
        public PlayerData playerData;
        public PlayerGameplayInputManager gameplayInputManager;
        public string characterName;
        public int deviceIndex;

        public PlayerSessionData(int playerIndex, int deviceIndex, PlayerGameplayInputManager charInputManager)
        {
            this.playerIndex = playerIndex;
            this.deviceIndex = deviceIndex;
            this.gameplayInputManager = charInputManager;
            playerData = new PlayerData();
        }
    }
    public class InputManager : MonoBehaviour
    {
        public int maxNumberOfPlayers;
        public List<PlayerGameplayInputManager> gameplayInputManagers;
        public List<PlayerUIInputManager> uiInputManagers;
        public List<string> keyboardSchemeNames;
        public string gamepadSchemeName;
        private List<UserDevice> userDevices;
        private List<PlayerSessionData> activePlayers;
        public bool Initialized
        {
            get; private set;
        } = false;
        private bool subscribed = false;
        public delegate void DeviceRegisteredHandler(UserDevice userDevice, PlayerGameplayInputManager gameplayInputManager, PlayerUIInputManager uiInputManager);
        public delegate void PlayerRegisteredHandler(PlayerSessionData playerSessionData);
        public delegate void InitHandler();
        public event InitHandler OnInputInitialized;
        public event DeviceRegisteredHandler OnDeviceRegistered;
        public event PlayerRegisteredHandler OnPlayerRegistered;

        private void Awake()
        {
            Initialize();
        }
        private void Initialize()
        {
            if (!Initialized)
            {

                Initialized = true;
                InputUser.listenForUnpairedDeviceActivity++;
                userDevices = new List<UserDevice>();
                gameplayInputManagers = new List<PlayerGameplayInputManager>();
                uiInputManagers = new List<PlayerUIInputManager>();
                activePlayers = new List<PlayerSessionData>();
                if (Keyboard.current != null)
                {
                    for (int i = 0; i < keyboardSchemeNames.Count; i++)
                    {
                        InitializeUnusedDevice(Keyboard.current, keyboardSchemeNames[i]);
                    }
                }
                Subscribe();
                OnInputInitialized?.Invoke();
            }

        }

        private void InitializeUnusedDevice(InputDevice device, string schemeName)
        {
            int deviceIndex = userDevices.Count;
            UserDevice uDevice = new UserDevice(deviceIndex, device, schemeName);
            userDevices.Add(uDevice);
            PlayerGameplayInputManager gameplayInputManager = new PlayerGameplayInputManager(uDevice.gameplayInput);
            gameplayInputManagers.Add(gameplayInputManager);
            PlayerUIInputManager uiInputManager = new PlayerUIInputManager(uDevice.uiInput, deviceIndex);
            uiInputManagers.Add(uiInputManager);
            //uDevice.EnableUIInput(true);
            //This should be elsewhere or conditional
            /*SelectScreenPlayerController playerSelectController = new SelectScreenPlayerController(deviceIndex, uiInputManager, this);
            playerSelectControllers.Add(playerSelectController);*/
            OnDeviceRegistered?.Invoke(uDevice, gameplayInputManager, uiInputManager);
            Debug.Log($"Registering device {device.name} with scheme {schemeName}");
        }
        private PlayerSessionData CreatePlayerWithDevice(int deviceIndex)
        {
            PlayerSessionData playerSessionData = new PlayerSessionData(activePlayers.Count, deviceIndex, gameplayInputManagers[deviceIndex]);
            activePlayers.Add(playerSessionData);
            OnPlayerRegistered?.Invoke(playerSessionData);
            Debug.Log($"Creating player id: {playerSessionData.playerIndex} with device id {deviceIndex}");
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
            if (Initialized && !subscribed)
            {
                subscribed = true;
                //InputUser.onChange += OnControlsChanged;
                InputUser.onUnpairedDeviceUsed += ListenForUnpairedGamepads;
            }

        }
        private void Unsubscribe()
        {
            if (Initialized && subscribed)
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
        public int GetDeviceCount() => userDevices.Count;
        public UserDevice GetDevice(int deviceIndex)
        {
            return userDevices[deviceIndex];
        }
        public PlayerGameplayInputManager GetGameplayInputManager(int deviceIndex)
        {
            return gameplayInputManagers[deviceIndex];
        }
        public PlayerUIInputManager GetUIInputManager(int deviceIndex)
        {
            return uiInputManagers[deviceIndex];
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