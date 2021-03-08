using System.Collections;
using System.Collections.Generic;
using GeneratedInputActions;
using NBLD.Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

namespace NBLD.Input
{
    public enum DeviceType { Keyboard, Gamepad, Unknown };

    public class UserDevice
    {
        public readonly int deviceIndex;
        public InputUser user;
        public InputDevice device;
        private string schemeName;
        public CharacterInput gameplayInput;
        public UIInput uiInput;
        public DeviceType deviceType;

        public UserDevice(int deviceIndex, InputDevice device, string schemeName)
        {
            if (device is Keyboard)
            {
                deviceType = DeviceType.Keyboard;
            }
            else if (device is Gamepad)
            {
                deviceType = DeviceType.Gamepad;
            }
            else
            {
                deviceType = DeviceType.Unknown;
            }
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
                user.AssociateActionsWithUser(gameplayInput);
                user.ActivateControlScheme(this.schemeName);
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
                user.AssociateActionsWithUser(uiInput);
                user.ActivateControlScheme(this.schemeName);
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
        //public PlayerData playerData;
        public PlayerGameplayInputManager gameplayInputManager;
        public PlayerUIInputManager uiInputManager;
        public string devotionName = "";
        public string spiritName = "";
        public CharacterSkinData skin;
        public string CharacterName => $"{devotionName} {spiritName}";
        public int deviceIndex;

        public PlayerSessionData(int playerIndex, int deviceIndex, PlayerGameplayInputManager charInputManager, PlayerUIInputManager uiInputManager)
        {
            this.playerIndex = playerIndex;
            this.deviceIndex = deviceIndex;
            this.gameplayInputManager = charInputManager;
            this.uiInputManager = uiInputManager;
            //playerData = new PlayerData();
        }
    }
    public class InputManager : MonoBehaviour
    {
        public int maxNumberOfPlayers;
        public List<PlayerGameplayInputManager> gameplayInputManagers;
        public List<PlayerUIInputManager> uiInputManagers;
        public List<string> keyboardSchemeNames;
        public string gamepadSchemeName;
        public bool useDefault = false;
        private List<UserDevice> userDevices;
        private List<PlayerSessionData> activePlayers;
        public bool Initialized
        {
            get; private set;
        } = false;

        private bool subscribed = false;
        public delegate void DeviceRegisteredHandler(UserDevice userDevice, PlayerGameplayInputManager gameplayInputManager, PlayerUIInputManager uiInputManager);
        public delegate void PlayerRegisteredHandler(PlayerSessionData playerSessionData);
        public delegate void PlayerChangedIndexHandler(PlayerSessionData playerSessionData, int oldPlayerIndex);
        public delegate void InitHandler();
        public delegate void UpdateHandler(float deltaTime);
        public static event InitHandler OnInputInitialized;
        public event DeviceRegisteredHandler OnDeviceRegistered;
        public event PlayerRegisteredHandler OnPlayerRegistered, OnPlayerRemoved;
        public event PlayerChangedIndexHandler OnPlayerChangedIndex;
        public static event UpdateHandler OnInputTick;

        #region Singleton
        public static InputManager Instance { get; private set; }
        #endregion
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
                Initialize();
            }
            else
            {
                Destroy(this);
            }
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
        private void InitializeDefaultSetup()
        {

        }

        private void InitializeUnusedDevice(InputDevice device, string schemeName)
        {
            int deviceIndex = userDevices.Count;
            UserDevice userDevice = new UserDevice(deviceIndex, device, schemeName);
            userDevices.Add(userDevice);
            PlayerGameplayInputManager gameplayInputManager = new PlayerGameplayInputManager(userDevice.gameplayInput);
            gameplayInputManagers.Add(gameplayInputManager);
            PlayerUIInputManager uiInputManager = new PlayerUIInputManager(userDevice.uiInput, deviceIndex);
            uiInputManagers.Add(uiInputManager);
            //uDevice.EnableUIInput(true);
            //This should be elsewhere or conditional
            /*SelectScreenPlayerController playerSelectController = new SelectScreenPlayerController(deviceIndex, uiInputManager, this);
            playerSelectControllers.Add(playerSelectController);*/
            OnDeviceRegistered?.Invoke(userDevice, gameplayInputManager, uiInputManager);
            Debug.Log($"Registering device {device.name} with scheme {schemeName}");
        }
        private PlayerSessionData CreatePlayerWithDevice(int deviceIndex, CharacterSkinData skinData = null, string devotionName = "", string spiritName = "")
        {
            PlayerSessionData playerSessionData = new PlayerSessionData(activePlayers.Count, deviceIndex, gameplayInputManagers[deviceIndex], uiInputManagers[deviceIndex]);
            playerSessionData.skin = skinData;
            playerSessionData.devotionName = devotionName;
            playerSessionData.spiritName = spiritName;
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
            if (userDevices != null)
            {
                for (int i = 0; i < userDevices.Count; i++)
                {
                    userDevices[i].Dispose();
                }
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


        private void Update()
        {
            OnInputTick?.Invoke(Time.deltaTime);
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
        public bool IsDeviceUsed(int deviceIndex)
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
        #region Players
        public bool RegisterPlayer(int deviceIndex, CharacterSkinData skinData = null, string devotionName = null, string spiritName = null)
        {
            if (deviceIndex.IsBetween(-1, userDevices.Count) && !IsDeviceUsed(deviceIndex))
            {
                CreatePlayerWithDevice(deviceIndex, skinData, devotionName, spiritName);
                return true;
            }
            return false;
        }
        public void RemovePlayer(int playerIndex)
        {
            if (activePlayers.Count > playerIndex)
            {
                var psData = activePlayers[playerIndex];
                activePlayers.RemoveAt(playerIndex);
                OnPlayerRemoved?.Invoke(psData);
                for (int i = playerIndex; i < maxNumberOfPlayers; i++)
                {
                    if (i < activePlayers.Count && activePlayers[i] != null)
                    {
                        PlayerSessionData pData = activePlayers[i];
                        pData.playerIndex = i;
                        OnPlayerChangedIndex?.Invoke(pData, i + 1);
                    }
                }
            }
        }
        public int GetPlayerCount()
        {
            return activePlayers.Count;
        }
        public PlayerSessionData GetPlayerSessionData(int playerIndex)
        {
            return activePlayers[playerIndex];
        }
        public void SetPlayerGameplayEnabled(int playerIndex, bool enable)
        {
            UserDevice device = GetDevice(activePlayers[playerIndex].deviceIndex);
            device.EnableGameplayInput(enable);
        }
        #endregion
    }
}