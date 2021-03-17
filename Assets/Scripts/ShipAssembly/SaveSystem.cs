using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.IO;

public class AllSaveData
{
    public const int CurrentSaveVersion = 2;
    public int saveVersion = 0;
    [SerializeField]
    public List<ShipData> shipData = new List<ShipData>();
    public int currentShipID = 0;

    public static AllSaveData GetDefaultSaveData()
    {
        return LoadDefaultSaveData();
    }

    private static AllSaveData LoadDefaultSaveData()
    {
        AllSaveDataSO allSaveDataSO = Resources.Load<AllSaveDataSO>("DefaultSaveData");
        if (allSaveDataSO != null)
        {
            var saveData = allSaveDataSO.GetSaveData();
            saveData.saveVersion = CurrentSaveVersion;
            return saveData;
        }
        else
        {
            Debug.LogError("DefaultSaveData not found!");
            AllSaveData allSaveData = new AllSaveData();
            allSaveData.shipData.Add(ShipData.GetDefaultShipData());
            allSaveData.currentShipID = 0;
            return allSaveData;
        }

    }

}
public static class SaveSystem
{
    private const string saveFolderName = "/SaveData/";
    private const string saveFileName = "save.nbld";

    public static void SaveSingleShip(ShipData ship, bool selectAsActive)
    {
        AllSaveData saveData = LoadData();
        saveData.shipData.Add(ship);
        if (selectAsActive)
        {
            saveData.currentShipID = saveData.shipData.Count - 1;
        }
        SaveData(saveData);
    }

    public static void SaveData(AllSaveData allSaveData)
    {
        DirectoryInfo dirInfo = Directory.CreateDirectory(Application.persistentDataPath + saveFolderName);

        string json = JsonUtility.ToJson(allSaveData);
        StreamWriter sw = File.CreateText(GetFullPath());
        sw.Write(json);
        sw.Close();
    }

    public static AllSaveData LoadData()
    {
        string path = GetFullPath();
        AllSaveData saveData;
        if (File.Exists(path))
        {
            string textData = File.ReadAllText(path);
            saveData = JsonUtility.FromJson<AllSaveData>(textData);
            if (saveData.saveVersion != AllSaveData.CurrentSaveVersion)
            {
                //Handle differences between versions
                VersionAdjuster.AdjustSaveData(saveData, AllSaveData.CurrentSaveVersion);
                SaveData(saveData);
            }
            return saveData;
        }
        else
        {
            saveData = AllSaveData.GetDefaultSaveData();
            SaveData(saveData);
            return saveData;
        }
    }

    private static string GetFullPath()
    {
        return Application.persistentDataPath + saveFolderName + saveFileName;
    }
}

public static class VersionAdjuster
{
    public static AllSaveData AdjustSaveData(AllSaveData saveData, int targetVersion)
    {
        if (saveData.saveVersion == 1 && saveData.saveVersion != targetVersion)
        {
            AdjustV1ToV2(saveData);
        }
        saveData.saveVersion = targetVersion;
        return saveData;
    }

    private static void AdjustV1ToV2(AllSaveData saveData)
    {
        for (int i = 0; i < saveData.shipData.Count; i++)
        {
            var shipData = saveData.shipData[i];
            for (int j = 0; j < shipData.availableSystems.Count; j++)
            {
                shipData.AddAvailableSystem(shipData.systemDatas[j].system);
            }
        }
    }
}