using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.IO;

public class AllSaveData
{
    public List<ShipData> shipData;
    public int currentShipID;
    
}

public static class SaveSystem
{
    private const string saveFolderName = "/SaveData/";
    private const string saveFileName = "save.nbld";

    public static void SaveData(AllSaveData allSaveData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        DirectoryInfo dirInfo = Directory.CreateDirectory(Application.persistentDataPath + saveFolderName);
        
        FileStream stream = new FileStream(GetFullPath(), FileMode.Create);

        formatter.Serialize(stream, allSaveData);
        stream.Close();
    }

    public static AllSaveData LoadData()
    {
        string path = GetFullPath();
        AllSaveData saveData;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            saveData = (AllSaveData)formatter.Deserialize(stream);
            stream.Close();
            return saveData;
        } else
        {
            return null;
        }
    }

    private static string GetFullPath()
    {
        return Application.persistentDataPath + saveFolderName + saveFileName;
    }
}
