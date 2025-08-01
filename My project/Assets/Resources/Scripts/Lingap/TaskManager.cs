using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }

    public event Action onTaskUpdated;
    private string currentAreaID = "area1";
    private Dictionary<string, Dictionary<string, int>> areaProgress = new();
    private Dictionary<string, Dictionary<string, int>> areaGoals = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            TriggerReload();
        }
        else
        {
            Debug.Log("Deleting new GameManager");

            // 🔹 Call LoadGame() on the persistent instance
            if (Instance != null)
                Instance.TriggerReload();

            Destroy(gameObject); // Destroy the duplicate
        }
    }

    public void SetCurrentArea(string areaID)
    {
        currentAreaID = areaID;

        if (!areaProgress.ContainsKey(areaID))
            areaProgress[areaID] = new();

        if (!areaGoals.ContainsKey(areaID))
            areaGoals[areaID] = new();
    }

    public void RegisterTask(string taskID, int requiredCount)
    {
        if (!areaGoals[currentAreaID].ContainsKey(taskID))
        {
            areaGoals[currentAreaID][taskID] = requiredCount;

            if (!areaProgress[currentAreaID].ContainsKey(taskID))
                areaProgress[currentAreaID][taskID] = 0;
        }
    }

    public void IncrementTask(string taskID)
    {
        if (!areaProgress.ContainsKey(currentAreaID) || !areaProgress[currentAreaID].ContainsKey(taskID)) return;

        areaProgress[currentAreaID][taskID]++;
        onTaskUpdated?.Invoke();
        if (AreAllTasksCompleteInArea())
        {
            Debug.Log($"✅ All tasks complete for area: {currentAreaID}");
        }
    }

    public bool IsTaskComplete(string taskID)
    {
        if (!areaProgress.ContainsKey(currentAreaID) || !areaGoals.ContainsKey(currentAreaID)) return false;
        return areaProgress[currentAreaID].GetValueOrDefault(taskID, 0) >= areaGoals[currentAreaID].GetValueOrDefault(taskID, 0);
    }

    public int GetProgress(string taskID)
    {
        if (!areaProgress.ContainsKey(currentAreaID)) return 0;
        return areaProgress[currentAreaID].GetValueOrDefault(taskID, 0);
    }

    public int GetGoal(string taskID)
    {
        if (!areaGoals.ContainsKey(currentAreaID)) return 0;
        return areaGoals[currentAreaID].GetValueOrDefault(taskID, 0);
    }

    public bool AreAllTasksCompleteInArea()
    {
        if (!areaGoals.ContainsKey(currentAreaID))
        {
            Debug.LogWarning($"⚠️ No goal data found for area: {currentAreaID}");
            return false;
        }

        foreach (var task in areaGoals[currentAreaID])
        {
            if (!IsTaskComplete(task.Key))
                return false;
        }

        Debug.Log($"✅ All tasks complete for area: {currentAreaID}");
        return true;
    }

    public bool IsAreaReady()
    {
        return areaGoals.ContainsKey(currentAreaID);
    }

    public bool IsTaskRegistered(string taskID)
    {
        return areaGoals.ContainsKey(currentAreaID) && areaGoals[currentAreaID].ContainsKey(taskID);
    }

    public void SaveProgress()
    {
        foreach (var area in areaGoals)
        {
            if (!areaProgress.ContainsKey(area.Key))
                areaProgress[area.Key] = new();

            foreach (var task in area.Value)
            {
                if (!areaProgress[area.Key].ContainsKey(task.Key))
                    areaProgress[area.Key][task.Key] = 0;
            }
        }

        AreaTaskSaveData data = new AreaTaskSaveData
        {
            areaTaskProgress = areaProgress
        };

        SaveSystem.SaveTasks(data);
    }

    public void LoadProgress()
    {
        AreaTaskSaveData data = SaveSystem.LoadTasks();
        if (data == null) return;

        areaProgress = data.areaTaskProgress;
        onTaskUpdated?.Invoke();
    }
    public void TriggerReload()
    {
        LingapGameManager GM = GetComponent<LingapGameManager>();
        if (GM != null)
        {
            GM.LoadGame();
        }
        else
        {
            Debug.LogError("Persistent LingapGameManager not found!");
        }
        LevelUnlockManager LevelManager = GetComponent<LevelUnlockManager>();
        LevelManager.ResetValues();

    }
}
