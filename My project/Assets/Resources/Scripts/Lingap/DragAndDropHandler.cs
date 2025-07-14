using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class DragAndDropHandler : MonoBehaviour
{
    [SerializeField] private string taskID; 
    private Camera cam;
    private bool isDragging = false;
    private Vector3 offset;
    private Vector3 originalPosition;

    void Awake()
    {
        cam = Camera.main;
    }

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
    // Prefer touch if available
        if (Touchscreen.current != null && 
            (Touchscreen.current.primaryTouch.press.isPressed ||
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame ||
            Touchscreen.current.primaryTouch.press.wasReleasedThisFrame))
        {
            HandleTouch();
        }
        else if (Mouse.current != null &&
                (Mouse.current.leftButton.wasPressedThisFrame ||
                Mouse.current.leftButton.isPressed ||
                Mouse.current.leftButton.wasReleasedThisFrame))
        {
            HandleMouse();
        }
    }

    void HandleMouse()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                isDragging = true;
                offset = transform.position - (Vector3)mousePos;
            }
        }

        if (Mouse.current.leftButton.isPressed && isDragging)
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            transform.position = mousePos + (Vector2)offset;
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && isDragging)
        {
            DropItem();
        }
    }

    void HandleTouch()
    {
        if (Touchscreen.current == null) return;

        var touch = Touchscreen.current.primaryTouch;
        Vector2 touchPos = cam.ScreenToWorldPoint(touch.position.ReadValue());

        if (touch.press.wasPressedThisFrame)
        {
            RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                isDragging = true;
                offset = transform.position - (Vector3)touchPos;
            }
        }

        if (touch.press.isPressed && isDragging)
        {
            transform.position = touchPos + (Vector2)offset;
        }

        if (touch.press.wasReleasedThisFrame && isDragging)
        {
            DropItem();
        }
    }

    void DropItem()
    {
        isDragging = false;

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.zero);

        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject != gameObject)
            {
                if (IsValidDropTarget(hit.collider.gameObject))
                {
                    Debug.Log("Dropped on valid target: " + hit.collider.name);

                    if (!string.IsNullOrEmpty(taskID))
                    {
                        TaskManager.Instance.IncrementTask(taskID);
                    }

                    Destroy(gameObject);
                    return;
                }
            }
        }

        transform.position = originalPosition;
    }
    bool IsValidDropTarget(GameObject target)
    {
        return (CompareTag("Toy") && target.CompareTag("ToyBox")) || 
                (CompareTag("Clothes") && target.CompareTag("ClothesBin")) ||
               (CompareTag("Trash") && target.CompareTag("TrashBin"));
    }
}