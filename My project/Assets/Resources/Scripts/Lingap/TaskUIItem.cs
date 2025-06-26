using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskUIItem : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text taskText;
    public Image checkboxImage;

    [Header("Checkbox Sprites")]
    public Sprite checkedSprite;
    public Sprite uncheckedSprite;

    [Header("Task Info")]
    public string taskID;
    [TextArea] public string descriptionOverride;

    private void OnEnable()
    {
        if (TaskManager.Instance != null)
            TaskManager.Instance.onTaskUpdated += Refresh;
    }

    private void OnDisable()
    {
        if (TaskManager.Instance != null)
            TaskManager.Instance.onTaskUpdated -= Refresh;
            
        
    }

    private void Start()
    {
        StartCoroutine(WaitForTaskManagerReady());
    }

    public void SetCustomDescription(string desc)
    {
        descriptionOverride = desc;
    }

    private System.Collections.IEnumerator WaitForTaskManagerReady()
    {
        // Wait until TaskManager has loaded the area
        while (!TaskManager.Instance.IsAreaReady())
            yield return null;

        // Wait until the task is registered
        while (!TaskManager.Instance.IsTaskRegistered(taskID))
            yield return null;

        Refresh();
    }

    public void Refresh()
    {
        if (string.IsNullOrEmpty(taskID) || TaskManager.Instance == null)
            return;

        bool isComplete = TaskManager.Instance.IsTaskComplete(taskID);

        // Set checkbox image sprite
        if (checkboxImage != null)
            checkboxImage.sprite = isComplete ? checkedSprite : uncheckedSprite;

        // Update task text
        string label = string.IsNullOrEmpty(descriptionOverride) ? taskID.Replace('_', ' ') : descriptionOverride;
        taskText.text = $"{label}";
    }
}
