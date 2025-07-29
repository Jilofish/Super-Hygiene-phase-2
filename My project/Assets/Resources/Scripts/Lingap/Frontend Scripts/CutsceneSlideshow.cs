using System.Collections;
using UnityEngine;

public class CutsceneSlideshow : MonoBehaviour
{
    public CanvasGroup[] slides;
    public float fadeDuration = 1f;
    public float holdDuration = 2f;
    public GameObject slideshowContainer;
    public ChangeSceneFromButton sceneChanger;  
    public string nextSceneName;               

    void Awake()
    {
        foreach (CanvasGroup cg in slides)
        {
            cg.alpha = 0f;
            cg.gameObject.SetActive(false);
        }

        slides[0].alpha = 1f;
        slides[0].gameObject.SetActive(true);
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
            CanvasGroup current = slides[i];
            CanvasGroup previous = slides[i - 1];

            current.gameObject.SetActive(true);
            yield return StartCoroutine(FadeIn(current));
            yield return new WaitForSeconds(holdDuration);
            yield return StartCoroutine(FadeOut(previous));
            previous.gameObject.SetActive(false);
        }

        // Fade out the last slide
        yield return StartCoroutine(FadeOut(slides[^1]));
        slides[^1].gameObject.SetActive(false);

        // Deactivate slideshow container
        if (slideshowContainer != null)
        {
            slideshowContainer.SetActive(false);
        }

        // ✅ Trigger scene load through your existing scene changer
        if (sceneChanger != null && !string.IsNullOrEmpty(nextSceneName))
        {
            sceneChanger.ChangeScene(nextSceneName);
        }
    }

    IEnumerator FadeIn(CanvasGroup cg)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        cg.alpha = 1f;
    }

    IEnumerator FadeOut(CanvasGroup cg)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }
        cg.alpha = 0f;
    }
}
