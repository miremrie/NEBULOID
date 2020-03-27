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
        private InputControlScheme keyboardScheme;
        public IInputActionCollection inputActions;
        private bool gamepadSet = false;
        private bool gamepadActive = false;
        private InputControlScheme gamepadScheme;

        public UserDevice(string keyboardScheme, string gamepadScheme)
        {
            this.user = InputUser.CreateUserWithoutPairedDevices();
            CharacterInput input = new CharacterInput();
            
            this.inputActions = input;
            user.AssociateActionsWithUser(inputActions);

            this.keyboardScheme = this.inputActions.controlSchemes.First(cs => cs.name == keyboardScheme);
            this.gamepadScheme = this.inputActions.controlSchemes.First(cs => cs.name == gamepadScheme);

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

            user.ActivateControlScheme(keyboardScheme);
            inputActions.Enable();
            user.AssociateActionsWithUser(inputActions);
        }

        public bool HasGamepadSet()
        {
            return gamepadSet;
        }

        public bool IsUsingGamepad()
        {
            return gamepadActive;
        }
    }
    public class InputManager : MonoBehaviour
    {
        public int numberOfUsers;
        public InsideCharController[] charControllers;
        public string[] usersKeyboardSchemeNames;
        public string gamepadSchemeName;
        public int firstGamepadUser = 1;
        private CharacterInput characterInput;
        private UserDevice[] users;
        private List<Gamepad> takenGamepads = new List<Gamepad>();

        void Start()
        {
            InputUser.listenForUnpairedDeviceActivity = numberOfUsers;
            users = new UserDevice[numberOfUsers];

            for (var i = 0; i < numberOfUsers; i++)
            {
                users[i] = new UserDevice(usersKeyboardSchemeNames[i], gamepadSchemeName);
                charControllers[i].InitializeInput(users[i]);
            }

            Subscribe();
        }

        private void Subscribe()
        {
            InputUser.onChange += OnControlsChanged;
            InputUser.onUnpairedDeviceUsed += ListenForUnpairedGamepads;
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Unsubscribe()
        {
            InputUser.onChange -= OnControlsChanged;
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
                for (var i = 0; i < users.Length; i++)
                {
                    int index = (firstGamepadUser + i) % users.Length;
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

