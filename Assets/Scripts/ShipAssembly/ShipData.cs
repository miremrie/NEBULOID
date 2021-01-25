using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SystemName
{
    Sonar, LeftArm, RightArm, Gun, Hook, Ejector, Shield
}
public enum RoomName
{
    F0, F1R, F2L, F2R, F3, F1L
}
[System.Serializable]
public class ShipData
{
    [SerializeField]
    public List<SystemData> systemDatas = new List<SystemData>();
    public string shipName;
    public ShipData()
    {

    }

    public void AddSysData(SystemData sysData)
    {
        systemDatas.Add(sysData);
    }

    public static ShipData GetDefaultShipData()
    {
        ShipData shipData = new ShipData();
        shipData.shipName = "NEBULOID";
        //Sonar;
        SystemData F0Data = new SystemData(Vector3.zero, Quaternion.identity, SystemName.Sonar, RoomName.F0);
        //Gun
        SystemData F1RData = new SystemData(Vector3.zero, Quaternion.identity, SystemName.Gun, RoomName.F1R);
        //RightArm
        SystemData F2RData = new SystemData(Vector3.zero, Quaternion.identity, SystemName.RightArm, RoomName.F2R);
        //LeftArm
        SystemData F2LData = new SystemData(Vector3.zero, Quaternion.identity, SystemName.LeftArm, RoomName.F2L);
        //Hook
        SystemData HookData = new SystemData(Vector3.zero, Quaternion.identity, SystemName.Hook, RoomName.F3);

        shipData.systemDatas.Add(F0Data);
        shipData.systemDatas.Add(F1RData);
        shipData.systemDatas.Add(F2RData);
        shipData.systemDatas.Add(F2LData);
        shipData.systemDatas.Add(HookData);

        return shipData;
    }

    public void FillEmptyData()
    {
        foreach (SystemData sysData in systemDatas)
        {
            sysData.FillEmptyData();
        }
    }
}

[System.Serializable]
public class SystemData
{
    public float[] rotation;
    public float[] position;
    [SerializeField]
    public SystemName system;
    [SerializeField]
    public RoomName room;

    public SystemData()
    {

    }

    public SystemData(Vector3 pos, Quaternion rot, SystemName system, RoomName room)
    {
        this.rotation = SplitVector(pos);
        this.position = SplitVector(rot.eulerAngles);
        this.system = system;
        this.room = room;
    }
    public void SetRotation(Vector3 rotation)
    {
        this.rotation = SplitVector(rotation);
    }
    private static float[] SplitVector(Vector3 vector)
    {
        float[] components = new float[3];
        components[0] = vector.x;
        components[1] = vector.y;
        components[2] = vector.z;
        return components;
    }

    public Quaternion GetRotation()
    {
        return Quaternion.Euler(GetRotationEuler());
    }

    public Vector3 GetRotationEuler()
    {
        return new Vector3(rotation[0], rotation[1], rotation[2]);
    }

    public Vector3 GetPosition()
    {
        return new Vector3(position[0], position[1], position[2]);
    }

    public void SetRotation(Quaternion rotation)
    {
        this.rotation = SplitVector(rotation.eulerAngles);
    }

    public void SetRotationEuler(Vector3 eulerRotation)
    {
        this.rotation = SplitVector(eulerRotation);
    }

    public void SetPosition(Vector3 position)
    {
        this.position = SplitVector(position);
    }

    public void FillEmptyData()
    {
        if (rotation == null)
        {
            rotation = SplitVector(Quaternion.identity.eulerAngles);
        }
        if (position == null)
        {
            position = SplitVector(Vector3.zero);
        }
    }

    public static SystemData GetDefaultSystemData()
    {
        SystemData sysData = new SystemData();
        sysData.position = SplitVector(Vector3.zero);
        sysData.rotation = SplitVector(Quaternion.identity.eulerAngles);
        return sysData;
    }
}
