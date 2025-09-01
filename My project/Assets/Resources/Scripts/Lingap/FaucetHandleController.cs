using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class FaucetHandleController : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 200f;
    public float minAngle; // Fully open
    public float maxAngle;   // Fully closed
    public Transform pivotPoint;  // ⬅️ New: the circular center of the handle
    public Transform handleArm;   // ⬅️ New: the actual handle to rotate

    [Header("Water Stream")]
    public SpriteRenderer waterRenderer; // ⬅️ New: assign the water sprite here

    [Header("Proceed Button (UI Canvas)")]
    public GameObject proceedButton;

    [Header("Optional Task Tracking")]
    [SerializeField] private string taskID;

    [Header("Faucet Audio")]
    public AudioSource faucetAudioSource;

    private Camera cam;
    private bool isDragging = false;
    private Vector2 startPos;
    private float currentAngle = 0f;

    void Awake()
    {
        cam = Camera.main;
        if (proceedButton != null) proceedButton.SetActive(false);
    }

    void Update()
    {
#if UNITY_EDITOR
        HandleMouse();
#else
        HandleTouch();
#endif
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
        var touch = Touchscreen.current?.primaryTouch;

        if (touch == null) return;

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
        currentAngle += deltaX * rotationSpeed * Time.deltaTime;
        currentAngle = Mathf.Clamp(currentAngle, minAngle, maxAngle);

        if (handleArm != null && pivotPoint != null)
        {
            handleArm.transform.RotateAround(pivotPoint.position, Vector3.forward, currentAngle - handleArm.localEulerAngles.z);
        }

        UpdateWaterOpacity();

        float z = handleArm.localEulerAngles.z;
        if (Mathf.Abs(z - maxAngle) <= 1f) // 90° = closed
        {
            if (proceedButton != null && !proceedButton.activeSelf)
            {
                Debug.Log("✅ Faucet is fully closed! Proceed button unlocked.");
                proceedButton.SetActive(true);
            }
        }
    }



   void UpdateWaterOpacity()
{
    if (waterRenderer != null || faucetAudioSource != null)
    {
        float z = handleArm.localEulerAngles.z;

        // Normalize angle from 0 (fully open) to maxAngle (fully closed)
        float normalized = Mathf.InverseLerp(minAngle, maxAngle, z);

        // Water opacity (reverse: 1 when open, 0 when closed)
        if (waterRenderer != null)
        {
            Color c = waterRenderer.color;
            c.a = 1f - normalized;
            waterRenderer.color = c;
        }

        // Faucet sound volume (same logic: full when open, silent when closed)
        if (faucetAudioSource != null)
        {
            faucetAudioSource.volume = 1f - normalized;
            if (!faucetAudioSource.isPlaying && faucetAudioSource.volume > 0f)
                faucetAudioSource.Play();
            else if (faucetAudioSource.volume <= 0f)
                faucetAudioSource.Stop();
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

        transform.parent.gameObject.SetActive(false);
    }
    
}
