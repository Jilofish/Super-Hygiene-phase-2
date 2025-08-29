using UnityEngine;
using UnityEngine.UI;

public class CutsceneTransitionManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject startCutscenePanel;
    [SerializeField] private GameObject endingCutscenePanel;

    [Header("Cutscene GameObjects (Preloaded in Scene)")]
    [SerializeField] private GameObject[] startCutscene1Objects;
    [SerializeField] private GameObject[] startCutscene2Objects;
    [SerializeField] private GameObject[] endingCutsceneObjects;

    [Header("Cutscene Objects")]
    [SerializeField] private GameObject[] CutsceneObjects;

    [Header("Task Areas")]
    [SerializeField] public GameObject[] taskEntryPoints;

    public GameObject CurrentTaskEntryPoints;

    private int currentLevelIndex = -1;
    private int currentStartSceneIndex = 0;

    private GameObject activeStartCutscene;
    private GameObject activeEndingCutscene;

    // Called when player selects a level
    public void OnLevelSelected(int index)
    {
        currentLevelIndex = index;
        currentStartSceneIndex = 0;

        startCutscenePanel.SetActive(true);
        ClearStartCutscene();


        // Show StartCutscene 1
        if (index >= 0 && index < startCutscene1Objects.Length)
        {
            activeStartCutscene = startCutscene1Objects[index];
            activeStartCutscene.SetActive(true);
            Debug.Log($"Showing Start Cutscene 1 for Level {index + 1}");
        }
    }

    // Clicked on StartCutscene GameObject (button)
    public void OnNextStartCutscene()
    {
        if (currentStartSceneIndex == 0)
        {
            currentStartSceneIndex = 1;
            ClearStartCutscene();

            if (currentLevelIndex >= 0 && currentLevelIndex < startCutscene2Objects.Length)
            {
                activeStartCutscene = startCutscene2Objects[currentLevelIndex];
                activeStartCutscene.SetActive(true);
                Debug.Log($"Showing Start Cutscene 2 for Level {currentLevelIndex + 1}");
            }
        }
        else
        {
            ClearStartCutscene();
            startCutscenePanel.SetActive(false);

            if (currentLevelIndex >= 0 && currentLevelIndex < taskEntryPoints.Length)
            {
                taskEntryPoints[currentLevelIndex].SetActive(true);
                Debug.Log($"Activated Area_{currentLevelIndex + 1}");
            }
        }
    }

    public void PlayEndingCutscene()
    {
        endingCutscenePanel.SetActive(true);
        ClearEndingCutscene();

        if (currentLevelIndex >= 0 && currentLevelIndex < endingCutsceneObjects.Length)
        {
            activeEndingCutscene = endingCutsceneObjects[currentLevelIndex];
            activeEndingCutscene.SetActive(true);
            Debug.Log($"Showing Ending Cutscene for Level {currentLevelIndex + 1}");
        }
    }

    public void LoadNextLevelCutscene()
    {
        int nextLevel = currentLevelIndex + 1;

        ClearEndingCutscene();
        endingCutscenePanel.SetActive(false);

        if (nextLevel < startCutscene1Objects.Length)
        {
            currentLevelIndex = nextLevel;
            currentStartSceneIndex = 0;

            startCutscenePanel.SetActive(true);
            ClearStartCutscene();

            activeStartCutscene = startCutscene1Objects[nextLevel];
            activeStartCutscene.SetActive(true);
            Debug.Log($"Loaded Start Cutscene 1 for Level {nextLevel + 1}");
        }
        else
        {
            Debug.Log("All levels completed!");
        }
    }

    private void ClearStartCutscene()
    {
        if (activeStartCutscene != null)
        {
            activeStartCutscene.SetActive(false);
            activeStartCutscene = null;
        }
    }

    private void ClearEndingCutscene()
    {
        if (activeEndingCutscene != null)
        {
            activeEndingCutscene.SetActive(false);
            activeEndingCutscene = null;
        }
    }
    public void OpenCutsceneObjects(int index)
    {
        if (index >= 0 && index < CutsceneObjects.Length)
        {
            CutsceneObjects[index].SetActive(true);
        }
    }
    
}