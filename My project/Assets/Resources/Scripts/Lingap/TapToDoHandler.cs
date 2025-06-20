using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class TapToDoHandler : MonoBehaviour
{
    [SerializeField] private string taskID;

    private Camera cam;

    void Awake()
    {
        cam = Camera.main;
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
            Vector2 mousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                DoAction();
            }
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
                DoAction();
            }
        }
    }

    void DoAction()
    {
        if (!string.IsNullOrEmpty(taskID))
        {
            TaskManager.Instance.IncrementTask(taskID);
        }

        Destroy(gameObject); // remove object after tapping, just like drag drop
    }
}
