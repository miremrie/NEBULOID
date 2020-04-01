using CustomInput;
using NBLD.Character;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

namespace NBLD.Input
{
    public class UserDevice
    {
        public InputUser user;
        public Gamepad gamepad;
        private string keyboardScheme;
        public CharacterInput inputActions;
        private bool gamepadSet = false;
        private bool gamepadActive = false;

        public UserDevice(string keyboardScheme, string gamepadScheme)
        {
            this.user = InputUser.CreateUserWithoutPairedDevices();
            CustomInput.CharacterInput input = new CustomInput.CharacterInput();
            this.inputActions = input;
            user.AssociateActionsWithUser(inputActions);

            this.keyboardScheme = keyboardScheme;            
            ActivateKeyboard();
        }

        public void SetGamepad(Gamepad gamepad)
        {
            gamepadSet = true;
            this.gamepad = gamepad;
        }

        public void ActivateGamepad()
        {
            gamepadActive = true;
            InputUser.PerformPairingWithDevice(gamepad, user);
            //user.ActivateControlScheme(gamepadScheme).AndPairRemainingDevices();
            user.AssociateActionsWithUser(inputActions);
        }

        public void DeactivateGamepad()
        {
            ActivateKeyboard();
        }

        public void ActivateKeyboard()
        {
            gamepadActive = false;
            InputUser.PerformPairingWithDevice(Keyboard.current, user);
            inputActions.Enable();
            user.AssociateActionsWithUser(inputActions);
            user.ActivateControlScheme(keyboardScheme);
        }

        public bool HasGamepadSet()
        {
            return gamepadSet;
        }

        public bool IsUsingGamepad()
        {
            return gamepadActive;
        }

        public void Dispose()
        {
            inputActions.Dispose();
            user.UnpairDevicesAndRemoveUser();
        }
    }
    public class InputManager : MonoBehaviour
    {
        public int numberOfUsers;
        public List<CharInputManager> characterManagers;
        public List<string> usersKeyboardSchemeNames;
        public string gamepadSchemeName;
        public int firstGamepadUser = 1;
        private CustomInput.CharacterInput characterInput;
        private List<UserDevice> users;
        private List<Gamepad> takenGamepads = new List<Gamepad>();

        void Start()
        {
            InputUser.listenForUnpairedDeviceActivity = numberOfUsers;
            users = new List<UserDevice>();

            for (var i = 0; i < numberOfUsers; i++)
            {
                users.Add(new UserDevice(usersKeyboardSchemeNames[i], gamepadSchemeName));
                //characterManagers[i].InitializeInput(users[i]);
            }
            
            Subscribe();
        }

        private void Subscribe()
        {
            //InputUser.onChange += OnControlsChanged;
            InputUser.onUnpairedDeviceUsed += ListenForUnpairedGamepads;
        }

        private void OnDisable()
        {
            Unsubscribe();
            for(int i = 0; i < users.Count; i++)
            {
                users[i].Dispose();
            }
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Unsubscribe()
        {
            //InputUser.onChange -= OnControlsChanged;
            InputUser.onUnpairedDeviceUsed -= ListenForUnpairedGamepads;
        }



        void OnControlsChanged(InputUser user, InputUserChange change, InputDevice device)
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
        }

        void ListenForUnpairedGamepads(InputControl control, InputEventPtr inputEventPtr)
        {
            if (control.device is Gamepad)
            {
                for (var i = 0; i < users.Count; i++)
                {
                    int index = (firstGamepadUser + i) % users.Count;
                    // find a user without a paired device
                    if (!users[index].HasGamepadSet() && !takenGamepads.Contains((Gamepad)control.device))
                    {
                        // pair the new Gamepad device to that user
                        takenGamepads.Add((Gamepad)control.device);
                        users[index].SetGamepad((Gamepad)control.device);
                        users[index].ActivateGamepad();
                        return;
                    } else if (users[index].gamepad == (Gamepad)control.device)
                    {
                        users[index].ActivateGamepad();
                    }
                }
            }
        }
    }
}

