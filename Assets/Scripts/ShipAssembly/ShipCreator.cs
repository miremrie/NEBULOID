﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace NBLD.ShipCreation
{
    public enum CreationStage
    {
        CreateName, SelectRoom, SelectSystem, PlaceSystem, Finished, Confirmed, Canceled
    }

    public class ShipCreator : MonoBehaviour
    {
        public ShipAssembler shipAssembler;
        [SerializeField]
        public ShipData shipData = new ShipData();
        public CreationStage currentStage = CreationStage.CreateName;
        private SystemData curSysData;
        //public delegate void ShipDataPrepared(ShipData shipData, bool newShip);
        //public delegate void CreationStageChanged(CreationStage stage, CreationStage previousStage);
        public event System.Action<CreationStage, CreationStage> onCreationStageChanged;
        public event System.Action<ShipData, bool> onShipDataPrepared;
        public bool hardSaveShip = true;
        public bool areRoomsReassignable = true;
        public bool updateAssembler;
        private List<GameObject> sampleSystems = new List<GameObject>();

        private void OnEnable()
        {
            shipAssembler.SetGameplayMechanicsEnabled(false);
            shipAssembler.DestroyCurrentAssignableSystems();
        }

        public void LoadData()
        {
            ShipData newShipData = shipAssembler.GetShipData();

            if (shipData == null)
            {
                //New Ship
                shipData = new ShipData();
                ChangeStage(CreationStage.CreateName);
            }
            else
            {
                shipData = new ShipData();
                ChangeStage(CreationStage.SelectRoom);
                foreach (SystemData sysData in newShipData.systemDatas)
                {
                    AssignFullSystemData(sysData);
                }
            }
            if (onShipDataPrepared != null)
            {
                onShipDataPrepared(shipData, hardSaveShip);
            }
        }

        private void ChangeStage(CreationStage creationStage)
        {
            CreationStage previousStage = currentStage;
            currentStage = creationStage;
            if (onCreationStageChanged != null)
            {
                onCreationStageChanged(creationStage, previousStage);
            }
        }

        public void SetShipName(string name)
        {
            shipData.shipName = name;
            ChangeStage(CreationStage.SelectRoom);
        }

        private void AssignFullSystemData(SystemData sysData)
        {
            if (currentStage == CreationStage.SelectRoom)
            {
                SelectRoom(sysData.room);
                SelectSystem(sysData.system);
                PlaceSystem(sysData.GetRotationEuler());
            }
            else
            {
                throw new ShipCreationException($"Can't Assign Full System Data: Invalid CreationStage: {currentStage}");
            }
        }

        public void SelectRoom(RoomName name)
        {
            if (currentStage == CreationStage.SelectRoom)
            {
                if (areRoomsReassignable || !shipData.systemDatas.Exists(sysData => sysData.room == name))
                {
                    curSysData = new SystemData();
                    curSysData.room = name;
                    ChangeStage(CreationStage.SelectSystem);
                }
                else
                {
                    throw new ShipCreationException($"Can't select room: Room {name} is already assigned too and rooms are not reassignable.");
                }
            }
            else
            {
                throw new ShipCreationException($"Can't select room: Invalid CreationStage: {currentStage}");
            }
        }

        public void SelectSystem(SystemName name)
        {
            if (currentStage == CreationStage.SelectSystem)
            {
                if (IsSystemAvailable(name))
                {
                    curSysData.system = name;
                    ChangeStage(CreationStage.PlaceSystem);
                }
                else
                {
                    throw new ShipCreationException($"Can't select system: System would exceed max system count for that system type: {name}");
                }

            }
            else
            {
                throw new ShipCreationException($"Can't select system: Invalid CreationStage: {currentStage}");
            }
        }

        public void PlaceSystem(Vector3 rotation)
        {
            if (currentStage == CreationStage.PlaceSystem)
            {
                curSysData.SetRotation(rotation);
                if (IsRoomOccuppied(curSysData.room))
                {
                    shipData.systemDatas.RemoveAll(sysData => sysData.room == curSysData.room);
                }
                shipData.AddSysData(curSysData);
                if (!areRoomsReassignable && shipData.systemDatas.Count >= shipAssembler.assignableRooms.Length)
                {
                    ChangeStage(CreationStage.Finished);
                }
                else
                {
                    ChangeStage(CreationStage.SelectRoom);
                }
            }
            else
            {
                throw new ShipCreationException($"Can't place system: Invalid CreationStage: {currentStage}");
            }
        }

        public RoomName GetCurrentEditRoom()
        {
            if (currentStage == CreationStage.SelectSystem || currentStage == CreationStage.PlaceSystem)
            {
                return curSysData.room;
            }
            else
            {
                throw new ShipCreationException("No room is currently being edited");
            }
        }

        public SystemName GetCurrentEditSystem()
        {
            if (currentStage == CreationStage.PlaceSystem)
            {
                return curSysData.system;
            }
            else
            {
                throw new ShipCreationException("No system is currently being edited");
            }
        }

        public Vector3 GetCurrentEditRotation()
        {
            return curSysData.GetRotationEuler();
        }

        public bool IsRoomAvailable(RoomName name)
        {
            return areRoomsReassignable || !IsRoomOccuppied(name);
        }
        public bool IsRoomOccuppied(RoomName name)
        {
            return shipData.systemDatas.Exists(sys => sys.room == name);
        }
        public bool IsSystemAvailable(SystemName name)
        {
            int systemCount = shipData.systemDatas.Count(sysData => sysData.system == name);
            return shipAssembler.GetMaxSystemsOnShip(name) > systemCount;
        }
        public int GetAttachableSystemsCount()
        {
            return shipAssembler.attachableSystems.Length;
        }
        public SystemName GetAttachableSystemNameForID(int systemID)
        {
            return shipAssembler.attachableSystems[systemID].name;
        }
        public void HideAllSystems()
        {
            foreach (ShipSystemWithID systemID in shipAssembler.attachableSystems)
            {
                systemID.root.SetActive(false);
            }
        }
        public void ShowSystem(SystemName systemName)
        {
            foreach (ShipSystemWithID systemID in shipAssembler.attachableSystems)
            {
                if (systemID.name == systemName)
                {
                    systemID.root.SetActive(true);
                }
                else
                {
                    systemID.root.SetActive(false);
                }
            }
        }

        public GameObject CreateSampleSystem(SystemName sysName)
        {
            foreach (ShipSystemWithID systemID in shipAssembler.attachableSystems)
            {
                if (systemID.name == sysName)
                {
                    GameObject go = Instantiate(systemID.root, shipAssembler.systemsRoot);
                    sampleSystems.Add(go);
                    go.SetActive(true);
                    return go;
                }
            }
            return null;
        }
        public void ConfirmShip()
        {
            shipData.FillEmptyData();
            DestroySamples();
            if (hardSaveShip)
            {
                SaveSystem.SaveSingleShip(shipData, true);
            }
            if (updateAssembler)
            {
                shipAssembler.ReassembleShip(shipData);
            }
            ChangeStage(CreationStage.Confirmed);
        }

        public void CancelShip()
        {
            DestroySamples();
            if (updateAssembler)
            {
                shipAssembler.ReassembleShipFromCurrent();
            }
            ChangeStage(CreationStage.Canceled);
        }
        private void DestroySamples()
        {
            foreach (GameObject sample in sampleSystems)
            {
                Destroy(sample);
            }
            sampleSystems.Clear();
        }
    }

    public class ShipCreationException : System.Exception
    {
        public ShipCreationException(string message) : base(message) { }
    }

}
