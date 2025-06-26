using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string SavePath => Application.persistentDataPath + "/taskdata.json";

    public static void SaveTasks(AreaTaskSaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        Debug.Log("Saving JSON: " + json); // Optional log
        File.WriteAllText(SavePath, json);
    }

    public static AreaTaskSaveData LoadTasks()
    {
        if (!File.Exists(SavePath)) return null;

        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<AreaTaskSaveData>(json);
    }
}
