using System.Collections;
using UnityEngine;

public class CutsceneSlideshow : MonoBehaviour
{
    public GameObject[] slides;
    public float holdDuration = 4f;
    public GameObject slideshowContainer;
    public ChangeSceneFromButton sceneChanger;
    public string nextSceneName;

    void Awake()
    {
        foreach (GameObject slide in slides)
        {
            slide.SetActive(false);
        }

        if (slides.Length > 0)
        {
            slides[0].SetActive(true);
        }
    }

    void Start()
    {
        InitiateSlides();
    }

    public void InitiateSlides()
    {
        StartCoroutine(PlaySlideshow());
    }

    IEnumerator PlaySlideshow()
    {
        for (int i = 1; i < slides.Length; i++)
        {
            GameObject current = slides[i];
            GameObject previous = slides[i - 1];

            yield return new WaitForSeconds(holdDuration);

            previous.SetActive(false);
            current.SetActive(true);
        }

        yield return new WaitForSeconds(holdDuration);

        slides[^1].SetActive(false);

        if (slideshowContainer != null)
        {
            slideshowContainer.SetActive(false);
        }

        if (sceneChanger != null && !string.IsNullOrEmpty(nextSceneName))
        {
            sceneChanger.ChangeScene(nextSceneName);

            if (nextSceneName == "Title")
            {
                DestroyTaskManager();
            }
        }
    }

    public void DestroyTaskManager()
    {
        if (TaskManager.Instance != null)
        {
            Destroy(TaskManager.Instance.gameObject);
        }
    }
}
