using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TrashSorter : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string correctBinTag;
    private Vector3 startPosition;
    private Transform originalParent;
    private CanvasGroup canvasGroup;
    public SortingCompletionChecker completionChecker;
    private Vector2 inputPosition;
    private InputAction pointerMovementAction;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Create input action for pointer movement (mouse or touch)
        pointerMovementAction = new InputAction("PointerMovement", InputActionType.Value, "<Pointer>/position");

        // Enable the action
        pointerMovementAction.Enable();
    }

    private void OnEnable()
    {
        // Bind action to callback
        pointerMovementAction.performed += OnPointerMoved;
    }

    private void OnDisable()
    {
        // Unbind action when the script is disabled
        pointerMovementAction.performed -= OnPointerMoved;

        // Disable the action
        pointerMovementAction.Disable();
    }

    private void OnPointerMoved(InputAction.CallbackContext context)
    {
        // Update the input position based on pointer (mouse or touch)
        inputPosition = context.ReadValue<Vector2>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = transform.position;
        originalParent = transform.parent;
        transform.SetParent(transform.root); // Bring to front

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = false;
        }

        Debug.Log($"🟡 Begin drag: {gameObject.name} | Expected bin tag: {correctBinTag}");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(inputPosition.x, inputPosition.y, 1f));
        worldPosition.z = 0f; 

        transform.position = worldPosition;

        transform.rotation = Quaternion.identity;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        GameObject hitBin = null;

        foreach (var result in results)
        {
            if (result.gameObject == gameObject) continue;

            Debug.Log($"🔎 Raycast hit: {result.gameObject.name} | Tag: {result.gameObject.tag}");

            if (result.gameObject.CompareTag(correctBinTag) && result.gameObject.GetComponent<TrashSorter>() == null)
            {
                hitBin = result.gameObject;
                break;
            }
        }

        if (hitBin != null)
        {
            Debug.Log($"✅ Correct bin! {gameObject.name} dropped on {hitBin.name} (Tag: {hitBin.tag})");
            Destroy(gameObject); 
            completionChecker.CheckIfAllSorted();
        }
        else
        {
            Debug.Log($"❌ Wrong bin! {gameObject.name} expected {correctBinTag}");
            transform.position = startPosition;
        }

        transform.SetParent(originalParent);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
    }
}
