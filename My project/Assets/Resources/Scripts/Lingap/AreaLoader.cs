using System.Collections.Generic;
using UnityEngine;

public class AreaLoader : MonoBehaviour
{
    [Header("Area Configuration")]
    [SerializeField] public GameObject[] areaGroups;

    [Header("Current Area Settings")]
    [SerializeField] private AreaConfig areaConfig;
    [SerializeField] public string areaID; // assign AreaTaskChecker scriptable object
    [Header("UI Prefabs")]
    [SerializeField] public Transform taskUIContainer; // assign AreaTaskChecker
    [SerializeField] public TaskUIItem taskUIPrefab;   // assign TaskUIItem prefab
    [SerializeField] public AreaConfig[] areaConfigs;
    [SerializeField] private LevelUnlockManager levelUnlockManager;
    public void LoadArea(int areaIndex)
    {
        if (areaIndex < 0 || areaIndex >= areaConfigs.Length)
        {
            Debug.LogError($"Invalid area index: {areaIndex}");
            return;
        }

        areaConfig = areaConfigs[areaIndex];
        LoadArea();
    }

    private void LoadArea()
    {
        areaID = areaConfig.areaID;

        // Activate only the selected area
        foreach (GameObject area in areaGroups)
            area.SetActive(area.name == areaID);

        TaskManager.Instance.SetCurrentArea(areaID);

        // Register tasks from config
        foreach (var task in areaConfig.tasks)
        {
            TaskManager.Instance.RegisterTask(task.taskID, task.goalCount);

            // Instantiate UI item
            TaskUIItem uiItem = Instantiate(taskUIPrefab, taskUIContainer);
            uiItem.taskID = task.taskID;
            uiItem.SetCustomDescription(task.description);
        }

        TaskManager.Instance.LoadProgress();
        levelUnlockManager.CheckAndEnableContinue();
        
    }
    
}
