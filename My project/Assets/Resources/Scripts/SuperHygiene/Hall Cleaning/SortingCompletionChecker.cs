using UnityEngine;
using UnityEngine.UI;

public class SortingCompletionChecker : MonoBehaviour
{
    public Transform TrashContainer;
    public Button finishSortingButton;
    public Image praiseImage;

    public void CheckIfAllSorted()
    {
        if (TrashContainer.childCount == 0)
        {
            praiseImage.gameObject.SetActive(true);
            finishSortingButton.gameObject.SetActive(true);
            Debug.Log("🎉 All trash sorted in this group! Button enabled.");
        }
        else
        {
            Debug.Log($"🧮 Still {TrashContainer.childCount} trash objects left in {TrashContainer.name}");
        }
    }
}
