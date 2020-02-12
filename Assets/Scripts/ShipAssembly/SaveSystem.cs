using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.IO;

public class AllSaveData
{
    [SerializeField]
    public List<ShipData> shipData = new List<ShipData>();
    public int currentShipID = 0;

    public static AllSaveData GetDefaultSaveData()
    {
        AllSaveData allSaveData = new AllSaveData();
        allSaveData.shipData.Add(ShipData.GetDefaultShipData());
        allSaveData.currentShipID = 0;
        return allSaveData;
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
        //BinaryFormatter formatter = new BinaryFormatter();
        DirectoryInfo dirInfo = Directory.CreateDirectory(Application.persistentDataPath + saveFolderName);

        //FileStream stream = new FileStream(GetFullPath(), FileMode.Create);
        string json = JsonUtility.ToJson(allSaveData);
        //Debug.Log("Writing:\n" + json);
        StreamWriter sw = File.CreateText(GetFullPath());
        sw.Write(json);
        sw.Close();
        //File.WriteAllText(GetFullPath(), json);
        //formatter.Serialize(stream, allSaveData);
        //stream.Close();
    }

    public static AllSaveData LoadData()
    {
        string path = GetFullPath();
        AllSaveData saveData;
        if (File.Exists(path))
        {
            //BinaryFormatter formatter = new BinaryFormatter();
            //FileStream stream = new FileStream(path, FileMode.Open);
            //saveData = (AllSaveData)formatter.Deserialize(stream);
            //Debug.Log(path);
            string textData = File.ReadAllText(path);
            saveData = JsonUtility.FromJson<AllSaveData>(textData);
            //stream.Close();
            //Debug.Log("Loading:\n" + textData);
            return saveData;
        } else
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
