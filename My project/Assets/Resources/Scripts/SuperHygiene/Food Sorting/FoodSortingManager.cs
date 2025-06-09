using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class FoodSortingManager : MonoBehaviour
{
    public Transform foodItemsParent;
    public GameObject proceedButton;
    public Image praiseImage;

    private List<FoodDragHandler> foodItems = new List<FoodDragHandler>();

    private void Start()
    {
        // Initialize food items
        foreach (Transform child in foodItemsParent)
        {
            FoodDragHandler handler = child.GetComponent<FoodDragHandler>();
            if (handler != null)
            {
                foodItems.Add(handler);
            }
        }

        // Initially hide the proceed button
        proceedButton.SetActive(false);
    }

    public void CheckAllFoodSorted()
    {
        foreach (Transform child in foodItemsParent)
        {
            if (child.gameObject.activeSelf)
            {
                return; // If any food item is still active, don't show the button
            }
        }

        // If all food items are sorted, show the proceed button
        proceedButton.SetActive(true);
        praiseImage.gameObject.SetActive(true);
    }

    public void OnProceedButtonClicked()
    {
        // Reset the positions and set active
        foreach (FoodDragHandler item in foodItems)
        {
            item.gameObject.SetActive(true);  // Reactivate the food item
            item.transform.position = item.GetComponent<FoodDragHandler>().originalPosition; // Reset to original position
        }

        // Hide the proceed button again for the next round
        proceedButton.SetActive(false);
    }
}
