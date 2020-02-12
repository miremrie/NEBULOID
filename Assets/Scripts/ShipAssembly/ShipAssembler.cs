using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RoomControlWithID {
    public RoomName name;
    public RoomControl control;
}
[System.Serializable]
public struct ShipSystemWithID
{
    public SystemName name;
    public GameObject root;
    public int maxSystemsOnShip;
}

public class ShipAssembler : MonoBehaviour
{
    public RoomControlWithID[] rooms;
    public ShipSystemWithID[] systems;
    public Transform systemsRoot;
    private AllSaveData saveData;
    private ShipData curShipData;
    public bool inEditMode = false;

    private void Awake()
    {
        if (!inEditMode)
        {
            saveData = SaveSystem.LoadData();
            curShipData = saveData.shipData[saveData.currentShipID];
            AssembleShip(curShipData);
        }
    }

    public void AssembleShip(ShipData shipData)
    {
        foreach(SystemData systemData in shipData.systemDatas)
        {
            RoomControl roomControl = GetRoomControl(systemData.room);
            ShipSystem shipSystem = CreateSystemObject(systemData.system, systemData.GetPosition(), systemData.GetRotation());
            if (shipSystem == null)
            {
                Debug.LogError("Tried to create non existant system: " + systemData.system);
            } else
            {
                shipSystem.Reinitialize();
                roomControl.shipSystem = shipSystem;
                roomControl.hasAssignedSystem = true;
            }

        }
    }

    private RoomControl GetRoomControl(RoomName roomName)
    {
        for (int i = 0; i < rooms.Length; i++)
        {
            if (rooms[i].name == roomName)
            {
                return rooms[i].control;
            }
        }
        return null;
    }

    private ShipSystem CreateSystemObject(SystemName systemName, Vector3 position, Quaternion rotation)
    {
        for (int i = 0; i < systems.Length; i++)
        {
            if (systems[i].name == systemName)
            {
                GameObject go = Instantiate(systems[i].root, systemsRoot);
                go.transform.rotation = rotation;
                go.transform.position = position;
                go.SetActive(true);
                return go.GetComponentInChildren<ShipSystem>();
            }
        }
        return null;
    }
}
