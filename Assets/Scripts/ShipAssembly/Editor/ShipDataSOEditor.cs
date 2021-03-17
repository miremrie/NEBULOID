using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(ShipDataSO))]
public class ShipDataSOEditor : Editor
{
    private ShipDataSO shipDataSO;
    private int unconfirmedRoom = -1;
    private List<RoomName> availableRooms;
    private int curSelectedRoom = 0;
    private int currentRoomCount = 0;
    private int maxRoomCount = 0;
    private List<bool> isRoomGroupShown = new List<bool>();

    private void OnEnable()
    {
        shipDataSO = (ShipDataSO)target;
        SetupData();
    }

    private void SetupData()
    {
        if (shipDataSO.shipData == null)
        {
            shipDataSO.shipData = new ShipData();
        }
        availableRooms = new List<RoomName>();

        availableRooms = Enum.GetValues(typeof(RoomName)).Cast<RoomName>().ToList();
        maxRoomCount = availableRooms.Count;
        for (int i = 0; i < shipDataSO.shipData.systemDatas.Count; i++)
        {
            availableRooms.Remove(shipDataSO.shipData.systemDatas[i].room);
            isRoomGroupShown.Add(false);
        }
        unconfirmedRoom = -1;
        curSelectedRoom = 0;
        currentRoomCount = shipDataSO.shipData.systemDatas.Count;

    }

    public override void OnInspectorGUI()
    {

        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        shipDataSO.shipData.shipName = EditorGUILayout.TextField(new GUIContent("Ship Name"), shipDataSO.shipData.shipName);
        target.name = shipDataSO.shipData.shipName;
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < shipDataSO.shipData.systemDatas.Count; i++)
        {
            isRoomGroupShown[i] = EditorGUILayout.Foldout(isRoomGroupShown[i], shipDataSO.shipData.systemDatas[i].room.ToString());
            if (isRoomGroupShown[i])
            {
                shipDataSO.shipData.systemDatas[i] = DrawSingleRoom(shipDataSO.shipData.systemDatas[i], i);
            }

        }
        if (unconfirmedRoom != -1)
        {
            DrawChooseRoom();
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginDisabledGroup(unconfirmedRoom != -1 || currentRoomCount == maxRoomCount);
        if (GUILayout.Button("Assign new room"))
        {
            unconfirmedRoom = currentRoomCount;
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorUtility.SetDirty(target);
    }
    public void DrawChooseRoom()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        curSelectedRoom = EditorGUILayout.Popup("Room Name", curSelectedRoom, AvailableRoomNames());
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("Confirm"))
        {
            shipDataSO.shipData.AddSysData(SelectARoom(curSelectedRoom));
            unconfirmedRoom = -1;
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }
    public SystemData DrawSingleRoom(SystemData systemData, int sysDataIndex)
    {
        SystemData sysData = systemData;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        //RoomName
        /*EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField(sysData.room.ToString());
        EditorGUILayout.EndVertical();*/
        //SystemName
        EditorGUILayout.BeginHorizontal();
        sysData.system = (SystemName)EditorGUILayout.EnumPopup("System", sysData.system);
        EditorGUILayout.EndHorizontal();
        //Position
        EditorGUILayout.BeginHorizontal();
        sysData.SetPosition(EditorGUILayout.Vector3Field("Position", sysData.GetPosition()));
        EditorGUILayout.EndHorizontal();
        //Rotation
        EditorGUILayout.BeginHorizontal();
        sysData.SetRotation(EditorGUILayout.Vector3Field("Rotation", sysData.GetRotation().eulerAngles));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        return sysData;
    }

    private SystemData SelectARoom(int availableRoom)
    {
        RoomName room = availableRooms[availableRoom];
        availableRooms.Remove(room);
        SystemData sysData = SystemData.GetDefaultSystemData();
        sysData.room = room;
        unconfirmedRoom = -1;
        isRoomGroupShown.Add(true);
        currentRoomCount++;
        curSelectedRoom = 0;
        return sysData;
    }

    private string[] AvailableRoomNames()
    {
        string[] avRoomStr = new string[availableRooms.Count];
        for (int i = 0; i < availableRooms.Count; i++)
        {
            avRoomStr[i] = availableRooms[i].ToString();
        }
        return avRoomStr;
    }
}
