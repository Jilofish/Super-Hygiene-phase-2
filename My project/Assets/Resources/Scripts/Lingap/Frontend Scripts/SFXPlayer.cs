using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    public AudioClip[] sfxClips; // [0] = Correct, [1] = Wrong
    public AudioSource audioSource;

    public void PlayCorrect()
    {
        if (audioSource && sfxClips.Length > 0 && sfxClips[0] != null)
        {
            audioSource.PlayOneShot(sfxClips[0]);
            Debug.Log("Playing Correct SFX");
        }
        else
        {
            Debug.LogWarning("Correct SFX not configured!");
        }
    }

    public void PlayWrong()
    {
        if (audioSource && sfxClips.Length > 1 && sfxClips[1] != null)
        {
            audioSource.PlayOneShot(sfxClips[1]);
            Debug.Log("Playing Wrong SFX");
        }
        else
        {
            Debug.LogWarning("Wrong SFX not configured!");
        }
    }
}
