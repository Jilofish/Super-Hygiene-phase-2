using UnityEngine;
using ScratchCard;
using UnityEngine.UI;

public class SuperHygieneUIManager : MonoBehaviour
{

    public ScratchCardMaskUGUI dirtMask;
    public CharacterMaskController characterMaskController;
    public Button continueButton; 
    public Image praiseImage;
    private void CheckBathProgress()
    {
        float progress = dirtMask.GetRevealProgress() * 100f;
        if (progress >= 93f && !continueButton.gameObject.activeSelf)
            {
                continueButton.gameObject.SetActive(true);
                praiseImage.gameObject.SetActive(true);
            }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckBathProgress();
    }
    public void OnResettingMask()
    {
        dirtMask.Restore(); 
        characterMaskController.ApplyMaskBasedOnGender(); 
        continueButton.gameObject.SetActive(false); 
    }
}
