using UnityEngine;

public class PlayCustsceneAudio : MonoBehaviour
{
    [System.Serializable]
    public class AudioTrigger
    {
        public GameObject targetObject;
        public AudioClip audioClip;
        [HideInInspector] public bool hasPlayed = false;
    }

    [Header("Audio Triggers")]
    public AudioTrigger[] triggers;

    private AudioSource audioSource;
    private int currentlyPlayingIndex = -1;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = 1.0f; // Max volume (range is 0.0 to 1.0)

    }

    void Update()
    {
        for (int i = 0; i < triggers.Length; i++)
        {
            var trigger = triggers[i];

            // If the object is active and the clip hasn't played
            if (trigger.targetObject.activeInHierarchy && !trigger.hasPlayed)
            {
                PlayAudio(trigger.audioClip, i);
                trigger.hasPlayed = true;
            }

            // If the object becomes inactive and it was the one playing
            if (!trigger.targetObject.activeInHierarchy && trigger.hasPlayed)
            {
                trigger.hasPlayed = false;

                if (currentlyPlayingIndex == i && audioSource.isPlaying)
                {
                    audioSource.Stop();
                    currentlyPlayingIndex = -1;
                }
            }
        }
    }

    void PlayAudio(AudioClip clip, int triggerIndex)
    {
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.volume = 1.0f; // You can set a custom volume here
            audioSource.Play();
            currentlyPlayingIndex = triggerIndex;
        }
        else
        {
            Debug.LogWarning("No audio clip assigned.");
        }
    }
}
