using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelUnlockManager : MonoBehaviour
{
    [Header("Level Buttons (in order)")]
    [SerializeField] public Button[] levelButtons;

    [Header("Area Loader Reference")]
    [SerializeField] private AreaLoader areaLoader;

    [Header("Continue Button")]
    [SerializeField] public GameObject continueButton;

    [Header("Black Overlay")]
    [SerializeField] public GameObject blackOverlay;

    [Header("Stars Animation")]
    [SerializeField] public Transform[] stars;   // drag 3 stars in Inspector
    [SerializeField] private float starDelay = 0.5f;
    [SerializeField] private float popTime = 0.3f;

    [Header("Tracking")]
    [SerializeField] private int unlockedLevelIndex = 0;

    private void Start()
    {
        UpdateButtonStates();

        continueButton.SetActive(false);
        if (blackOverlay != null) blackOverlay.SetActive(false);

        // Hide stars initially
        if (stars != null)
        {
            foreach (var s in stars)
            {
                if (s != null)
                    s.localScale = Vector3.zero;
            }
        }
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

            // Show Continue Button
            continueButton.SetActive(true);

            // Show Black Overlay
            if (blackOverlay != null)
                blackOverlay.SetActive(true);

            // 🎉 Trigger stars animation
            if (stars != null && stars.Length > 0)
                StartCoroutine(PlayStarsSequentially());
        }
    }

    private IEnumerator PlayStarsSequentially()
    {
        foreach (var star in stars)
        {
            if (star == null) continue;

            yield return StartCoroutine(PopStar(star));
            yield return new WaitForSeconds(starDelay);
        }
    }


    private IEnumerator PopStar(Transform star)
    {
        float elapsed = 0f;

        // Grow star (pop effect)
        while (elapsed < popTime)
        {
            float t = elapsed / popTime;
            float scale = Mathf.Lerp(0f, 1.2f, t);
            star.localScale = Vector3.one * scale;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Settle back to normal size
        star.localScale = Vector3.one;
    }

    public void OnContinueButtonClicked()
    {
        // 🔹 Hide stars for reuse in the next level
        foreach (var star in stars)
        {
            if (star != null)
                star.localScale = Vector3.zero;
        }

        // 🔹 Clear task UI (your existing method)
        ClearTaskUI();

        // 🔹 Hide Continue button
        continueButton.SetActive(false);

        // 🔹 Hide black overlay if present
        if (blackOverlay != null)
            blackOverlay.SetActive(false);

        // 🔹 Unlock next level if available
        if (unlockedLevelIndex < levelButtons.Length - 1)
        {
            unlockedLevelIndex++;
            UpdateButtonStates();
            Debug.Log($"🔓 Next level unlocked: Level {unlockedLevelIndex + 1}");
        }
        else
        {
            Debug.Log("🎉 All levels completed. Waiting for future content.");
        }
    }


    private void UpdateButtonStates()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i < unlockedLevelIndex)
            {
                levelButtons[i].interactable = false;
                levelButtons[i].gameObject.SetActive(false);
            }
            else if (i == unlockedLevelIndex)
            {
                levelButtons[i].interactable = true;
                levelButtons[i].gameObject.SetActive(true);
            }
            else
            {
                levelButtons[i].interactable = false;
                levelButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void DisableCurrentAreaGroup()
    {
        string currentAreaID = areaLoader.areaID;

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
        foreach (Transform child in areaLoader.AreaTaskChecker)
        {
            Destroy(child.gameObject);
        }

        Debug.Log("🧹 Cleared all Task UI items.");
    }

    public void ResetValues()
    {
        unlockedLevelIndex = 0;
        UpdateButtonStates();
        continueButton.SetActive(false);

        if (blackOverlay != null)
            blackOverlay.SetActive(false);

        ClearTaskUI();
        Debug.Log("🔄 All values have been reset!");
    }
}
