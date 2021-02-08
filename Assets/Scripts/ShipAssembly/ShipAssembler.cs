using NBLD.UseActions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NBLD.Ship;

[System.Serializable]
public struct RoomControlWithID
{
    public RoomName name;
    public RoomControl control;
}
[System.Serializable]
public struct ShipSystemWithID
{
    public SystemName name;
    public GameObject root;
    public ShipSystem system;
    public int maxSystemsOnShip;
}
[System.Serializable]
public struct BuiltInSystemWithID
{
    public SystemName systemName;
    public GameObject systemRoot;
    public ShipSystem system;
    public RoomName roomName;
    public RoomControl roomControl;
}
public class ShipAssembler : MonoBehaviour
{
    public RoomControlWithID[] assignableRooms;
    public ShipSystemWithID[] attachableSystems;
    public Transform systemsRoot;
    private AllSaveData saveData;
    private ShipData curShipData;
    private List<GameObject> currentSystems = new List<GameObject>();
    public BuiltInSystemWithID[] builtInSystemRooms;

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
    public void ReassembleShipFromCurrent()
    {
        ReassembleShip(curShipData);
    }
    public void ReassembleShip(ShipData shipData)
    {
        foreach (SystemData systemData in curShipData.systemDatas)
        {
            RoomControl roomControl = GetRoomControl(systemData.room);
            roomControl.shipSystem = null;
            roomControl.hasAssignedSystem = false;
        }
        DestroyCurrentAssignableSystems();
        AssembleShip(shipData);
        SetGameplayMechanicsEnabled(true);
        curShipData = shipData;
    }
    public void DestroyCurrentAssignableSystems()
    {
        for (int i = 0; i < currentSystems.Count; i++)
        {
            Destroy(currentSystems[i]);
        }
        currentSystems.Clear();
    }
    public void AssembleShip(ShipData shipData)
    {
        foreach (SystemData systemData in shipData.systemDatas)
        {
            RoomControl roomControl = GetRoomControl(systemData.room);
            ShipSystem shipSystem = CreateSystemObject(systemData.system, systemData.GetPosition(), systemData.GetRotation());
            if (shipSystem == null)
            {
                Debug.LogError("Tried to create non existant system: " + systemData.system);
            }
            else
            {
                AssignSystemToRoomAndInit(shipSystem, roomControl);
            }

        }
        foreach (BuiltInSystemWithID systemRoom in builtInSystemRooms)
        {
            AssignSystemToRoomAndInit(systemRoom.system, systemRoom.roomControl);
        }
    }

    private void AssignSystemToRoomAndInit(ShipSystem system, RoomControl room)
    {
        system.Initialize();
        room.InitializeSystem(system);
        system.enabled = true;
        room.enabled = true;
    }

    private RoomControl GetRoomControl(RoomName roomName)
    {
        for (int i = 0; i < assignableRooms.Length; i++)
        {
            if (assignableRooms[i].name == roomName)
            {
                return assignableRooms[i].control;
            }
        }
        return null;
    }

    private ShipSystem CreateSystemObject(SystemName systemName, Vector3 position, Quaternion rotation)
    {
        for (int i = 0; i < attachableSystems.Length; i++)
        {
            if (attachableSystems[i].name == systemName)
            {
                GameObject go = Instantiate(attachableSystems[i].root, systemsRoot);
                go.transform.localRotation = rotation;
                //go.transform.position = position;
                go.SetActive(true);
                currentSystems.Add(go);

                return go.GetComponentInChildren<ShipSystem>();
            }
        }
        return null;
    }

    public ShipData GetShipData()
    {
        return curShipData;
    }

    public int GetMaxSystemsOnShip(SystemName name)
    {
        for (int i = 0; i < attachableSystems.Length; i++)
        {
            if (attachableSystems[i].name == name)
            {
                return attachableSystems[i].maxSystemsOnShip;
            }
        }
        Debug.LogWarning($"System entry for system: {name} doesn't exist");
        return 0;
    }

    public void SetGameplayMechanicsEnabled(bool enabled)
    {
        foreach (ShipSystemWithID sys in attachableSystems)
        {
            sys.system.enabled = enabled;
        }
        foreach (BuiltInSystemWithID builtIn in builtInSystemRooms)
        {
            builtIn.system.enabled = enabled;
            builtIn.roomControl.enabled = enabled;
        }
    }
}
