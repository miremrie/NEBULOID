using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllSaveData", menuName = "CustomData/AllSaveData", order = 1)]
public class AllSaveDataSO : ScriptableObject
{
    [SerializeField]
    public ShipDataSO[] allShips;
    [SerializeField]
    public int selectedShip;

    public AllSaveData GetSaveData()
    {
        AllSaveData allSaveData = new AllSaveData();
        allSaveData.currentShipID = selectedShip;
        allSaveData.shipData = new List<ShipData>();
        for (int i = 0; i < allShips.Length; i++)
        {
            allSaveData.shipData.Add(allShips[i].shipData);
        }
        return allSaveData;
    }
}
