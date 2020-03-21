using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

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
    public delegate void ShipDataPrepared(ShipData shipData, bool newShip);
    public delegate void CreationStageChanged(CreationStage stage);
    public event CreationStageChanged onCreationStageChanged;
    public event ShipDataPrepared onShipDataPrepared;
    public bool hardSaveShip = true;
    public bool areRoomsReassignable = true;

    private void Start()
    {

    }

    private void PrepareData()
    {
        shipData = shipAssembler.GetShipData();

        if (shipData == null)
        {
            //New Ship
            shipData = new ShipData();
            ChangeStage(CreationStage.CreateName);
        }
        if (onShipDataPrepared != null)
        {
            onShipDataPrepared(shipData, hardSaveShip);
        }
    }

    private void ChangeStage(CreationStage creationStage)
    {
        currentStage = creationStage;
        if (onCreationStageChanged != null)
        {
            onCreationStageChanged(creationStage);
        }
    }

    public void SetShipName(string name)
    {
        shipData.shipName = name;
        ChangeStage(CreationStage.SelectRoom);
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
        } else
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
            } else
            {
                throw new ShipCreationException($"Can't select system: System would exceed max system count for that system type: {name}");
            }

        } else
        {
            throw new ShipCreationException($"Can't select system: Invalid CreationStage: {currentStage}");
        }

        //HideAllSystems();
    }

    public void PlaceSystem(Vector3 rotation)
    {
        if (currentStage == CreationStage.PlaceSystem)
        {
            curSysData.SetRotation(rotation);
            shipData.AddSysData(curSysData);
            if (!areRoomsReassignable && shipData.systemDatas.Count >= shipAssembler.rooms.Length)
            {
                ChangeStage(CreationStage.Finished);
            }
            else
            {
                ChangeStage(CreationStage.SelectRoom);
            }
        } else
        {
            throw new ShipCreationException($"Can't place system: Invalid CreationStage: {currentStage}");
        }
    }

    public bool IsSystemAvailable(SystemName name)
    {
        int systemCount = shipData.systemDatas.Count(sysData => sysData.system == name);
        return shipAssembler.GetMaxSystemsOnShip(name) > systemCount;
    }
    public void HideAllSystems()
    {
        foreach (ShipSystemWithID systemID in shipAssembler.systems)
        {
            systemID.root.SetActive(false);
        }
    }
    public void ShowSystem(SystemName systemName)
    {
        foreach (ShipSystemWithID systemID in shipAssembler.systems)
        {
            if (systemID.name == systemName)
            {
                systemID.root.SetActive(true);
            } else
            {
                systemID.root.SetActive(false);
            }
        }
    }

    public GameObject CreateSampleSystem(SystemName sysName)
    {
        foreach (ShipSystemWithID systemID in shipAssembler.systems)
        {
            if (systemID.name == sysName)
            {
                GameObject go = Instantiate(systemID.root, shipAssembler.systemsRoot);
                go.SetActive(true);
                return go;
            }
        }
        return null;
    }
    public void ConfirmShip()
    {
        shipData.FillEmptyData();
        if (hardSaveShip)
        {
            SaveSystem.SaveSingleShip(shipData, true);
        }
        ChangeStage(CreationStage.Confirmed);
    }

    public void CancelShip()
    {
        ChangeStage(CreationStage.Canceled);
    }
}

public class ShipCreationException : System.Exception
{
    public ShipCreationException(string message) : base(message)
    {
    }
}
