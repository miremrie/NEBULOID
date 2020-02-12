using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SystemName
{
    Sonar, LeftArm, RightArm, Gun, Hook
}
public enum RoomName
{
    F0, F1R, F2L, F2R, F3
}

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

    public SystemData(string shipName, Transform systemRoot, SystemName system, RoomName room)
    {
        this.rotation = SplitVector(systemRoot.rotation.eulerAngles);
        this.position = SplitVector(systemRoot.position);
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
        return Quaternion.Euler(rotation[0], rotation[1], rotation[2]);
    }

    public Vector3 GetPosition()
    {
        return new Vector3(position[0], position[1], position[2]);
    }
}
