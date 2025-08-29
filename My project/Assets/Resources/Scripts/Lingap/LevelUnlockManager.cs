using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUnlockManager : MonoBehaviour
{
    [Header("Level Buttons (in order)")]
    [SerializeField] public Button[] levelButtons;
    [Header("Level Selector UI")]
    [SerializeField] public Image[] arrowImages; // drag arrow images in Inspector
    [SerializeField] public TMP_Text DialogText;
    [SerializeField] public GameObject LevelSelector; // messages for each level
    [SerializeField] public CutsceneTransitionManager cutsceneTransitionManager; // messages for each level



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
    public int CurrentLevel;
    private string template;

    private void Start()
    {

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
        template = DialogText.text;
        UpdateLevelIndicatorUI();
    }

    public void OnLevelButtonClicked(int index)
    {
        if (index != unlockedLevelIndex)
        {
            Debug.LogWarning($"⚠️ Level {index + 1} is locked.");
            return;
        }
        else if (index == unlockedLevelIndex)
        {
            Debug.Log($"➡️ Loading Level {index + 1}");
            LevelSelector.SetActive(false);
            cutsceneTransitionManager.OnLevelSelected(index);
            cutsceneTransitionManager.OpenCutsceneObjects(index);
            areaLoader.taskUIContainer.SetActive(true);
            DisableCurrentAreaGroup();
        }
        else
        {
            Debug.LogError($"❌ Invalid level index: {index}");
            return;
        }
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
            Debug.Log($"🔓 Next level unlocked: Level {unlockedLevelIndex + 1}");
        }
        else
        {
            Debug.Log("🎉 All levels completed. Waiting for future content.");
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
    public void UpdateLevelIndicatorUI()
    {
        CurrentLevel = unlockedLevelIndex + 1;
        DialogText.text = template.Replace("{level_Index}", CurrentLevel.ToString());
        // Hide all arrows first
        foreach (var arrow in arrowImages)
        {
            if (arrow != null)
                arrow.gameObject.SetActive(false);
        }
        arrowImages[unlockedLevelIndex].gameObject.SetActive(true);
    }
    public void ResetValues()
    {
        unlockedLevelIndex = 0;
        continueButton.SetActive(false);

        if (blackOverlay != null)
            blackOverlay.SetActive(false);

        ClearTaskUI();
        Debug.Log("🔄 All values have been reset!");
    }

}
