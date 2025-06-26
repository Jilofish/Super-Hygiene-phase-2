using UnityEngine;
using UnityEngine.EventSystems;

public class FoodDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Canvas canvas;
    public string correctCategory;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    public Vector3 originalPosition;

    // Reference to FoodSortingManager
    public FoodSortingManager sortingManager;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        originalPosition = transform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        GameObject hitObject = eventData.pointerEnter;

        if (hitObject != null && hitObject.CompareTag(correctCategory))
        {
            Debug.Log($"{gameObject.name} sorted correctly into {correctCategory} bin.");
            gameObject.SetActive(false);

            sortingManager.CheckAllFoodSorted();
        }
        else
        {
            Debug.Log($"{gameObject.name} dropped in wrong bin or empty space.");
            rectTransform.position = originalPosition;
        }
    }
}
