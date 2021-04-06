using System.Collections;
using System.Collections.Generic;
using NBLD.Character;
using NBLD.Data;
using UnityEngine;

namespace NBLD.Input
{
    public class DefaultInputAssigner : MonoBehaviour
    {
        [System.Serializable]
        public struct DefaultPlayerSessionData
        {
            public DeviceType deviceType;
            public int characterSkinIndex;
            public int devotionNameIndex;
            public int spiritNameIndex;
            public CharToolType charToolType;
        }
        public CharAssembler charAssembler;
        public CharacterSkins characterSkins;
        public CharacterNames characterNames;
        public List<DefaultPlayerSessionData> defaultDeviceTypes = new List<DefaultPlayerSessionData>();
        private List<DefaultPlayerSessionData> defaultDevicesExpected = new List<DefaultPlayerSessionData>();
        private bool initialized = false;
        private bool subscribed = false;

        public void Awake()
        {
            if (InputManager.Instance != null && InputManager.Instance.Initialized)
            {
                Initialize();
            }
        }
        private void Initialize()
        {
            initialized = true;
            if (!InputManager.Instance.useDefault || InputManager.Instance.GetPlayerCount() > 0)
            {
                Destroy(this);
                return;
            }

            defaultDevicesExpected = new List<DefaultPlayerSessionData>();
            for (int i = 0; i < defaultDeviceTypes.Count; i++)
            {
                defaultDevicesExpected.Add(defaultDeviceTypes[i]);
            }
            int deviceCount = InputManager.Instance.GetDeviceCount();
            for (int i = defaultDevicesExpected.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < deviceCount; j++)
                {
                    UserDevice device = InputManager.Instance.GetDevice(j);
                    TryToAssignDefaultDevice(device);
                }
            }
            Subscribe();
        }
        private void Subscribe()
        {
            if (initialized && !subscribed)
            {
                subscribed = true;
                InputManager.Instance.OnDeviceRegistered += OnDeviceRegistered;
            }
        }

        private void Unsubscribe()
        {
            if (initialized && subscribed)
            {
                subscribed = false;
                InputManager.Instance.OnDeviceRegistered -= OnDeviceRegistered;
            }
        }

        private void OnEnable()
        {
            Subscribe();
        }
        private void OnDisable()
        {
            Unsubscribe();
        }
        private void Update()
        {
            if (!initialized && InputManager.Instance != null && InputManager.Instance.Initialized)
            {
                Initialize();
            }
        }
        private void OnDeviceRegistered(UserDevice userDevice, PlayerGameplayInputManager gameplayInputManager, UIInputManager uiInputManager)
        {
            TryToAssignDefaultDevice(userDevice);
        }
        private void TryToAssignDefaultDevice(UserDevice userDevice)
        {
            if (defaultDevicesExpected == null || defaultDevicesExpected.Count == 0)
            {
                return;
            }
            for (int i = defaultDevicesExpected.Count - 1; i >= 0; i--)
            {
                if (!InputManager.Instance.IsDeviceUsed(userDevice.deviceIndex) && userDevice.deviceType == defaultDevicesExpected[i].deviceType)
                {
                    var skin = characterSkins.GetSkinData(defaultDevicesExpected[i].characterSkinIndex);
                    var devotionName = characterNames.GetDevotionName(defaultDevicesExpected[i].devotionNameIndex);
                    var spiritName = characterNames.GetSpiritName(defaultDevicesExpected[i].spiritNameIndex);
                    var charToolType = defaultDevicesExpected[i].charToolType;
                    if (InputManager.Instance.RegisterPlayer(userDevice.deviceIndex, skin, devotionName, spiritName, charToolType))
                    {
                        PlayerSessionData psData = InputManager.Instance.GetPlayerByDevice(userDevice.deviceIndex);
                        charAssembler.CreateCharacter(psData);
                        defaultDevicesExpected.RemoveAt(i);
                        return;
                    }
                }
            }
            return;
        }
    }

}