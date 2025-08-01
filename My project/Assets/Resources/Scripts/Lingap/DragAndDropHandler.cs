using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class DragAndDropHandler : MonoBehaviour
{
    [SerializeField] private string taskID; 
    [SerializeField] private SFXPlayer sfxPlayer;
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
        if (sfxPlayer == null)
    {
        sfxPlayer = FindFirstObjectByType<SFXPlayer>();
    }

    }

    void Update()
    {
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

IEnumerator MoveBackToOriginalPosition()
{
    float duration = 0.5f;
    float elapsed = 0f;
    Vector3 startPos = transform.position;

    float shakeAmplitude = 0.2f; // How far left and right
    int shakeFrequency = 8;      // How many full shakes during duration

    while (elapsed < duration)
    {
        float t = elapsed / duration;

        // Main return movement
        Vector3 smoothPos = Vector3.Lerp(startPos, originalPosition, t);

        // Horizontal "no" shake using sine wave
        float shakeX = Mathf.Sin(t * Mathf.PI * 2 * shakeFrequency) * shakeAmplitude * (1f - t); // fade out at end
        Vector3 shakeOffset = new Vector3(shakeX, 0f, 0f);

        transform.position = smoothPos + shakeOffset;

        elapsed += Time.deltaTime;
        yield return null;
    }

    transform.position = originalPosition;
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
                    sfxPlayer?.PlayCorrect();
                    if (!string.IsNullOrEmpty(taskID))
                    {
                        TaskManager.Instance.IncrementTask(taskID);
                    }

                    Destroy(gameObject);
                    return;
                }
            }
        }
        sfxPlayer?.PlayWrong();
        StartCoroutine(MoveBackToOriginalPosition());
        transform.position = originalPosition;
    }
    bool IsValidDropTarget(GameObject target)
    {
        return (CompareTag("Toy") && target.CompareTag("ToyBox")) || 
                (CompareTag("Clothes") && target.CompareTag("ClothesBin")) ||
               (CompareTag("Trash") && target.CompareTag("TrashBin"));
    }
}