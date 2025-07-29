using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class BedMatsMove : MonoBehaviour
{
    [SerializeField] private string taskID;
    public GameObject BedMats;
    private Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            HandleTouch();
        }
        else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
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
                MoveChair();
            }
        }
    }

    void HandleTouch()
    {
        var touch = Touchscreen.current.primaryTouch;

        if (touch.press.wasPressedThisFrame)
        {
            // Debug.Log("Simulated touch press detected!");

            Vector2 touchPos = cam.ScreenToWorldPoint(touch.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                MoveChair();
            }
        }
    }

    void MoveChair()
    {
        TriggerSetActive();
        if (!string.IsNullOrEmpty(taskID))
        {
            TaskManager.Instance.IncrementTask(taskID);
        }
        Destroy(gameObject);
    }
    public void TriggerSetActive()
    {
        BedMats.SetActive(true);
    }
}
