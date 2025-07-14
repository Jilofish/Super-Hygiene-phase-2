using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class FaucetHandleController : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 200f;
    public float minAngle = -90f; // Fully open
    public float maxAngle = 0f;   // Fully closed

    [Header("Proceed Button (UI Canvas)")]
    public GameObject proceedButton; // Canvas UI button shown when faucet is open

    [Header("Optional Task Tracking")]
    [SerializeField] private string taskID;

    private Camera cam;
    private bool isDragging = false;
    private Vector2 startPos;

    void Awake()
    {
        cam = Camera.main;
        if (proceedButton != null) proceedButton.SetActive(false);
    }

    void Update()
    {
        if (Touchscreen.current?.primaryTouch.press.isPressed == true)
        {
            HandleTouch();
        }
        else if (Mouse.current?.leftButton.isPressed == true)
        {
            HandleMouse();
        }
    }

    void HandleMouse()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 pos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            if (IsTouchingHandle(pos))
            {
                isDragging = true;
                startPos = pos;
            }
        }

        if (isDragging && Mouse.current.leftButton.isPressed)
        {
            Vector2 pos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            float deltaX = pos.x - startPos.x;
            RotateHandle(deltaX);
            startPos = pos;
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
        }
    }

    void HandleTouch()
    {
        var touch = Touchscreen.current.primaryTouch;

        if (touch.press.wasPressedThisFrame)
        {
            Vector2 pos = cam.ScreenToWorldPoint(touch.position.ReadValue());
            if (IsTouchingHandle(pos))
            {
                isDragging = true;
                startPos = pos;
            }
        }

        if (isDragging && touch.press.isPressed)
        {
            Vector2 pos = cam.ScreenToWorldPoint(touch.position.ReadValue());
            float deltaX = pos.x - startPos.x;
            RotateHandle(deltaX);
            startPos = pos;
        }

        if (touch.press.wasReleasedThisFrame)
        {
            isDragging = false;
        }
    }

    bool IsTouchingHandle(Vector2 worldPos)
    {
        var col = GetComponent<Collider2D>();
        return col != null && col.OverlapPoint(worldPos);
    }

    void RotateHandle(float deltaX)
    {
        float currentY = transform.localEulerAngles.y;
        if (currentY > 180f) currentY -= 360f; // Normalize to -180 ~ 180

        float newY = currentY + (-deltaX * rotationSpeed * Time.deltaTime);
        newY = Mathf.Clamp(newY, minAngle, maxAngle);

        transform.localEulerAngles = new Vector3(0f, newY, 0f);

        // Show button when fully open
        if (Mathf.Abs(newY - minAngle) < 1f)
        {
            if (proceedButton != null && !proceedButton.activeSelf)
            {
                Debug.Log("✅ Faucet is fully opened! Proceed button unlocked.");
                proceedButton.SetActive(true);
            }
        }
    }

    public void OnProceedPressed()
    {
        Debug.Log("✅ Proceed button clicked!");

        if (proceedButton != null)
            proceedButton.SetActive(false);

        if (!string.IsNullOrEmpty(taskID))
            TaskManager.Instance.IncrementTask(taskID);

        // Optional: disable faucet minigame or load next step here
        transform.parent.gameObject.SetActive(false); // assumes handle is child of faucet minigame
    }
}
