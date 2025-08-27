using UnityEngine;
using UnityEngine.InputSystem;

public class EraserController : MonoBehaviour
{
    private Camera cam;
    private Collider2D col;

    private Vector3 startPosition;
    private Vector3 dragOffset;
    private bool isDragging = false;

    private enum DragSource { None, Mouse, Touch }
    private DragSource activeSource = DragSource.None;

    void Awake()
    {
        cam = Camera.main;
        col = GetComponent<Collider2D>();
        startPosition = transform.position;
    }

    void Update()
    {
        HandlePressBegan();    // detect new press over this sponge
        HandleDragMove();      // follow while dragging
        HandlePressEnded();    // release & snap back
    }

    void HandlePressBegan()
    {
        // Touch press begin
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 screen = Touchscreen.current.primaryTouch.position.ReadValue();
            Vector3 world = cam.ScreenToWorldPoint(new Vector3(screen.x, screen.y, -cam.transform.position.z));
            world.z = transform.position.z;

            if (Physics2D.OverlapPoint(world) == col)
            {
                isDragging = true;
                activeSource = DragSource.Touch;
                dragOffset = transform.position - world;
            }
        }

        // Mouse press begin
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 screen = Mouse.current.position.ReadValue();
            Vector3 world = cam.ScreenToWorldPoint(new Vector3(screen.x, screen.y, -cam.transform.position.z));
            world.z = transform.position.z;

            if (Physics2D.OverlapPoint(world) == col)
            {
                isDragging = true;
                activeSource = DragSource.Mouse;
                dragOffset = transform.position - world;
            }
        }
    }

    void HandleDragMove()
    {
        if (!isDragging) return;

        if (activeSource == DragSource.Touch && Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 screen = Touchscreen.current.primaryTouch.position.ReadValue();
            Vector3 world = cam.ScreenToWorldPoint(new Vector3(screen.x, screen.y, -cam.transform.position.z));
            world.z = transform.position.z;
            transform.position = world + dragOffset;
        }
        else if (activeSource == DragSource.Mouse && Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            Vector2 screen = Mouse.current.position.ReadValue();
            Vector3 world = cam.ScreenToWorldPoint(new Vector3(screen.x, screen.y, -cam.transform.position.z));
            world.z = transform.position.z;
            transform.position = world + dragOffset;
        }
    }

    void HandlePressEnded()
    {
        // Touch released
        if (activeSource == DragSource.Touch &&
            (Touchscreen.current == null || Touchscreen.current.primaryTouch.press.wasReleasedThisFrame))
        {
            ResetPosition();
        }

        // Mouse released
        if (activeSource == DragSource.Mouse &&
            (Mouse.current == null || Mouse.current.leftButton.wasReleasedThisFrame))
        {
            ResetPosition();
        }
    }

    void ResetPosition()
    {
        isDragging = false;
        activeSource = DragSource.None;
        transform.position = startPosition;
    }
}
