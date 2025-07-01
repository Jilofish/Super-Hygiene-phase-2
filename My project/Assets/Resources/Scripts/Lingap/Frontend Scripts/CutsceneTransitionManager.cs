using UnityEngine;
using UnityEngine.UI;

public class CutsceneTransitionManager : MonoBehaviour
{
    [Header("Area Loader Reference")]
    [SerializeField] private AreaLoader areaLoader; // Assign in inspector
    [Header("Panels")]
    [SerializeField] private GameObject levelCutscenePanel;
    [SerializeField] private GameObject startCutscenePanel;
    [SerializeField] private GameObject endingCutscenePanel;

    [Header("Start Cutscene Image")]
    [SerializeField] private Image startCutsceneImage;

    [Header("Ending Cutscene Button (with Image component)")]
    [SerializeField] private Button endingCutsceneButton;

    [Header("Cutscene Sprites Per Level")]
    [SerializeField] private Image levelBackgroundImage; 
    [SerializeField] private Sprite[] levelCutsceneSprites;    // Optional
    [SerializeField] private Sprite[] startCutscene1Sprites;   // First scene per level
    [SerializeField] private Sprite[] startCutscene2Sprites;   // Second scene per level
    [SerializeField] private Sprite[] endingCutsceneSprites;   // One per level

    [Header("Task Entry Points Per Level")]
    [SerializeField] private GameObject[] taskEntryPoints;     // Activate the task logic per level

    private int currentLevelIndex = -1;
    private int currentStartSceneIndex = 0;

    // Called from intro button's OnClick
    public void OpenLevelCutscene()
    {
        levelCutscenePanel.SetActive(true);
    }

    // Called from AreaButton's OnClick (e.g., Level 1 button: OnLevelSelected(0))
    public void OnLevelSelected(int index)
    {
        currentLevelIndex = index;
        currentStartSceneIndex = 0;

        // Set the level background
        if (levelBackgroundImage != null && index >= 0 && index < levelCutsceneSprites.Length)
        {
            levelBackgroundImage.sprite = levelCutsceneSprites[index];
            Debug.Log($"Set background to Level {index + 1}");
        }

        // Show start cutscene 1
        if (startCutscenePanel != null && startCutsceneImage != null)
        {
            startCutscenePanel.SetActive(true);
            startCutsceneImage.sprite = startCutscene1Sprites[index];
        }
    }


    // Called from invisible button(s) in StartCutscenePanel
    public void OnNextStartCutscene()
    {
        if (currentStartSceneIndex == 0)
        {
            currentStartSceneIndex = 1;
            startCutsceneImage.sprite = startCutscene2Sprites[currentLevelIndex];
            Debug.Log($"Showing START cutscene 2 for Level {currentLevelIndex + 1}");
        }
        else
        {
            startCutscenePanel.SetActive(false);

            // Call LoadArea AFTER second cutscene
            areaLoader.LoadArea(currentLevelIndex);
            Debug.Log($"Triggered LoadArea({currentLevelIndex}) after start cutscene.");
        }
    }


    public void PlayEndingCutscene()
    {
        endingCutscenePanel.SetActive(true);

        if (currentLevelIndex >= 0 && currentLevelIndex < endingCutsceneSprites.Length)
            endingCutsceneButton.image.sprite = endingCutsceneSprites[currentLevelIndex];
    }

    public void LoadNextLevelCutscene()
    {
        int nextLevel = currentLevelIndex + 1;

        if (nextLevel < startCutscene1Sprites.Length)
        {
            currentLevelIndex = nextLevel;
            currentStartSceneIndex = 0;

            // Update background
            if (levelBackgroundImage != null && nextLevel < levelCutsceneSprites.Length)
            {
                levelBackgroundImage.sprite = levelCutsceneSprites[nextLevel];
                Debug.Log($"Updated background to Level {nextLevel + 1}");
            }

            // Start the next level's StartCutscene
            endingCutscenePanel.SetActive(false);
            startCutsceneImage.sprite = startCutscene1Sprites[nextLevel];

            Debug.Log($"Loaded StartCutscene 1 for Level {nextLevel + 1}");
        }
        else
        {
            // No more levels
            endingCutscenePanel.SetActive(false);
            Debug.Log("All levels completed!");
        }
    }


}
