using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CreationStage
{
    SelectRoom, SelectSystem, PlaceSystem, Finished
}

public class ShipCreator : MonoBehaviour
{
    public ShipAssembler shipAssembler;
    [SerializeField]
    public ShipData shipData;
    public CreationStage currentStage = CreationStage.SelectRoom;
    private SystemData curSysData;

    private void Start()
    {
        shipData = new ShipData();
    }

    public void SetShipName(string name)
    {
        shipData.shipName = name;
    }

    public void OnRoomSelected(RoomName name)
    {
        curSysData = new SystemData();
        curSysData.room = name;
        currentStage = CreationStage.SelectSystem;
    }

    public void OnSystemSelected(SystemName name)
    {
        curSysData.system = name;
        currentStage = CreationStage.PlaceSystem;
        HideAllSystems();
    }

    public void OnSystemPlaced(Vector3 rotation)
    {
        curSysData.SetRotation(rotation);
        shipData.AddSysData(curSysData);
        if (shipData.systemDatas.Count >= shipAssembler.rooms.Length)
        {
            currentStage = CreationStage.Finished;
        } else
        {
            currentStage = CreationStage.SelectRoom;
        }
    }

    public bool IsSystemAvailable(SystemName systemName)
    {
        int counter = 0;
        foreach (SystemData sysData in shipData.systemDatas)
        {
            if (sysData.system == systemName)
            {
                counter++;
            }
        }
        foreach (ShipSystemWithID systemID in shipAssembler.systems)
        {
            if (systemID.name == systemName && counter >= systemID.maxSystemsOnShip)
            {
                return false;
            }
        }
        return true;
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
}
