using System.Collections.Generic;
using UnityEngine;

public class AreaLoader : MonoBehaviour
{
    [Header("Area Configuration")]
    [SerializeField] public GameObject[] areaGroups;

    [Header("Current Area Settings")]
    [SerializeField] private AreaConfig areaConfig;
    public string areaID; 
    [Header("UI Prefabs")]
    public GameObject taskUIContainer;
    public Transform AreaTaskChecker;
    public TaskUIItem taskUIPrefab;
    public AreaConfig[] areaConfigs;
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
            area.SetActive(area.name == areaID+"(Clone)");

        TaskManager.Instance.SetCurrentArea(areaID);

        // Register tasks from config
        foreach (var task in areaConfig.tasks)
        {
            TaskManager.Instance.RegisterTask(task.taskID, task.goalCount);
            TaskUIItem uiItem = Instantiate(taskUIPrefab, AreaTaskChecker,false);
            uiItem.taskID = task.taskID;
            uiItem.SetCustomDescription(task.description);
            Debug.Log("Instantiate UI item");
        }

        TaskManager.Instance.LoadProgress();
        levelUnlockManager.CheckAndEnableContinue();
        
    }
    
}
