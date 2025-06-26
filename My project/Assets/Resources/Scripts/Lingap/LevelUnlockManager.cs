using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelUnlockManager : MonoBehaviour
{
    [Header("Level Buttons (in order)")]
    [SerializeField] private Button[] levelButtons;

    [Header("Area Loader Reference")]
    [SerializeField] private AreaLoader areaLoader;

    [Header("Continue Button")]
    [SerializeField] private GameObject continueButton;

    [Header("Tracking")]
    [SerializeField] private int unlockedLevelIndex = 0;

    private void Start()
    {
        UpdateButtonStates();
        continueButton.SetActive(false);
    }

    public void OnLevelButtonClicked(int index)
    {
        areaLoader.LoadArea(index);
    }
    private void OnEnable()
    {
        StartCoroutine(WaitForAreaReady());
    }

    private IEnumerator WaitForAreaReady()
    {
        while (TaskManager.Instance == null || !TaskManager.Instance.IsAreaReady())
            yield return null;

        TaskManager.Instance.onTaskUpdated += CheckAndEnableContinue;

        // Optional: check once after loading
        CheckAndEnableContinue();
    }

    private void OnDisable()
    {
        if (TaskManager.Instance != null)
            TaskManager.Instance.onTaskUpdated -= CheckAndEnableContinue;
    }
    public void CheckAndEnableContinue()
    {
        Debug.Log("👀 LevelUnlockManager.CheckAndEnableContinue() called");
        if (TaskManager.Instance.AreAllTasksCompleteInArea())
        {
            Debug.Log("✅ All tasks complete for current area.");
            continueButton.SetActive(true);
        }
    }

    public void OnContinueButtonClicked()
    {
        ClearTaskUI();
        continueButton.SetActive(false);

        if (unlockedLevelIndex < levelButtons.Length - 1)
        {
            unlockedLevelIndex++;
            UpdateButtonStates();
            Debug.Log($"🔓 Next level unlocked: Level {unlockedLevelIndex + 1}");
        }
        else
        {
            Debug.Log("🎉 All levels completed. Waiting for future content.");
            // Reserved for future behavior (like end screen)
        }
    }

    private void UpdateButtonStates()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            levelButtons[i].interactable = (i == unlockedLevelIndex);
        }
    }
    public void DisableCurrentAreaGroup()
    {
        string currentAreaID = areaLoader.areaID; // create a public getter in AreaLoader if needed

        foreach (GameObject area in areaLoader.areaGroups)
        {
            if (area.name == currentAreaID)
            {
                area.SetActive(false);
                Debug.Log($"🛑 Area group '{currentAreaID}' has been disabled.");
                return;
            }
        }

        Debug.LogWarning($"⚠️ No matching area group found for ID: {currentAreaID}");
    }
    public void ClearTaskUI()
    {
        foreach (Transform child in areaLoader.taskUIContainer)
        {
            Destroy(child.gameObject);
        }

        Debug.Log("🧹 Cleared all Task UI items.");
    }
}